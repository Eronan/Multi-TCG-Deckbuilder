namespace IGamePlugInBase
{
    /// <summary>
    /// A Format within a Game.
    /// </summary>
    public class Format
    {
        string name;
        string longName;
        byte[] icon;
        string description;
        Card[] cardList;
        Deck[] decks;

        /// <summary>
        /// Constructor for a Format
        /// </summary>
        /// <param name="name">Short Name of the Format</param>
        /// <param name="longName">Long Name of the Format</param>
        /// <param name="iconImage">Icon used for the Format</param>
        /// <param name="description">Description of the Format</param>
        /// <param name="cards">Cards Available for the Format</param>
        /// <param name="decks">All Decks that appear in the Format</param>
        /// <param name="defaultDeckName">Name of the Deck that is always used as the Default</param>
        /// <param name="validateMaximum">A Function that determines whether a card has reached the maximum number of copies allowed.</param>
        public Format(string name, string longName, byte[] iconImage, string description, Card[] cards, Deck[] decks, string defaultDeckName, Func<DeckBuilderCard, Dictionary<string, IEnumerable<DeckBuilderCard>>, bool> validateMaximum)
        {
            this.name = name;
            this.longName = longName;
            this.icon = iconImage;
            this.description = description;
            this.cardList = cards;
            this.decks = decks;
            this.DefaultDeckName = delegate (DeckBuilderCard card)
            {
                return defaultDeckName;
            };
            this.ValidateMaximum = validateMaximum;
        }

        /// <summary>
        /// Constructor for a Format
        /// </summary>
        /// <param name="name">Short Name of the Format</param>
        /// <param name="longName">Long Name of the Format</param>
        /// <param name="iconImage">Icon used for the Format</param>
        /// <param name="description">Description of the Format</param>
        /// <param name="cards">Cards Available for the Format</param>
        /// <param name="decks">All Decks that appear in the Format</param>
        /// <param name="defaultDeckFunction">A Function that determines which Deck a Card goes into by default.</param>
        /// <param name="validateMaximum">A Function that determines whether a card has reached the maximum number of copies allowed.</param>
        public Format(string name, string longName, byte[] iconImage, string description, Card[] cards, Deck[] decks, Func<DeckBuilderCard, string> defaultDeckFunction, Func<DeckBuilderCard, Dictionary<string, IEnumerable<DeckBuilderCard>>, bool> validateMaximum)
        {
            this.name = name;
            this.longName = longName;
            this.icon = iconImage;
            this.description = description;
            this.cardList = cards;
            this.decks = decks;
            this.DefaultDeckName = defaultDeckFunction;
            this.ValidateMaximum = validateMaximum;
        }

        /// <summary>
        /// Short Name of the Format to be saved into the Deck File.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Long Name of the Format to be shown in the Load Plugin Window.
        /// </summary>
        public string LongName
        {
            get { return this.longName; }
        }

        /// <summary>
        /// Image used to represent the Format in the Load Plugin Window.
        /// Can be loaded internally or externally.
        /// </summary>
        public byte[] Icon
        {
            get { return this.icon; }
        }

        /// <summary>
        /// Description of the Format to be shown in the Load Plugin Window.
        /// Basic Differences should be explained.
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// A Card List exclusive to the Format.
        /// Should be grabbed the Game Class.
        /// </summary>
        public Card[] CardList
        {
            get { return this.cardList; }
        }

        /// <summary>
        /// The Decks that Compromise a Proper Game Deck in the Format
        /// </summary>
        public Deck[] Decks
        {
            get { return this.decks; }
        }

        /// <summary>
        /// The Default Deck that a card is added to when Right-Clicked.
        /// </summary>
        public Func<DeckBuilderCard, string> DefaultDeckName { get; set; }

        /// <summary>
        /// Determines whether the Card has already reached the Maximum number of copies allowed in a Deck.
        /// This can be determined by any card field. The Function can contain statements for specific cards to allow more copies.
        /// The Function should take in the Card that is being added.
        /// The Function should take in a Dictionary, where each List of Cards (Deck) is assigned to the name of a Deck.
        /// </summary>
        public Func<DeckBuilderCard, Dictionary<string, IEnumerable<DeckBuilderCard>>, bool> ValidateMaximum { get; set; }
    }
}
