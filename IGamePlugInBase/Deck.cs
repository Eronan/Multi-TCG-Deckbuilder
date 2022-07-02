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
        /// A Feature to Mark a Specific Card as a Special Card
        /// </summary>
        public bool MarkSpecialCard { get; }
        /// <summary>
        /// The Label to be used when Marking the Special Card
        /// </summary>
        public string? SpecialCardLabel { get; }
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
        public Func<DeckBuilderCard, IEnumerable<DeckBuilderCard>, bool> ValidateAdd { get; set; }

        /// <summary>
        /// The Function used to determine whether a Deck is valid to the Rules.
        /// The Function should take in a list of Cards (the Deck).
        /// The Function should output a boolean value. True if it is valid, and False if it is not.
        /// </summary>
        public Func<IEnumerable<DeckBuilderCard>, bool> ValidateDeck { get; set; }
        /// <summary>
        /// The Function used to determine whether a card can be Marked as Special.
        /// The Function should take in a Card (the card to be Marked).
        /// THe FUnction should output a boolean value. True if it can be marked, and False if it cannot.
        /// </summary>
        public Func<DeckBuilderCard, bool>? ValidateMarkSpecial { get; set; }

        public Deck(string name, string label, int expectedDeckSize, Func<DeckBuilderCard, IEnumerable<DeckBuilderCard>, bool> validateAdd,
            Func<IEnumerable<DeckBuilderCard>, bool> validateDeck, bool markSpecialCard, string? specialCardLabel = null,
            Func<DeckBuilderCard, bool>? validateMarkSpecial = null)
        {
            this.Name = name;
            this.Label = label;
            this.MarkSpecialCard = markSpecialCard;
            this.ExpectedDeckSize = expectedDeckSize;
            this.ValidateAdd = validateAdd;
            this.ValidateDeck = validateDeck;

            if (markSpecialCard)
            {
                this.SpecialCardLabel = specialCardLabel;
                this.ValidateMarkSpecial = validateMarkSpecial;
            }
        }
    }
}
