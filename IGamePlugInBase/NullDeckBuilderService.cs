namespace IGamePlugInBase
{
    /// <summary>
    /// Used as the Default Deck Builder Service when not specfied.
    /// </summary>
    internal class NullDeckBuilderService : IDeckBuilderService
    {
        /// <summary>
        /// Empty Array of Advanced Search Fields
        /// </summary>
        public IEnumerable<SearchField> SearchFields => Array.Empty<SearchField>();

        /// <summary>
        /// Empty Array of Cards
        /// </summary>
        public IEnumerable<DeckBuilderCardArt> CardList => Array.Empty<DeckBuilderCardArt>();
    }
}
