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

        public string[] ValidateDeck(IEnumerable<DeckBuilderCard> deck)
        {
            if (deck.Count() != 1)
            {
                return new string[1] { "You must choose a card to be your Main Character, and only have 1 Main Character." };
            }
            else
            {
                FECard feCard = this.cardList.Where(listCard => listCard.ID == deck.First().CardID).First();
                if (feCard != null && feCard.cost != "1")
                {
                    return new string[1] { "Your Main Character must be a 1 Cost Card." };
                }
            }

            return new string[0];
        }
    }
}
