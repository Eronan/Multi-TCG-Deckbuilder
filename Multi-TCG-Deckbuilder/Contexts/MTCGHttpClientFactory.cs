using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Contexts
{
    internal class MTCGHttpClientFactory
    {
        //private Dictionary<string, HttpMessageHandler> handlers = new Dictionary<string, HttpMessageHandler>();
        public static HttpMessageHandler clientHandler = new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 3,
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5)
        };
        public static bool disableDownloading = false;
        private static HttpClient? _httpClient;
        private static int activeTasks = 0;
        private static int awaitingTasks = 0;

        public static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient(clientHandler);
                }
                return _httpClient;
            }
        }


        public static async Task DownloadFile(UrlToFile urlToFile)
        {
            string? directoryPath = Path.GetDirectoryName(urlToFile.FileName); 
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            while (activeTasks > 100)
            {

            }

            activeTasks++;
            var byteFile = await HttpClient.GetByteArrayAsync(urlToFile.Url).ConfigureAwait(false);
            activeTasks--;
            await File.WriteAllBytesAsync(urlToFile.FileName, byteFile);
        }

        public static async Task DownloadFile(string url, string fileLocation)
        {
            string? directoryPath = Path.GetDirectoryName(fileLocation);
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            awaitingTasks++;
            while (activeTasks > 100)
            {

            }

            activeTasks++;
            var byteFile = await HttpClient.GetByteArrayAsync(url).ConfigureAwait(false);
            activeTasks--;
            awaitingTasks--;
            await File.WriteAllBytesAsync(fileLocation, byteFile);
        }
    }
}
