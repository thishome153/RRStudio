// cspUtils.h
#include <windows.h>  //���� ��������

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace cspUtils {


	//  Certificate context struct CLR wrapper:
	public struct PCCERT_CONTEXT_WR {
		int id;
		PCCERT_CONTEXT* API;
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
		/// <summary>
		/// ����������� ���������� ���� ������� �������
		/// </summary>
		/// <param name="FileSign">��� ����� sig</param>
		/// <param name="Parent">Handle ���� ��������</param>
		/// <returns></returns>
	public:	 int  DisplaySig(System::String ^ FileSign, System::IntPtr Parent);
			 //
	public:	System::String ^        DisplayCertInfo(System::String ^  SubjectName);
	public: List<System::String ^> ^ GetCertificates();

	public: System::String ^		GetCertificatSerialNumber(System::String ^ SubjectName);
	public:	System::Int16			SignFile(System::String ^ filename, System::String ^ SubjectName); // ������� ����� �� WinCrypt
	public:	System::Int16			Sign_GOST(System::String ^ filename, System::String ^ SubjectName); // ������� �� CADES, � �������� hash
	public:	System::Int16			Sign_GOST_2012(System::String^ filename, System::String^ SubjectName); // ������� �� CADES, � �������� hash
	public: System::Int16			Sign_Example1(System::String ^ filename, System::String ^ SubjectName);
	public: System::Int16			Sign_Examples(System::String ^ filename, System::String ^ SubjectName);
	//private:PCCERT_CONTEXT			GetCertificat(System::String ^ SubjectName); //������ ����������� �� ����, ����������� ������������ ������������'MY'
	};




}
