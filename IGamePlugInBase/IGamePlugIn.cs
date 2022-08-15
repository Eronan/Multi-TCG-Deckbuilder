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
        public IFormat[] Formats { get; }
        /// <summary>
        /// The Card List for the Plug-In, can be loaded from an external file or internally created by the plug-in.
        /// </summary>
        ICard[] CardList { get; }
        /// <summary>
        /// A List of Menu Item Functions that the Plug-In can Import from.
        /// </summary>
        public ImportMenuItem[] ImportFunctions { get; }
        /// <summary>
        /// A List of Menu Item Functions that the Plug-In can Export to.
        /// </summary>
        public ExportMenuItem[] ExportFunctions{ get; }
        /// <summary>
        /// The Text that shows when the User presses on the "About Plug-In" Menu Item.
        /// It is recommended to place links to important websites to update the Plug-In here.
        /// </summary>
        public string AboutInformation { get; }
        /// <summary>
        /// Fields used by the Deck Builder to build the Advanced Search
        /// </summary>
        public SearchField[] SearchFields { get; }
        /// <summary>
        /// A Function that removes cards from the List based on searchFields.
        /// </summary>
        /// <param name="cards">The List of Cards to be Filtered.</param>
        /// <param name="searchFields">All the Search Fields and their Values.</param>
        /// <returns>List of Cards from cards that matching searchFields.</returns>
        public List<DeckBuilderCard> AdvancedFilterSearchList(IEnumerable<DeckBuilderCard> cards, SearchField[] searchFields);
        /// <summary>
        /// Used to Sort Card Lists
        /// </summary>
        /// <param name="x">First Card</param>
        /// <param name="y">Second Card</param>
        /// <returns>If x precedes y, it returns a number less than 0, if x and y are the same it returns 0, and if y precedes x it returns a number greater than 0.</returns>
        public int CompareCards(DeckBuilderCard x, DeckBuilderCard y);
        /// <summary>
        /// Updates the Plug-In
        /// </summary>
        /// <returns>The URL to find the Update</returns>
        public string UpdatePlugIn();
    }
}
