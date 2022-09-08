namespace IGamePlugInBase.IO
{
    /// <summary>
    /// Interface used to Download Necessary Files from the Internet
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// The URL which users users should download new versions of the Plug-In from. Please only use trusted Websites.
        /// Preferably GitLab or GitHub with the Source Code.
        /// </summary>
        public string DownloadLink { get; }

        /// <summary>
        /// A list of files that need to be downloaded for the Program to Run.
        /// Images should not be kept here, they are downloaded as the Program Runs.
        /// </summary>
        public IEnumerable<UrlToFile> FileDownloads { get; }
    }
}
