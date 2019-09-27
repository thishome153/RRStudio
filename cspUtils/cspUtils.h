// cspUtils.h
#include <windows.h>  //типы основные

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
	/// Инкапсулирует методы работы с API криптопровайдера CryptoPro.
	/// Требует установленнoго CSP (cades.dll) Runtime
	/// </summary>
	public ref class CadesWrapper
	{



	public:
		CadesWrapper();
		//PCCERT_CONTEXT_WR GetCert_Context();
		/// <summary>
		/// Отображение системного окна свойств подписи
		/// </summary>
		/// <param name="FileSign">Имя файла sig</param>
		/// <param name="Parent">Handle окна родителя</param>
		/// <returns></returns>
	public:	 int  DisplaySig(System::String ^ FileSign, System::IntPtr Parent);
			 //
	public:	System::String ^        DisplayCertInfo(System::String ^  SubjectName);
	public: List<System::String ^> ^ GetCertificates();

	public: System::String ^		GetCertificatSerialNumber(System::String ^ SubjectName);
	public:	System::Int16			SignFile(System::String ^ filename, System::String ^ SubjectName); // подпись чисто по WinCrypt
	public:	System::Int16			Sign_GOST(System::String ^ filename, System::String ^ SubjectName); // подпись по CADES, с расчетом hash
	public:	System::Int16			Sign_GOST_2012(System::String^ filename, System::String^ SubjectName); // подпись по CADES, с расчетом hash
	public: System::Int16			Sign_Example1(System::String ^ filename, System::String ^ SubjectName);
	public: System::Int16			Sign_Examples(System::String ^ filename, System::String ^ SubjectName);
	//private:PCCERT_CONTEXT			GetCertificat(System::String ^ SubjectName); //Чтение сертификата из сист, справочника сертификатов пользователя'MY'
	};




}
