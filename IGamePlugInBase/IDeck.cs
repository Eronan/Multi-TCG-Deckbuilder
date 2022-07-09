namespace IGamePlugInBase
{
    /// <summary>
    /// A Deck within the overall Deck. (e.g. Main Deck, Side Deck)
    /// </summary>
    public interface IDeck
    {
        /// <summary>
        /// Name of the Deck to be saved into the Deck File
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
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        /// <param name="deck"></param>
        /// <returns>Deck follows the rules of the Game.</returns>
        public bool ValidateDeck(IEnumerable<DeckBuilderCard> deck);
    }
}
