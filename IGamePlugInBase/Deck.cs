namespace IGamePlugInBase
{
    /// <summary>
    /// The Decks that exist within the Game.
    /// </summary>
    public interface Deck
    {
        /// <summary>
        /// Name of the Deck to be saved into the Deck File
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Name of the Deck to be shown on the Deck Builder Window
        /// </summary>
        public string Label { get; }

        //Methods
        /// <summary>
        /// The Function used to determine whether a card can be added to the Deck.
        /// The Function should take in a Card (the Card to be Added), a list of Cards (the current Deck)
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        public Func<DeckBuilderCard, DeckBuilderCard[], bool> ValidateAdd { get; set; }

        /// <summary>
        /// The Function used to determine whether a Deck is valid to the Rules.
        /// The Function should take in a list of Cards (the Deck).
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        public Func<DeckBuilderCard[], bool> ValidateDeck { get; set; }
    }
}
