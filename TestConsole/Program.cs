using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            ///TODO kill  System.Web.Script.Serialization.JavaScriptSerializer sr;

            /*
            System.Net.IPAddress addr = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
            System.Net.Sockets.TcpListener srv = new System.Net.Sockets.TcpListener(addr, 3307);
            srv.Start();
            Console.WriteLine("TCP internal server started:\n" + srv.Server.Connected.ToString() + "\n\n");
            */
            BackendServer srv2 = new BackendServer();
            srv2.DoWork();

            Console.WriteLine("All available network interfaces:\n");
            var instances = Utilities.GetNetworkInterfaces();

            for (var i = 0; i < instances.Count; i++)
            {
                Console.WriteLine(i + ": " + instances[i]);
            }


  

            var choice = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Selected network interface:\n" + instances[choice] + "\n\n");

            while (true)
            {
                var stats = Utilities.GetNetworkStatistics(instances[choice]);
                Console.WriteLine("Download speed: " + stats.DownloadSpeed + " KBytes/s");
                Console.WriteLine("Upload speed: " + stats.UploadSpeed + " KBytes/s");
                Console.WriteLine("--------------------------------------------------------------\n\n");
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
