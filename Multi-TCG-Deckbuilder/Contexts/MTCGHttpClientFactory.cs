using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
        private static HttpClient? _httpClient;
        public static List<string> FileNames = new List<string>();

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
            if (FileNames.Contains(urlToFile.FileName)) { return; }

            FileNames.Add(urlToFile.FileName);

            string? directoryPath = Path.GetDirectoryName(urlToFile.FileName);
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var byteFile = await HttpClient.GetByteArrayAsync(urlToFile.Url).ConfigureAwait(false);
            await File.WriteAllBytesAsync(urlToFile.FileName, byteFile);
            FileNames.Remove(urlToFile.FileName);
        }

        public static async Task DownloadFile(string url, string fileLocation)
        {
            if (FileNames.Contains(fileLocation)) { return; }

            FileNames.Add(fileLocation);

            string? directoryPath = Path.GetDirectoryName(fileLocation);
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var byteFile = await HttpClient.GetByteArrayAsync(url).ConfigureAwait(false);
            await File.WriteAllBytesAsync(fileLocation, byteFile);
            FileNames.Remove(fileLocation);
        }
    }
}
