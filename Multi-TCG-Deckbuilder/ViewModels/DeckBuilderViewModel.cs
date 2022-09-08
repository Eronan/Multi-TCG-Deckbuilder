using IGamePlugInBase;
using IGamePlugInBase.IO;
using Multi_TCG_Deckbuilder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Multi_TCG_Deckbuilder.ViewModels
{
    public class DeckBuilderViewModel : INotifyPropertyChanged
    {
        IGamePlugIn game;
        IFormat format;

        List<CardModel> fullList = new List<CardModel>();
        IEnumerable<SearchField> advancedSearchCriteria = new List<SearchField>();
        Dictionary<string, DeckModel> deckBuilderDecks = new Dictionary<string, DeckModel>();

        public DeckBuilderViewModel(IGamePlugIn gamePlugIn, IFormat format)
        {
            this.game = gamePlugIn;
            this.format = format;

            // Re-Change Values
            ChangeFormat(format);
        }

        // Change Format
        private void ChangeFormat(IFormat format)
        {
            this.format = format;

            this.DeckBuilderService.InitializeService();
            this.fullList = this.DeckBuilderService.CardList.Select(card => new CardModel(card.CardID, card.ArtID, card.Name, card.FileLocation, card.DownloadLocation, card.Orientation, card.ViewDetails)).ToList();
            this.advancedSearchCriteria = this.DeckBuilderService.SearchFields;
            this.deckBuilderDecks = format.Decks.ToDictionary(deck => deck.Name, deck => new DeckModel(deck));
        }

        public string OpenedFilePath { get; set; } = "";

        public string WindowTitle
        {
            get { return string.Format("Multi-TCG Deck Builder: {0} - {1}", game.LongName, format.LongName); }
        }

        public string GameName
        {
            get { return game.Name; }
        }

        public string GameLongName
        {
            get { return game.LongName; }
        }

        public string PlugInAbout
        {
            get { return game.AboutInformation; }
        }

        public string FormatName
        {
            get { return format.Name; }
        }
        public string FormatLongName
        {
            get { return format.LongName; }
        }

        public IFormat Format
        { 
            get { return this.format; }
            set
            {
                ChangeFormat(value);
                OnPropertyChanged("Format");
            }
        }

        public IDeckBuilderService DeckBuilderService
        {
            get { return this.format.DeckBuilderService; }
        }

        public VerticalAlignment DeckListBoxAlignment
        {
            get
            {
                if (this.deckBuilderDecks.Count == 1)
                {
                    return VerticalAlignment.Stretch;
                }

                return VerticalAlignment.Top;
            }
        }

        public IEnumerable<DeckModel> Decks
        {
            get { return this.deckBuilderDecks.Values; }
        }

        public bool DecksValid
        {
            get { return this.CheckDecksValid().Count() == 0; }
        }

        public bool ImportEnabled
        { 
            get { return game.ImportMenus != null && game.ImportMenus.Count() > 0; }
        }

        public IEnumerable<IImportMenuItem>? ImportMenus
        {
            get { return game.ImportMenus; }
        }

        public IEnumerable<IExportMenuItem>? ExportMenus
        {
            get { return game.ExportMenus; }
        }

        public string BasicSearchText { get; set; } = string.Empty;

        public IEnumerable<SearchField> AdvancedSearchCriteria
        {
            get { return this.advancedSearchCriteria; }
            set
            {
                this.advancedSearchCriteria = value;
                OnPropertyChanged("AdvancedSearchCriteria");
            }
        }

        public IEnumerable<CardModel> FilteredList
        {
            get
            {
                var advancedSearchList = this.DeckBuilderService.AdvancedFilterSearchList(this.fullList, this.advancedSearchCriteria);
                return advancedSearchList.Where(card => card.ViewDetails.Contains(BasicSearchText, StringComparison.InvariantCultureIgnoreCase)).Cast<CardModel>();
            }
        }

        public string BasicDetailsText
        {
            get { return this.DeckBuilderService.GetStats(this.GetAllDecks()); }
        }

        public string AdvancedDetailsText
        {
            get { return this.DeckBuilderService.GetDetailedStats(this.GetAllDecks()); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        // Load Deck from Deck File
        public void LoadFromDeckFile(DeckBuilderDeckFile deckFile)
        {
            ClearDecks();

            foreach (DeckBuilderDeck deck in deckFile.Decks)
            {
                DeckModel? deckModel = Decks.FirstOrDefault(deckModel => deckModel.DeckName == deck.DeckName);

                if (deckModel != null)
                {
                    foreach (DeckBuilderCard card in deck.Cards)
                    {
                        CardModel cardArt = this.fullList.First(item => item.CardID == card.CardID && item.ArtID == card.ArtID);
                        deckModel.Add(cardArt);
                    }
                }
            }
        }


        // Sub-Routine for getting Decks in a Format for being parsed to Plug-In Functions
        private Dictionary<string, IEnumerable<DeckBuilderCard>> GetAllDecks()
        {
            return this.deckBuilderDecks.ToDictionary(
                keyValue => keyValue.Key,
                keyValue => keyValue.Value.Cards.Cast<DeckBuilderCard>()
            );
        }

        // Sub-Routine for Adding a Card to a ListBox Item Collection
        public bool AddCard(CardModel card, string deckModelName)
        {
            // Convert ListBox Items to Cardlist Format
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = this.GetAllDecks();
            DeckModel? deckModel = this.deckBuilderDecks.GetValueOrDefault(deckModelName);

            // Verify whether card has not reached its maximum allowable copies and can be added to the Deck
            if (deckModel != null && !this.DeckBuilderService.ValidateMaximum(card, allDecks) && deckModel.ValidateAdd(card))
            {
                deckModel.Add(card);
                return true;
            }

            return false;
        }

        // Sub-Routine for Adding a Card to a ListBox Item Collection
        public bool AddCardToFirst(CardModel card)
        {
            // Convert ListBox Items to Cardlist Format
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = this.GetAllDecks();
            DeckModel? deckModel = this.deckBuilderDecks.First().Value;

            // Verify whether card has not reached its maximum allowable copies and can be added to the Deck
            if (deckModel != null && !this.DeckBuilderService.ValidateMaximum(card, allDecks) && deckModel.ValidateAdd(card))
            {
                deckModel.Add(card);
                return true;
            }

            return false;
        }


        public bool RemoveCard(CardModel card, string deckModelName)
        {
            DeckModel? deckModel = this.deckBuilderDecks.GetValueOrDefault(deckModelName);
            return deckModel == null || deckModel.Remove(card);
        }

        // Sub-Routine for Moving a Card from a ListBox Item Collection to Another
        public bool MoveCard(CardModel card, string deckModelNameFrom, string deckModelNameTo)
        {
            DeckModel? deckModelFrom = this.deckBuilderDecks.GetValueOrDefault(deckModelNameFrom);
            DeckModel? deckModelTo = this.deckBuilderDecks.GetValueOrDefault(deckModelNameTo);
            if (deckModelTo != null && deckModelFrom != null &&
                deckModelTo.ValidateAdd(card) && deckModelFrom.Remove(card))
            {
                deckModelTo.Add(card);
                return true;
            }
            return false;
        }

        // Check Deck is Valid
        public IEnumerable<string> CheckDecksValid()
        {
            List<string> errorMessages = new List<string>();

            int cardsInDeck = 0;
            foreach (var controls_Deck in this.Decks)
            {
                cardsInDeck += controls_Deck.Cards.Count;
                try
                {
                    errorMessages.AddRange(controls_Deck.ValidateDeck());
                }
                catch (NotImplementedException error)
                {
                    Console.WriteLine("{0}\n{1}", error.Message, error.StackTrace);
                }
            }

            if (cardsInDeck == 0)
            {
                errorMessages.Add("There are no cards are in your Deck!");
            }

            return errorMessages;
        }

        // Clear Decks
        public IFormat? GetGameFormat(string formatName)
        {
            return this.game.Formats.FirstOrDefault(item => item.Name == formatName);
        }

        public void ClearDecks()
        {
            foreach (var deckModel in Decks)
            {
                deckModel.Clear();
            }
        }

        public string GetDeckBuilderDeckFileAsJSON()
        {
            return Contexts.FileLoadContext.ConvertToJSON(game.Name, format.Name, Decks);
        }

        public void SortDecks()
        {
            foreach (var deckModel in Decks)
            {
                deckModel.Sort(DeckBuilderService.CompareCards);
            }
        }

        public void ClearFilters()
        {
            foreach (var criteria in this.advancedSearchCriteria)
            {
                criteria.Value = criteria.DefaultValue;
            }

            BasicSearchText = "";
        }
    }
}
