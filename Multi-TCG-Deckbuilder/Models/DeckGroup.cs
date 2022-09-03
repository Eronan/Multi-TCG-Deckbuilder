using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class DeckGroup
    {
        ObservableCollection<DeckBuilderCardArt> cards;
        private ListBox listBox;
        string infoText;

        public DeckGroup(TextBlock label, ListBox listBox, IDeck deck)
        {
            cards = new ObservableCollection<DeckBuilderCardArt>();
            infoText = "(0) ❌";

            this.Label = label;
            this.listBox = listBox;
            this.listBox.ItemsSource = cards;
            this.Deck = deck;
        }

        private ObservableCollection<DeckBuilderCardArt> SortListBoxDeck(int leftIndex, int rightIndex, Comparison<DeckBuilderCardArt> comparer)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = cards[leftIndex];

            while (i <= j)
            {
                while (comparer(cards[i], pivot) < 0)
                {
                    i++;
                }

                while (comparer(cards[j], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    DeckBuilderCardArt temp = cards[i];
                    cards[i] = cards[j];
                    cards[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortListBoxDeck(leftIndex, j, comparer);

            if (i < rightIndex)
                SortListBoxDeck(i, rightIndex, comparer);

            return cards;
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
            SortListBoxDeck(0, cards.Count - 1, comparison);
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
