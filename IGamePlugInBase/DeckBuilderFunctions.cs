using System.Text;

namespace IGamePlugInBase
{
    public abstract class DeckBuilderFunctions
    {
        /// <summary>
        /// Fields used by the Deck Builder to build the Advanced Search
        /// </summary>
        public IEnumerable<SearchField> SearchFields { get; } = new SearchField[0];

        /// <summary>
        /// Function called before opening a Format in the Deck Builder Window.
        /// Any time-consuming actions should be run here.
        /// </summary>
        public void InitializeService()
        {

        }

        /// <summary>
        /// Determines whether a card has reached the Maximum Allowable number of copies in a Decklist.
        /// </summary>
        /// <param name="card">Card that is being added.</param>
        /// <param name="decks">All Cards in each Deck.</param>
        /// <returns>The card has reached the number of copies in the Deck.</returns>
        public bool ValidateMaximum(DeckBuilderCard card, Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            return false;
        }

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. It should be smaller, and fit on the Deck Builder Window's Button.</returns>
        public string GetStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            return "Detailed Stats";
        }

        /// <summary>
        /// Returns Text detailing the types of cards in the Decks.
        /// </summary>
        /// <param name="decks">All the Cards in the Deck List separated by which Deck they are in.</param>
        /// <returns>The Text that is displayed with Labels and Counts. The text will show on a separate Window.</returns>
        public string GetDetailedStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var deck in decks)
            {
                stringBuilder.Append(deck.Key);
                stringBuilder.Append(": ");
                stringBuilder.Append(deck.Value.Count());
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// A Function that removes cards from the List based on searchFields.
        /// </summary>
        /// <param name="cards">The List of Cards to be Filtered.</param>
        /// <param name="searchFields">All the Search Fields and their Values.</param>
        /// <returns>List of Cards from cards that matching searchFields.</returns>
        public IEnumerable<DeckBuilderCardArt> AdvancedFilterSearchList(IEnumerable<DeckBuilderCardArt> cards, IEnumerable<SearchField> searchFields)
        {
            return cards;
        }

        /// <summary>
        /// Used to Sort Card Lists
        /// </summary>
        /// <param name="x">First Card</param>
        /// <param name="y">Second Card</param>
        /// <returns>If x precedes y, it returns a number less than 0, if x and y are the same it returns 0, and if y precedes x it returns a number greater than 0.</returns>
        public int CompareCards(DeckBuilderCardArt x, DeckBuilderCardArt y)
        {
            return x.CardID.CompareTo(x.CardID);
        }

        
        /// <summary>
        /// Gets the Deck that the Card should be added to by Default.
        /// </summary>
        /// <param name="card">Card which is being added.</param>
        /// <returns>The Index of the Deck that the card will be added to by Default.</returns>
        public string DefaultDeckName(DeckBuilderCard card)
        {
            return "";
        }
    }
}
