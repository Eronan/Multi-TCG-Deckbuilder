namespace IGamePlugInBase
{
    /// <summary>
    /// Determiens which Orientation a card is in: Portrait or Landscape
    /// </summary>
    public enum CardArtOrientation
    {
        Portrait,
        Landscape,
    }

    /// <summary>
    /// Utilised by the Deck Builder to display necessary Information
    /// </summary>
    public class DeckBuilderCardArt : DeckBuilderCard
    {
        /// <summary>
        /// Initializes DeckBuilderCadrArt
        /// </summary>
        /// <param name="cardID">Card Identifier</param>
        /// <param name="artID">Art Identifier</param>
        /// <param name="name">Name of the Card to be shown in the Deck Builder</param>
        /// <param name="fileLocation">Relative Path for the Image to be Displayed</param>
        /// <param name="orientation">The Orientation the Image is in.</param>
        /// <param name="viewDetails">The Description and Information displayed when a card is Selected.</param>
        public DeckBuilderCardArt(string cardID, string artID, string name, string fileLocation, string downloadLocation, CardArtOrientation orientation, string viewDetails) : base(cardID, artID)
        {
            Name = name;
            FileLocation = fileLocation;
            DownloadLocation = downloadLocation;
            Orientation = orientation;
            ViewDetails = viewDetails;
        }
        

        /// <summary>
        /// Card Name that is Displayed
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Relative Path for the Card's Image File
        /// </summary>
        public string FileLocation { get; set; }

        /// <summary>
        /// URL for the Program to download an Image from Online if the Image is missing.
        /// </summary>
        public string DownloadLocation { get; set; }

        /// <summary>
        /// Orientation in which its FileLocation Image is
        /// </summary>
        public CardArtOrientation Orientation { get; set; }

        /// <summary>
        /// Description about the Card
        /// </summary>
        public string ViewDetails { get; set; }
    }
}
