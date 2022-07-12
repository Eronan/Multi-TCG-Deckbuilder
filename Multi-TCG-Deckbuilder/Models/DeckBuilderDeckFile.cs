using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class DeckBuilderDeckFile
    {
        public string Game { get; set; }
        public string Format { get; set; }
        public DeckBuilderDeck[] Decks { get; set; }

        public DeckBuilderDeckFile(string game, string format, DeckBuilderDeck[] decks)
        {
            Game = game;
            Format = format;
            Decks = decks;
        }
    }
}
