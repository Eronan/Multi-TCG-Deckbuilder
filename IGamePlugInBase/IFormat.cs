namespace IGamePlugInBase
{
    /// <summary>
    /// A Format defined within a Plug-In.
    /// Any number of Formats can be defined for a Game Plug-In.
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Short Name of the Format to be saved into the Deck File.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Long Name of the Format to be shown in the Deck Builder Program.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Image used to represent the Format in the Deck Builder Program.
        /// Can be loaded internally or externally.
        /// </summary>
        public byte[] Icon { get; }

        /// <summary>
        /// Description of the Format to be shown in the Deck Builder Program.
        /// Basic Differences between this Format and other Formats should be explained.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A collection of Decks that make up a Decklist in the Format.
        /// </summary>
        public IEnumerable<IDeck> Decks { get; }

        /// <summary>
        /// The necessary Functions that are used to verify Legal Deck Building in the Format.
        /// </summary>
        public IDeckBuilderService DeckBuilderService { get => new NullDeckBuilderService(); }
    }
}
