using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace TestConsole
{
    class networks
    {
    }

    //src:   https://codereview.stackexchange.com/questions/28367/basic-network-utilisation-display
    public interface IStatistics
    {
        string NetworkInterface { get; }
        float DataSent { get; }
        float DataReceived { get; }
        float UploadSpeed { get; }
        float DownloadSpeed { get; }
        Queue<float> LatestDownTransfers { get; }
        Queue<float> LatestUpTransfers { get; }
    }

    public class Statistics : IStatistics
    {
        public Statistics(string name)
        {
            NetworkInterface = name;
            LatestDownTransfers = new Queue<float>(3);
            LatestUpTransfers = new Queue<float>(3);
        }

        // <summary>
        // Holds the name of the selected network interface
        // </summary>
        public string NetworkInterface { get; set; }

        // <summary>
        // Contains the data sent in the most recent time interval
        // </summary>
        public float DataSent { get; set; }

        // <summary>
        // Contains the data received in the most recent time interval
        // </summary>
        public float DataReceived { get; set; }

        // <summary>
        // Returns the upload speed in KiloBytes / Second
        // </summary>
        public float UploadSpeed
        {
            get { return LatestUpTransfers.Sum() / LatestUpTransfers.Count / 1028 / StatisticsFactory.MULTIPLIER; }
        }

        // <summary>
        // Returns the download speed in KiloBytes / Second
        // </summary>
        public float DownloadSpeed
        {
            get { return LatestDownTransfers.Sum() / LatestDownTransfers.Count / 1028 / StatisticsFactory.MULTIPLIER; }
        }

        // <summary>
        // Contains the data received in the three most recent time intervals
        // </summary>
        public Queue<float> LatestDownTransfers { get; set; }

        // <summary>
        // Contains the data sent in the three most recent time intervals
        // </summary>
        public Queue<float> LatestUpTransfers { get; set; }
    }

    public static class StatisticsFactory
    {
        private static Queue<float> _latestDownTransfers = new Queue<float>();
        private static Queue<float> _latestUpTransfers = new Queue<float>();
        private static Statistics stats;

        public const int MULTIPLIER = 25;

        // <summary>
        // Creates a new statistic and uses the latest transferrates from the previous stats
        // </summary>
        public static Statistics CreateStatistics(string interfaceName)
        {
            stats = new Statistics(interfaceName)
            {
                LatestDownTransfers = _latestDownTransfers,
                LatestUpTransfers = _latestUpTransfers
            };

            return stats;
        }

        // <summary>
        // Adds a value to the current running statistics summary's upload list
        // </summary>
        public static void AddSentData(float d)
        {
            stats.DataSent = d;
            if (_latestUpTransfers.Count == 3)
            {
                _latestUpTransfers.Dequeue();
            }
            _latestUpTransfers.Enqueue(d);

            stats.LatestUpTransfers = _latestUpTransfers;
        }

        // <summary>
        // Adds a value to the current running statistics summary's download list
        // </summary>
        public static void AddReceivedData(float d)
        {
            stats.DataReceived = d;

            if (_latestDownTransfers.Count == 3)
            {
                _latestDownTransfers.Dequeue();
            }

            _latestDownTransfers.Enqueue(d);

            stats.LatestDownTransfers = _latestDownTransfers;
        }
    }

    public static class Utilities
    {
        static Utilities()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
        }

        // <summary>
        // Returns statistics about the given network interface
        // </summary>
        public static IStatistics GetNetworkStatistics(string interfaceName)
        {
            var stats = StatisticsFactory.CreateStatistics(interfaceName);

            var dataSentCounter = new System.Diagnostics.PerformanceCounter("Network Interface", "Bytes Sent/sec", interfaceName);
            var dataReceivedCounter = new System.Diagnostics.PerformanceCounter("Network Interface", "Bytes Received/sec", interfaceName);

            float sendSum = 0;
            float receiveSum = 0;

            for (var index = 0; index < StatisticsFactory.MULTIPLIER; index++)
            {
                sendSum += dataSentCounter.NextValue();
                receiveSum += dataReceivedCounter.NextValue();
            }

            if (sendSum > 0 || receiveSum > 0)
            {
                StatisticsFactory.AddReceivedData(receiveSum);
                StatisticsFactory.AddSentData(sendSum);
            }

            return stats;
        }

        // <summary>
        // Returns a list of all available network interfaces
        // </summary>
        public static IList<string> GetNetworkInterfaces()
        {
            return new System.Diagnostics.PerformanceCounterCategory("Network Interface").GetInstanceNames().ToList();
        }
    }

    public class BackendServer
    {
        public void DoWork()
        {
            const int port = 8888;
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);

                // запуск слушателя
                server.Start();

                while (true)
                {
                    Console.WriteLine("Server listen :8888 ");

                    // получаем входящее подключение
                    TcpClient client = server.AcceptTcpClient();
                    
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");

                    // получаем сетевой поток для чтения и записи
                    NetworkStream stream = client.GetStream();

                   // string response = "Привет мир";
                    string responseFake = "{'Servicename': 'nodeapi' }";
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.UTF8.GetBytes(responseFake);

                    // отправка сообщения
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Response: {0}", responseFake);
                    // закрываем поток
                    stream.Close();
                    // закрываем подключение
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }
    }
}
