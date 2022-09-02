namespace IGamePlugInBase.IO
{
    /// <summary>
    /// An Interface used to define a Menu Item and its Action.
    /// </summary>
    public interface IExportMenuItem
    {
        /// <summary>
        /// The Text for the Menu Item
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// The Default File Extension for the File. (e.g. *.mtdk)
        /// </summary>
        public string DefaultExtension { get; }

        /// <summary>
        /// Filter to get Specific Files (e.g. "Multi-TCG Deck Builder File (.mtdk)|*.mtdk")
        /// </summary>
        public string FileFilter { get; }

        /// <summary>
        /// The Function performed in order to Export to a File.
        /// </summary>
        /// <param name="filePath">The File Location that it Exports To.</param>
        public void Export(string filePath, DeckBuilderDeckFile decks);
    }
}
