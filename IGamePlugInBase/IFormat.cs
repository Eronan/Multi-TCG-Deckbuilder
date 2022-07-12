namespace IGamePlugInBase
{
    /// <summary>
    /// A Format within a Game.
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Short Name of the Format to be saved into the Deck File.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Long Name of the Format to be shown in the Load Plugin Window.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Image used to represent the Format in the Load Plugin Window.
        /// Can be loaded internally or externally.
        /// </summary>
        public byte[] Icon { get; }

        /// <summary>
        /// Description of the Format to be shown in the Load Plugin Window.
        /// Basic Differences should be explained.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A Card List exclusive to the Format.
        /// Should be grabbed from the Game Class.
        /// </summary>
        public ICard[] CardList { get; }

        /// <summary>
        /// The Decks that Compromise a Proper Game Deck in the Format
        /// </summary>
        public IDeck[] Decks { get; }

        /// <summary>
        /// Gets the Deck that the Card should be added to by Default.
        /// </summary>
        /// <param name="card">Card which is being added.</param>
        /// <returns>The Deck Name that the card will be added to by Default.</returns>
        public string DefaultDeckName(DeckBuilderCard card);

        /// <summary>
        /// Determines whether a card has reached the Maximum Allowable number of copies in a Decklist.
        /// </summary>
        /// <param name="card">Card that is being added.</param>
        /// <param name="decks">All Cards in each Deck.</param>
        /// <returns></returns>
        public bool ValidateMaximum(DeckBuilderCard card, Dictionary<string, IEnumerable<DeckBuilderCard>> decks);

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. It should be smaller, and fit on the Deck Builder Window's Button.</returns>
        public string GetStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks);

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. The text will show on a separate Window.</returns>
        public string GetDetailedStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks);
    }
}
