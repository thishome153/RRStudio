// FormSigner2.cpp : main project file.

#include "stdafx.h"
#include "FSignerMainForm.h"

using namespace FormSigner2;

char *inFileName= "empty"; //Глобальная строка?
int iscmdLine=0;

[STAThreadAttribute]
int main(array<System::String ^> ^args)
{
	// Enabling Windows XP visual effects before any controls are created
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false); 

	// Create the main window and run it
	if (args->Length > 0) 
	{

		for (int i=0; i<= args->Length; i++)
		{
			int c=0;// = args[i]->;
			switch (c) {
		  case 's' : iscmdLine=1;
//		  case 'i' : inFileName =args[i+1];
		}
		}
	}
		iscmdLine =1;
	Application::Run(gcnew Form1());
	return 0;
}
