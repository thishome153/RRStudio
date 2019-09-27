/*
 * Rufus: The Reliable USB Formatting Utility
 * PKI functions (code signing, etc.)
 * Copyright © 2015-2016 Pete Batard <pete@akeo.ie>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

/* Memory leaks detection - define _CRTDBG_MAP_ALLOC as preprocessor macro */
#ifdef _CRTDBG_MAP_ALLOC
//#include <stdlib.h>
//#include <crtdbg.h>
#endif

//#include "Stdafx.h"
#include "pki.h"

#include <windows.h>
#include <stdio.h>
#include <wincrypt.h>
#include <wintrust.h>
#include <assert.h>



//#include "rufus.h"
#include "resource.h"
#include "msapi_utf8.h"
//#include "localization.h"




#define ENCODING (X509_ASN_ENCODING | PKCS_7_ASN_ENCODING)

// MinGW doesn't seem to have this one
#if !defined(szOID_NESTED_SIGNATURE)
#define szOID_NESTED_SIGNATURE "1.3.6.1.4.1.311.2.4.1"
#endif

// Signatures names we accept. Must be the the exact name, including capitalization,
// that CertGetNameStringA(CERT_NAME_ATTR_TYPE, szOID_COMMON_NAME) returns.
const char* cert_name[3] = { "Akeo Consulting", "Akeo Systems", "Pete Batard" };
// For added security, we also validate the country code of the certificate recipient.
const char* cert_country = "IE";

typedef struct {
	LPWSTR lpszProgramName;
	LPWSTR lpszPublisherLink;
	LPWSTR lpszMoreInfoLink;
} SPROG_PUBLISHERINFO, *PSPROG_PUBLISHERINFO;

// https://msdn.microsoft.com/en-us/library/ee442238.aspx
typedef struct {
	BLOBHEADER BlobHeader;
	RSAPUBKEY  RsaHeader;
	BYTE       Modulus[256];	// 2048 bit modulus
} RSA_2048_PUBKEY;

// The RSA public key modulus for the private key we use to sign the files on the server.
// NOTE 1: This openssl modulus must be *REVERSED* to be usable with Microsoft APIs
// NOTE 2: Also, this modulus is 2052 bits, and not 2048, because openssl adds an extra
// 0x00 at the beginning to force an integer sign. These extra 8 bits *MUST* be removed.
static uint8_t rsa_pubkey_modulus[] = {
	/*
		$ openssl genrsa -aes256 -out private.pem 2048
		$ openssl rsa -in private.pem -pubout -out public.pem
		$ openssl rsa -pubin -inform PEM -text -noout < public.pem
		Public-Key: (2048 bit)
		Modulus:
			00:b6:40:7d:d1:98:7b:81:9e:be:23:0f:32:5d:55:
			60:c6:bf:b4:41:bb:43:1b:f1:e1:e6:f9:2b:d6:dd:
			11:50:e8:b9:3f:19:97:5e:a7:8b:4a:30:c6:76:58:
			72:1c:ac:ff:a1:f8:96:6c:51:5d:13:11:e3:5b:11:
			82:f5:9a:69:e4:28:97:0f:ca:1f:02:ea:1f:7d:dc:
			f9:fc:79:2f:61:ff:8e:45:60:65:ba:37:9b:de:49:
			05:6a:a8:fd:70:d0:0c:79:b6:d7:81:aa:54:c3:c6:
			4a:87:a0:45:ee:ca:d5:d5:c5:c2:ac:86:42:b3:58:
			27:d2:43:b9:37:f2:e6:75:66:17:53:d0:38:d0:c6:
			57:c2:55:36:a2:43:87:ea:24:f0:96:ec:34:dd:79:
			4d:80:54:9d:84:81:a7:cf:0c:a5:7c:d6:63:fa:7a:
			66:30:a9:50:ee:f0:e5:f8:a2:2d:ac:fc:24:21:fe:
			ef:e8:d3:6f:0e:27:b0:64:22:95:3e:6d:a6:66:97:
			c6:98:c2:47:b3:98:69:4d:b1:b5:d3:6f:43:f5:d7:
			a5:13:5e:8c:28:4f:62:4e:01:48:0a:63:89:e7:ca:
			34:aa:7d:2f:bb:70:e0:31:bb:39:49:a3:d2:c9:2e:
			a6:30:54:9a:5c:4d:58:17:d9:fc:3a:43:e6:8e:2a:
			18:e9
		Exponent: 65537 (0x10001)
	*/
	0x00, 0xb6, 0x40, 0x7d, 0xd1, 0x98, 0x7b, 0x81, 0x9e, 0xbe, 0x23, 0x0f, 0x32, 0x5d, 0x55,
	0x60, 0xc6, 0xbf, 0xb4, 0x41, 0xbb, 0x43, 0x1b, 0xf1, 0xe1, 0xe6, 0xf9, 0x2b, 0xd6, 0xdd,
	0x11, 0x50, 0xe8, 0xb9, 0x3f, 0x19, 0x97, 0x5e, 0xa7, 0x8b, 0x4a, 0x30, 0xc6, 0x76, 0x58,
	0x72, 0x1c, 0xac, 0xff, 0xa1, 0xf8, 0x96, 0x6c, 0x51, 0x5d, 0x13, 0x11, 0xe3, 0x5b, 0x11,
	0x82, 0xf5, 0x9a, 0x69, 0xe4, 0x28, 0x97, 0x0f, 0xca, 0x1f, 0x02, 0xea, 0x1f, 0x7d, 0xdc,
	0xf9, 0xfc, 0x79, 0x2f, 0x61, 0xff, 0x8e, 0x45, 0x60, 0x65, 0xba, 0x37, 0x9b, 0xde, 0x49,
	0x05, 0x6a, 0xa8, 0xfd, 0x70, 0xd0, 0x0c, 0x79, 0xb6, 0xd7, 0x81, 0xaa, 0x54, 0xc3, 0xc6,
	0x4a, 0x87, 0xa0, 0x45, 0xee, 0xca, 0xd5, 0xd5, 0xc5, 0xc2, 0xac, 0x86, 0x42, 0xb3, 0x58,
	0x27, 0xd2, 0x43, 0xb9, 0x37, 0xf2, 0xe6, 0x75, 0x66, 0x17, 0x53, 0xd0, 0x38, 0xd0, 0xc6,
	0x57, 0xc2, 0x55, 0x36, 0xa2, 0x43, 0x87, 0xea, 0x24, 0xf0, 0x96, 0xec, 0x34, 0xdd, 0x79,
	0x4d, 0x80, 0x54, 0x9d, 0x84, 0x81, 0xa7, 0xcf, 0x0c, 0xa5, 0x7c, 0xd6, 0x63, 0xfa, 0x7a,
	0x66, 0x30, 0xa9, 0x50, 0xee, 0xf0, 0xe5, 0xf8, 0xa2, 0x2d, 0xac, 0xfc, 0x24, 0x21, 0xfe,
	0xef, 0xe8, 0xd3, 0x6f, 0x0e, 0x27, 0xb0, 0x64, 0x22, 0x95, 0x3e, 0x6d, 0xa6, 0x66, 0x97,
	0xc6, 0x98, 0xc2, 0x47, 0xb3, 0x98, 0x69, 0x4d, 0xb1, 0xb5, 0xd3, 0x6f, 0x43, 0xf5, 0xd7,
	0xa5, 0x13, 0x5e, 0x8c, 0x28, 0x4f, 0x62, 0x4e, 0x01, 0x48, 0x0a, 0x63, 0x89, 0xe7, 0xca,
	0x34, 0xaa, 0x7d, 0x2f, 0xbb, 0x70, 0xe0, 0x31, 0xbb, 0x39, 0x49, 0xa3, 0xd2, 0xc9, 0x2e,
	0xa6, 0x30, 0x54, 0x9a, 0x5c, 0x4d, 0x58, 0x17, 0xd9, 0xfc, 0x3a, 0x43, 0xe6, 0x8e, 0x2a,
	0x18, 0xe9
};



WORD selected_langid = MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT);
int force_update = 0;
BOOL op_in_progress = TRUE, right_to_left_mode = FALSE, has_uefi_csm = FALSE, is_me = FALSE;

// Count on Microsoft to add a new API while not bothering updating the existing error facilities,
// so that the new error messages have to be handled manually. Now, since I don't have all day:
// 1. Copy text from https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-vds/5102cc53-3143-4268-ba4c-6ea39e999ab4
// 2. awk '{l[NR%7]=$0} {if (NR%7==0) printf "\tcase %s:\t// %s\n\t\treturn \"%s\";\n", l[1], l[3], l[6]}' vds.txt
// 3. Filter out the crap we don't need.
static const char* GetVdsError(DWORD error_code)
{
	switch (error_code) {
	case 0x80042400:	// VDS_E_NOT_SUPPORTED
		return "The operation is not supported by the object.";
	case 0x80042401:	// VDS_E_INITIALIZED_FAILED
		return "VDS or the provider failed to initialize.";
	case 0x80042402:	// VDS_E_INITIALIZE_NOT_CALLED
		return "VDS did not call the hardware provider's initialization method.";
	case 0x80042403:	// VDS_E_ALREADY_REGISTERED
		return "The provider is already registered.";
	case 0x80042404:	// VDS_E_ANOTHER_CALL_IN_PROGRESS
		return "A concurrent second call is made on an object before the first call is completed.";
	case 0x80042405:	// VDS_E_OBJECT_NOT_FOUND
		return "The specified object was not found.";
	case 0x80042406:	// VDS_E_INVALID_SPACE
		return "The specified space is neither free nor valid.";
	case 0x80042407:	// VDS_E_PARTITION_LIMIT_REACHED
		return "No more partitions can be created on the specified disk.";
	case 0x80042408:	// VDS_E_PARTITION_NOT_EMPTY
		return "The extended partition is not empty.";
	case 0x80042409:	// VDS_E_OPERATION_PENDING
		return "The operation is still in progress.";
	case 0x8004240A:	// VDS_E_OPERATION_DENIED
		return "The operation is not permitted on the specified disk, partition, or volume.";
	case 0x8004240B:	// VDS_E_OBJECT_DELETED
		return "The object no longer exists.";
	case 0x8004240C:	// VDS_E_CANCEL_TOO_LATE
		return "The operation can no longer be canceled.";
	case 0x8004240D:	// VDS_E_OPERATION_CANCELED
		return "The operation has already been canceled.";
	case 0x8004240E:	// VDS_E_CANNOT_EXTEND
		return "The file system does not support extending this volume.";
	case 0x8004240F:	// VDS_E_NOT_ENOUGH_SPACE
		return "There is not enough space to complete the operation.";
	case 0x80042410:	// VDS_E_NOT_ENOUGH_DRIVE
		return "There are not enough free disk drives in the subsystem to complete the operation.";
	case 0x80042411:	// VDS_E_BAD_COOKIE
		return "The cookie was not found.";
	case 0x80042412:	// VDS_E_NO_MEDIA
		return "There is no removable media in the drive.";
	case 0x80042413:	// VDS_E_DEVICE_IN_USE
		return "The device is currently in use.";
	case 0x80042414:	// VDS_E_DISK_NOT_EMPTY
		return "The disk contains partitions or volumes.";
	case 0x80042415:	// VDS_E_INVALID_OPERATION
		return "The specified operation is not valid.";
	case 0x80042416:	// VDS_E_PATH_NOT_FOUND
		return "The specified path was not found.";
	case 0x80042417:	// VDS_E_DISK_NOT_INITIALIZED
		return "The specified disk has not been initialized.";
	case 0x80042418:	// VDS_E_NOT_AN_UNALLOCATED_DISK
		return "The specified disk is not an unallocated disk.";
	case 0x80042419:	// VDS_E_UNRECOVERABLE_ERROR
		return "An unrecoverable error occurred. The service MUST shut down.";
	case 0x0004241A:	// VDS_S_DISK_PARTIALLY_CLEANED
		return "The clean operation was not a full clean or was canceled before it could be completed.";
	case 0x8004241B:	// VDS_E_DMADMIN_SERVICE_CONNECTION_FAILED
		return "The provider failed to connect to the LDMA service.";
	case 0x8004241C:	// VDS_E_PROVIDER_INITIALIZATION_FAILED
		return "The provider failed to initialize.";
	case 0x8004241D:	// VDS_E_OBJECT_EXISTS
		return "The object already exists.";
	case 0x8004241E:	// VDS_E_NO_DISKS_FOUND
		return "No disks were found on the target machine.";
	case 0x8004241F:	// VDS_E_PROVIDER_CACHE_CORRUPT
		return "The cache for a provider is corrupt.";
	case 0x80042420:	// VDS_E_DMADMIN_METHOD_CALL_FAILED
		return "A method call to the LDMA service failed.";
	case 0x00042421:	// VDS_S_PROVIDER_ERROR_LOADING_CACHE
		return "The provider encountered errors while loading the cache.";
	case 0x80042422:	// VDS_E_PROVIDER_VOL_DEVICE_NAME_NOT_FOUND
		return "The device form of the volume pathname could not be retrieved.";
	case 0x80042423:	// VDS_E_PROVIDER_VOL_OPEN
		return "Failed to open the volume device";
	case 0x80042424:	// VDS_E_DMADMIN_CORRUPT_NOTIFICATION
		return "A corrupt notification was sent from the LDMA service.";
	case 0x80042425:	// VDS_E_INCOMPATIBLE_FILE_SYSTEM
		return "The file system is incompatible with the specified operation.";
	case 0x80042426:	// VDS_E_INCOMPATIBLE_MEDIA
		return "The media is incompatible with the specified operation.";
	case 0x80042427:	// VDS_E_ACCESS_DENIED
		return "Access is denied. A VDS operation MUST run elevated.";
	case 0x80042428:	// VDS_E_MEDIA_WRITE_PROTECTED
		return "The media is write-protected.";
	case 0x80042429:	// VDS_E_BAD_LABEL
		return "The volume label is not valid.";
	case 0x8004242A:	// VDS_E_CANT_QUICK_FORMAT
		return "The volume cannot be quick-formatted.";
	case 0x8004242B:	// VDS_E_IO_ERROR
		return "An I/O error occurred during the operation.";
	case 0x8004242C:	// VDS_E_VOLUME_TOO_SMALL
		return "The volume size is too small.";
	case 0x8004242D:	// VDS_E_VOLUME_TOO_BIG
		return "The volume size is too large.";
	case 0x8004242E:	// VDS_E_CLUSTER_SIZE_TOO_SMALL
		return "The cluster size is too small.";
	case 0x8004242F:	// VDS_E_CLUSTER_SIZE_TOO_BIG
		return "The cluster size is too large.";
	case 0x80042430:	// VDS_E_CLUSTER_COUNT_BEYOND_32BITS
		return "The number of clusters is too large to be represented as a 32-bit integer.";
	case 0x80042431:	// VDS_E_OBJECT_STATUS_FAILED
		return "The component that the object represents has failed.";
	case 0x80042432:	// VDS_E_VOLUME_INCOMPLETE
		return "The volume is incomplete.";
	case 0x80042433:	// VDS_E_EXTENT_SIZE_LESS_THAN_MIN
		return "The specified extent size is too small.";
	case 0x00042434:	// VDS_S_UPDATE_BOOTFILE_FAILED
		return "The operation was successful, but VDS failed to update the boot options.";
	case 0x00042436:	// VDS_S_BOOT_PARTITION_NUMBER_CHANGE
	case 0x80042436:	// VDS_E_BOOT_PARTITION_NUMBER_CHANGE
		return "The boot partition's partition number will change as a result of the operation.";
	case 0x80042437:	// VDS_E_NO_FREE_SPACE
		return "The specified disk does not have enough free space to complete the operation.";
	case 0x80042438:	// VDS_E_ACTIVE_PARTITION
		return "An active partition was detected on the selected disk.";
	case 0x80042439:	// VDS_E_PARTITION_OF_UNKNOWN_TYPE
		return "The partition information cannot be read.";
	case 0x8004243A:	// VDS_E_LEGACY_VOLUME_FORMAT
		return "A partition with an unknown type was detected on the specified disk.";
	case 0x8004243C:	// VDS_E_MIGRATE_OPEN_VOLUME
		return "A volume on the specified disk could not be opened.";
	case 0x8004243D:	// VDS_E_VOLUME_NOT_ONLINE
		return "The volume is not online.";
	case 0x8004243E:	// VDS_E_VOLUME_NOT_HEALTHY
		return "The volume is failing or has failed.";
	case 0x8004243F:	// VDS_E_VOLUME_SPANS_DISKS
		return "The volume spans multiple disks.";
	case 0x80042440:	// VDS_E_REQUIRES_CONTIGUOUS_DISK_SPACE
		return "The volume does not consist of a single disk extent.";
	case 0x80042441:	// VDS_E_BAD_PROVIDER_DATA
		return "A provider returned bad data.";
	case 0x80042442:	// VDS_E_PROVIDER_FAILURE
		return "A provider failed to complete an operation.";
	case 0x00042443:	// VDS_S_VOLUME_COMPRESS_FAILED
		return "The file system was formatted successfully but could not be compressed.";
	case 0x80042444:	// VDS_E_PACK_OFFLINE
		return "The pack is offline.";
	case 0x80042445:	// VDS_E_VOLUME_NOT_A_MIRROR
		return "The volume is not a mirror.";
	case 0x80042446:	// VDS_E_NO_EXTENTS_FOR_VOLUME
		return "No extents were found for the volume.";
	case 0x80042447:	// VDS_E_DISK_NOT_LOADED_TO_CACHE
		return "The migrated disk failed to load to the cache.";
	case 0x80042448:	// VDS_E_INTERNAL_ERROR
		return "VDS encountered an internal error.";
	case 0x8004244A:	// VDS_E_PROVIDER_TYPE_NOT_SUPPORTED
		return "The method call is not supported for the specified provider type.";
	case 0x8004244B:	// VDS_E_DISK_NOT_ONLINE
		return "One or more of the specified disks are not online.";
	case 0x8004244C:	// VDS_E_DISK_IN_USE_BY_VOLUME
		return "One or more extents of the disk are already being used by the volume.";
	case 0x0004244D:	// VDS_S_IN_PROGRESS
		return "The asynchronous operation is in progress.";
	case 0x8004244E:	// VDS_E_ASYNC_OBJECT_FAILURE
		return "Failure initializing the asynchronous object.";
	case 0x8004244F:	// VDS_E_VOLUME_NOT_MOUNTED
		return "The volume is not mounted.";
	case 0x80042450:	// VDS_E_PACK_NOT_FOUND
		return "The pack was not found.";
	case 0x80042453:	// VDS_E_OBJECT_OUT_OF_SYNC
		return "The reference to the object might be stale.";
	case 0x80042454:	// VDS_E_MISSING_DISK
		return "The specified disk could not be found.";
	case 0x80042455:	// VDS_E_DISK_PNP_REG_CORRUPT
		return "The provider's list of PnP registered disks has become corrupted.";
	case 0x80042457:	// VDS_E_NO_DRIVELETTER_FLAG
		return "The provider does not support the VDS_VF_NO DRIVELETTER volume flag.";
	case 0x80042459:	// VDS_E_REVERT_ON_CLOSE_SET
		return "Some volume flags are already set.";
	case 0x0004245B:	// VDS_S_UNABLE_TO_GET_GPT_ATTRIBUTES
		return "Unable to retrieve the GPT attributes for this volume.";
	case 0x8004245C:	// VDS_E_VOLUME_TEMPORARILY_DISMOUNTED
		return "The volume is already dismounted temporarily.";
	case 0x8004245D:	// VDS_E_VOLUME_PERMANENTLY_DISMOUNTED
		return "The volume is already permanently dismounted.";
	case 0x8004245E:	// VDS_E_VOLUME_HAS_PATH
		return "The volume cannot be dismounted permanently because it still has an access path.";
	case 0x8004245F:	// VDS_E_TIMEOUT
		return "The operation timed out.";
	case 0x80042461:	// VDS_E_LDM_TIMEOUT
		return "The operation timed out in the LDMA service. Retry the operation.";
	case 0x80042462:	// VDS_E_REVERT_ON_CLOSE_MISMATCH
		return "The flags to be cleared do not match the flags that were set previously.";
	case 0x80042463:	// VDS_E_RETRY
		return "The operation failed. Retry the operation.";
	case 0x80042464:	// VDS_E_ONLINE_PACK_EXISTS
		return "The operation failed, because an online pack object already exists.";
	case 0x80042468:	// VDS_E_MAX_USABLE_MBR
		return "Only the first 2TB are usable on large MBR disks.";
	case 0x80042500:	// VDS_E_NO_SOFTWARE_PROVIDERS_LOADED
		return "There are no software providers loaded.";
	case 0x80042501:	// VDS_E_DISK_NOT_MISSING
		return "The disk is not missing.";
	case 0x80042502:	// VDS_E_NO_VOLUME_LAYOUT
		return "The volume's layout could not be retrieved.";
	case 0x80042503:	// VDS_E_CORRUPT_VOLUME_INFO
		return "The volume's driver information is corrupted.";
	case 0x80042504:	// VDS_E_INVALID_ENUMERATOR
		return "The enumerator is corrupted";
	case 0x80042505:	// VDS_E_DRIVER_INTERNAL_ERROR
		return "An internal error occurred in the volume management driver.";
	case 0x80042507:	// VDS_E_VOLUME_INVALID_NAME
		return "The volume name is not valid.";
	case 0x00042508:	// VDS_S_DISK_IS_MISSING
		return "The disk is missing and not all information could be returned.";
	case 0x80042509:	// VDS_E_CORRUPT_PARTITION_INFO
		return "The disk's partition information is corrupted.";
	case 0x0004250A:	// VDS_S_NONCONFORMANT_PARTITION_INFO
		return "The disk's partition information does not conform to what is expected on a dynamic disk.";
	case 0x8004250B:	// VDS_E_CORRUPT_EXTENT_INFO
		return "The disk's extent information is corrupted.";
	case 0x0004250E:	// VDS_S_SYSTEM_PARTITION
		return "Warning: There was a failure while checking for the system partition.";
	case 0x8004250F:	// VDS_E_BAD_PNP_MESSAGE
		return "The PNP service sent a corrupted notification to the provider.";
	case 0x80042510:	// VDS_E_NO_PNP_DISK_ARRIVE
	case 0x80042511:	// VDS_E_NO_PNP_VOLUME_ARRIVE
		return "No disk/volume arrival notification was received.";
	case 0x80042512:	// VDS_E_NO_PNP_DISK_REMOVE
	case 0x80042513:	// VDS_E_NO_PNP_VOLUME_REMOVE
		return "No disk/volume removal notification was received.";
	case 0x80042514:	// VDS_E_PROVIDER_EXITING
		return "The provider is exiting.";
	case 0x00042517:	// VDS_S_NO_NOTIFICATION
		return "No volume arrival notification was received.";
	case 0x80042519:	// VDS_E_INVALID_DISK
		return "The specified disk is not valid.";
	case 0x8004251A:	// VDS_E_INVALID_PACK
		return "The specified disk pack is not valid.";
	case 0x8004251B:	// VDS_E_VOLUME_ON_DISK
		return "This operation is not allowed on disks with volumes.";
	case 0x8004251C:	// VDS_E_DRIVER_INVALID_PARAM
		return "The driver returned an invalid parameter error.";
	case 0x8004253D:	// VDS_E_DRIVER_OBJECT_NOT_FOUND
		return "The object was not found in the driver cache.";
	case 0x8004253E:	// VDS_E_PARTITION_NOT_CYLINDER_ALIGNED
		return "The disk layout contains partitions which are not cylinder aligned.";
	case 0x8004253F:	// VDS_E_DISK_LAYOUT_PARTITIONS_TOO_SMALL
		return "The disk layout contains partitions which are less than the minimum required size.";
	case 0x80042540:	// VDS_E_DISK_IO_FAILING
		return "The I/O to the disk is failing.";
	case 0x80042543:	// VDS_E_GPT_ATTRIBUTES_INVALID
		return "Invalid GPT attributes were specified.";
	case 0x8004254D:	// VDS_E_UNEXPECTED_DISK_LAYOUT_CHANGE
		return "An unexpected layout change occurred external to the volume manager.";
	case 0x8004254E:	// VDS_E_INVALID_VOLUME_LENGTH
		return "The volume length is invalid.";
	case 0x8004254F:	// VDS_E_VOLUME_LENGTH_NOT_SECTOR_SIZE_MULTIPLE
		return "The volume length is not a multiple of the sector size.";
	case 0x80042550:	// VDS_E_VOLUME_NOT_RETAINED
		return "The volume does not have a retained partition association.";
	case 0x80042551:	// VDS_E_VOLUME_RETAINED
		return "The volume already has a retained partition association.";
	case 0x80042553:	// VDS_E_ALIGN_BEYOND_FIRST_CYLINDER
		return "The specified alignment is beyond the first cylinder.";
	case 0x80042554:	// VDS_E_ALIGN_NOT_SECTOR_SIZE_MULTIPLE
		return "The specified alignment is not a multiple of the sector size.";
	case 0x80042555:	// VDS_E_ALIGN_NOT_ZERO
		return "The specified partition type cannot be created with a non-zero alignment.";
	case 0x80042556:	// VDS_E_CACHE_CORRUPT
		return "The service's cache has become corrupt.";
	case 0x80042557:	// VDS_E_CANNOT_CLEAR_VOLUME_FLAG
		return "The specified volume flag cannot be cleared.";
	case 0x80042558:	// VDS_E_DISK_BEING_CLEANED
		return "The operation is not allowed on a disk that is in the process of being cleaned.";
	case 0x8004255A:	// VDS_E_DISK_REMOVEABLE
		return "The operation is not supported on removable media.";
	case 0x8004255B:	// VDS_E_DISK_REMOVEABLE_NOT_EMPTY
		return "The operation is not supported on a non-empty removable disk.";
	case 0x8004255C:	// VDS_E_DRIVE_LETTER_NOT_FREE
		return "The specified drive letter is not free to be assigned.";
	case 0x8004255E:	// VDS_E_INVALID_DRIVE_LETTER
		return "The specified drive letter is not valid.";
	case 0x8004255F:	// VDS_E_INVALID_DRIVE_LETTER_COUNT
		return "The specified number of drive letters to retrieve is not valid.";
	case 0x80042560:	// VDS_E_INVALID_FS_FLAG
		return "The specified file system flag is not valid.";
	case 0x80042561:	// VDS_E_INVALID_FS_TYPE
		return "The specified file system is not valid.";
	case 0x80042562:	// VDS_E_INVALID_OBJECT_TYPE
		return "The specified object type is not valid.";
	case 0x80042563:	// VDS_E_INVALID_PARTITION_LAYOUT
		return "The specified partition layout is invalid.";
	case 0x80042564:	// VDS_E_INVALID_PARTITION_STYLE
		return "VDS only supports MBR or GPT partition style disks.";
	case 0x80042565:	// VDS_E_INVALID_PARTITION_TYPE
		return "The specified partition type is not valid for this operation.";
	case 0x80042566:	// VDS_E_INVALID_PROVIDER_CLSID
	case 0x80042567:	// VDS_E_INVALID_PROVIDER_ID
	case 0x8004256A:	// VDS_E_INVALID_PROVIDER_VERSION_GUID
		return "A NULL GUID was passed to the provider.";
	case 0x80042568:	// VDS_E_INVALID_PROVIDER_NAME
		return "The specified provider name is invalid.";
	case 0x80042569:	// VDS_E_INVALID_PROVIDER_TYPE
		return "The specified provider type is invalid.";
	case 0x8004256B:	// VDS_E_INVALID_PROVIDER_VERSION_STRING
		return "The specified provider version string is invalid.";
	case 0x8004256C:	// VDS_E_INVALID_QUERY_PROVIDER_FLAG
		return "The specified query provider flag is invalid.";
	case 0x8004256D:	// VDS_E_INVALID_SERVICE_FLAG
		return "The specified service flag is invalid.";
	case 0x8004256E:	// VDS_E_INVALID_VOLUME_FLAG
		return "The specified volume flag is invalid.";
	case 0x8004256F:	// VDS_E_PARTITION_NOT_OEM
		return "The operation is only supported on an OEM, ESP, or unknown partition.";
	case 0x80042570:	// VDS_E_PARTITION_PROTECTED
		return "Cannot delete a protected partition without the force protected parameter set.";
	case 0x80042571:	// VDS_E_PARTITION_STYLE_MISMATCH
		return "The specified partition style is not the same as the disk's partition style.";
	case 0x80042572:	// VDS_E_PROVIDER_INTERNAL_ERROR
		return "An internal error has occurred in the provider.";
	case 0x80042575:	// VDS_E_UNRECOVERABLE_PROVIDER_ERROR
		return "An unrecoverable error occurred in the provider.";
	case 0x80042576:	// VDS_E_VOLUME_HIDDEN
		return "Cannot assign a mount point to a hidden volume.";
	case 0x00042577:	// VDS_S_DISMOUNT_FAILED
	case 0x00042578:	// VDS_S_REMOUNT_FAILED
		return "Failed to dismount/remount the volume after setting the volume flags.";
	case 0x80042579:	// VDS_E_FLAG_ALREADY_SET
		return "Cannot set the specified flag as revert-on-close because it is already set.";
	case 0x8004257B:	// VDS_E_DISTINCT_VOLUME
		return "The input volume id cannot be the id of the volume that is the target of the operation.";
	case 0x00042583:	// VDS_S_FS_LOCK
		return "Failed to obtain a file system lock.";
	case 0x80042584:	// VDS_E_READONLY
		return "The volume is read only.";
	case 0x80042585:	// VDS_E_INVALID_VOLUME_TYPE
		return "The volume type is invalid for this operation.";
	case 0x80042588:	// VDS_E_VOLUME_MIRRORED
		return "This operation is not supported on a mirrored volume.";
	case 0x80042589:	// VDS_E_VOLUME_SIMPLE_SPANNED
		return "The operation is only supported on simple or spanned volumes.";
	case 0x8004258C:	// VDS_E_PARTITION_MSR
	case 0x8004258D:	// VDS_E_PARTITION_LDM
		return "The operation is not supported on this type of partitions.";
	case 0x0004258E:	// VDS_S_WINPE_BOOTENTRY
		return "The boot entries cannot be updated automatically on WinPE.";
	case 0x8004258F:	// VDS_E_ALIGN_NOT_A_POWER_OF_TWO
		return "The alignment is not a power of two.";
	case 0x80042590:	// VDS_E_ALIGN_IS_ZERO
		return "The alignment is zero.";
	case 0x80042593:	// VDS_E_FS_NOT_DETERMINED
		return "The default file system could not be determined.";
	case 0x80042595:	// VDS_E_DISK_NOT_OFFLINE
		return "This disk is already online.";
	case 0x80042596:	// VDS_E_FAILED_TO_ONLINE_DISK
		return "The online operation failed.";
	case 0x80042597:	// VDS_E_FAILED_TO_OFFLINE_DISK
		return "The offline operation failed.";
	case 0x80042598:	// VDS_E_BAD_REVISION_NUMBER
		return "The operation could not be completed because the specified revision number is not supported.";
	case 0x00042700:	// VDS_S_NAME_TRUNCATED
		return "The name was set successfully but had to be truncated.";
	case 0x80042701:	// VDS_E_NAME_NOT_UNIQUE
		return "The specified name is not unique.";
	case 0x8004270F:	// VDS_E_NO_DISK_PATHNAME
		return "The disk's path could not be retrieved. Some operations on the disk might fail.";
	case 0x80042711:	// VDS_E_NO_VOLUME_PATHNAME
		return "The path could not be retrieved for one or more volumes.";
	case 0x80042712:	// VDS_E_PROVIDER_CACHE_OUTOFSYNC
		return "The provider's cache is not in sync with the driver cache.";
	case 0x80042713:	// VDS_E_NO_IMPORT_TARGET
		return "No import target was set for the subsystem.";
	case 0x00042714:	// VDS_S_ALREADY_EXISTS
		return "The object already exists.";
	case 0x00042715:	// VDS_S_PROPERTIES_INCOMPLETE
		return "Some, but not all, of the properties were successfully retrieved.";
	case 0x80042803:	// VDS_E_UNABLE_TO_FIND_BOOT_DISK
		return "Volume disk extent information could not be retrieved for the boot volume.";
	case 0x80042807:	// VDS_E_BOOT_DISK
		return "Disk attributes cannot be changed on the boot disk.";
	case 0x00042808:	// VDS_S_DISK_MOUNT_FAILED
	case 0x00042809:	// VDS_S_DISK_DISMOUNT_FAILED
		return "One or more of the volumes on the disk could not be mounted/dismounted.";
	case 0x8004280A:	// VDS_E_DISK_IS_OFFLINE
	case 0x8004280B:	// VDS_E_DISK_IS_READ_ONLY
		return "The operation cannot be performed on a disk that is offline or read-only.";
	case 0x8004280C:	// VDS_E_PAGEFILE_DISK
	case 0x8004280D:	// VDS_E_HIBERNATION_FILE_DISK
	case 0x8004280E:	// VDS_E_CRASHDUMP_DISK
		return "The operation cannot be performed on a disk that contains a pagefile, hibernation or crashdump volume.";
	case 0x8004280F:	// VDS_E_UNABLE_TO_FIND_SYSTEM_DISK
		return "A system error occurred while retrieving the system disk information.";
	case 0x80042810:	// VDS_E_INCORRECT_SYSTEM_VOLUME_EXTENT_INFO
		return "Multiple disk extents reported for the system volume - system error.";
	case 0x80042811:	// VDS_E_SYSTEM_DISK
		return "Disk attributes cannot be changed on the current system disk or BIOS disk 0.";
	case 0x80042823:	// VDS_E_SECTOR_SIZE_ERROR
		return "The sector size MUST be non-zero, a power of 2, and less than the maximum sector size.";
	case 0x80042907:	// VDS_E_SUBSYSTEM_ID_IS_NULL
		return "The provider returned a NULL subsystem identification string.";
	case 0x8004290C:	// VDS_E_REBOOT_REQUIRED
		return "A reboot is required before any further operations are initiated.";
	case 0x8004290D:	// VDS_E_VOLUME_GUID_PATHNAME_NOT_ALLOWED
		return "Volume GUID pathnames are not valid input to this method.";
	case 0x8004290E:	// VDS_E_BOOT_PAGEFILE_DRIVE_LETTER
		return "Assigning or removing drive letters on the current boot or pagefile volume is not allowed.";
	case 0x8004290F:	// VDS_E_DELETE_WITH_CRITICAL
		return "Delete is not allowed on a critical volume.";
	case 0x80042910:	// VDS_E_CLEAN_WITH_DATA
	case 0x80042911:	// VDS_E_CLEAN_WITH_OEM
		return "The FORCE parameter MUST be set to TRUE in order to clean a disk that contains a data or OEM volume.";
	case 0x80042912:	// VDS_E_CLEAN_WITH_CRITICAL
		return "Clean is not allowed on a critical disk.";
	case 0x80042913:	// VDS_E_FORMAT_CRITICAL
		return "Format is not allowed on a critical volume.";
	case 0x80042914:	// VDS_E_NTFS_FORMAT_NOT_SUPPORTED
	case 0x80042915:	// VDS_E_FAT32_FORMAT_NOT_SUPPORTED
	case 0x80042916:	// VDS_E_FAT_FORMAT_NOT_SUPPORTED
		return "The requested file system format is not supported on this volume.";
	case 0x80042917:	// VDS_E_FORMAT_NOT_SUPPORTED
		return "The volume is not formattable.";
	case 0x80042918:	// VDS_E_COMPRESSION_NOT_SUPPORTED
		return "The specified file system does not support compression.";
	default:
		return NULL;
	}
}

// Convert a windows error to human readable string
const char* WindowsErrorString(void)
{
	static char err_string[256] = { 0 };

	DWORD size, presize;
	DWORD error_code, format_error;

	error_code = GetLastError();
	// Check for VDS error codes
	if ((SCODE_FACILITY(error_code) == FACILITY_ITF) && (GetVdsError(error_code) != NULL)) {
		static_sprintf(err_string, "[0x%08lX] %s", error_code, GetVdsError(error_code));
		return err_string;
	}

	static_sprintf(err_string, "[0x%08lX] ", error_code);
	presize = (DWORD)strlen(err_string);

	size = FormatMessageU(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, HRESULT_CODE(error_code),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), &err_string[presize],
		sizeof(err_string) - (DWORD)strlen(err_string), NULL);
	if (size == 0) {
		format_error = GetLastError();
		if ((format_error) && (format_error != 0x13D))		// 0x13D, decode error, is returned for unknown codes
			static_sprintf(err_string, "Windows error code 0x%08lX (FormatMessage error code 0x%08lX)",
				error_code, format_error);
		else
			static_sprintf(err_string, "Unknown error 0x%08lX", error_code);
	}
	else {
		// Microsoft may suffix CRLF to error messages, which we need to remove...
		assert(presize > 2);
		size += presize - 2;
		// Cannot underflow if the above assert passed since our first char is neither of the following
		while ((err_string[size] == 0x0D) || (err_string[size] == 0x0A) || (err_string[size] == 0x20))
			err_string[size--] = 0;
	}

	SetLastError(error_code);	// Make sure we don't change the errorcode on exit
	return err_string;
}

/*
 * FormatMessage does not handle PKI errors
 */
const char* WinPKIErrorString(void)
{
	static char error_string[64];
	DWORD error_code = GetLastError();

	if (((error_code >> 16) != 0x8009) && ((error_code >> 16) != 0x800B))
		return WindowsErrorString();

	switch (error_code) {
	// See also https://docs.microsoft.com/en-gb/windows/desktop/com/com-error-codes-4
	case NTE_BAD_UID:
		return "Bad UID.";
	case NTE_NO_KEY:
		return "Key does not exist.";
	case NTE_BAD_KEYSET:
		return "Keyset does not exist.";
	case NTE_BAD_ALGID:
		return "Invalid algorithm specified.";
	case NTE_BAD_VER:
		return "Bad version of provider.";
	case NTE_BAD_SIGNATURE:
		return "Invalid Signature.";
	case CRYPT_E_MSG_ERROR:
		return "An error occurred while performing an operation on a cryptographic message.";
	case CRYPT_E_UNKNOWN_ALGO:
		return "Unknown cryptographic algorithm.";
	case CRYPT_E_INVALID_MSG_TYPE:
		return "Invalid cryptographic message type.";
	case CRYPT_E_HASH_VALUE:
		return "The hash value is not correct";
	case CRYPT_E_ISSUER_SERIALNUMBER:
		return "Invalid issuer and/or serial number.";
	case CRYPT_E_BAD_LEN:
		return "The length specified for the output data was insufficient.";
	case CRYPT_E_BAD_ENCODE:
		return "An error occurred during encode or decode operation.";
	case CRYPT_E_FILE_ERROR:
		return "An error occurred while reading or writing to a file.";
	case CRYPT_E_NOT_FOUND:
		return "Cannot find object or property.";
	case CRYPT_E_EXISTS:
		return "The object or property already exists.";
	case CRYPT_E_NO_PROVIDER:
		return "No provider was specified for the store or object.";
	case CRYPT_E_DELETED_PREV:
		return "The previous certificate or CRL context was deleted.";
	case CRYPT_E_NO_MATCH:
		return "Cannot find the requested object.";
	case CRYPT_E_UNEXPECTED_MSG_TYPE:
	case CRYPT_E_NO_KEY_PROPERTY:
	case CRYPT_E_NO_DECRYPT_CERT:
		return "Private key or certificate issue";
	case CRYPT_E_BAD_MSG:
		return "Not a cryptographic message.";
	case CRYPT_E_NO_SIGNER:
		return "The signed cryptographic message does not have a signer for the specified signer index.";
	case CRYPT_E_REVOKED:
		return "The certificate is revoked.";
	case CRYPT_E_NO_REVOCATION_DLL:
	case CRYPT_E_NO_REVOCATION_CHECK:
	case CRYPT_E_REVOCATION_OFFLINE:
	case CRYPT_E_NOT_IN_REVOCATION_DATABASE:
		return "Cannot check certificate revocation.";
	case CRYPT_E_INVALID_NUMERIC_STRING:
	case CRYPT_E_INVALID_PRINTABLE_STRING:
	case CRYPT_E_INVALID_IA5_STRING:
	case CRYPT_E_INVALID_X500_STRING:
	case  CRYPT_E_NOT_CHAR_STRING:
		return "Invalid string.";
	case CRYPT_E_SECURITY_SETTINGS:
		return "The cryptographic operation failed due to a local security option setting.";
	case CRYPT_E_NO_VERIFY_USAGE_CHECK:
	case CRYPT_E_VERIFY_USAGE_OFFLINE:
		return "Cannot complete usage check.";
	case CRYPT_E_NO_TRUSTED_SIGNER:
		return "None of the signers of the cryptographic message or certificate trust list is trusted.";
	case CERT_E_UNTRUSTEDROOT:
		return "The root certificate is not trusted.";
	case TRUST_E_NOSIGNATURE:
		return "Not digitally signed.";
	case TRUST_E_EXPLICIT_DISTRUST:
		return "One of the certificates used was marked as untrusted by the user.";
	case TRUST_E_TIME_STAMP:
		return "The timestamp could not be verified.";
	default:
		static_sprintf(error_string, "Unknown PKI error 0x%08lX", error_code);
		return error_string;
	}
}

// Mostly from https://support.microsoft.com/en-us/kb/323809
char* GetSignatureName(const char* path, const char* country_code)
{
	static char szSubjectName[128];
	char szCountry[3] = "__";
	char *p = NULL, *mpath = NULL;
	int i;
	BOOL r;
	HMODULE hm;
	HCERTSTORE hStore = NULL;
	HCRYPTMSG hMsg = NULL;
	PCCERT_CONTEXT pCertContext = NULL;
	DWORD dwSize, dwEncoding, dwContentType, dwFormatType;
	PCMSG_SIGNER_INFO pSignerInfo = NULL;
	DWORD dwSignerInfo = 0;
	CERT_INFO CertInfo = { 0 };
	SPROG_PUBLISHERINFO ProgPubInfo = { 0 };
	wchar_t *szFileName;

	// If the path is NULL, get the signature of the current runtime
	if (path == NULL) {
		szFileName = calloc(MAX_PATH, sizeof(wchar_t));
		if (szFileName == NULL)
			return NULL;
		hm = GetModuleHandle(NULL);
		if (hm == NULL) {
			uprintf("PKI: Could not get current executable handle: %s", WinPKIErrorString());
			goto out;
		}
		dwSize = GetModuleFileNameW(hm, szFileName, MAX_PATH);
		if ((dwSize == 0) || ((dwSize == MAX_PATH) && (GetLastError() == ERROR_INSUFFICIENT_BUFFER))) {
			uprintf("PKI: Could not get module filename: %s", WinPKIErrorString());
			goto out;
		}
		mpath = wchar_to_utf8(szFileName);
	} else {
		szFileName = utf8_to_wchar(path);
	}

	// Get message handle and store handle from the signed file.
	for (i = 0; i < 5; i++) {
		r = CryptQueryObject(CERT_QUERY_OBJECT_FILE, szFileName,
			CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED, CERT_QUERY_FORMAT_FLAG_BINARY,
			0, &dwEncoding, &dwContentType, &dwFormatType, &hStore, &hMsg, NULL);
		if (r)
			break;
		if (i == 0)
			uprintf("PKI: Failed to get signature for '%s': %s", (path==NULL)?mpath:path, WinPKIErrorString());
		if (path == NULL)
			break;
		uprintf("PKI: Retrying...");
		Sleep(2000);
	}
	if (!r)
		goto out;

	// Get signer information size.
	r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, NULL, &dwSignerInfo);
	if (!r) {
		uprintf("PKI: Failed to get signer size: %s", WinPKIErrorString());
		goto out;
	}

	// Allocate memory for signer information.
	pSignerInfo = (PCMSG_SIGNER_INFO)calloc(dwSignerInfo, 1);
	if (!pSignerInfo) {
		uprintf("PKI: Could not allocate memory for signer information");
		goto out;
	}

	// Get Signer Information.
	r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, (PVOID)pSignerInfo, &dwSignerInfo);
	if (!r) {
		uprintf("PKI: Failed to get signer information: %s", WinPKIErrorString());
		goto out;
	}

	// Search for the signer certificate in the temporary certificate store.
	CertInfo.Issuer = pSignerInfo->Issuer;
	CertInfo.SerialNumber = pSignerInfo->SerialNumber;

	pCertContext = CertFindCertificateInStore(hStore, ENCODING, 0, CERT_FIND_SUBJECT_CERT, (PVOID)&CertInfo, NULL);
	if (!pCertContext) {
		uprintf("PKI: Failed to locate signer certificate in temporary store: %s", WinPKIErrorString());
		goto out;
	}

	// If a country code is provided, validate that the certificate we have is for the same country
	if (country_code != NULL) {
		dwSize = CertGetNameStringA(pCertContext, CERT_NAME_ATTR_TYPE, 0, szOID_COUNTRY_NAME,
			szCountry, sizeof(szCountry));
		if (dwSize < 2) {
			uprintf("PKI: Failed to get Country Code");
			goto out;
		}
		if (strcmpi(country_code, szCountry) != 0) {
			uprintf("PKI: Unexpected Country Code (Found '%s', expected '%s')", szCountry, country_code);
			goto out;
		}
	}

	// Isolate the signing certificate subject name
	dwSize = CertGetNameStringA(pCertContext, CERT_NAME_ATTR_TYPE, 0, szOID_COMMON_NAME,
		szSubjectName, sizeof(szSubjectName));
	if (dwSize <= 1) {
		uprintf("PKI: Failed to get Subject Name");
		goto out;
	}

	if (szCountry[0] == '_')
		uprintf("Binary executable is signed by '%s'", szSubjectName);
	else
		uprintf("Binary executable is signed by '%s' (%s)", szSubjectName, szCountry);
	p = szSubjectName;

out:
	safe_free(mpath);
	safe_free(szFileName);
	safe_free(ProgPubInfo.lpszProgramName);
	safe_free(ProgPubInfo.lpszPublisherLink);
	safe_free(ProgPubInfo.lpszMoreInfoLink);
	safe_free(pSignerInfo);
	if (pCertContext != NULL)
		CertFreeCertificateContext(pCertContext);
	if (hStore != NULL)
		CertCloseStore(hStore, 0);
	if (hMsg != NULL)
		CryptMsgClose(hMsg);
	return p;
}

// The timestamping authorities we use are RFC 3161 compliant
static uint64_t GetRFC3161TimeStamp(PCMSG_SIGNER_INFO pSignerInfo)
{
	BOOL r, found = FALSE;
	DWORD n, dwSize = 0;
	PCRYPT_CONTENT_INFO pCounterSignerInfo = NULL;
	uint64_t ts = 0ULL;
	uint8_t *timestamp_token;
	size_t timestamp_token_size;
	char* timestamp_str;
	size_t timestamp_str_size;

	// Loop through unauthenticated attributes for szOID_RFC3161_counterSign OID
	for (n = 0; n < pSignerInfo->UnauthAttrs.cAttr; n++) {
		if (lstrcmpA(pSignerInfo->UnauthAttrs.rgAttr[n].pszObjId, szOID_RFC3161_counterSign) == 0) {
			// Depending on how Microsoft implemented their timestamp checks, and the fact that we are dealing
			// with UnauthAttrs, there's a possibility that an attacker may add a "fake" RFC 3161 countersigner
			// to try to trick us into using their timestamp data. Detect that.
			if (found) {
				uprintf("PKI: Multiple RFC 3161 countersigners found. This could indicate something very nasty...");
				return 0ULL;
			}
			found = TRUE;

			// Read the countersigner message data
			r = CryptDecodeObjectEx(PKCS_7_ASN_ENCODING, PKCS_CONTENT_INFO,
				pSignerInfo->UnauthAttrs.rgAttr[n].rgValue[0].pbData,
				pSignerInfo->UnauthAttrs.rgAttr[n].rgValue[0].cbData,
				CRYPT_DECODE_ALLOC_FLAG, NULL, (PVOID)&pCounterSignerInfo, &dwSize);
			if (!r) {
				uprintf("PKI: Could not retrieve RFC 3161 countersigner data: %s", WinPKIErrorString());
				continue;
			}

			// Get the RFC 3161 timestamp message
			timestamp_token = get_data_from_asn1(pCounterSignerInfo->Content.pbData,
				pCounterSignerInfo->Content.cbData, szOID_TIMESTAMP_TOKEN,
				// 0x04 = "Octet String" ASN.1 tag
				0x04, &timestamp_token_size);
			if (timestamp_token) {
				timestamp_str = get_data_from_asn1(timestamp_token, timestamp_token_size, NULL,
					// 0x18 = "Generalized Time" ASN.1 tag
					0x18, &timestamp_str_size);
				if (timestamp_str) {
					// As per RFC 3161 The syntax is: YYYYMMDDhhmmss[.s...]Z
					if ((timestamp_str_size < 14) || (timestamp_str[timestamp_str_size - 1] != 'Z')) {
						// Sanity checks
						uprintf("PKI: Not an RFC 3161 timestamp");
						DumpBufferHex(timestamp_str, timestamp_str_size);
					} else {
						ts = strtoull(timestamp_str, NULL, 10);
					}
				}
			}
			LocalFree(pCounterSignerInfo);
		}
	}
	return ts;
}

// The following is used to get the RFP 3161 timestamp of a nested signature
static uint64_t GetNestedRFC3161TimeStamp(PCMSG_SIGNER_INFO pSignerInfo)
{
	BOOL r, found = FALSE;
	DWORD n, dwSize = 0;
	PCRYPT_CONTENT_INFO pNestedSignature = NULL;
	PCMSG_SIGNER_INFO pNestedSignerInfo = NULL;
	HCRYPTMSG hMsg = NULL;
	uint64_t ts = 0ULL;

	// Loop through unauthenticated attributes for szOID_NESTED_SIGNATURE OID
	for (n = 0; ; n++) {
		if (pNestedSignature != NULL) {
			LocalFree(pNestedSignature);
			pNestedSignature = NULL;
		}
		if (hMsg != NULL) {
			CryptMsgClose(hMsg);
			hMsg = NULL;
		}
		safe_free(pNestedSignerInfo);
		if (n >= pSignerInfo->UnauthAttrs.cAttr)
			break;
		if (lstrcmpA(pSignerInfo->UnauthAttrs.rgAttr[n].pszObjId, szOID_NESTED_SIGNATURE) == 0) {
			if (found) {
				uprintf("PKI: Multiple nested signatures found. This could indicate something very nasty...");
				return 0ULL;
			}
			found = TRUE;
			r = CryptDecodeObjectEx(PKCS_7_ASN_ENCODING, PKCS_CONTENT_INFO,
				pSignerInfo->UnauthAttrs.rgAttr[n].rgValue[0].pbData,
				pSignerInfo->UnauthAttrs.rgAttr[n].rgValue[0].cbData,
				CRYPT_DECODE_ALLOC_FLAG, NULL, (PVOID)&pNestedSignature, &dwSize);
			if (!r) {
				uprintf("PKI: Could not retrieve nested signature data: %s", WinPKIErrorString());
				continue;
			}

			hMsg = CryptMsgOpenToDecode(ENCODING, CMSG_DETACHED_FLAG, CMSG_SIGNED, (HCRYPTPROV)NULL, NULL, NULL);
			if (hMsg == NULL) {
				uprintf("PKI: Could not create nested signature message: %s", WinPKIErrorString());
				continue;
			}
			r = CryptMsgUpdate(hMsg, pNestedSignature->Content.pbData, pNestedSignature->Content.cbData, TRUE);
			if (!r) {
				uprintf("PKI: Could not update message: %s", WinPKIErrorString());
				continue;
			}
			// Get nested signer
			r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, NULL, &dwSize);
			if (!r) {
				uprintf("PKI: Failed to get nested signer size: %s", WinPKIErrorString());
				continue;
			}
			pNestedSignerInfo = (PCMSG_SIGNER_INFO)calloc(dwSize, 1);
			if (!pNestedSignerInfo) {
				uprintf("PKI: Could not allocate memory for nested signer");
				continue;
			}
			r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, (PVOID)pNestedSignerInfo, &dwSize);
			if (!r) {
				uprintf("PKI: Failed to get nested signer information: %s", WinPKIErrorString());
				continue;
			}
			ts = GetRFC3161TimeStamp(pNestedSignerInfo);
		}
	}
	return ts;
}

// Return the signature timestamp (as a YYYYMMDDHHMMSS value) or 0 on error
uint64_t GetSignatureTimeStamp(const char* path)
{
	char *mpath = NULL;
	BOOL r;
	HMODULE hm;
	HCERTSTORE hStore = NULL;
	HCRYPTMSG hMsg = NULL;
	DWORD dwSize, dwEncoding, dwContentType, dwFormatType;
	PCMSG_SIGNER_INFO pSignerInfo = NULL;
	DWORD dwSignerInfo = 0;
	wchar_t *szFileName;
	uint64_t timestamp = 0ULL, nested_timestamp;

	// If the path is NULL, get the signature of the current runtime
	if (path == NULL) {
		szFileName = calloc(MAX_PATH, sizeof(wchar_t));
		if (szFileName == NULL)
			goto out;
		hm = GetModuleHandle(NULL);
		if (hm == NULL) {
			uprintf("PKI: Could not get current executable handle: %s", WinPKIErrorString());
			goto out;
		}
		dwSize = GetModuleFileNameW(hm, szFileName, MAX_PATH);
		if ((dwSize == 0) || ((dwSize == MAX_PATH) && (GetLastError() == ERROR_INSUFFICIENT_BUFFER))) {
			uprintf("PKI: Could not get module filename: %s", WinPKIErrorString());
			goto out;
		}
		mpath = wchar_to_utf8(szFileName);
	} else {
		szFileName = utf8_to_wchar(path);
	}

	// Get message handle and store handle from the signed file.
	r = CryptQueryObject(CERT_QUERY_OBJECT_FILE, szFileName,
		CERT_QUERY_CONTENT_FLAG_PKCS7_SIGNED_EMBED, CERT_QUERY_FORMAT_FLAG_BINARY,
		0, &dwEncoding, &dwContentType, &dwFormatType, &hStore, &hMsg, NULL);
	if (!r) {
		uprintf("PKI: Failed to get signature for '%s': %s", (path==NULL)?mpath:path, WinPKIErrorString());
		goto out;
	}

	// Get signer information size.
	r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, NULL, &dwSignerInfo);
	if (!r) {
		uprintf("PKI: Failed to get signer size: %s", WinPKIErrorString());
		goto out;
	}

	// Allocate memory for signer information.
	pSignerInfo = (PCMSG_SIGNER_INFO)calloc(dwSignerInfo, 1);
	if (!pSignerInfo) {
		uprintf("PKI: Could not allocate memory for signer information");
		goto out;
	}

	// Get Signer Information.
	r = CryptMsgGetParam(hMsg, CMSG_SIGNER_INFO_PARAM, 0, (PVOID)pSignerInfo, &dwSignerInfo);
	if (!r) {
		uprintf("PKI: Failed to get signer information: %s", WinPKIErrorString());
		goto out;
	}

	// Get the RFC 3161 timestamp
	timestamp = GetRFC3161TimeStamp(pSignerInfo);
	if (timestamp)
		uprintf("Note: '%s' has timestamp %s", (path==NULL)?mpath:path, TimestampToHumanReadable(timestamp));
	// Because we are currently using both SHA-1 and SHA-256 signatures, we are in the very specific
	// situation that Windows may say our executable passes Authenticode validation on Windows 7 or
	// later (which includes timestamp validation) even if the SHA-1 signature or timestamps have
	// been altered.
	// This means that, if we don't also check the nested SHA-256 signature timestamp, an attacker
	// could alter the SHA-1 one (which is the one we use by default for chronology validation) and
	// trick us into using an invalid timestamp value. To prevent this, we validate that, if we have
	// both a regular and nested timestamp, they are within 60 seconds of each other.
	nested_timestamp = GetNestedRFC3161TimeStamp(pSignerInfo);
	if (nested_timestamp)
		uprintf("Note: '%s' has nested timestamp %s", (path==NULL)?mpath:path, TimestampToHumanReadable(nested_timestamp));
	if ((timestamp != 0ULL) && (nested_timestamp != 0ULL)) {
		if (_abs64(nested_timestamp - timestamp) > 100) {
			uprintf("PKI: Signature timestamp and nested timestamp differ by more than a minute. "
				"This could indicate something very nasty...", timestamp, nested_timestamp);
			timestamp = 0ULL;
		}
	}

out:
	safe_free(mpath);
	safe_free(szFileName);
	safe_free(pSignerInfo);
	if (hStore != NULL)
		CertCloseStore(hStore, 0);
	if (hMsg != NULL)
		CryptMsgClose(hMsg);
	return timestamp;
}

// From https://msdn.microsoft.com/en-us/library/windows/desktop/aa382384.aspx
LONG ValidateSignature(HWND hDlg, const char* path)
{
	LONG r;
	WINTRUST_DATA trust_data = { 0 };
	WINTRUST_FILE_INFO trust_file = { 0 };
	GUID guid_generic_verify =	// WINTRUST_ACTION_GENERIC_VERIFY_V2
		{ 0xaac56b, 0xcd44, 0x11d0,{ 0x8c, 0xc2, 0x0, 0xc0, 0x4f, 0xc2, 0x95, 0xee } };
	char *signature_name;
	size_t i;
	uint64_t current_ts, update_ts;

	// Check the signature name. Make it specific enough (i.e. don't simply check for "Akeo")
	// so that, besides hacking our server, it'll place an extra hurdle on any malicious entity
	// into also fooling a C.A. to issue a certificate that passes our test.
	signature_name = GetSignatureName(path, cert_country);
	if (signature_name == NULL) {
		uprintf("PKI: Could not get signature name");
		MessageBoxExU(hDlg, lmprintf(MSG_284), lmprintf(MSG_283), MB_OK | MB_ICONERROR | MB_IS_RTL, selected_langid);
		return TRUST_E_NOSIGNATURE;
	}
	for (i = 0; i < ARRAYSIZE(cert_name); i++) {
		if (strcmp(signature_name, cert_name[i]) == 0)
			break;
	}
	if (i >= ARRAYSIZE(cert_name)) {
		uprintf("PKI: Signature '%s' is unexpected...", signature_name);
		if (MessageBoxExU(hDlg, lmprintf(MSG_285, signature_name), lmprintf(MSG_283),
			MB_YESNO | MB_ICONWARNING | MB_IS_RTL, selected_langid) != IDYES)
			return TRUST_E_EXPLICIT_DISTRUST;
	}

	trust_file.cbStruct = sizeof(trust_file);
	trust_file.pcwszFilePath = utf8_to_wchar(path);
	if (trust_file.pcwszFilePath == NULL) {
		uprintf("PKI: Unable to convert '%s' to UTF16", path);
		return ERROR_SEVERITY_ERROR | FAC(FACILITY_CERT) | ERROR_NOT_ENOUGH_MEMORY;
	}

	trust_data.cbStruct = sizeof(trust_data);
	// NB: WTD_UI_ALL can result in ERROR_SUCCESS even if the signature validation fails,
	// because it still prompts the user to run untrusted software, even after explicitly
	// notifying them that the signature invalid (and of course Microsoft had to make
	// that UI prompt a bit too similar to the other benign prompt you get when running
	// trusted software, which, as per cert.org's assessment, may confuse non-security
	// conscious-users who decide to gloss over these kind of notifications).
	trust_data.dwUIChoice = WTD_UI_NONE;
	// We just downloaded from the Internet, so we should be able to check revocation
	trust_data.fdwRevocationChecks = WTD_REVOKE_WHOLECHAIN;
	// 0x400 = WTD_MOTW  for Windows 8.1 or later
	trust_data.dwProvFlags = WTD_REVOCATION_CHECK_CHAIN | 0x400;
	trust_data.dwUnionChoice = WTD_CHOICE_FILE;
	trust_data.pFile = &trust_file;

	//be aware:  r = WinVerifyTrustEx( INVALID_HANDLE_VALUE, &guid_generic_verify, &trust_data);
	r = WinVerifyTrustEx(hDlg, &guid_generic_verify, &trust_data);
	safe_free(trust_file.pcwszFilePath);
	switch (r) {
	case ERROR_SUCCESS:
		// Verify that the timestamp of the downloaded update is in the future of our current one.
		// This is done to prevent the use of an officially signed, but older binary, as potential attack vector.
		current_ts = GetSignatureTimeStamp(NULL);
		if (current_ts == 0ULL) {
			uprintf("PKI: Cannot retrieve the current binary's timestamp - Aborting update");
			r = TRUST_E_TIME_STAMP;
		} else {
			update_ts = GetSignatureTimeStamp(path);
			if (update_ts < current_ts) {
				uprintf("PKI: Update timestamp (%" PRIi64 ") is younger than ours (%" PRIi64 ") - Aborting update", update_ts, current_ts);
				r = TRUST_E_TIME_STAMP;
			}
		}

		if ((r != ERROR_SUCCESS) && (force_update < 2))
		// **	MessageBoxExU(hDlg, lmprintf(MSG_300), lmprintf(MSG_299), MB_OK | MB_ICONERROR | MB_IS_RTL, selected_langid);

		break;
	case TRUST_E_NOSIGNATURE:
		// Should already have been reported, but since we have a custom message for it...
		uprintf("PKI: File does not appear to be signed: %s", WinPKIErrorString());
		MessageBoxExU(hDlg, lmprintf(MSG_284), lmprintf(MSG_283), MB_OK | MB_ICONERROR | MB_IS_RTL, selected_langid);
		break;
	default:
		uprintf("PKI: Failed to validate signature: %s", WinPKIErrorString());
		MessageBoxExU(hDlg, lmprintf(MSG_240), lmprintf(MSG_283), MB_OK | MB_ICONERROR | MB_IS_RTL, selected_langid);
		break;
	}

	return r;
}

// Why-oh-why am I the only one on github doing this openssl vs MS signature validation?!?
// For once, I'd like to find code samples from *OTHER PEOPLE* who went through this ordeal first...
BOOL ValidateOpensslSignature(BYTE* pbBuffer, DWORD dwBufferLen, BYTE* pbSignature, DWORD dwSigLen)
{
	HCRYPTPROV hProv = 0;
	HCRYPTHASH hHash = 0;
	HCRYPTKEY hPubKey;
	// We could load and convert an openssl PEM, but since we know what we need...
	RSA_2048_PUBKEY pbMyPubKey = {
		{ PUBLICKEYBLOB, CUR_BLOB_VERSION, 0, CALG_RSA_KEYX },
		// $ openssl genrsa -aes256 -out private.pem 2048
		// Generating RSA private key, 2048 bit long modulus
		// e is 65537 (0x010001)
		// => 0x010001 below. Also 0x31415352 = "RSA1"
		{ 0x31415352, sizeof(pbMyPubKey.Modulus) * 8, 0x010001 },
		{ 0 }	// Modulus is initialized below
	};
	USHORT dwMyPubKeyLen = sizeof(pbMyPubKey);
	BOOL r;
	BYTE t;
	int i, j;

	// Get a handle to the default PROV_RSA_AES provider (AES so we get SHA-256 support).
	// 2 passes in case we need to create a new container.
	r = CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_AES, CRYPT_NEWKEYSET | CRYPT_VERIFYCONTEXT);
	if (!r) {
		uprintf("PKI: Could not create the default key container: %s", WinPKIErrorString());
		goto out;
	}

	// Reverse the modulus bytes from openssl (and also remove the extra unwanted 0x00)
	assert(sizeof(rsa_pubkey_modulus) >= sizeof(pbMyPubKey.Modulus));
	for (i = 0; i < sizeof(pbMyPubKey.Modulus); i++)
		pbMyPubKey.Modulus[i] = rsa_pubkey_modulus[sizeof(rsa_pubkey_modulus) -1 - i];

	// Import our RSA public key so that the MS API can use it
	r = CryptImportKey(hProv, (BYTE*)&pbMyPubKey.BlobHeader, dwMyPubKeyLen, 0, 0, &hPubKey);
	if (!r) {
		uprintf("PKI: Could not import public key: %s", WinPKIErrorString());
		goto out;
	}

	// Create the hash object.
	r = CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash);
	if (!r) {
		uprintf("PKI: Could not create empty hash: %s", WinPKIErrorString());
		goto out;
	}

	// Compute the cryptographic hash of the buffer.
	r = CryptHashData(hHash, pbBuffer, dwBufferLen, 0);
	if (!r) {
		uprintf("PKI: Could not hash data: %s", WinPKIErrorString());
		goto out;
	}

	// Reverse the signature bytes
	for (i = 0, j = dwSigLen - 1; i < j; i++, j--) {
		t = pbSignature[i];
		pbSignature[i] = pbSignature[j];
		pbSignature[j] = t;
	}

	// Now that we have all of the public key, hash and signature data in a
	// format that Microsoft can handle, we can call CryptVerifySignature().
	r = CryptVerifySignature(hHash, pbSignature, dwSigLen, hPubKey, NULL, 0);
	if (!r) {
		// If the signature is invalid, clear the buffer so that
		// we don't keep potentially nasty stuff in memory.
		memset(pbBuffer, 0, dwBufferLen);
		uprintf("Signature validation failed: %s", WinPKIErrorString());
	}

out:
	if (hHash)
		CryptDestroyHash(hHash);
	if (hProv)
		CryptReleaseContext(hProv, 0);
	return r;
}

/*
 * Produce a formatted localized message.
 * Like printf, this call takes a variable number of argument, and uses
 * the message ID to identify the formatted message to use.
 * Uses a rolling list of buffers to allow concurrency
 * TODO: use dynamic realloc'd buffer in case LOC_MESSAGE_SIZE is not enough
 */



char* lmprintf(uint32_t msg_id, ...)
{
	static int buf_id = 0;
	static char buf[LOC_MESSAGE_NB][LOC_MESSAGE_SIZE];
	char* format = NULL;
	size_t pos = 0;
	va_list args;
	BOOL is_rtf = (msg_id & MSG_RTF);

	buf_id %= LOC_MESSAGE_NB;
	buf[buf_id][0] = 0;

	msg_id &= MSG_MASK;
	if ((msg_id >= MSG_000) && (msg_id < MSG_MAX)) {
		format = msg_table[msg_id - MSG_000];
	}

	if (format == NULL) {
		safe_sprintf(buf[buf_id], LOC_MESSAGE_SIZE - 1, "MSG_%03d UNTRANSLATED", msg_id - MSG_000);
	}
	else {
		if (right_to_left_mode && (msg_table != default_msg_table)) {
			if (is_rtf) {
				safe_strcpy(&buf[buf_id][pos], LOC_MESSAGE_SIZE - 1, "\\rtlch");
				pos += 6;
			}
			safe_strcpy(&buf[buf_id][pos], LOC_MESSAGE_SIZE - 1, RIGHT_TO_LEFT_EMBEDDING);
			pos += sizeof(RIGHT_TO_LEFT_EMBEDDING) - 1;
		}
		va_start(args, msg_id);
		safe_vsnprintf(&buf[buf_id][pos], LOC_MESSAGE_SIZE - 1 - 2 * pos, format, args);
		va_end(args);
		if (right_to_left_mode && (msg_table != default_msg_table)) {
			safe_strcat(buf[buf_id], LOC_MESSAGE_SIZE - 1, POP_DIRECTIONAL_FORMATTING);
			if (is_rtf)
				safe_strcat(buf[buf_id], LOC_MESSAGE_SIZE - 1, "\\ltrch");
		}
		buf[buf_id][LOC_MESSAGE_SIZE - 1] = '\0';
	}
	return buf[buf_id++];
}



/*
 * Internal recursive call for get_data_from_asn1(). Returns FALSE on error, TRUE otherwise.
 */
static BOOL get_data_from_asn1_internal(const uint8_t* buf, size_t buf_len, const void* oid,
	size_t oid_len, uint8_t asn1_type, void** data, size_t* data_len, BOOL* matched)
{
	size_t pos = 0, len, len_len, i;
	uint8_t tag;
	BOOL is_sequence, is_universal_tag;

	while (pos < buf_len) {
		is_sequence = buf[pos] & 0x20;
		is_universal_tag = ((buf[pos] & 0xC0) == 0x00);
		tag = buf[pos++] & 0x1F;
		if (tag == 0x1F) {
			uprintf("get_data_from_asn1: Long form tags are unsupported");
			return FALSE;
		}

		// Compute the length
		len = 0;
		len_len = 1;
		if ((is_universal_tag) && (tag == 0x05)) {	// ignore "NULL" tag
			pos++;
		}
		else {
			if (buf[pos] & 0x80) {
				len_len = buf[pos++] & 0x7F;
				// The data we're dealing with is not expected to ever be larger than 64K
				if (len_len > 2) {
					uprintf("get_data_from_asn1: Length fields larger than 2 bytes are unsupported");
					return FALSE;
				}
				for (i = 0; i < len_len; i++) {
					len <<= 8;
					len += buf[pos++];
				}
			}
			else {
				len = buf[pos++];
			}

			if (len > buf_len - pos) {
				uprintf("get_data_from_asn1: Overflow error (computed length %d is larger than remaining data)", len);
				return FALSE;
			}
		}

		if (len != 0) {
			if (is_sequence) {
				if (!get_data_from_asn1_internal(&buf[pos], len, oid, oid_len, asn1_type, data, data_len, matched))
					return FALSE;	// error
				if (*data != NULL)
					return TRUE;
			}
			else if (is_universal_tag) {	// Only process tags that belong to the UNIVERSAL class
			 // NB: 0x06 = "OID" tag
				if ((!*matched) && (tag == 0x06) && (len == oid_len) && (memcmp(&buf[pos], oid, oid_len) == 0)) {
					*matched = TRUE;
				}
				else if ((*matched) && (tag == asn1_type)) {
					*data_len = len;
					*data = (void*)& buf[pos];
					return TRUE;
				}
			}
			pos += len;
		}
	};

	return TRUE;
}

/*
 * Helper functions to convert an OID string to an OID byte array
 * Taken from from openpgp-oid.c
 */
static size_t make_flagged_int(unsigned long value, uint8_t* buf, size_t buf_len)
{
	BOOL more = FALSE;
	int shift;

	for (shift = 28; shift > 0; shift -= 7) {
		if (more || value >= ((unsigned long)1 << shift)) {
			buf[buf_len++] = (uint8_t)(0x80 | (value >> shift));
			value -= (value >> shift) << shift;
			more = TRUE;
		}
	}
	buf[buf_len++] = (uint8_t)value;
	return buf_len;
}

/*
 * Convert OID string 'oid_str' to an OID byte array of size 'ret_len'
 * The returned array must be freed by the caller.
 */
static uint8_t* oid_from_str(const char* oid_str, size_t* ret_len)
{
	uint8_t* oid = NULL;
	unsigned long val1 = 0, val;
	const char* endp;
	int arcno = 0;
	size_t oid_len = 0;

	if ((oid_str == NULL) || (oid_str[0] == 0))
		return NULL;

	// We can safely assume that the encoded OID is shorter than the string.
	oid = malloc(1 + strlen(oid_str) + 2);

	if (oid == NULL)
		return NULL;

	do {
		arcno++;
		val = strtoul(oid_str, (char**)& endp, 10);
		if (!isdigit(*oid_str) || !(*endp == '.' || !*endp))
			goto err;
		if (*endp == '.')
			oid_str = endp + 1;

		if (arcno == 1) {
			if (val > 2)
				break; // Not allowed, error caught below.
			val1 = val;
		}
		else if (arcno == 2) {
			// Need to combine the first two arcs in one byte.
			if (val1 < 2) {
				if (val > 39)
					goto err;
				oid[oid_len++] = (uint8_t)(val1 * 40 + val);
			}
			else {
				val += 80;
				oid_len = make_flagged_int(val, oid, oid_len);
			}
		}
		else {
			oid_len = make_flagged_int(val, oid, oid_len);
		}
	} while (*endp == '.');

	// It is not possible to encode only the first arc.
	if (arcno == 1 || oid_len < 2 || oid_len > 254)
		goto err;

	*ret_len = oid_len;
	return oid;

err:
	free(oid);
	return NULL;
}



void* get_data_from_asn1(const uint8_t* buf, size_t buf_len, const char* oid_str, uint8_t asn1_type, size_t* data_len)
{
	void* data = NULL;
	uint8_t* oid = NULL;
	size_t oid_len = 0;
	BOOL matched = ((oid_str == NULL) || (oid_str[0] == 0));

	if (buf_len >= 65536) {
		uprintf("get_data_from_asn1: Buffers larger than 64KB are not supported");
		return NULL;
	}

	if (!matched) {
		// We have an OID string to convert
		oid = oid_from_str(oid_str, &oid_len);
		if (oid == NULL) {
			uprintf("get_data_from_asn1: Could not convert OID string '%s'", oid_str);
			return NULL;
		}
	}

	// No need to check for the return value as data is always NULL on error
	get_data_from_asn1_internal(buf, buf_len, oid, oid_len, asn1_type, &data, data_len, &matched);
	free(oid);
	return data;
}


// Convert a YYYYMMDDHHMMSS UTC timestamp to a more human readable version
char* TimestampToHumanReadable(uint64_t ts)
{
	uint64_t rem = ts, divisor = 10000000000ULL;
	uint16_t data[6];
	int i;
	static char str[64];

	for (i = 0; i < 6; i++) {
		data[i] = (uint16_t)((divisor == 0) ? rem : (rem / divisor));
		rem %= divisor;
		divisor /= 100ULL;
	}
	static_sprintf(str, "%04d.%02d.%02d %02d:%02d:%02d (UTC)", data[0], data[1], data[2], data[3], data[4], data[5]);
	return str;
}

// Display an hex dump of buffer 'buf'
void DumpBufferHex(void* buf, size_t size)
{
	unsigned char* buffer = (unsigned char*)buf;
	size_t i, j, k;
	char line[80] = "";

	for (i = 0; i < size; i += 16) {
		if (i != 0)
			uprintf("%s\n", line);
		line[0] = 0;
		sprintf(&line[strlen(line)], "  %08x  ", (unsigned int)i);
		for (j = 0, k = 0; k < 16; j++, k++) {
			if (i + j < size) {
				sprintf(&line[strlen(line)], "%02x", buffer[i + j]);
			}
			else {
				sprintf(&line[strlen(line)], "  ");
			}
			sprintf(&line[strlen(line)], " ");
		}
		sprintf(&line[strlen(line)], " ");
		for (j = 0, k = 0; k < 16; j++, k++) {
			if (i + j < size) {
				if ((buffer[i + j] < 32) || (buffer[i + j] > 126)) {
					sprintf(&line[strlen(line)], ".");
				}
				else {
					sprintf_s(&line[strlen(line)], "%c", buffer[i + j]);
				}
			}
		}
	}
	uprintf("%s\n", line);
}

