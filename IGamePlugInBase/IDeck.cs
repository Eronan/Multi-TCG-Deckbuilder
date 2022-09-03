namespace IGamePlugInBase
{
    /// <summary>
    /// A Deck within the overall Deck. (e.g. Main Deck, Side Deck)
    /// Is Defined by Format.
    /// </summary>
    public interface IDeck
    {
        /// <summary>
        /// Name of the Deck to be read and saved from/into the Deck File
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Name of the Deck to be shown on the Deck Builder Window
        /// </summary>
        public string Label { get; }
        /// <summary>
        /// The Expected Size of the Deck.
        /// Used to determine the Size of the ListBox on the Deck Builder Window.
        /// It is not the Minimum or Maximum, it merely denotes the number that is most commonly played in this Deck.
        /// </summary>
        public int ExpectedDeckSize { get; }

        //Methods
        /// <summary>
        /// The Function used to determine whether a card can be added to the Deck.
        /// </summary>
        /// <param name="card">Card to be Added to the Deck.</param>
        /// <param name="deck">Deck to be used for Validating.</param>
        /// <returns>Card is allowed to be Added to the Deck.</returns>
        public bool ValidateAdd(DeckBuilderCard card, IEnumerable<DeckBuilderCard> deck);

        /// <summary>
        /// The Function used to determine whether a Deck is valid to the Rules.
        /// The Function should take in a list of Cards (the Deck).
        /// The Function should output a string array listing the errors with the Deck.
        /// </summary>
        /// <param name="deck">A List of Cards within the Deck</param>
        /// <returns>A list of Messages saying what's wrong with the Deck.</returns>
        public string[] ValidateDeck(IEnumerable<DeckBuilderCard> deck);
    }
}
