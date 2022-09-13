using System.Text.Json.Serialization;

namespace IGamePlugInBase.IO
{
    /// <summary>
    /// A Deck within an overall Decklist File (.mtdk).
    /// </summary>
    public class DeckBuilderDeck
    {
        /// <summary>
        /// The Short Name of the Deck
        /// </summary>
        public string DeckName { get; set; }

        /// <summary>
        /// The Cards that were added to the Deck
        /// </summary>
        public DeckBuilderCard[] Cards { get; set; }

        /// <summary>
        /// Constructor that Initializes the Deck
        /// </summary>
        /// <param name="deckName">Short Name of the Deck</param>
        /// <param name="cards">The List of Cards added to the Deck.</param>
        public DeckBuilderDeck(string deckName, IEnumerable<DeckBuilderCard> cards)
        {
            DeckName = deckName;
            Cards = cards.ToArray();
        }

        /// <summary>
        /// Constructor that Initializes the Deck
        /// </summary>
        /// <param name="deckName">Short Name of the Deck</param>
        /// <param name="cards">The List of Cards added to the Deck.</param>
        [JsonConstructor]
        public DeckBuilderDeck(string deckName, DeckBuilderCard[] cards)
        {
            DeckName = deckName;
            Cards = cards.ToArray();
        }
    }
}
