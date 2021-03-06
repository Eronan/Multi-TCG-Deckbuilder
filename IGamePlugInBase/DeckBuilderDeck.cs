using System.Text.Json.Serialization;

namespace IGamePlugInBase
{
    public class DeckBuilderDeck
    {
        public string DeckName { get; set; }
        public DeckBuilderCard[] Cards { get; set; }

        public DeckBuilderDeck(string deckName, IEnumerable<DeckBuilderCard> cards)
        {
            DeckName = deckName;
            Cards = cards.ToArray();
        }

        [JsonConstructor]
        public DeckBuilderDeck(string deckName, DeckBuilderCard[] cards)
        {
            DeckName = deckName;
            Cards = cards.ToArray();
        }
    }
}
