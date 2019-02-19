using System;
using System.Runtime.InteropServices; //IdleCheck etc

/// <summary>
/// Runtime tool of library.
/// Interop with operating system in runtime
/// </summary>
namespace netFteo.Runtime
{
	/// <summary>
	/// Class for query system about user input.
	/// Usefull for application iddle timeout.
	/// Not need ThreadIdle event (is only available as of 4.5)
	/// </summary>
	public static class UserInput
	{
		/// <summary>
		/// Physical layout of the data fields of a class or structure
		//     in memory
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct LASTINPUTINFOStruct
		{
			[MarshalAs(UnmanagedType.U4)]
			public int cbSize;
			[MarshalAs(UnmanagedType.U4)]
			public int dwTime;
		}

		[DllImport("user32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFOStruct x);

		public static int GetLastInputTime()
		{
			var inf = new LASTINPUTINFOStruct();
			inf.cbSize = Marshal.SizeOf(inf);
			inf.dwTime = 0;
			return GetLastInputInfo(ref inf) ? Environment.TickCount - inf.dwTime : 0;
		}
	}


}
