using System.Drawing.Imaging;

namespace IGamePlugInBase
{
    public class Format
    {
        string name;
        string longName;
        byte[] icon;
        string description;
        Card[] cardList;

        public Format(string name, string longName, byte[] iconImage, string description, Card[] cards)
        {
            this.name = name;
            this.longName = longName;
            this.icon = iconImage;
            this.description = description;
            this.cardList = cards;
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
    }
}
