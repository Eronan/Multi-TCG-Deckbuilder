using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Multi_TCG_Deckbuilder.Models
{
    /// <summary>
    /// A Collection of Window Controls for a Deck and Necessary Variables
    /// </summary>
    internal class DeckControls
    {
        ObservableCollection<DeckBuilderCardArt> cards;
        private ListBox listBox;
        string infoText;

        /// <summary>
        /// Initializes a DeckControls to Collect the Controls together
        /// </summary>
        /// <param name="label">Label Control of the Group</param>
        /// <param name="listBox">ListBox Control of the Group</param>
        /// <param name="deck">IDeck Interface it was created from.</param>
        public DeckControls(TextBlock label, ListBox listBox, IDeck deck)
        {
            cards = new ObservableCollection<DeckBuilderCardArt>();
            infoText = "(0) ❌";

            this.Label = label;
            this.listBox = listBox;
            this.listBox.ItemsSource = cards;
            this.Deck = deck;
        }

        // Sorts the Cardlist
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
        /// <summary>
        /// Label Control of the Group
        /// </summary>
        public TextBlock Label { get; }

        /// <summary>
        /// IDeck Interface the DeckControls was created from
        /// </summary>
        public IDeck Deck { get; }

        /// <summary>
        /// Cards that are in the ListBox
        /// </summary>
        public IList<DeckBuilderCardArt> Cardlist { get => cards; }

        // Functions
        /// <summary>
        /// Adds a card to the DeckControls
        /// </summary>
        /// <param name="card">Card to be Added to the DeckControls</param>
        public void Add(DeckBuilderCardArt card)
        {
            cards.Add(card);
            infoText = string.Format("({0}) {1}", cards.Count, ValidateDeck().Length == 0 ? "✔" : "❌");
            Label.Text = Deck.Label + ": " + infoText;
        }

        /// <summary>
        /// Removes a card from the DeckControls
        /// </summary>
        /// <param name="card">Card to be removed from the DeckControls</param>
        /// <returns>Card was successfully removed from the DeckControls</returns>
        public bool Remove(DeckBuilderCardArt card)
        {
            if (!cards.Remove(card))
            {
                return false;
            }

            infoText = string.Format("({0}) {1}", cards.Count, ValidateDeck().Length == 0 ? "✔" : "❌");
            Label.Text = Deck.Label + ": " + infoText;

            return true;
        }

        /// <summary>
        /// Removes all cards in the DeckControls
        /// </summary>
        public void Clear()
        {
            cards.Clear();

            infoText = "(0) ❌";
            Label.Text = Deck.Label + ": " + infoText;
        }

        /// <summary>
        /// Sorts all cards in the DeckControls
        /// </summary>
        /// <param name="comparison">A Comparison Function derived from the Plug-In</param>
        public void Sort(Comparison<DeckBuilderCardArt> comparison)
        {
            SortListBoxDeck(0, cards.Count - 1, comparison);
        }

        /// <summary>
        /// Validates that a card can be Added to the DeckControls
        /// </summary>
        /// <param name="card">Card to be Added to the DeckControls</param>
        /// <returns>Card is allowed to be Added.</returns>
        public bool ValidateAdd(DeckBuilderCard card)
        {
            return Deck.ValidateAdd(card, cards);
        }

        /// <summary>
        /// Validates that a Deck is Legal for the Format
        /// </summary>
        /// <returns>List of Errors with the Decklist</returns>
        public string[] ValidateDeck()
        {
            return Deck.ValidateDeck(cards);
        }
    }
}
