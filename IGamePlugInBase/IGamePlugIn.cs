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
    }
}
