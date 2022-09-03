namespace IGamePlugInBase
{
    /// <summary>
    /// A Format defined within a Plug-In.
    /// Any number of Formats can be defined.
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Short Name of the Format to be saved into the Deck File.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Long Name of the Format to be shown in the Load Plugin Window.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Image used to represent the Format in the Load Plugin Window.
        /// Can be loaded internally or externally.
        /// </summary>
        public byte[] Icon { get; }

        /// <summary>
        /// Description of the Format to be shown in the Load Plugin Window.
        /// Basic Differences should be explained.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The Decks that Compromise a Proper Game Deck in the Format
        /// </summary>
        public IEnumerable<IDeck> Decks { get; }

        /// <summary>
        /// An abstract class to implement the Functions used in Deck Building for the Format.
        /// </summary>
        public IDeckBuilderService DeckBuilderService { get => new NullDeckBuilderService(); }
    }
}
