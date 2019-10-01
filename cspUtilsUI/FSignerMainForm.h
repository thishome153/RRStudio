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
	private: System::Windows::Forms::ListBox^ Certs_listBox;
	private: System::Windows::Forms::MenuStrip^ menuStrip1;
	private: System::Windows::Forms::ToolStripMenuItem^ Ù‡ÈÎToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ ‚˚ıÓ‰ToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ toolStripMenuItem1;
	private: System::Windows::Forms::ToolStripMenuItem^ ÔÓÏÓ˘¸ToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ ÓœÓ„‡ÏÏÂToolStripMenuItem;
	private: System::String^ FileName;
	private: System::Windows::Forms::OpenFileDialog^ openFileDialog1;
	private: System::Windows::Forms::TextBox^ textBox1;
	private: System::Windows::Forms::Button^ button2;

	private: System::Windows::Forms::StatusStrip^ statusStrip1;
	private: System::Windows::Forms::ToolStripStatusLabel^ toolStripStatusLabel1;
	private: System::Windows::Forms::ToolStripStatusLabel^ toolStripStatusLabel2;


	private: System::Windows::Forms::Button^ button3;
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
			this->Certs_listBox = (gcnew System::Windows::Forms::ListBox());
			this->menuStrip1 = (gcnew System::Windows::Forms::MenuStrip());
			this->Ù‡ÈÎToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripMenuItem1 = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->‚˚ıÓ‰ToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ÔÓÏÓ˘¸ToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ÓœÓ„‡ÏÏÂToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->textBox1 = (gcnew System::Windows::Forms::TextBox());
			this->button2 = (gcnew System::Windows::Forms::Button());
			this->statusStrip1 = (gcnew System::Windows::Forms::StatusStrip());
			this->toolStripStatusLabel1 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->toolStripStatusLabel2 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->button3 = (gcnew System::Windows::Forms::Button());
			this->menuStrip1->SuspendLayout();
			this->statusStrip1->SuspendLayout();
			this->SuspendLayout();
			// 
			// Certs_listBox
			// 
			this->Certs_listBox->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 11, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(204)));
			this->Certs_listBox->FormattingEnabled = true;
			this->Certs_listBox->ItemHeight = 18;
			this->Certs_listBox->Location = System::Drawing::Point(0, 25);
			this->Certs_listBox->Name = L"Certs_listBox";
			this->Certs_listBox->Size = System::Drawing::Size(595, 274);
			this->Certs_listBox->TabIndex = 2;
			this->Certs_listBox->Click += gcnew System::EventHandler(this, &Form1::Certs_listBox_Click);
			this->Certs_listBox->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::Certs_listBox_MouseClick);
			this->Certs_listBox->KeyUp += gcnew System::Windows::Forms::KeyEventHandler(this, &Form1::Certs_listBox_KeyUp);
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
			this->menuStrip1->Size = System::Drawing::Size(739, 25);
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
			this->textBox1->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 11, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(204)));
			this->textBox1->Location = System::Drawing::Point(0, 305);
			this->textBox1->Multiline = true;
			this->textBox1->Name = L"textBox1";
			this->textBox1->Size = System::Drawing::Size(739, 159);
			this->textBox1->TabIndex = 0;
			// 
			// button2
			// 
			this->button2->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button2->FlatAppearance->BorderSize = 2;
			this->button2->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button2->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button2->Location = System::Drawing::Point(601, 28);
			this->button2->Name = L"button2";
			this->button2->Size = System::Drawing::Size(126, 42);
			this->button2->TabIndex = 4;
			this->button2->Text = L"œÓ‰ÔËÒ‡Ú¸\r\n";
			this->button2->UseVisualStyleBackColor = true;
			this->button2->Click += gcnew System::EventHandler(this, &Form1::button2_Click);
			// 
			// statusStrip1
			// 
			this->statusStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->toolStripStatusLabel1,
					this->toolStripStatusLabel2
			});
			this->statusStrip1->Location = System::Drawing::Point(0, 482);
			this->statusStrip1->Name = L"statusStrip1";
			this->statusStrip1->Size = System::Drawing::Size(739, 22);
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
			// button3
			// 
			this->button3->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button3->FlatAppearance->BorderSize = 2;
			this->button3->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button3->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button3->Location = System::Drawing::Point(601, 140);
			this->button3->Name = L"button3";
			this->button3->Size = System::Drawing::Size(126, 42);
			this->button3->TabIndex = 9;
			this->button3->Text = L"œÓ‰ÔËÒ‡Ú¸  √Œ—“ 34.11-94 2012";
			this->button3->UseVisualStyleBackColor = true;
			this->button3->Click += gcnew System::EventHandler(this, &Form1::button3_Click_1);
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(739, 504);
			this->Controls->Add(this->button3);
			this->Controls->Add(this->statusStrip1);
			this->Controls->Add(this->button2);
			this->Controls->Add(this->textBox1);
			this->Controls->Add(this->Certs_listBox);
			this->Controls->Add(this->menuStrip1);
			this->MainMenuStrip = this->menuStrip1;
			this->Name = L"Form1";
			this->Text = L"—Â‰ÒÚ‚Ó ‡·ÓÚ˚ Ò ›÷œ. @2014-19";
			this->Load += gcnew System::EventHandler(this, &Form1::Form1_Load);
			this->menuStrip1->ResumeLayout(false);
			this->menuStrip1->PerformLayout();
			this->statusStrip1->ResumeLayout(false);
			this->statusStrip1->PerformLayout();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	private: System::Void button1_Click(System::Object^ sender, System::EventArgs^ e) {


	}



	private: System::Void ListCertificates() {

		cspUtils::CadesWrapper^ cw;
		this->Certs_listBox->Items->Clear();
		for each (string var in cw->GetCertificates())
		{
			this->Certs_listBox->Items->Add(var);
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
	public: System::Void My_UI_SignFile()
	{
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;
			String^ Subject = (string)Certs_listBox->SelectedItem;

			PCCERT_CONTEXT ret = cw->GetCertificat(Subject);
			//PCCERT_CONTEXT ret = SignerUtils::wincrypt::GetCertificat((string)Certs_listBox->SelectedItem);  // direct call from api
			if (ret)
			{
				int funcRes = cw->SignFileWinCrypt(this->FileName, Subject);
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
	public: System::Void My_UI_SignFile_CSP_Utils_GOST2012()
	{
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;
			String^ Subject = (string)Certs_listBox->SelectedItem;
			PCCERT_CONTEXT ret = cw->GetCertificat(Subject);
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



	private: System::Void Certs_listBox_Click(System::Object^ sender, System::EventArgs^ e) {
		UpdateViewCertInfo((string)Certs_listBox->SelectedItem);

	}

			 /// ŒÚÓ·Ó‡ÁËÏ UI signature
	private: System::Void ViewCMSInfo(string signatureName) {
		this->openFileDialog1->DefaultExt = "*.sig";
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			textBox1->Text = "";
			cw->DisplaySig(this->openFileDialog1->FileName, Handle);

		int ret=cw->ReadTimeStamp(StringtoChar( this->openFileDialog1->FileName));
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
			textBox1->Text = "Signature correct. ŒÚÒÛÚÒÚ‚Û˛Ú ‚ÎÓÊÂÌÌ˚Â ‚ ÒÓÓ·˘ÂÌËÂ ‰ÓÍ‡Á‡ÚÂÎ¸ÒÚ‚‡ \n"+
							 "ÔÓ‚ÂÍË Ì‡ ÓÚÁ˚‚ (Á‡ÍÓ‰ËÓ‚‡ÌÌ˚Â ÒÔËÒÍË ÓÚÓÁ‚‡ÌÌ˚ı ÒÂÚËÙËÍ‡ÚÓ‚ Ë \n"+
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



	private: System::Void UpdateViewCertInfo(string SubjectName)
	{
		textBox1->Text = cw->DisplayCertInfo(SubjectName);
		toolStripStatusLabel1->Text ="Serial Number " +  cw->GetCertificatSerialNumber(SubjectName);
		toolStripStatusLabel2->Text = SubjectName;
	}


	private: System::Void Form1_Load(System::Object^ sender, System::EventArgs^ e) {
		this->ListCertificates();
	}

	private: System::Void Certs_listBox_KeyUp(System::Object^ sender, System::Windows::Forms::KeyEventArgs^ e)
	{

		UpdateViewCertInfo((string)Certs_listBox->SelectedItem);
	}

	private: System::Void button3_Click(System::Object^ sender, System::EventArgs^ e) {

	}


	private: System::Void Certs_listBox_MouseClick(System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e) {
		UpdateViewCertInfo((string)Certs_listBox->SelectedItem);
	}


			 //
	private: System::Void button4_Click(System::Object^ sender, System::EventArgs^ e) {
		
	}

			 // sign wincrypt
	private: System::Void button2_Click(System::Object^ sender, System::EventArgs^ e) {
		My_UI_SignFile();
	}

	private: System::Void button1_Click_1(System::Object^ sender, System::EventArgs^ e) {

	}

	private: System::Void button3_Click_1(System::Object^ sender, System::EventArgs^ e) {
		My_UI_SignFile_CSP_Utils_GOST2012();
	}
	};
}

