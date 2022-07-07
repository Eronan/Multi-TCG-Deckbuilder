namespace IGamePlugInBase
{
    /// <summary>
    /// A Deck within the overall Deck. (e.g. Main Deck, Side Deck)
    /// </summary>
    public class Deck
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
        /// The Function should take in a Card (the Card to be Added), a list of Cards (the current Deck)
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        public Func<DeckBuilderCard, IEnumerable<DeckBuilderCard>, Dictionary<string, IEnumerable<DeckBuilderCard>>, bool> ValidateAdd { get; set; }

        /// <summary>
        /// The Function used to determine whether a Deck is valid to the Rules.
        /// The Function should take in a list of Cards (the Deck).
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        public Func<IEnumerable<DeckBuilderCard>, bool> ValidateDeck { get; set; }

        /// <summary>
        /// Constructor for a Deck that will allow a Special Card to be Chosen
        /// </summary>
        /// <param name="name">Name of the Deck</param>
        /// <param name="label">Label that appears in the Deck Builder</param>
        /// <param name="expectedDeckSize">The Size the Deck is expected to be normally</param>
        /// <param name="validateAdd">Function that Validates if a card can be Added to the Deck</param>
        /// <param name="validateDeck">Function that Validates if the Deck is legal</param>
        public Deck(string name, string label, int expectedDeckSize, Func<DeckBuilderCard, IEnumerable<DeckBuilderCard>, Dictionary<string, IEnumerable<DeckBuilderCard>>, bool> validateAdd,
            Func<IEnumerable<DeckBuilderCard>, bool> validateDeck)
        {
            this.Name = name;
            this.Label = label;
            this.ExpectedDeckSize = expectedDeckSize;
            this.ValidateAdd = validateAdd;
            this.ValidateDeck = validateDeck;
        }
    }
}
