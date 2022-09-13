using IGamePlugInBase.IO;

namespace IGamePlugInBase
{
    /// <summary>
    /// The Information for the Plug-In.
    /// Only one class with this interface should be defined in the Library.
    /// </summary>
    public interface IGamePlugIn
    {
        //Variables
        /// <summary>
        /// Short Name of the Game Plug-In to be used recognising the Game from the Deck File.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Long Name of the Game Plug-In to be shown on the Deck Builder Program.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Image shown on the Deck Builder Program.
        /// </summary>
        public byte[] IconImage { get; }

        /// <summary>
        /// The Text that shows when the User presses on the "About Plug-In" Menu Item.
        /// It is recommended to place links to important websites to update the Plug-In here.
        /// </summary>
        public string AboutInformation { get; }

        /// <summary>
        /// The List of Formats that a Game Plug-In Supports
        /// </summary>
        public IEnumerable<IFormat> Formats { get; }

        /// <summary>
        /// A Service used to Download Necessary Files for the Plug-In to Run.
        /// Be extremely careful with defining the Service.
        /// </summary>
        public IDownloader? Downloader { get => null; }

        /// <summary>
        /// All of the Import Menu Items that should be defined for the Plug-In.
        /// Loads a (.mtdk) Decklist File from non-(.mtdk) Files.
        /// </summary>
        public IEnumerable<IImportMenuItem>? ImportMenus { get => null; }

        /// <summary>
        /// All of the Export Menu Items that should be defined for the Plug-In.
        /// They create new Files based on a (.mtdk) File.
        /// </summary>
        public IEnumerable<IExportMenuItem>? ExportMenus { get => null; }
    }
}
