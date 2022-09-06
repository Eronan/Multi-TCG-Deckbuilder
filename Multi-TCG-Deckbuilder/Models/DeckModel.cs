using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Multi_TCG_Deckbuilder.Models
{
    /// <summary>
    /// A Collection of Window Controls for a Deck and Necessary Variables
    /// </summary>
    public class DeckModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a <see cref="DeckModel"/>
        /// </summary>
        /// <param name="deck"><see cref="IDeck"/> Interface to be created from.</param>
        public DeckModel(IDeck deck)
        {
            Deck = deck;
        }

        /// <summary>
        /// Short Name of the <see cref="IDeck"/> Interface
        /// </summary>
        public string DeckName
        {
            get => Deck.Name;
        }

        /// <summary>
        /// Cards added to the <see cref="ListBox"/>
        /// </summary>
        public ObservableCollection<CardModel> Cards { get; } = new ObservableCollection<CardModel>();

        /// <summary>
        /// Text that appears in the <see cref="Label"/> above the <see cref="ListBox"/>
        /// </summary>
        public string LabelText
        {
            get
            {
                return string.Format("{0}: ({1}) {2}", Deck.Label, Cards.Count, Deck.ValidateDeck(Cards).Length == 0 ? "✔" : "❌");
            }
        }

        /// <summary>
        /// <see cref="IDeck"/> Interface the Model was created from
        /// </summary>
        public IDeck Deck { get; }


        public event PropertyChangedEventHandler? PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Sorts the Cardlist
        private ObservableCollection<CardModel> SortListBoxDeck(int leftIndex, int rightIndex, Comparison<CardModel> comparer)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = Cards[leftIndex];

            while (i <= j)
            {
                while (comparer(Cards[i], pivot) < 0)
                {
                    i++;
                }

                while (comparer(Cards[j], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    var temp = Cards[i];
                    Cards[i] = Cards[j];
                    Cards[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortListBoxDeck(leftIndex, j, comparer);

            if (i < rightIndex)
                SortListBoxDeck(i, rightIndex, comparer);

            return Cards;
        }

        /// <summary>
        /// Adds a card to the <see cref="DeckModel"/> Cardlist
        /// </summary>
        /// <param name="card">Card to be Added to the DeckControls</param>
        public void Add(CardModel card)
        {
            Cards.Add(card);
            OnPropertyChanged(nameof(LabelText));
        }

        /// <summary>
        /// Removes a card from the Cardlist in <see cref="DeckModel"/>
        /// </summary>
        /// <param name="card">Card to be removed from the DeckControls</param>
        /// <returns>Card was successfully removed from the DeckControls</returns>
        public bool Remove(CardModel card)
        {
            var result = Cards.Remove(card);
            OnPropertyChanged(nameof(LabelText));
            return result;
        }

        /// <summary>
        /// Removes all cards in the Cardlist in <see cref="DeckModel"/>
        /// </summary>
        public void Clear()
        {
            Cards.Clear();
            OnPropertyChanged(nameof(LabelText));
        }

        /// <summary>
        /// Sorts all cards in the <see cref="DeckModel"/> Cardlist
        /// </summary>
        /// <param name="comparison">A <see cref="Comparison{DeckBuilderCardArt}"/> Function to determine the Order between Cards</param>
        public void Sort(Comparison<CardModel> comparison)
        {
            SortListBoxDeck(0, Cards.Count - 1, comparison);
        }

        /// <summary>
        /// Validates that a card can be added to the <see cref="DeckModel"/>
        /// </summary>
        /// <param name="card">Card to be Added to the List</param>
        /// <returns>Card is allowed to be Added.</returns>
        public bool ValidateAdd(CardModel card)
        {
            return Deck.ValidateAdd(card, Cards);
        }

        /// <summary>
        /// Validates that a <see cref="DeckModel"/> is Legal according to the <see cref="IDeck"/> Interface.
        /// </summary>
        /// <returns>List of Errors with the Decklist</returns>
        public string[] ValidateDeck()
        {
            return Deck.ValidateDeck(Cards);
        }

        /// <summary>
        /// Gets the element at a specific index
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns></returns>
        public CardModel this[int index]
        {
            get => this.Cards[index];
        }
    }
}
