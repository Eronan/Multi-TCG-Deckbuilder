using IGamePlugInBase;

namespace FECipher
{
    internal class FEStandard : IFormat
    {
        FECard[] cardlist;

        public FEStandard(FECard[] cardlist)
        {
            this.cardlist = cardlist.Where(item => item.seriesNo > 4).ToArray();

            this.Decks = new IDeck[2]
            {
                new FEMainCharacter(this.cardlist),
                new FEMainDeck(),
            };
        }

        public string Name { get => "standard"; }

        public string LongName { get => "Standard"; }

        public byte[] Icon { get => Properties.Resources.StandardIcon; }

        public string Description { get => "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format."; }

        public ICard[] CardList { get => this.cardlist; }

        public IDeck[] Decks { get; }

        public string DefaultDeckName(DeckBuilderCard card)
        {
            return "main";
        }

        public bool ValidateMaximum(DeckBuilderCard card, Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            int count = 0;
            FECard feCardCheck = this.cardlist.Where(cardlistItem => cardlistItem.ID == card.CardID).First();
            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> decklist in decks)
            {
                count += decklist.Value.Count(predicate: item => item.CardID == feCardCheck.ID || feCardCheck.Name == this.cardlist.Where(cardlistItem => cardlistItem.ID == item.CardID).First().Name);
                if (count >= 4) { return true; }
            }
            return false;
        }
    }
}
