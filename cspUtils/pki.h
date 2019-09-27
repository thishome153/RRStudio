
#include <stdio.h>
#include <stdint.h>
#include <inttypes.h>
#include "resource.h"


// Number of concurrent localization messages (i.e. messages we can concurrently
// reference at the same time). Must be a power of 2.
#define LOC_MESSAGE_NB          32
#define LOC_MESSAGE_SIZE        2048

#define MSG_RTF                 0x10000000
#define MSG_MASK                0x0FFFFFFF

/* Message table */
char* default_msg_table[MSG_MAX - MSG_000] = { "%s", 0 };
char* current_msg_table[MSG_MAX - MSG_000] = { "%s", 0 };
char** msg_table = NULL;

#define MB_IS_RTL                   (right_to_left_mode?MB_RTLREADING|MB_RIGHT:0)

// Yes, there exist characters between these seemingly empty quotes!
#define POP_DIRECTIONAL_FORMATTING  "‬"
#define LEFT_TO_RIGHT_MARK          "‎"
#define RIGHT_TO_LEFT_MARK          "‏"
#define LEFT_TO_RIGHT_EMBEDDING     "‪"
#define RIGHT_TO_LEFT_EMBEDDING     "‫"


/* Custom application errors */
#define FAC(f)                         (f<<16)
#define APPERR(err)                    (APPLICATION_ERROR_MASK|err)
#define ERROR_INCOMPATIBLE_FS          0x1201
#define ERROR_CANT_QUICK_FORMAT        0x1202
#define ERROR_INVALID_CLUSTER_SIZE     0x1203
#define ERROR_INVALID_VOLUME_SIZE      0x1204
#define ERROR_CANT_START_THREAD        0x1205
#define ERROR_BADBLOCKS_FAILURE        0x1206
#define ERROR_ISO_SCAN                 0x1207
#define ERROR_ISO_EXTRACT              0x1208
#define ERROR_CANT_REMOUNT_VOLUME      0x1209
#define ERROR_CANT_PATCH               0x120A
#define ERROR_CANT_ASSIGN_LETTER       0x120B
#define ERROR_CANT_MOUNT_VOLUME        0x120C
#define ERROR_BAD_SIGNATURE            0x120D


extern void _uprintf(const char* format, ...);
extern void _uprintfs(const char* str);
#define uprintf(...) _uprintf(__VA_ARGS__)
#define uprintfs(s) _uprintfs(s)
#define vuprintf(...) do { if (verbose) _uprintf(__VA_ARGS__); } while(0)
#define vvuprintf(...) do { if (verbose > 1) _uprintf(__VA_ARGS__); } while(0)
#define suprintf(...) do { if (!bSilent) _uprintf(__VA_ARGS__); } while(0)
#define uuprintf(...) do { if (usb_debug) _uprintf(__VA_ARGS__); } while(0)
#define ubprintf(...) do { safe_sprintf(&ubuffer[ubuffer_pos], UBUFFER_SIZE - ubuffer_pos - 2, __VA_ARGS__) 
#define safe_sprintf(dst, count, ...) do {_snprintf(dst, count, __VA_ARGS__); (dst)[(count)-1] = 0; } while(0)
#define static_sprintf(dst, ...) safe_sprintf(dst, sizeof(dst), __VA_ARGS__)

#define safe_strcp(dst, dst_max, src, count) do {memcpy(dst, src, safe_min(count, dst_max)); \
	((char*)dst)[safe_min(count, dst_max)-1] = 0;} while(0)
#define safe_strcpy(dst, dst_max, src) safe_strcp(dst, dst_max, src, safe_strlen(src)+1)
#define static_strcpy(dst, src) safe_strcpy(dst, sizeof(dst), src)
#define safe_min(a, b) min((size_t)(a), (size_t)(b))
#define safe_strcpy(dst, dst_max, src) safe_strcp(dst, dst_max, src, safe_strlen(src)+1)
#define safe_strncat(dst, dst_max, src, count) strncat(dst, src, safe_min(count, dst_max - safe_strlen(dst) - 1))
#define safe_strcat(dst, dst_max, src) safe_strncat(dst, dst_max, src, safe_strlen(src)+1)
#define static_strcat(dst, src) safe_strcat(dst, sizeof(dst), src)
#define safe_free(p) do {free((void*)p); p = NULL;} while(0)
#define safe_strlen(str) ((((char*)str)==NULL)?0:strlen(str))

#if defined(_MSC_VER)
#define safe_vsnprintf(buf, size, format, arg) _vsnprintf_s(buf, size, _TRUNCATE, format, arg)
#else
#define safe_vsnprintf vsnprintf
#endif


/* for C code not need do declare function */

const char* WinPKIErrorString(void);
const char* WindowsErrorString(void);
char* lmprintf(uint32_t msg_id, ...);
uint64_t GetSignatureTimeStamp(const char* path);
LONG ValidateSignature(HWND hDlg, const char* path);
char* TimestampToHumanReadable(uint64_t ts);
void* get_data_from_asn1(const uint8_t* buf, size_t buf_len, const char* oid_str, uint8_t asn1_type, size_t* data_len);
void DumpBufferHex(void* buf, size_t size);
