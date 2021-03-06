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
	/// Summary for FSignerMainForm
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class FSignerMainForm : public System::Windows::Forms::Form
	{
	public:
		FSignerMainForm(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
			//this->cw = gcnew cspUtils::CadesWrapper();
			// TODO - remove link to cadesSharp assembly:	

		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~FSignerMainForm()
		{
			if (components)
			{
				delete components;
			}
		}



	private: System::Windows::Forms::MenuStrip^ menuStrip1;
	private: System::Windows::Forms::ToolStripMenuItem^ файлToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ выходToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ toolStripMenuItem1;
	private: System::Windows::Forms::ToolStripMenuItem^ помощьToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ оПрограммеToolStripMenuItem;
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
	private: System::Windows::Forms::ContextMenuStrip^ contextMenu_Certs;

	private: System::Windows::Forms::ToolStripMenuItem^ экспортToolStripMenuItem;
	private: System::Windows::Forms::ColumnHeader^ columnHeader3;

	private: System::Windows::Forms::ToolStripSeparator^ toolStripSeparator1;
	private: System::Windows::Forms::ToolStripButton^ toolStripButton_Sign;
	private: System::Windows::Forms::ToolStripButton^ toolStripButton_SignAPILite;
	private: System::Windows::Forms::ToolStripMenuItem^ cSPToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ провайдерыCSPToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ провайдерыКлючейToolStripMenuItem;
	private: System::Windows::Forms::ToolStripMenuItem^ ToolStripMenuItem_SignWithAPILite;
	private: System::Windows::Forms::ToolStripMenuItem^ типыПровайдеровToolStripMenuItem;
	private: System::Windows::Forms::ToolStripSeparator^ toolStripSeparator2;
	private: System::Windows::Forms::ColumnHeader^ columnHeader4;
	private: System::Windows::Forms::ColumnHeader^ columnHeader5;
	private: System::Windows::Forms::ToolStripMenuItem^ UpdateToolStripMenuItem;



	private: System::ComponentModel::IContainer^ components;
	protected:

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^ resources = (gcnew System::ComponentModel::ComponentResourceManager(FSignerMainForm::typeid));
			System::Windows::Forms::ListViewItem^ listViewItem1 = (gcnew System::Windows::Forms::ListViewItem(gcnew cli::array< System::String^  >(2) {
				L"Item1",
					L"Sub1"
			}, -1));
			this->menuStrip1 = (gcnew System::Windows::Forms::MenuStrip());
			this->файлToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripMenuItem1 = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->выходToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->cSPToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->провайдерыCSPToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->провайдерыКлючейToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->типыПровайдеровToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->toolStripSeparator2 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->ToolStripMenuItem_SignWithAPILite = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->помощьToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->оПрограммеToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->textBox1 = (gcnew System::Windows::Forms::TextBox());
			this->statusStrip1 = (gcnew System::Windows::Forms::StatusStrip());
			this->toolStripStatusLabel1 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->toolStripStatusLabel2 = (gcnew System::Windows::Forms::ToolStripStatusLabel());
			this->listView_Certs = (gcnew System::Windows::Forms::ListView());
			this->columnHeader1 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader2 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader3 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader4 = (gcnew System::Windows::Forms::ColumnHeader());
			this->columnHeader5 = (gcnew System::Windows::Forms::ColumnHeader());
			this->contextMenu_Certs = (gcnew System::Windows::Forms::ContextMenuStrip(this->components));
			this->экспортToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->panel1 = (gcnew System::Windows::Forms::Panel());
			this->button1 = (gcnew System::Windows::Forms::Button());
			this->button3 = (gcnew System::Windows::Forms::Button());
			this->button2 = (gcnew System::Windows::Forms::Button());
			this->toolStrip1 = (gcnew System::Windows::Forms::ToolStrip());
			this->toolStripButton1 = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripSeparator1 = (gcnew System::Windows::Forms::ToolStripSeparator());
			this->toolStripButton_Sign = (gcnew System::Windows::Forms::ToolStripButton());
			this->toolStripButton_SignAPILite = (gcnew System::Windows::Forms::ToolStripButton());
			this->UpdateToolStripMenuItem = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->menuStrip1->SuspendLayout();
			this->statusStrip1->SuspendLayout();
			this->contextMenu_Certs->SuspendLayout();
			this->panel1->SuspendLayout();
			this->toolStrip1->SuspendLayout();
			this->SuspendLayout();
			// 
			// menuStrip1
			// 
			this->menuStrip1->Font = (gcnew System::Drawing::Font(L"Tahoma", 10, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(0)));
			this->menuStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(3) {
				this->файлToolStripMenuItem,
					this->cSPToolStripMenuItem, this->помощьToolStripMenuItem
			});
			this->menuStrip1->Location = System::Drawing::Point(0, 0);
			this->menuStrip1->Name = L"menuStrip1";
			this->menuStrip1->ShowItemToolTips = true;
			this->menuStrip1->Size = System::Drawing::Size(687, 25);
			this->menuStrip1->TabIndex = 3;
			this->menuStrip1->Text = L"menuStrip1";
			// 
			// файлToolStripMenuItem
			// 
			this->файлToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->toolStripMenuItem1,
					this->выходToolStripMenuItem
			});
			this->файлToolStripMenuItem->Name = L"файлToolStripMenuItem";
			this->файлToolStripMenuItem->Size = System::Drawing::Size(53, 21);
			this->файлToolStripMenuItem->Text = L"Файл";
			// 
			// toolStripMenuItem1
			// 
			this->toolStripMenuItem1->Image = (cli::safe_cast<System::Drawing::Image^>(resources->GetObject(L"toolStripMenuItem1.Image")));
			this->toolStripMenuItem1->Name = L"toolStripMenuItem1";
			this->toolStripMenuItem1->Size = System::Drawing::Size(150, 22);
			this->toolStripMenuItem1->Text = L"Открыть....";
			this->toolStripMenuItem1->Click += gcnew System::EventHandler(this, &FSignerMainForm::toolStripMenuItem1_Click);
			// 
			// выходToolStripMenuItem
			// 
			this->выходToolStripMenuItem->Name = L"выходToolStripMenuItem";
			this->выходToolStripMenuItem->Size = System::Drawing::Size(150, 22);
			this->выходToolStripMenuItem->Text = L"Выход";
			this->выходToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::выходToolStripMenuItem_Click);
			// 
			// cSPToolStripMenuItem
			// 
			this->cSPToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(5) {
				this->провайдерыCSPToolStripMenuItem,
					this->провайдерыКлючейToolStripMenuItem, this->типыПровайдеровToolStripMenuItem, this->toolStripSeparator2, this->ToolStripMenuItem_SignWithAPILite
			});
			this->cSPToolStripMenuItem->Name = L"cSPToolStripMenuItem";
			this->cSPToolStripMenuItem->Size = System::Drawing::Size(45, 21);
			this->cSPToolStripMenuItem->Text = L"CSP";
			// 
			// провайдерыCSPToolStripMenuItem
			// 
			this->провайдерыCSPToolStripMenuItem->Name = L"провайдерыCSPToolStripMenuItem";
			this->провайдерыCSPToolStripMenuItem->Size = System::Drawing::Size(209, 22);
			this->провайдерыCSPToolStripMenuItem->Text = L"Провайдеры CSP";
			this->провайдерыCSPToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::ПровайдерыCSPToolStripMenuItem_Click);
			// 
			// провайдерыКлючейToolStripMenuItem
			// 
			this->провайдерыКлючейToolStripMenuItem->Name = L"провайдерыКлючейToolStripMenuItem";
			this->провайдерыКлючейToolStripMenuItem->Size = System::Drawing::Size(209, 22);
			this->провайдерыКлючейToolStripMenuItem->Text = L"Провайдеры ключей";
			this->провайдерыКлючейToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::ПровайдерыКлючейToolStripMenuItem_Click);
			// 
			// типыПровайдеровToolStripMenuItem
			// 
			this->типыПровайдеровToolStripMenuItem->Name = L"типыПровайдеровToolStripMenuItem";
			this->типыПровайдеровToolStripMenuItem->Size = System::Drawing::Size(209, 22);
			this->типыПровайдеровToolStripMenuItem->Text = L"Типы провайдеров";
			this->типыПровайдеровToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::ТипыПровайдеровToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this->toolStripSeparator2->Name = L"toolStripSeparator2";
			this->toolStripSeparator2->Size = System::Drawing::Size(206, 6);
			// 
			// ToolStripMenuItem_SignWithAPILite
			// 
			this->ToolStripMenuItem_SignWithAPILite->Image = (cli::safe_cast<System::Drawing::Image^>(resources->GetObject(L"ToolStripMenuItem_SignWithAPILite.Image")));
			this->ToolStripMenuItem_SignWithAPILite->Name = L"ToolStripMenuItem_SignWithAPILite";
			this->ToolStripMenuItem_SignWithAPILite->Size = System::Drawing::Size(209, 22);
			this->ToolStripMenuItem_SignWithAPILite->Text = L"Подписать (api lite)";
			this->ToolStripMenuItem_SignWithAPILite->Click += gcnew System::EventHandler(this, &FSignerMainForm::ToolStripMenuItem_SignWithAPILite_Click);
			// 
			// помощьToolStripMenuItem
			// 
			this->помощьToolStripMenuItem->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(1) { this->оПрограммеToolStripMenuItem });
			this->помощьToolStripMenuItem->Name = L"помощьToolStripMenuItem";
			this->помощьToolStripMenuItem->Size = System::Drawing::Size(72, 21);
			this->помощьToolStripMenuItem->Text = L"Помощь";
			// 
			// оПрограммеToolStripMenuItem
			// 
			this->оПрограммеToolStripMenuItem->Name = L"оПрограммеToolStripMenuItem";
			this->оПрограммеToolStripMenuItem->Size = System::Drawing::Size(160, 22);
			this->оПрограммеToolStripMenuItem->Text = L"О программе";
			this->оПрограммеToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::оПрограммеToolStripMenuItem_Click);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->FileName = L"openFileDialog1";
			this->openFileDialog1->Filter = L"All files|*.*|Signature detached files|*.sig";
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
			this->listView_Certs->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(5) {
				this->columnHeader1,
					this->columnHeader2, this->columnHeader3, this->columnHeader4, this->columnHeader5
			});
			this->listView_Certs->ContextMenuStrip = this->contextMenu_Certs;
			this->listView_Certs->Dock = System::Windows::Forms::DockStyle::Top;
			this->listView_Certs->ForeColor = System::Drawing::SystemColors::HotTrack;
			this->listView_Certs->FullRowSelect = true;
			this->listView_Certs->GridLines = true;
			this->listView_Certs->HideSelection = false;
			listViewItem1->Tag = L"Tag0";
			this->listView_Certs->Items->AddRange(gcnew cli::array< System::Windows::Forms::ListViewItem^  >(1) { listViewItem1 });
			this->listView_Certs->Location = System::Drawing::Point(0, 56);
			this->listView_Certs->Name = L"listView_Certs";
			this->listView_Certs->Size = System::Drawing::Size(536, 202);
			this->listView_Certs->TabIndex = 10;
			this->listView_Certs->UseCompatibleStateImageBehavior = false;
			this->listView_Certs->View = System::Windows::Forms::View::Details;
			this->listView_Certs->Click += gcnew System::EventHandler(this, &FSignerMainForm::ListView_Certs_Click);
			this->listView_Certs->KeyUp += gcnew System::Windows::Forms::KeyEventHandler(this, &FSignerMainForm::ListView_Certs_KeyUp);
			// 
			// columnHeader1
			// 
			this->columnHeader1->Text = L"Сертификаты";
			this->columnHeader1->Width = 139;
			// 
			// columnHeader2
			// 
			this->columnHeader2->Text = L"Дата действ.";
			this->columnHeader2->TextAlign = System::Windows::Forms::HorizontalAlignment::Center;
			this->columnHeader2->Width = 145;
			// 
			// columnHeader3
			// 
			this->columnHeader3->Text = L"Экcпорт";
			this->columnHeader3->TextAlign = System::Windows::Forms::HorizontalAlignment::Center;
			// 
			// columnHeader4
			// 
			this->columnHeader4->Text = L"KeyParams";
			this->columnHeader4->Width = 70;
			// 
			// columnHeader5
			// 
			this->columnHeader5->Text = L"Контейнер";
			this->columnHeader5->Width = 70;
			// 
			// contextMenu_Certs
			// 
			this->contextMenu_Certs->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->экспортToolStripMenuItem,
					this->UpdateToolStripMenuItem
			});
			this->contextMenu_Certs->Name = L"contextMenuStrip1";
			this->contextMenu_Certs->Size = System::Drawing::Size(181, 70);
			// 
			// экспортToolStripMenuItem
			// 
			this->экспортToolStripMenuItem->Name = L"экспортToolStripMenuItem";
			this->экспортToolStripMenuItem->Size = System::Drawing::Size(180, 22);
			this->экспортToolStripMenuItem->Text = L"Экспорт";
			// 
			// panel1
			// 
			this->panel1->Controls->Add(this->button1);
			this->panel1->Controls->Add(this->button3);
			this->panel1->Controls->Add(this->button2);
			this->panel1->Dock = System::Windows::Forms::DockStyle::Right;
			this->panel1->Location = System::Drawing::Point(536, 56);
			this->panel1->Name = L"panel1";
			this->panel1->Size = System::Drawing::Size(151, 405);
			this->panel1->TabIndex = 11;
			// 
			// button1
			// 
			this->button1->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button1->FlatAppearance->BorderSize = 2;
			this->button1->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button1->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button1->Location = System::Drawing::Point(13, 277);
			this->button1->Name = L"button1";
			this->button1->Size = System::Drawing::Size(126, 65);
			this->button1->TabIndex = 12;
			this->button1->Text = L"Подписать  ГОСТ 34.11-94 2012\r\nwith apilite.dll\r\n";
			this->button1->UseVisualStyleBackColor = true;
			this->button1->Click += gcnew System::EventHandler(this, &FSignerMainForm::Button1_Click_2);
			// 
			// button3
			// 
			this->button3->FlatAppearance->BorderColor = System::Drawing::Color::Teal;
			this->button3->FlatAppearance->BorderSize = 2;
			this->button3->FlatStyle = System::Windows::Forms::FlatStyle::Flat;
			this->button3->ImageAlign = System::Drawing::ContentAlignment::MiddleLeft;
			this->button3->Location = System::Drawing::Point(12, 67);
			this->button3->Name = L"button3";
			this->button3->Size = System::Drawing::Size(126, 42);
			this->button3->TabIndex = 11;
			this->button3->Text = L"Подписать  ГОСТ 34.11-94 2012";
			this->button3->UseVisualStyleBackColor = true;
			this->button3->Click += gcnew System::EventHandler(this, &FSignerMainForm::button3_Click);
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
			this->button2->Text = L"Подписать\r\n";
			this->button2->UseVisualStyleBackColor = true;
			this->button2->Click += gcnew System::EventHandler(this, &FSignerMainForm::button2_Click);
			// 
			// toolStrip1
			// 
			this->toolStrip1->ImageScalingSize = System::Drawing::Size(24, 24);
			this->toolStrip1->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(4) {
				this->toolStripButton1,
					this->toolStripSeparator1, this->toolStripButton_Sign, this->toolStripButton_SignAPILite
			});
			this->toolStrip1->Location = System::Drawing::Point(0, 25);
			this->toolStrip1->Name = L"toolStrip1";
			this->toolStrip1->Size = System::Drawing::Size(687, 31);
			this->toolStrip1->TabIndex = 13;
			this->toolStrip1->Text = L"toolStrip1";
			// 
			// toolStripButton1
			// 
			this->toolStripButton1->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton1->Image = (cli::safe_cast<System::Drawing::Image^>(resources->GetObject(L"toolStripButton1.Image")));
			this->toolStripButton1->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->toolStripButton1->Name = L"toolStripButton1";
			this->toolStripButton1->Size = System::Drawing::Size(28, 28);
			this->toolStripButton1->Text = L"Sig";
			this->toolStripButton1->ToolTipText = L"Open signature";
			this->toolStripButton1->Click += gcnew System::EventHandler(this, &FSignerMainForm::ToolStripButton1_Click);
			// 
			// toolStripSeparator1
			// 
			this->toolStripSeparator1->Name = L"toolStripSeparator1";
			this->toolStripSeparator1->Size = System::Drawing::Size(6, 31);
			// 
			// toolStripButton_Sign
			// 
			this->toolStripButton_Sign->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton_Sign->Image = (cli::safe_cast<System::Drawing::Image^>(resources->GetObject(L"toolStripButton_Sign.Image")));
			this->toolStripButton_Sign->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->toolStripButton_Sign->Name = L"toolStripButton_Sign";
			this->toolStripButton_Sign->Size = System::Drawing::Size(28, 28);
			this->toolStripButton_Sign->Text = L"Sign file";
			this->toolStripButton_Sign->Click += gcnew System::EventHandler(this, &FSignerMainForm::ToolStripButton_Sign_Click);
			// 
			// toolStripButton_SignAPILite
			// 
			this->toolStripButton_SignAPILite->DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle::Image;
			this->toolStripButton_SignAPILite->Image = (cli::safe_cast<System::Drawing::Image^>(resources->GetObject(L"toolStripButton_SignAPILite.Image")));
			this->toolStripButton_SignAPILite->ImageTransparentColor = System::Drawing::Color::Magenta;
			this->toolStripButton_SignAPILite->Name = L"toolStripButton_SignAPILite";
			this->toolStripButton_SignAPILite->Size = System::Drawing::Size(28, 28);
			this->toolStripButton_SignAPILite->Text = L"toolStripButton2";
			this->toolStripButton_SignAPILite->ToolTipText = L"Sign with apiLite";
			this->toolStripButton_SignAPILite->Click += gcnew System::EventHandler(this, &FSignerMainForm::ToolStripButton_SignAPILite_Click);
			// 
			// UpdateToolStripMenuItem
			// 
			this->UpdateToolStripMenuItem->Name = L"UpdateToolStripMenuItem";
			this->UpdateToolStripMenuItem->ShortcutKeys = System::Windows::Forms::Keys::F5;
			this->UpdateToolStripMenuItem->Size = System::Drawing::Size(180, 22);
			this->UpdateToolStripMenuItem->Text = L"Обновить";
			this->UpdateToolStripMenuItem->Click += gcnew System::EventHandler(this, &FSignerMainForm::UpdateToolStripMenuItem_Click);
			// 
			// FSignerMainForm
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
			this->Name = L"FSignerMainForm";
			this->Text = L"Средство работы с ЭЦП. @2014-21";
			this->Load += gcnew System::EventHandler(this, &FSignerMainForm::FSignerMainForm_Load);
			this->menuStrip1->ResumeLayout(false);
			this->menuStrip1->PerformLayout();
			this->statusStrip1->ResumeLayout(false);
			this->statusStrip1->PerformLayout();
			this->contextMenu_Certs->ResumeLayout(false);
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
		typedef  int (*LP_func_SignFile_api_Lite) (char* FileName, PCCERT_CONTEXT SignerCertificat);	//Pointer of fucntion	 
		HMODULE hMod_L = LoadLibrary(dllName_);

		if (hMod_L != NULL) //0x00000000 < NULL > if not found dll
		{
			LP_func_SignFile_api_Lite func = (LP_func_SignFile_api_Lite)GetProcAddress(hMod_L, SignFile_api_Lite);
			if (func != NULL)
			{
				//PCCERT_CONTEXT cert = NULL;
				int ret = func(FileName, SignerCertificat);
				FreeLibrary(hMod_L);
				return ret;
			}
			else return 503;//service out
		}
		else
			return 404; //notFound
	}

	private: System::Boolean ApiLitePresent() {

		LPCWSTR dllName_ = L"cspApiLite.dll";
		LPCSTR SignFile_api_Lite = "SignFile_api_Lite";
		typedef  int (*LP_func_SignFile_api_Lite) (char* FileName, PCCERT_CONTEXT SignerCertificat);	//Новый тип - указатель на функцию	 
		HMODULE hMod_L = LoadLibrary(dllName_);

		if (hMod_L != NULL) //0x00000000 < NULL > if not found dll
		{
			FreeLibrary(hMod_L);
			return true;
		}
		else
			return false; //notFound
	}

	private: System::Void ListCertificates() {
		cspUtils::CadesWrapper^ cw;
		listView_Certs->Items->Clear();
		List<cspUtils::CertInfo^>^ Certs = cw->GetCertificatesObj();

		for each (cspUtils::CertInfo ^ ci  in Certs)
		{
			ListViewItem^ Cert_Item = gcnew ListViewItem(ci->SubjectName);
			if (ci->ValidNotAfterCode != 0)
				Cert_Item->ForeColor = Color::Red;
			//else Cert_Item->ForeColor = Color::
			Cert_Item->Tag = ci->SerialNumber;
			Cert_Item->SubItems->Add(ci->ValidNotAfter);
			if (ci->EXPORT_KEY)
				Cert_Item->SubItems->Add("Ok");
			else Cert_Item->SubItems->Add("-");
			if (ci->KeyParams > 0)
				Cert_Item->SubItems->Add(ci->KeyParams.ToString());
			else Cert_Item->SubItems->Add("-");
			if (ci->PrivateKeyPresent)
				Cert_Item->SubItems->Add(ci->PrivateKeyHandle.ToString());
			else Cert_Item->SubItems->Add("-");
			void* blob = ci->Serial;
			listView_Certs->Items->Add(Cert_Item);
		}
	}

	private: System::Void ListCNGKeysProviders() {
		cspUtils::CNGWrapper^ cngW;
		textBox1->Text = "CNG providers:\r\n";
		List<String^>^ items = cngW->EnumerateProviders();
		for each (String ^ var in items)
		{
			textBox1->Text += var + "\r\n";
		}
	}

	private: System::Void ListProviders() {
		textBox1->Text = "\r\n\ WinCrypt(Advapi32.lib) CSP providers:\r\n";
		cspUtils::WinCryptWrapper^ wcrwr;
		List<String^>^ winCryptCSP_items = wcrwr->EnumerateCSP();
		for each (String ^ var in winCryptCSP_items)
		{
			textBox1->Text += var + "\r\n";
		}
	}

	private: System::Void ListProvidersTypes() {
		textBox1->Text = "\r\n\ WinCrypt(Advapi32.lib) CSP providers types:\r\n";
		cspUtils::WinCryptWrapper^ wcrwr;
		List<String^>^ winCryptCSP_items = wcrwr->EnumerateCSPTypes();
		for each (String ^ var in winCryptCSP_items)
		{
			textBox1->Text += var + "\r\n";
		}
	}

	private: System::Void выходToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		this->Close();
	}

	private: System::Void toolStripMenuItem1_Click(System::Object^ sender, System::EventArgs^ e) {
		ViewCMSInfo("*.sig");
	}
		   /*
  private: System::Void FSignerMainForm_Paint(System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e) {

  }
*/
	private: System::Void оПрограммеToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		FS2_About^ FormAbout = (gcnew FS2_About());
		FormAbout->ShowDialog();
		//FormAbout->freeDispose();
	}

		   // Подписать файл  direct in signerUtils.h
	public: System::Void My_UI_SignFile(String^ Serial)
	{
		this->openFileDialog1->FilterIndex = 1;
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;
			LogOnFile(this->FileName);
			cspUtils::CadesWrapper^ cw;
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
					LogOnFile("Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage));
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
					LogOnFile("file " + this->FileName + "\r\n    signed ok");
				}
			}
		}
	}





		  // Подписать файл
	public: System::Void My_UI_SignFile_CSP_Utils_GOST2012(String^ Subject)
	{
		this->openFileDialog1->FilterIndex = 1;
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;
			LogOnFile(this->FileName);
			cspUtils::CadesWrapper^ cw;
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
					LogOnFile("Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage));
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
					LogOnFile("file " + this->FileName + "\r\n    signed ok");
				}
			}
		}
	}

	public: System::Void LogOnFile(String^ MessageLog)
	{
		StreamWriter^ sw = gcnew StreamWriter("logscpUI.log", true);
		try
		{
			//Console::WriteLine("writing data to file:");
			sw->Write("----------------------------------------------------");
			sw->Write(DateTime::Now + " ");
			sw->WriteLine("----------------------------------------------------");
			sw->WriteLine(MessageLog);

			sw->Close();
		}
		catch (Exception^)
		{
			//Console::WriteLine("data could not be written");
			//return -1;
		}

	}

	public: System::Void My_UI_SignFile_CSP_apiLite_GOST2012(String^ Serial)
	{
		this->openFileDialog1->FilterIndex = 1;
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			this->FileName = openFileDialog1->FileName;
			cspUtils::CadesWrapper^ cw;
			PCCERT_CONTEXT ret = cw->GetCertificatbySN(Serial);
			LogOnFile("Mode: apiLite \r\n File to sign:" + this->FileName+ "\r\n CertInfo: \r\n" + cw->DisplayCertInfo(Serial));
			if (ret)
			{
				int funcRes = CheckApiLite(StringtoChar(this->FileName), ret);

				if (funcRes > 1)
				{
					toolStripStatusLabel2->Text = "cades error " + funcRes.ToString();
					int err = GetLastError(); // system error stack
					LPTSTR errMeesage = SignerUtils::cades::Error2Message(err);
					textBox1->Text = "Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage);
					LogOnFile("Error " + err.ToString() + ":\r\n" + LPTSTRToString(errMeesage));
				}
				else
				{
					toolStripStatusLabel2->Text = "Signed success ";
					textBox1->Text = "file " + this->FileName + "\r\n    signed ok";
					LogOnFile("File signed ok");
				}
			}
		}

	}

	public: System::Void My_UI_ToggleExport(String^ Serial)
	{
		cspUtils::CadesWrapper^ cw;
		PCCERT_CONTEXT ret = cw->GetCertificatbySN(Serial);
		if (ret)
		{
			// allow/disallow key export
		}
	}

	private: System::Void Certs_listBox_Click(System::Object^ sender, System::EventArgs^ e) {


	}

		   // View UI signature
	private: System::Void ViewCMSInfo(string signatureName) {
		this->openFileDialog1->DefaultExt = "*.sig";
		this->openFileDialog1->FilterIndex = 2;
		if (this->openFileDialog1->ShowDialog() == DlgRes::OK)
		{
			textBox1->Text = "";
			cspUtils::CadesWrapper^ cw;
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
				textBox1->Text = "Signature correct. Отсутствуют вложенные в сообщение доказательства \n" +
					"проверки на отзыв (закодированные списки отозванных сертификатов и \n" +
					"закодированные ответы службы OCSP) в виде массивов\n";
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
		cspUtils::CadesWrapper^ CW = gcnew cspUtils::CadesWrapper();
		PCCERT_CONTEXT Certificat = CW->GetCertificatbySN((String^)Tag);

		textBox1->Text = CW->DisplayCertInfo(Certificat);
		toolStripStatusLabel1->Text = "Serial Number " + CW->GetCertificatSerialNumber(Certificat);
		toolStripStatusLabel2->Text = "Issuer " + CW->GetCertIssuerName(Certificat);

	}

	private: System::Void FSignerMainForm_Load(System::Object^ sender, System::EventArgs^ e) {
		//CheckApiLite();
		ListCertificates();
		ListProviders();
		if (ApiLitePresent())
		{
			button1->Visible = true;
			ToolStripMenuItem_SignWithAPILite->Enabled = true;
			toolStripButton_SignAPILite->Enabled = true;
			toolStripStatusLabel1->Text = "CSP Api lite ok";
			toolStripStatusLabel2->Text = "";
		}
		else
		{
			button1->Visible = false;
			ToolStripMenuItem_SignWithAPILite->Enabled = false;
			toolStripButton_SignAPILite->Enabled = false;
			toolStripStatusLabel1->Text = "";
			toolStripStatusLabel2->Text = "";
		}
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
	private: System::Void ToolStripButton_Sign_Click(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile_CSP_Utils_GOST2012((string)listView_Certs->SelectedItems[0]->Tag);
	}

	private: System::Void ToolStripButton_SignAPILite_Click(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile_CSP_apiLite_GOST2012((string)listView_Certs->SelectedItems[0]->Tag);
	}
	private: System::Void ПровайдерыCSPToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		ListProviders();
	}
	private: System::Void ПровайдерыКлючейToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		ListCNGKeysProviders();
	}
	private: System::Void ToolStripMenuItem_SignWithAPILite_Click(System::Object^ sender, System::EventArgs^ e) {
		if (listView_Certs->SelectedItems->Count == 1)
			My_UI_SignFile_CSP_apiLite_GOST2012((string)listView_Certs->SelectedItems[0]->Tag);
	}
	private: System::Void ТипыПровайдеровToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
		ListProvidersTypes();
	}
	private: System::Void UpdateToolStripMenuItem_Click(System::Object^ sender, System::EventArgs^ e) {
	}
	};
}

