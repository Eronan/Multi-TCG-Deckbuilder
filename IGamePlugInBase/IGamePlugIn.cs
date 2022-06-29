namespace IGamePlugInBase
{
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
        /// A List of Formats the Game Plug-In Supports
        /// </summary>
        public Format[] Formats { get; }
        /// <summary>
        /// The Card List for the Plug-In, can be loaded from an external file or internally created by the plug-in.
        /// </summary>
        Card[] CardList { get; }
    }
}
