#pragma once
#include "cades.h"
#include "MyStrMarshal.h" // fixosoft String Collection Utils
#include "MyNETWrappers.h"
#include "SignerUtils.h"
#include "cspWrapper.h"
#include <stdio.h>
#include <vector>
#include "FS2_About.h"


namespace FormSigner2 {

	using namespace System;
	using namespace System::IO;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;



	/// <summary>
	/// Summary for Form1
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class Form1 : public System::Windows::Forms::Form
	{
	public:
		Form1(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
			this->cw = gcnew cspUtils::CadesWrapper();
			// TODO - remove link to cadesSharp assembly:	

		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}

	public:	cspUtils::CadesWrapper^ cw;

	private: System::Windows::Forms::MenuStrip^ menuStrip1;
	private: System::Windows::Forms::ToolStripMenuItem^ Ù‡ÈÎToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ ‚˚ıÓ‰ToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ toolStripMenuItem1;
	private: System::Windows::Forms::ToolStripMenuItem^ ÔÓÏÓ˘¸ToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ ÓœÓ„‡ÏÏÂToolStripMenuItem;
	private: System::String^ FileName;
	private: System::Windows::Forms::OpenFileDialog^ openFileDialog1;
	private: System::Windows::Forms::TextBox^ textBox1;


	private: System::Windows::Forms::StatusStrip^ statusStrip1;
	private: System::Windows::Forms::ToolStripStatusLabel^ toolStripStatusLabel1;
	private: System::Windows::Forms::ToolStripStatusLabel^ toolStripStatusLabel2;



	private: System::Windows::Forms::ListView^ listView_Certs;
	private: System::Windows::Forms::ColumnHeader^ columnHeader1;
	private: System::Windows::Forms::ColumnHeader^ columnHeader2;
	private: System::Windows::Forms::Panel^ panel1;
	private: System::Windows::Forms::Button^ button3;
	private: System::Windows::Forms::Button^ button2;
	private: System::Windows::Forms::ToolStrip^ toolStrip1;
	private: System::Windows::Forms::ToolStripButton^ toolStripButton1;
	private: System::Windows::Forms::Button^ button1;
	protected:

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container^ components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			System::Windows::Forms::ListViewItem^ listViewItem1 = (gcnew System::Windows::Forms::ListViewItem(gcnew cli::array< System::String^  >(2) {
				L"Item1",
					L"Sub1"
			}, -1));
			this->menuStrip1 = (gcnew System::Windows::Forms::MenuStrip());
			this->Ù‡ÈÎToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripMenuItem1 = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->‚˚ıÓ‰ToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ÔÓÏÓ˘¸ToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->textBox1 = (gcnew System::Windows::Forms::TextBox());
			this->statusStrip1 = (gcnew System::Windows::Forms::StatusStrip());
			this->toolStripStatusLabel1 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->toolStripStatusLabel2 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->listView_Certs = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->panel1 = (gcnew System::Windows::Forms::Panel());
			this->button3 = (gcnew System::Windows::Forms::Button());
			this->button2 = (gcnew System::Windows::Forms::Button());
			this->toolStrip1 = (gcnew System::Windows::Forms::ToolStrip());
			this->toolStripButton1 = (gcnew System::Windows::Forms::ToolStripButton());
			this->button1 = (gcnew System::Windows::Forms::Button());
			this->menuStrip1->SuspendLayout();
			this->statusStrip1->SuspendLayout();
			this->panel1->SuspendLayout();
			this->toolStrip1->SuspendLayout();
			this->SuspendLayout();
			// 
			// menuStrip1
			// 
			this->menuStrip1->Font = (gcnew System::Drawing::Font(L"Tahoma", 10, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->menuStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->Ù‡ÈÎToolStripMenuItem,
					this->ÔÓÏÓ˘¸ToolStripMenuItem
			});
			this->menuStrip1->Location = System::Drawing::Point(0, 0);
			this->menuStrip1->Name = L"menuStrip1";
			this->menuStrip1->Size = System::Drawing::Size(687, 25);
			this->menuStrip1->TabIndex = 3;
			this->menuStrip1->Text = L"menuStrip1";
			// 
			// Ù‡ÈÎToolStripMenuItem
			// 
			this->Ù‡ÈÎToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->toolStripMenuItem1,
					this->‚˚ıÓ‰ToolStripMenuItem
			});
			this->Ù‡ÈÎToolStripMenuItem->Name = L"Ù‡ÈÎToolStripMenuItem";
			this->Ù‡ÈÎToolStripMenuItem->Size = System::Drawing::Size(53, 21);
			this->Ù‡ÈÎToolStripMenuItem->Text = L"‘‡ÈÎ";
			// 
			// toolStripMenuItem1
			// 
			this->toolStripMenuItem1->Name = L"toolStripMenuItem1";
			this->toolStripMenuItem1->Size = System::Drawing::Size(150, 22);
			this->toolStripMenuItem1->Text = L"ŒÚÍ˚Ú¸....";
			this->toolStripMenuItem1->Click += gcnew System::EventHandler(this, &Form1::toolStripMenuItem1_Click);
			// 
			// ‚˚ıÓ‰ToolStripMenuItem
			// 
			this->‚˚ıÓ‰ToolStripMenuItem->Name = L"‚˚ıÓ‰ToolStripMenuItem";
			this->‚˚ıÓ‰ToolStripMenuItem->Size = System::Drawing::Size(150, 22);
			this->‚˚ıÓ‰ToolStripMenuItem->Text = L"¬˚ıÓ‰";
			this->‚˚ıÓ‰ToolStripMenuItem->Click += gcnew System::EventHandler(this, &Form1::‚˚ıÓ‰ToolStripMenuItem_Click);
			// 
			// ÔÓÏÓ˘¸ToolStripMenuItem
			// 
			this->ÔÓÏÓ˘¸ToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(1) { this->ÓœÓ„‡ÏÏÂToolStripMenuItem });
			this->ÔÓÏÓ˘¸ToolStripMenuItem->Name = L"ÔÓÏÓ˘¸ToolStripMenuItem";
			this->ÔÓÏÓ˘¸ToolStripMenuItem->Size = System::Drawing::Size(72, 21);
			this->ÔÓÏÓ˘¸ToolStripMenuItem->Text = L"œÓÏÓ˘¸";
			// 
			// ÓœÓ„‡ÏÏÂToolStripMenuItem
			// 
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem->Name = L"ÓœÓ„‡ÏÏÂToolStripMenuItem";
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem->Size = System::Drawing::Size(160, 22);
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem->Text = L"Œ ÔÓ„‡ÏÏÂ";
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem->Click += gcnew System::EventHandler(this, &Form1::ÓœÓ„‡ÏÏÂToolStripMenuItem_Click);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->FileName = L"openFileDialog1";
			// 
			// textBox1
			// 
			this->textBox1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom)
				| System::Windows::Forms::AnchorStyles::Left)
				| System::Windows::Forms::AnchorStyles::Right));
			this->textBox1->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 11, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(204)));
			this->textBox1->Location = System::Drawing::Point(0, 258);
			this->textBox1->Multiline = true;
			this->textBox1->Name = L"textBox1";
			this->textBox1->Size = System::Drawing::Size(536, 203);
			this->textBox1->TabIndex = 0;
			// 
			// statusStrip1
			// 
			this->statusStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->toolStripStatusLabel1,
					this->toolStripStatusLabel2
			});
			this->statusStrip1->Location = System::Drawing::Point(0, 461);
			this->statusStrip1->Name = L"statusStrip1";
			this->statusStrip1->Size = System::Drawing::Size(687, 22);
			this->statusStrip1->TabIndex = 6;
			this->statusStrip1->Text = L"statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this->toolStripStatusLabel1->AutoSize = false;
			this->toolStripStatusLabel1->BorderSides = static_cast<System::Windows::Forms::ToolStripStatusLabelBorderSides>((((System::Windows::Forms::ToolStripStatusLabelBorderSides::Left | System::Windows::Forms::ToolStripStatusLabelBorderSides::Top)
				| System::Windows::Forms::ToolStripStatusLabelBorderSides::Right)
				| System::Windows::Forms::ToolStripStatusLabelBorderSides::Bottom));
			this->toolStripStatusLabel1->BorderStyle = System::Windows::Forms::Border3DStyle::SunkenOuter;
			this->toolStripStatusLabel1->ForeColor = System::Drawing::SystemColors::Desktop;
			this->toolStripStatusLabel1->Name = L"toolStripStatusLabel1";
			this->toolStripStatusLabel1->Size = System::Drawing::Size(320, 17);
			this->toolStripStatusLabel1->Text = L"toolStripStatusLabel1";
			// 
			// toolStripStatusLabel2
			// 
			this->toolStripStatusLabel2->AutoSize = false;
			this->toolStripStatusLabel2->BorderSides = static_cast<System::Windows::Forms::ToolStripStatusLabelBorderSides>((((System::Windows::Forms::ToolStripStatusLabelBorderSides::Left | System::Windows::Forms::ToolStripStatusLabelBorderSides::Top)
				| System::Windows::Forms::ToolStripStatusLabelBorderSides::Right)
				| System::Windows::Forms::ToolStripStatusLabelBorderSides::Bottom));
			this->toolStripStatusLabel2->BorderStyle = System::Windows::Forms::Border3DStyle::SunkenOuter;
			this->toolStripStatusLabel2->ForeColor = System::Drawing::SystemColors::Desktop;
			this->toolStripStatusLabel2->Name = L"toolStripStatusLabel2";
			this->toolStripStatusLabel2->Size = System::Drawing::Size(320, 17);
			this->toolStripStatusLabel2->Text = L"toolStripStatusLabel1";
			// 
			// listView_Certs
			// 
			this->listView_Certs->BackColor = System::Drawing::SystemColors::Info;
			this->listView_Certs->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(2) {
				this->columnHeader1,
					this->columnHeader2
			});
			this->listView_Certs->Dock = System::Windows::Forms::DockStyle::Top;
			this->listView_Certs->ForeColor = System::Drawing::SystemColors::MenuHighlight;
			this->listView_Certs->FullRowSelect = true;
			this->listView_Certs->GridLines = true;
			this->listView_Certs->HideSelection = false;
			listViewItem1->Tag = L"Tag0";
			this->listView_Certs->Items->AddRange(gcnew cli::array< System::Windows::Forms::ListViewItem^  >(1) { listViewItem1 });
			this->listView_Certs->Location = System::Drawing::Point(0, 50);
			this->listView_Certs->Name = L"listView_Certs";
			this->listView_Certs->Size = System::Drawing::Size(536, 202);
			this->listView_Certs->TabIndex = 10;
			this->listView_Certs->UseCompatibleStateImageBehavior = false;
			this->listView_Certs->View = System::Windows::Forms::View::Details;
			this->listView_Certs->Click += gcnew System::EventHandler(this, &Form1::ListView_Certs_Click);
			this->listView_Certs->KeyUp += gcnew System::Windows::Forms::KeyEventHandler(this, &Form1::ListView_Certs_KeyUp);
			// 
			// columnHeader1
			// 
			this->columnHeader1->Text = L"Subject";
			this->columnHeader1->Width = 139;
			// 
			// columnHeader2
			// 
			this->columnHeader2->Text = L"Valid date";
			this->columnHeader2->Width = 145;
			// 
			// panel1
			// 
			this->panel1->Controls->Add(this->button1);
			this->panel1->Controls->Add(this->button3);
			this->panel1->Controls->Add(this->button2);
			this->panel1->Dock = System::Windows::Forms::DockStyle::Right;
			this->panel1->Location = System::Drawing::Point(536, 50);
			this->panel1->Name = L"panel1";
			this->panel1->Size = System::Drawing::Size(151, 411);
			this->panel1->TabIndex = 11;
			// 
			// button3
			// 
			this->button3->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button3->FlatAppearance->BorderSize = 2;
			this->button3->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button3->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button3->Location = System::Drawing::Point(12, 131);
			this->button3->Name = L"button3";
			this->button3->Size = System::Drawing::Size(126, 42);
			this->button3->TabIndex = 11;
			this->button3->Text = L"œÓ‰ÔËÒ‡Ú¸  √Œ—“ 34.11-94 2012";
			this->button3->UseVisualStyleBackColor = true;
			this->button3->Click += gcnew System::EventHandler(this, &Form1::button3_Click);
			// 
			// button2
			// 
			this->button2->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button2->FlatAppearance->BorderSize = 2;
			this->button2->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button2->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button2->Location = System::Drawing::Point(12, 19);
			this->button2->Name = L"button2";
			this->button2->Size = System::Drawing::Size(126, 42);
			this->button2->TabIndex = 10;
			this->button2->Text = L"œÓ‰ÔËÒ‡Ú¸\r\n";
			this->button2->UseVisualStyleBackColor = true;
			this->button2->Click += gcnew System::EventHandler(this, &Form1::button2_Click);
			// 
			// toolStrip1
			// 
			this->toolStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(1) { this->toolStripButton1 });
			this->toolStrip1->Location = System::Drawing::Point(0, 25);
			this->toolStrip1->Name = L"toolStrip1";
			this->toolStrip1->Size = System::Drawing::Size(687, 25);
			this->toolStrip1->TabIndex = 13;
			this->toolStrip1->Text = L"toolStrip1";
			// 
			// toolStripButton1
			// 
			this->toolStripButton1->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton1->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->toolStripButton1->Name = L"toolStripButton1";
			this->toolStripButton1->Size = System::Drawing::Size(23, 22);
			this->toolStripButton1->Text = L"Sig";
			this->toolStripButton1->ToolTipText = L"Open signature";
			this->toolStripButton1->Click += gcnew System::EventHandler(this, &Form1::ToolStripButton1_Click);
			// 
			// button1
			// 
			this->button1->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button1->FlatAppearance->BorderSize = 2;
			this->button1->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button1->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button1->Location = System::Drawing::Point(12, 184);
			this->button1->Name = L"button1";
			this->button1->Size = System::Drawing::Size(126, 65);
			this->button1->TabIndex = 12;
			this->button1->Text = L"œÓ‰ÔËÒ‡Ú¸  √Œ—“ 34.11-94 2012\r\nwith apilite.dll\r\n";
			this->button1->UseVisualStyleBackColor = true;
			this->button1->Click += gcnew System::EventHandler(this, &Form1::Button1_Click_2);
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(687, 483);
			this->Controls->Add(this->listView_Certs);
			this->Controls->Add(this->panel1);
			this->Controls->Add(this->statusStrip1);
			this->Controls->Add(this->textBox1);
			this->Controls->Add(this->toolStrip1);
			this->Controls->Add(this->menuStrip1);
			this->MainMenuStrip = this->menuStrip1;
			this->MinimumSize = System::Drawing::Size(461, 431);
			this->Name = L"Form1";
			this->Text = L"—Â‰ÒÚ‚Ó ‡·ÓÚ˚ Ò ›÷œ. @2014-19";
			this->Load += gcnew System::EventHandler(this, &Form1::Form1_Load);
			this->menuStrip1->ResumeLayout(false);
			this->menuStrip1->PerformLayout();
			this->statusStrip1->ResumeLayout(false);
			this->statusStrip1->PerformLayout();
			this->panel1->ResumeLayout(false);
			this->toolStrip1->ResumeLayout(false);
			this->toolStrip1->PerformLayout();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	private: System::Void button1_Click(System::Object^ sender, System::EventArgs^ e) {


	}

	private: System::Int16 CheckApiLite(char* FileName, PCCERT_CONTEXT SignerCertificat) {

		LPCWSTR dllName_ = L"cspApiLite.dll";
		LPCSTR SignFile_api_Lite = "SignFile_api_Lite";
		typedef  int (*LP_func_SignFile_api_Lite) (char* FileName, PCCERT_CONTEXT SignerCertificat);	//ÕÓ‚˚È ÚËÔ - ÛÍ‡Á‡ÚÂÎ¸ Ì‡ ÙÛÌÍˆË˛	 
		HMODULE hMod_L = LoadLibrary(dllName_);
		   
		if (hMod_L != NULL) //0x00000000 < NULL > if not found dll
		{
			LP_func_SignFile_api_Lite func = (LP_func_SignFile_api_Lite)GetProcAddress(hMod_L, SignFile_api_Lite);
			if (func != NULL)
			{
				//PCCERT_CONTEXT cert = NULL;
				int ret = func(FileName, SignerCertificat);
				return ret;
			}
			else return 503;//service out
		}
		else
			return 404; //notFound
	}



	private: System::Void ListCertificates() {

		cspUtils::CadesWrapper^ cw;
		listView_Certs->Items->Clear();
		List<cspUtils::CertInfo^>^ Certs = cw->GetCertificatesObj();
		for each (cspUtils::CertInfo ^ ci  in Certs)
		{
			ListViewItem^ Cert_Item = gcnew ListViewItem(ci->SubjectName);
			Cert_Item->Tag = ci->SerialNumber;
			Cert_Item->SubItems->Add(ci->ValidNotAfter);
			void* blob = ci->Serial;
			listView_Certs->Items->Add(Cert_Item);
		}
	}


	private: System::Void ‚˚ıÓ‰ToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		this->Close();
	}

	private: System::Void toolStripMenuItem1_Click(System::Object^ sender, System::EventArgs^ e) {
		ViewCMSInfo("*.sig");
	}

	private: System::Void Form1_Paint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e) {

	}

	private: System::Void ÓœÓ„‡ÏÏÂToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		FS2_About^ FormAbout = (gcnew FS2_About());
		FormAbout->ShowDialog();
		//FormAbout->freeDispose();
	}

			 // œÓ‰ÔËÒ‡Ú¸ Ù‡ÈÎ  direct in signerUtils.h
	public: System::Void My_UI_SignFile(String^ Serial)
	{
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;


			PCCERT_CONTEXT ret = cw->GetCertificatbySN(Serial);
			//PCCERT_CONTEXT ret = SignerUtils::wincrypt::GetCertificat((string)Certs_listBox->SelectedItem);  // direct call from api
			if (ret)
			{
				int funcRes = cw->SignFileWinCrypt(this->FileName, Serial);
				//SignerUtils::wincrypt::SignFileWinCrypt(this->FileName, ret);
				if (funcRes > 1)
				{
					toolStripStatusLabel2->Text = "Wincrypt Error " + funcRes.ToString();
					int err = GetLastError(); // system error stack
					LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
					textBox1->Text = "Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage);
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
				}
			}
		}
	}





			// œÓ‰ÔËÒ‡Ú¸ Ù‡ÈÎ
	public: System::Void My_UI_SignFile_CSP_Utils_GOST2012(String^ Subject)
	{
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;

			PCCERT_CONTEXT ret = cw->GetCertificatbySN(Subject);
			if (ret)
			{
				int funcRes = cw->Sign_GOST_2012(this->FileName, Subject);

				if (funcRes > 1)
				{
					toolStripStatusLabel2->Text = "cades error " + funcRes.ToString();
					int err = GetLastError(); // system error stack
					LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
					textBox1->Text = "Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage);
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
				}
			}
		}
	}

	public: System::Void My_UI_SignFile_CSP_apiLite_GOST2012(String^ Serial)
	{
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;

			PCCERT_CONTEXT ret = cw->GetCertificatbySN(Serial);
			if (ret)
			{
				int funcRes =  CheckApiLite(StringtoChar(this->FileName), ret);

				if (funcRes > 1)
				{
					toolStripStatusLabel2->Text = "cades error " + funcRes.ToString();
					int err = GetLastError(); // system error stack
					LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
					textBox1->Text = "Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage);
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
				}
			}
		}
	}

	private: System::Void Certs_listBox_Click(System::Object^ sender, System::EventArgs^ e) {


	}

			 // View UI signature
	private: System::Void ViewCMSInfo(string signatureName) {
		this->openFileDialog1->DefaultExt = "*.sig";
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			textBox1->Text = "";
			cw->DisplaySig(this->openFileDialog1->FileName, Handle);

			int ret = cw->ReadTimeStamp(StringtoChar(this->openFileDialog1->FileName));
			switch (ret)   // Pointer to CERT_INFO.
			{
			case -1:
			{
				textBox1->Text = ret.ToString();
				break;
			}
			case 2:
			{
				textBox1->Text = "Signature correct. No Timestamp present \n";
				break;
			}

			case 31:
			{
				textBox1->Text = "Signature correct. ŒÚÒÛÚÒÚ‚Û˛Ú ‚ÎÓÊÂÌÌ˚Â ‚ ÒÓÓ·˘ÂÌËÂ ‰ÓÍ‡Á‡ÚÂÎ¸ÒÚ‚‡ \n" +
					"ÔÓ‚ÂÍË Ì‡ ÓÚÁ˚‚ (Á‡ÍÓ‰ËÓ‚‡ÌÌ˚Â ÒÔËÒÍË ÓÚÓÁ‚‡ÌÌ˚ı ÒÂÚËÙËÍ‡ÚÓ‚ Ë \n" +
					"Á‡ÍÓ‰ËÓ‚‡ÌÌ˚Â ÓÚ‚ÂÚ˚ ÒÎÛÊ·˚ OCSP) ‚ ‚Ë‰Â Ï‡ÒÒË‚Ó‚\n";
				break;
			}



			case 0:
			{
				textBox1->Text = "ok";
				break;
			}
			};
		}
	}



	private: System::Void UpdateViewCertInfo(string SubjectName, Object^ Tag)
	{
		PCCERT_CONTEXT Certificat = cw->GetCertificatbySN((String^)Tag);
		textBox1->Text = cw->DisplayCertInfo(Certificat);
		toolStripStatusLabel1->Text = "Serial Number " + cw->GetCertificatSerialNumber(Certificat);
		toolStripStatusLabel2->Text = "Issuer " + cw->GetCertIssuerName(Certificat);
	}


	private: System::Void Form1_Load(System::Object^ sender, System::EventArgs^ e) {
		//CheckApiLite();
		this->ListCertificates();
	}

	private: System::Void Certs_listBox_KeyUp(System::Object^ sender, System::Windows::Forms::KeyEventArgs^ e)
	{


	}

	private: System::Void button3_Click(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile_CSP_Utils_GOST2012((string)listView_Certs->SelectedItems[0]->Tag);
	}


	private: System::Void Certs_listBox_MouseClick(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e) {

	}


			 //
	private: System::Void button4_Click(System::Object^ sender, System::EventArgs^ e) {

	}

			 // sign wincrypt
	private: System::Void button2_Click(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile((string)listView_Certs->SelectedItems[0]->Tag);
	}

	private: System::Void button1_Click_1(System::Object^ sender, System::EventArgs^ e) {

	}

	private: System::Void button3_Click_1(System::Object^ sender, System::EventArgs^ e) {

	}
	private: System::Void ListView_Certs_Click(System::Object^ sender, System::EventArgs^ e) {

		if (listView_Certs->SelectedItems->Count == 1)
			UpdateViewCertInfo((string)listView_Certs->SelectedItems[0]->Text, listView_Certs->SelectedItems[0]->Tag);
	}
	private: System::Void ListView_Certs_KeyUp(System::Object^ sender, System::Windows::Forms::KeyEventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			UpdateViewCertInfo((string)listView_Certs->SelectedItems[0]->Text, listView_Certs->SelectedItems[0]->Tag);
	}


	private: System::Void ToolStripButton1_Click(System::Object^ sender, System::EventArgs^ e) {
		ViewCMSInfo("*.sig");
	}
	private: System::Void Button1_Click_2(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile_CSP_apiLite_GOST2012((string)listView_Certs->SelectedItems[0]->Tag);
	}
};
}

