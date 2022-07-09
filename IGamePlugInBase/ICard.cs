namespace IGamePlugInBase
{
    /// <summary>
    /// The Information of a Card that defines Information in the Deck Builder
    /// </summary>
    public interface ICard
    {
        /// <summary>
        /// ID of the Card to be saved in the Deck File.
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// Name of the Card to be shown on the Deck Builder.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A set of Alternate Arts the card has. All cards must have at least 1 art.
        /// </summary>
        public Dictionary<string, AlternateArt> AltArts { get; }

        /// <summary>
        /// The Text Details of the Card to be shown on the Deck Builder
        /// </summary>
        public string ViewDetails { get; }
    }
}
