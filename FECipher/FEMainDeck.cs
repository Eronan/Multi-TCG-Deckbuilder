using IGamePlugInBase;

namespace FECipher
{
    public class FEMainDeck : IDeck
    {
        public string Name { get => "main"; }

        public string Label { get => "Main Deck"; }

        public int ExpectedDeckSize { get => 49; }

        public bool ValidateAdd(DeckBuilderCard card, IEnumerable<DeckBuilderCard> deck)
        {
            return true;
        }

        public bool ValidateDeck(IEnumerable<DeckBuilderCard> deck)
        {
            return deck.Count() >= 49;
        }
    }
}
