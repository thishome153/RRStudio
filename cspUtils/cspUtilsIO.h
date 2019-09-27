#pragma once
#ifndef _cspUtilsIO_h_INCLUDED // типа защита от множественного включения
#define _cspUtilsIO_h_INCLUDED

#include <windows.h>  //типы основные

namespace cspUtils {

	namespace IO {
		//int get_file_data_pointer(LPCSTR infile, size_t *len, LPVOID *buffer);
		int read_file(LPCSTR infile, size_t * len, LPVOID * buffer);
		int write_file(const char *file, long len, const unsigned char *buffer);
	}
}
#endif
