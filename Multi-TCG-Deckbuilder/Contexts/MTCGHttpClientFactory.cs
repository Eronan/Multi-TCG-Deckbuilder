using IGamePlugInBase.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            var byteFile = await HttpClient.GetByteArrayAsync(urlToFile.Url);
            await File.WriteAllBytesAsync(urlToFile.FileName, byteFile);
        }

        public static async Task DownloadFile(string url, string fileLocation)
        {
            var byteFile = await HttpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(fileLocation, byteFile);
        }
    }
}
