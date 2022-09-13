namespace IGamePlugInBase.IO
{
    public class UrlToFile
    {
        /// <summary>
        /// Initializes a <see cref="UrlToFile"/> with the necessary Url and FilePath.
        /// </summary>
        /// <param name="url">The URL of the Raw File on the Internet. Should not be the URL of a Web Page.</param>
        /// <param name="fileName">The relative File Path of the new File.</param>
        public UrlToFile(string url, string fileName)
        {
            Url = new Uri(url);
            FileName = fileName;
        }

        /// <summary>
        /// Uri for the Raw File.
        /// It should not be to a WebPage containing the Download Link.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Relative Path of where the File should be Downloaded to.
        /// The Relative Path should be in respect to the Multi-TCG Deck Builder Program.
        /// </summary>
        public string FileName { get; set; }
    }
}
