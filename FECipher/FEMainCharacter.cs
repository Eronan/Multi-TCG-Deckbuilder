using IGamePlugInBase;

namespace FECipher
{
    public class FEMainCharacter : IDeck
    {
        FECard[] cardList;

        public FEMainCharacter(FECard[] formatCardlist)
        {
            this.cardList = formatCardlist;
        }

        public string Name { get => "maincharacter"; }

        public string Label { get => "Main Character"; }

        public int ExpectedDeckSize { get => 1; }

        public bool ValidateAdd(DeckBuilderCard card, IEnumerable<DeckBuilderCard> deck)
        {
            if (deck.Count() > 0) { return false; }
            FECard feCard = this.cardList.Where(listCard => listCard.ID == card.CardID).First();
            return deck.Count() == 0 && feCard != null && feCard.cost == "1";
        }

        public bool ValidateDeck(IEnumerable<DeckBuilderCard> deck)
        {
            if (deck.Count() != 1) { return false; }
            FECard feCard = this.cardList.Where(listCard => listCard.ID == deck.First().CardID).First();
            return feCard != null && feCard.cost == "1";
        }
    }
}
