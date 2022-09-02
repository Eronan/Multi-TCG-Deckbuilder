namespace IGamePlugInBase
{
    public enum CardArtOrientation
    {
        Portrait,
        Landscape,
    }

    public abstract class DeckBuilderCardArt : DeckBuilderCard
    {
        protected DeckBuilderCardArt(string cardID, string artID, string name, string fileLocation, CardArtOrientation orientation, string viewDetails = "") : base(cardID, artID)
        {
            this.Name = name;
            this.FileLocation = fileLocation;
            this.Orientation = orientation;
            this.ViewDetails = viewDetails;
        }

        public string Name { get; private set; }

        public string FileLocation { get; private set; }

        public CardArtOrientation Orientation { get; private set; }

        public string ViewDetails { get; private set; }
    }
}
