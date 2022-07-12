using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IGamePlugInBase;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class DeckBuilderDeck
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
