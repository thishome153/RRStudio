#include "Stdafx.h"
#include "cspUtilsIO.h"
#include <vector>
//#include <ctime>

namespace cspUtils {

	namespace IO {
		/*----------------------------------------------------------------------*/
		/* Чтение файла в буфер - get_file_data_pointer*/
		int read_file(LPCSTR infile, size_t *len, LPVOID *buffer)
		{
			DWORD dwSize;
			HANDLE hFile;
			HANDLE hMap;
			//unsigned char *pStart;
			LPVOID pStart;

			if (!infile || !len || !buffer) {
				//fprintf (stderr, "Invalid argument specified\n");
				return 0;
			}
			// Используем ANSI версию
			hFile = CreateFileA(infile, GENERIC_READ, FILE_SHARE_READ,
				NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_SEQUENTIAL_SCAN,
				NULL);
			if (INVALID_HANDLE_VALUE == hFile) {
				//fprintf (stderr, "Unable to open file\n");
				return 0;
			}
			dwSize = GetFileSize(hFile, NULL); // test file= 238 bytes
			hMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, 0, NULL);
			if (NULL == hMap) {
				//fprintf (stderr, "Unable to create map file\n");
				CloseHandle(hFile);
				return 0;
			}
			pStart = MapViewOfFile(hMap, FILE_MAP_READ, 0, 0, 0);
			if (NULL == pStart) {
				//fprintf (stderr, "Unable to map file into memory\n");
				CloseHandle(hMap);
				CloseHandle(hFile);
				return 0;
			}
			CloseHandle(hMap);
			CloseHandle(hFile);
			*len = dwSize;
			*buffer = pStart;
			return 1;
		}
		// чтения в vector. Вычитывается весь файл целиком.
		int read_file_to_vector(const char* filename, std::vector<unsigned char>& buffer)
		{
			enum { bytesSize = 512 };

			unsigned long bytesRead(1);
			char buf[bytesSize];

			FILE* f = fopen(filename, "r+b");

			if (!f)
			{
				//cout << "Opening file " << filename << " failed" << endl;
				return -1;
			}

			while (!feof(f))
			{
				bytesRead = (unsigned long)fread(buf, 1, bytesSize, f);

				if (bytesSize != bytesRead && ferror(f))
					return -1;

				std::copy(buf, buf + bytesRead, back_inserter(buffer));
			}
			fclose(f);

			return 0;
		}


		/*----------------------------------------------------------------------*/
		/* Запись в файл*/
		int write_file(const char *file, long len, const unsigned char *buffer)
		{
			FILE *f = NULL;
			int ret = 0;

			f = fopen(file, "wb");
			if (!f) {
				//fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,"Cannot open file for writing\n");
				goto err;
			}
			if (1 != fwrite(buffer, len, 1, f)) {
				//fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,   "Cannot write to file\n");
				goto err;
			}
			if (ferror(f)) {
				//fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file, "Write to file error (ferror)\n");
				goto err;
			}
			if (fclose(f)) {
				//fprintf (stderr, __FILE__":%d:%s:%s", __LINE__, file,   "Write to file error (fclose)\n");
				goto err;
			}
			f = NULL;
			ret = 1;
		err:
			if (f) fclose(f);
			return ret;
		}

		LPWSTR StrTime(FILETIME ft)
		{
			SYSTEMTIME st;
			LPWSTR szLocalDate = new WCHAR  [255];
			WCHAR  szLocalTime[255];

			FileTimeToLocalFileTime(&ft, &ft);
			FileTimeToSystemTime(&ft, &st);
			/*  Parameters:
			    _In_ LCID Locale,
				_In_ DWORD dwFlags,
				_In_opt_ CONST SYSTEMTIME* lpTime,
				_In_opt_ LPCWSTR lpFormat,
				_Out_writes_opt_(cchTime) LPWSTR lpTimeStr,
				_In_ int cchTime
				*/
			GetDateFormat(LOCALE_USER_DEFAULT, DATE_LONGDATE, &st, NULL, szLocalDate, 255);
			GetTimeFormat(LOCALE_USER_DEFAULT, 0, &st, NULL, szLocalTime, 255);
			return szLocalDate;
		}

	}
	
}