namespace IGamePlugInBase.IO
{
    /// <summary>
    /// Interface used to Download Necessary Files from the Internet
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// The URL which users users should download new updates from. Please only use trusted Websites.
        /// Preferably GitLab or GitHub with the Source Code.
        /// </summary>
        public string DownloadLink { get; }

        /// <summary>
        /// Used to Download any necessary files for the Plug-In.
        /// Make sure to use proper Exception-Handling and throwing.
        /// </summary>
        /// <returns>The Asynchronous Task that is run.</returns>
        public Task DownloadFiles();
    }
}
