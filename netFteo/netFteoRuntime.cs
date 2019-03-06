using System;
using System.Runtime.InteropServices; //IdleCheck etc
using System.Drawing.Imaging;

/// <summary>
/// Runtime tool of library.
/// Interop with operating system in runtime
/// </summary>
namespace netFteo.Runtime
{

	/// <summary>
	/// Physical layout of the data fields of a class or structure
	//     in memory
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct LASTINPUTINFOStruct
	{
		[MarshalAs(UnmanagedType.U4)]
		public int cbSize;
		[MarshalAs(UnmanagedType.U4)]
		public int dwTime;
	}

	/// <summary>
	/// RECT just rect. Not rectal (anal) ===:)
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}

	/// <summary>
	/// Class for query system about user input.
	/// Usefull for application iddle timeout.
	/// Not need ThreadIdle event (is only available as of 4.5)
	/// </summary>
	public static class UserInput
	{
	

		//[DllImport("user32.dll")]
		//private static extern bool GetLastInputInfo(ref LASTINPUTINFOStruct x);

		/// <summary>
		/// Helper class containing User32 API functions
		/// </summary>

		public static int GetLastInputTime()
		{
			var inf = new LASTINPUTINFOStruct();
			inf.cbSize = Marshal.SizeOf(inf);
			inf.dwTime = 0;
			return User32.GetLastInputInfo(ref inf) ? Environment.TickCount - inf.dwTime : 0;
		}
	}

	/// <summary>
	/// user32.dll wrapper
	/// </summary>
	public class User32
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
		[DllImport("user32.dll")]
		public static extern bool GetLastInputInfo(ref LASTINPUTINFOStruct x);
	}

	/// <summary>
	/// Gdi32 API functions wrapper
	/// </summary>
	public class GDI32
	{

		public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
			int nWidth, int nHeight, IntPtr hObjectSource,
			int nXSrc, int nYSrc, int dwRop);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
			int nHeight);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC(IntPtr hDC);
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
	}

	public class ScreenWriter
	{
		public System.Drawing.Image CaptureScreen()
		{
			//return null;
			return CaptureWindow(netFteo.Runtime.User32.GetDesktopWindow());
		}


		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
		/// <returns></returns>
		public System.Drawing.Image CaptureWindow(IntPtr handle)
		{
			// get te hDC of the target window
			IntPtr hdcSrc = User32.GetWindowDC(handle);
			// get the size
			RECT windowRect = new RECT();
			User32.GetWindowRect(handle, ref windowRect);
			int width = windowRect.right - windowRect.left;
			int height = windowRect.bottom - windowRect.top;
			// create a device context we can copy to
			IntPtr hdcDest = netFteo.Runtime.GDI32.CreateCompatibleDC(hdcSrc);
			// create a bitmap we can copy it to,
			// using GetDeviceCaps to get the width/height
			IntPtr hBitmap = netFteo.Runtime.GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
			// select the bitmap object
			IntPtr hOld = netFteo.Runtime.GDI32.SelectObject(hdcDest, hBitmap);
			// bitblt over
			GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, netFteo.Runtime.GDI32.SRCCOPY);
			// restore selection
			GDI32.SelectObject(hdcDest, hOld);
			// clean up 
			GDI32.DeleteDC(hdcDest);
			User32.ReleaseDC(handle, hdcSrc);
			// get a .NET image object for it
			System.Drawing.Image img = System.Drawing.Image.FromHbitmap(hBitmap);
			// free up the Bitmap object
			GDI32.DeleteObject(hBitmap);
			return img;
		}

		/// <summary>
		/// Captures a screen shot of a specific window, and saves it to a file
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="filename"></param>
		/// <param name="format"></param>
		public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
		{
			System.Drawing.Image img = CaptureWindow(handle);
			img.Save(filename, format);
		}

		/// <summary>
		/// Captures a screen shot of the entire desktop, and saves it to a file
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="format"></param>
		public void CaptureScreenToFile(string filename, ImageFormat format)
		{
			System.Drawing.Image img = CaptureScreen();
			img.Save(filename, format);
		}

		/// <summary>
		/// Saving screen portion to file
		/// </summary>
		/// <param name="FileName"></param>
		[Obsolete("Save is deprecated (ЗапЧАСТИ), please use CaptureScreenToFile instead.", true)]
		public void Save(string FileName)
		{
			System.Drawing.Bitmap memoryImage;
			memoryImage = new System.Drawing.Bitmap(1000, 900);
			System.Drawing.Size s = new System.Drawing.Size(memoryImage.Width, memoryImage.Height);

			// Create graphics 
			System.Drawing.Graphics memoryGraphics = System.Drawing.Graphics.FromImage(memoryImage);

			// Copy data from screen 
			memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

			// Save it! 
			memoryImage.Save(FileName);
		}
	}

	/// <summary>
	/// Provides detailed information about the host operating system.
	/// </summary>
	static public class OSInfo
	{
		#region BITS
		/// <summary>
		/// Determines if the current application is 32 or 64-bit.
		/// </summary>
		static public int Bits
		{
			get
			{
				return IntPtr.Size * 8;
			}
		}
		#endregion BITS

		#region EDITION
		static private string s_Edition;
		/// <summary>
		/// Gets the edition of the operating system running on this computer.
		/// </summary>
		static public string Edition
		{
			get
			{
				if (s_Edition != null)
					return s_Edition;  //***** RETURN *****//

				string edition = String.Empty;

				OperatingSystem osVersion = Environment.OSVersion;
				OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
				osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

				if (GetVersionEx(ref osVersionInfo))
				{
					int majorVersion = osVersion.Version.Major;
					int minorVersion = osVersion.Version.Minor;
					byte productType = osVersionInfo.wProductType;
					short suiteMask = osVersionInfo.wSuiteMask;

					#region VERSION 4
					if (majorVersion == 4)
					{
						if (productType == VER_NT_WORKSTATION)
						{
							// Windows NT 4.0 Workstation
							edition = "Workstation";
						}
						else if (productType == VER_NT_SERVER)
						{
							if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
							{
								// Windows NT 4.0 Server Enterprise
								edition = "Enterprise Server";
							}
							else
							{
								// Windows NT 4.0 Server
								edition = "Standard Server";
							}
						}
					}
					#endregion VERSION 4

					#region VERSION 5
					else if (majorVersion == 5)
					{
						if (productType == VER_NT_WORKSTATION)
						{
							if ((suiteMask & VER_SUITE_PERSONAL) != 0)
							{
								// Windows XP Home Edition
								edition = "Home";
							}
							else
							{
								// Windows XP / Windows 2000 Professional
								edition = "Professional";
							}
						}
						else if (productType == VER_NT_SERVER)
						{
							if (minorVersion == 0)
							{
								if ((suiteMask & VER_SUITE_DATACENTER) != 0)
								{
									// Windows 2000 Datacenter Server
									edition = "Datacenter Server";
								}
								else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
								{
									// Windows 2000 Advanced Server
									edition = "Advanced Server";
								}
								else
								{
									// Windows 2000 Server
									edition = "Server";
								}
							}
							else
							{
								if ((suiteMask & VER_SUITE_DATACENTER) != 0)
								{
									// Windows Server 2003 Datacenter Edition
									edition = "Datacenter";
								}
								else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
								{
									// Windows Server 2003 Enterprise Edition
									edition = "Enterprise";
								}
								else if ((suiteMask & VER_SUITE_BLADE) != 0)
								{
									// Windows Server 2003 Web Edition
									edition = "Web Edition";
								}
								else
								{
									// Windows Server 2003 Standard Edition
									edition = "Standard";
								}
							}
						}
					}
					#endregion VERSION 5

					#region VERSION 6
					else if (majorVersion == 6)
					{
						int ed;
						if (GetProductInfo(majorVersion, minorVersion,
							osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
							out ed))
						{
							switch (ed)
							{
								case PRODUCT_BUSINESS:
									edition = "Business";
									break;
								case PRODUCT_BUSINESS_N:
									edition = "Business N";
									break;
								case PRODUCT_CLUSTER_SERVER:
									edition = "HPC Edition";
									break;
								case PRODUCT_DATACENTER_SERVER:
									edition = "Datacenter Server";
									break;
								case PRODUCT_DATACENTER_SERVER_CORE:
									edition = "Datacenter Server (core installation)";
									break;
								case PRODUCT_ENTERPRISE:
									edition = "Enterprise";
									break;
								case PRODUCT_ENTERPRISE_N:
									edition = "Enterprise N";
									break;
								case PRODUCT_ENTERPRISE_SERVER:
									edition = "Enterprise Server";
									break;
								case PRODUCT_ENTERPRISE_SERVER_CORE:
									edition = "Enterprise Server (core installation)";
									break;
								case PRODUCT_ENTERPRISE_SERVER_CORE_V:
									edition = "Enterprise Server without Hyper-V (core installation)";
									break;
								case PRODUCT_ENTERPRISE_SERVER_IA64:
									edition = "Enterprise Server for Itanium-based Systems";
									break;
								case PRODUCT_ENTERPRISE_SERVER_V:
									edition = "Enterprise Server without Hyper-V";
									break;
								case PRODUCT_HOME_BASIC:
									edition = "Home Basic";
									break;
								case PRODUCT_HOME_BASIC_N:
									edition = "Home Basic N";
									break;
								case PRODUCT_HOME_PREMIUM:
									edition = "Home Premium";
									break;
								case PRODUCT_HOME_PREMIUM_N:
									edition = "Home Premium N";
									break;
								case PRODUCT_HYPERV:
									edition = "Microsoft Hyper-V Server";
									break;
								case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
									edition = "Windows Essential Business Management Server";
									break;
								case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
									edition = "Windows Essential Business Messaging Server";
									break;
								case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
									edition = "Windows Essential Business Security Server";
									break;
								case PRODUCT_SERVER_FOR_SMALLBUSINESS:
									edition = "Windows Essential Server Solutions";
									break;
								case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
									edition = "Windows Essential Server Solutions without Hyper-V";
									break;
								case PRODUCT_SMALLBUSINESS_SERVER:
									edition = "Windows Small Business Server";
									break;
								case PRODUCT_STANDARD_SERVER:
									edition = "Standard Server";
									break;
								case PRODUCT_STANDARD_SERVER_CORE:
									edition = "Standard Server (core installation)";
									break;
								case PRODUCT_STANDARD_SERVER_CORE_V:
									edition = "Standard Server without Hyper-V (core installation)";
									break;
								case PRODUCT_STANDARD_SERVER_V:
									edition = "Standard Server without Hyper-V";
									break;
								case PRODUCT_STARTER:
									edition = "Starter";
									break;
								case PRODUCT_STORAGE_ENTERPRISE_SERVER:
									edition = "Enterprise Storage Server";
									break;
								case PRODUCT_STORAGE_EXPRESS_SERVER:
									edition = "Express Storage Server";
									break;
								case PRODUCT_STORAGE_STANDARD_SERVER:
									edition = "Standard Storage Server";
									break;
								case PRODUCT_STORAGE_WORKGROUP_SERVER:
									edition = "Workgroup Storage Server";
									break;
								case PRODUCT_UNDEFINED:
									edition = "Unknown product";
									break;
								case PRODUCT_ULTIMATE:
									edition = "Ultimate";
									break;
								case PRODUCT_ULTIMATE_N:
									edition = "Ultimate N";
									break;
								case PRODUCT_WEB_SERVER:
									edition = "Web Server";
									break;
								case PRODUCT_WEB_SERVER_CORE:
									edition = "Web Server (core installation)";
									break;
							}
						}
					}
					#endregion VERSION 6
				}

				s_Edition = edition;
				return edition;
			}
		}
		#endregion EDITION

		#region NAME
		static private string s_Name;
		/// <summary>
		/// Gets the name of the operating system running on this computer.
		/// </summary>
		static public string Name
		{
			get
			{
				if (s_Name != null)
					return s_Name;  //***** RETURN *****//

				string name = "unknown";

				OperatingSystem osVersion = Environment.OSVersion;
				OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
				osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

				if (GetVersionEx(ref osVersionInfo))
				{
					int majorVersion = osVersion.Version.Major;
					int minorVersion = osVersion.Version.Minor;

					switch (osVersion.Platform)
					{
						case PlatformID.Win32Windows:
							{
								if (majorVersion == 4)
								{
									string csdVersion = osVersionInfo.szCSDVersion;
									switch (minorVersion)
									{
										case 0:
											if (csdVersion == "B" || csdVersion == "C")
												name = "Windows 95 OSR2";
											else
												name = "Windows 95";
											break;
										case 10:
											if (csdVersion == "A")
												name = "Windows 98 Second Edition";
											else
												name = "Windows 98";
											break;
										case 90:
											name = "Windows Me";
											break;
									}
								}
								break;
							}

						case PlatformID.Win32NT:
							{
								byte productType = osVersionInfo.wProductType;

								switch (majorVersion)
								{
									case 3:
										name = "Windows NT 3.51";
										break;
									case 4:
										switch (productType)
										{
											case 1:
												name = "Windows NT 4.0";
												break;
											case 3:
												name = "Windows NT 4.0 Server";
												break;
										}
										break;
									case 5:
										switch (minorVersion)
										{
											case 0:
												name = "Windows 2000";
												break;
											case 1:
												name = "Windows XP";
												break;
											case 2:
												name = "Windows Server 2003";
												break;
										}
										break;
									case 6:
										switch (minorVersion)
										{
											case 0:

												switch (productType)
												{
													case 1:
														name = "Windows Vista";
														break;
													case 3:
														name = "Windows Server 2008";
														break;
												}
												break;
											case 1:
												switch (productType)
												{
													case 1:
														name = "Windows 7";
														break;
													case 3:
														name = "Windows Server 2008 R2";
														break;
												}
												break;
										}
										break;
								}
								break;
							}
					}
				}

				s_Name = name;
				return name;
			}
		}
		#endregion NAME

		#region PINVOKE
		#region GET
		#region PRODUCT INFO
		[DllImport("Kernel32.dll")]
		internal static extern bool GetProductInfo(
			int osMajorVersion,
			int osMinorVersion,
			int spMajorVersion,
			int spMinorVersion,
			out int edition);
		#endregion PRODUCT INFO

		#region VERSION
		[DllImport("kernel32.dll")]
		private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
		#endregion VERSION
		#endregion GET

		#region OSVERSIONINFOEX
		[StructLayout(LayoutKind.Sequential)]
		private struct OSVERSIONINFOEX
		{
			public int dwOSVersionInfoSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
			public short wServicePackMajor;
			public short wServicePackMinor;
			public short wSuiteMask;
			public byte wProductType;
			public byte wReserved;
		}
		#endregion OSVERSIONINFOEX

		#region PRODUCT
		private const int PRODUCT_UNDEFINED = 0x00000000;
		private const int PRODUCT_ULTIMATE = 0x00000001;
		private const int PRODUCT_HOME_BASIC = 0x00000002;
		private const int PRODUCT_HOME_PREMIUM = 0x00000003;
		private const int PRODUCT_ENTERPRISE = 0x00000004;
		private const int PRODUCT_HOME_BASIC_N = 0x00000005;
		private const int PRODUCT_BUSINESS = 0x00000006;
		private const int PRODUCT_STANDARD_SERVER = 0x00000007;
		private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
		private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
		private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
		private const int PRODUCT_STARTER = 0x0000000B;
		private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
		private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
		private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
		private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
		private const int PRODUCT_BUSINESS_N = 0x00000010;
		private const int PRODUCT_WEB_SERVER = 0x00000011;
		private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
		private const int PRODUCT_HOME_SERVER = 0x00000013;
		private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
		private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
		private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
		private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
		private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
		private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
		private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
		private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
		private const int PRODUCT_ULTIMATE_N = 0x0000001C;
		private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
		private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
		private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
		private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
		private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
		private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
		private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
		private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
		private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
		private const int PRODUCT_HYPERV = 0x0000002A;
		#endregion PRODUCT

		#region VERSIONS
		private const int VER_NT_WORKSTATION = 1;
		private const int VER_NT_DOMAIN_CONTROLLER = 2;
		private const int VER_NT_SERVER = 3;
		private const int VER_SUITE_SMALLBUSINESS = 1;
		private const int VER_SUITE_ENTERPRISE = 2;
		private const int VER_SUITE_TERMINAL = 16;
		private const int VER_SUITE_DATACENTER = 128;
		private const int VER_SUITE_SINGLEUSERTS = 256;
		private const int VER_SUITE_PERSONAL = 512;
		private const int VER_SUITE_BLADE = 1024;
		#endregion VERSIONS
		#endregion PINVOKE

		#region SERVICE PACK
		/// <summary>
		/// Gets the service pack information of the operating system running on this computer.
		/// </summary>
		static public string ServicePack
		{
			get
			{
				string servicePack = String.Empty;
				OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

				osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

				if (GetVersionEx(ref osVersionInfo))
				{
					servicePack = osVersionInfo.szCSDVersion;
				}

				return servicePack;
			}
		}
		#endregion SERVICE PACK

		#region VERSION
		#region BUILD
		/// <summary>
		/// Gets the build version number of the operating system running on this computer.
		/// </summary>
		static public int BuildVersion
		{
			get
			{
				return Environment.OSVersion.Version.Build;
			}
		}
		#endregion BUILD

		#region FULL
		#region STRING
		/// <summary>
		/// Gets the full version string of the operating system running on this computer.
		/// </summary>
		static public string VersionString
		{
			get
			{
				return Environment.OSVersion.Version.ToString();
			}
		}
		#endregion STRING

		#region VERSION
		/// <summary>
		/// Gets the full version of the operating system running on this computer.
		/// </summary>
		static public Version Version
		{
			get
			{
				return Environment.OSVersion.Version;
			}
		}
		#endregion VERSION
		#endregion FULL

		#region MAJOR
		/// <summary>
		/// Gets the major version number of the operating system running on this computer.
		/// </summary>
		static public int MajorVersion
		{
			get
			{
				return Environment.OSVersion.Version.Major;
			}
		}
		#endregion MAJOR

		#region MINOR
		/// <summary>
		/// Gets the minor version number of the operating system running on this computer.
		/// </summary>
		static public int MinorVersion
		{
			get
			{
				return Environment.OSVersion.Version.Minor;
			}
		}
		#endregion MINOR

		#region REVISION
		/// <summary>
		/// Gets the revision version number of the operating system running on this computer.
		/// </summary>
		static public int RevisionVersion
		{
			get
			{
				return Environment.OSVersion.Version.Revision;
			}
		}
		#endregion REVISION
		#endregion VERSION
	}

}
