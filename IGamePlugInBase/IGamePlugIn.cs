using IGamePlugInBase.IO;

namespace IGamePlugInBase
{
    /// <summary>
    /// The Information for the Plug-In.
    /// Only one class with this interface should be defined.
    /// </summary>
    public interface IGamePlugIn
    {
        //Variables
        /// <summary>
        /// Short Name of the Game Plug-In to be used recognising the Game from the Deck File.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Long Name of the Game Plug-In to be shown on the Load Plugin Window.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Image shown on the Load Plugin Window.
        /// </summary>
        public byte[] IconImage { get; }

        /// <summary>
        /// The Text that shows when the User presses on the "About Plug-In" Menu Item.
        /// It is recommended to place links to important websites to update the Plug-In here.
        /// </summary>
        public string AboutInformation { get; }

        /// <summary>
        /// A List of Formats that are opened from the Plug-In.
        /// </summary>
        public IEnumerable<IFormat> Formats { get; }

        /// <summary>
        /// A Service used to Download Necessary Files
        /// </summary>
        public IDownloader? Downloader { get => null; }

        /// <summary>
        /// All of the Import Menu Items that should be defined for the Plug-In.
        /// </summary>
        public IEnumerable<IImportMenuItem>? ImportMenus { get => null; }

        /// <summary>
        /// All of the Export Menu Items that should be defined for the Plug-In.
        /// </summary>
        public IEnumerable<IExportMenuItem>? ExportMenus { get => null; }
    }
}
