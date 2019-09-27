//2019 Fixosoft CSP Wrapper
// ����������� �����
#ifndef _csp_Wrapper_h_INCLUDED // ���� ������ �� �������������� ���������
#define _csp_Wrapper_h_INCLUDED

#include <windows.h>  //���� ��������

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace cspUtils {

#pragma managed
	//  Certificate context struct CLR wrapper:
	public ref struct PCCERT_CONTEXT_CLR {
	public: PCCERT_CONTEXT* Item;
	public:	int id;
	};

	public ref class PCCERT_CONTEXT_WR {
	public: int id;

		
	public: PCCERT_CONTEXT* Certificat;

	public:	property  PCCERT_CONTEXT* Native
			{
			public: PCCERT_CONTEXT* get()
			{
				return Certificat;
			}

			public: void set(PCCERT_CONTEXT *value)
			{
				this->Certificat = value;
			}
			}

	public: PCCERT_CONTEXT_WR();
	};



	/// <summary>
	/// CadesWrapper - GOST CSP Provider wrapper class for .NET. of CADES CSP
	/// ������������� ������ ������ � API ���������������� CryptoPro.
	/// ������� �����������o�� CSP (cades.dll) Runtime
	/// </summary>
	public ref class CadesWrapper
	{



	public:
		CadesWrapper();
		//PCCERT_CONTEXT_WR GetCert_Context();

		// ����������� ���������� ���� ������� �������
	public:	 int  DisplaySig(System::String^ FileSign, System::IntPtr Parent);
			 //
	public:	System::String^ DisplayCertInfo(System::String^ SubjectName);
	public: List<System::String^>^ GetCertificates();

			/// <summary>
			/// Select subject certificate by his name:
			///<para>Subject name </para>
			/// </summary>
			// Get cert
	public:	PCCERT_CONTEXT       GetCertificat(System::String^ SubjectName);
	public:	PCCERT_CONTEXT_CLR^  GetCertificatCLR(System::String^ SubjectName);
	public:	PCCERT_CONTEXT_WR^   GetCertificatWrapped(System::String^ SubjectName);

	public: System::String^ GetCertificatSerialNumber(System::String^ SubjectName);
	public:	System::Int16			SignFileWinCrypt(System::String^ filename, System::String^ SubjectName); // ������� ����� �� WinCrypt
	//public:	System::Int16			SignFile(System::String^ filename, System::String^ SubjectName); // ������� ����� �� WinCrypt
	public:	System::Int16			Sign_GOST(System::String^ filename, System::String^ SubjectName); // ������� �� CADES, � �������� hash
	public:	System::Int16			Sign_GOST_2012(System::String^ filename, System::String^ SubjectName); // ������� �� CADES, � �������� hash
	public: System::Int16			Sign_Example1(System::String^ filename, System::String^ SubjectName);
	public: System::Int16			Sign_Examples(System::String^ filename, System::String^ SubjectName);
			//private:PCCERT_CONTEXT			GetCertificat(System::String ^ SubjectName); //������ ����������� �� ����, ����������� ������������ ������������'MY'
	};


#endif /* _csp_Wrapper_h_INCLUDE */



}
