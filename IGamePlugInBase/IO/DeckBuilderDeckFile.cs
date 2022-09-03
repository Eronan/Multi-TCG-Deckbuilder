namespace IGamePlugInBase.IO
{
    /// <summary>
    /// (.mtdk) Decklist File that is Saved and Opened by the Deck Builder Program
    /// </summary>
    public class DeckBuilderDeckFile
    {
        /// <summary>
        /// Short Name of the Game Plug-In
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// Short Name of the Format the Decklist was built for.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// List of Decks and their Cards within the overall Decklist
        /// </summary>
        public DeckBuilderDeck[] Decks { get; set; }

        /// <summary>
        /// Initializes a Decklist File (.mtdk)
        /// </summary>
        /// <param name="game">Short Name of the Game Plug-In</param>
        /// <param name="format">Short Name of the Format.</param>
        /// <param name="decks">List of Decks and their Cards</param>
        public DeckBuilderDeckFile(string game, string format, DeckBuilderDeck[] decks)
        {
            Game = game;
            Format = format;
            Decks = decks;
        }
    }
}
