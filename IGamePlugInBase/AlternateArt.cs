namespace IGamePlugInBase
{
    public enum CardArtOrientation
    {
        Portrait = 0,
        Landscape = 1
    }

    /// <summary>
    /// An instance of an Art for a Specific Card.
    /// It shows up as a separate Card Instance on the Deck Builder.
    /// </summary>
    public interface AlternateArt
    {
        /// <summary>
        /// ID of the Alternate Art to be saved into the Deck File.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Location of the Image to be shown on the Deck Builder
        /// Can be loaded from a File Location externally or internally.
        /// </summary>
        public string ImageLocation { get; }
        /// <summary>
        /// The Orientation of the Image for the Card.
        /// Used to Rotate the Card in Export Image and TableTop
        /// </summary>
        public CardArtOrientation ArtOrientation { get; }
    }
}
