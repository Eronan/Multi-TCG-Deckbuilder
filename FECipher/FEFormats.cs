using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGamePlugInBase;

namespace FECipher
{
    internal class FEFormats : IFormat
    {
        FECard[] cardlist;

        public FEFormats(string name, string longname, byte[] icon, string description, FECard[] cardlist)
        {
            this.Name = name;
            this.LongName = longname;
            this.Icon = icon;
            this.Description = description;

            this.cardlist = cardlist;

            this.Decks = new IDeck[2]
            {
                new FEMainCharacter(this.cardlist),
                new FEMainDeck(),
            };
        }

        public string Name { get; }

        public string LongName { get; }

        public byte[] Icon { get; }

        public string Description { get; }

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

        public string GetStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            // Get Main Character Card
            IEnumerable<DeckBuilderCard>? mainCharDeck = decks.GetValueOrDefault(this.Decks[0].Name);
            string mainCharacterID = mainCharDeck != null && mainCharDeck.Count() == 1 ? mainCharDeck.First().CardID : "";
            IEnumerable<FECard> mainCharMatches = this.cardlist.Where(item => item.ID == mainCharacterID);
            FECard? mainCharacter = mainCharMatches.Count() == 1 ? mainCharMatches.First() : null;

            // Get Counts
            int deckSize = 0;
            int mainCharacterNames = 0;

            // Count Cards
            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> keyValue in decks)
            {
                // Perform Counts
                deckSize += keyValue.Value.Count();
                mainCharacterNames += mainCharacter != null
                    ? keyValue.Value.Count(card => this.cardlist.Where(item => item.ID == card.CardID).First().characterName == mainCharacter.characterName)
                    : 0;
            }

            string textFormat = "Deck Size: {0}\tMain Characters: {1}";

            return string.Format(textFormat, deckSize, mainCharacterNames);
        }

        public string GetDetailedStats(Dictionary<string, IEnumerable<DeckBuilderCard>> decks)
        {
            // Get Main Character Card
            IEnumerable<DeckBuilderCard>? mainCharDeck = decks.GetValueOrDefault(this.Decks[0].Name);
            string mainCharacterID = mainCharDeck != null && mainCharDeck.Count() == 1 ? mainCharDeck.First().CardID : "";
            IEnumerable<FECard> mainCharMatches = this.cardlist.Where(item => item.ID == mainCharacterID);
            FECard? mainCharacter = mainCharMatches.Count() == 1 ? mainCharMatches.First() : null;

            // Get Counts
            int deckSize = 0;
            int mainCharacterNames = 0;
            int range0 = 0;
            int range1 = 0;
            int range2 = 0;
            int range3 = 0;
            int support0 = 0;
            int support10 = 0;
            int support20 = 0;
            int support30 = 0;

            // Count Cards
            foreach (KeyValuePair<string, IEnumerable<DeckBuilderCard>> keyValue in decks)
            {
                // Count Deck Size
                deckSize += keyValue.Value.Count();

                foreach (DeckBuilderCard card in keyValue.Value)
                {
                    FECard feCard = this.cardlist.Where(item => item.ID == card.CardID).First();

                    // Main Character Counting
                    if (mainCharacter != null && feCard.characterName == mainCharacter.characterName) { mainCharacterNames++; }

                    // Range Counting
                    if (feCard.minRange == 0 && feCard.maxRange == 0) { range0++; }
                    if (feCard.minRange <= 1 && feCard.maxRange >= 1) { range1++; }
                    if (feCard.minRange <= 2 && feCard.maxRange >= 2) { range2++; }
                    if (feCard.minRange <= 3 && feCard.maxRange >= 3) { range3++; }

                    // Support Counting
                    switch (feCard.support)
                    {
                        case "0":
                            support0++;
                            break;
                        case "10":
                            support10++;
                            break;
                        case "20":
                            support20++;
                            break;
                        case "30":
                            support30++;
                            break;
                    }
                }
            }

            string textFormat = "Deck Size: {0}\tMain Characters: {1} ({2}%)\n" +
                "0 Support: {3} ({4}%)\tNo Range: {11} ({12}%)\n" +
                "10 Support: {5} ({6}%)\tRange 1: {13} ({14}%)\n" +
                "20 Support: {7} ({8}%)\tRange 2: {15} ({16}%)\n" +
                "30 Support: {9} ({10}%)\tRange 3: {17} ({17}%)\n";

            return string.Format(textFormat, deckSize,
                mainCharacterNames, mainCharacterNames / deckSize * 100,
                support0, support0 / deckSize * 100,
                support10, support10 / deckSize * 100,
                support20, support20 / deckSize * 100,
                support30, support30 / deckSize * 100,
                range0, range0 / deckSize * 100,
                range1, range1 / deckSize * 100,
                range2, range2 / deckSize * 100,
                range3, range3 / deckSize * 100);
        }
    }
}
