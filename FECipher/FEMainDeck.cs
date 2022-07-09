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

        public string[] ValidateDeck(IEnumerable<DeckBuilderCard> deck)
        {
            if (deck.Count() < 49) { return new string[1] { "You must have 50 cards in the Deck including the Main Character." }; }
            return new string[0];
        }
    }
}
