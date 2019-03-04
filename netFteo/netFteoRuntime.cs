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





}
