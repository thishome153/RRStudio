using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace netFteo.NetWork
{
	public static class NetWrapper
	{
		public static string Host
		{
			get
			{
				return System.Net.Dns.GetHostName();
			}
		}

		public static string HostIP
		{
			get
			{
				string res = "";
				// Then using host name, get the IP address list..
				var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
				foreach (var addr in host.AddressList)
				{
					if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						res += addr.ToString();
				}
				return res;
			}
		}

		public static string UserName
		{
			get
			{
				return System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("\\", "/");
			}
		}
	}

}
