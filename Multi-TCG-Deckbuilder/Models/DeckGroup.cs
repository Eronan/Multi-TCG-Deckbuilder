using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class DeckGroup
    {
        List<DeckBuilderCardArt> cards;
        private ListBox listBox;
        string infoText;

        public DeckGroup(TextBlock label, ListBox listBox, IDeck deck)
        {
            cards = new List<DeckBuilderCardArt>();
            infoText = "(0) ❌";

            this.Label = label;
            this.listBox = listBox;
            this.listBox.ItemsSource = cards;
            this.Deck = deck;
        }

        // Accessors
        public TextBlock Label { get; }

        public IDeck Deck { get; }

        public IList<DeckBuilderCardArt> Cardlist { get => cards; }

        // Functions
        public void Add(DeckBuilderCardArt card)
        {
            cards.Add(card);
            infoText = string.Format("({0}) {1}", cards.Count, ValidateDeck().Length == 0 ? "✔" : "❌");
            Label.Text = Deck.Label + " " + infoText;
        }

        public bool Remove(DeckBuilderCardArt card)
        {
            if (!cards.Remove(card))
            {
                return false;
            }

            infoText = string.Format("({0}) {1}", cards.Count, ValidateDeck().Length == 0 ? "✔" : "❌");
            Label.Text = Deck.Label + " " + infoText;
            return true;
        }

        public void Clear()
        {
            cards.Clear();

            infoText = "(0) ❌";
            Label.Text = Deck.Label + " " + infoText;
        }

        public void Sort(Comparison<DeckBuilderCardArt> comparison)
        {
            cards.Sort(comparison);
        }


        public bool ValidateAdd(DeckBuilderCard card)
        {
            return Deck.ValidateAdd(card, cards);
        }

        public string[] ValidateDeck()
        {
            return Deck.ValidateDeck(cards);
        }
    }
}
