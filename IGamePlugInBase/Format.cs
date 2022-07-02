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
        string defaultDeckName;

        public Format(string name, string longName, byte[] iconImage, string description, Card[] cards, Deck[] decks, string defaultDeckName)
        {
            this.name = name;
            this.longName = longName;
            this.icon = iconImage;
            this.description = description;
            this.cardList = cards;
            this.decks = decks;
            this.defaultDeckName = defaultDeckName;
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
        public string DefaultDeckName
        {
            get { return this.defaultDeckName; }
        }
    }
}
