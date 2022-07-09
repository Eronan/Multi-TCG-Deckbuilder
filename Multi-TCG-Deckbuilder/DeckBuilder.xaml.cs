using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Multi_TCG_Deckbuilder
{

    /// <summary>
    /// Interaction logic for DeckBuilder.xaml
    /// </summary>
    public partial class DeckBuilder : Window
    {
        IGamePlugIn game;
        Format format;
        List<DeckBuilderCardArt> fullList;
        List<DeckBuilderCardArt> advancedSearchList;
        List<DeckBuilderCardArt> searchList;
        Dictionary<string, TextBlock> deckLabels;
        Dictionary<string, ListBox> deckListBoxes;

        public DeckBuilder(IGamePlugIn gamePlugIn, Format format)
        {
            InitializeComponent();
            this.Title = gamePlugIn.LongName + " Deck Builder";

            this.game = gamePlugIn;
            this.format = format;
            this.fullList = DeckBuilderCardArt.GetFromCards(format.CardList, AppDomain.CurrentDomain.BaseDirectory);
            this.advancedSearchList = this.fullList;
            this.searchList = new List<DeckBuilderCardArt>();

            this.deckLabels = new Dictionary<string, TextBlock>();
            this.deckListBoxes = new Dictionary<string, ListBox>();
            this.CreateDeckListBoxes(format.Decks);
        }

        // Creates all the Deck List Boxes from an Array of Decks
        private void CreateDeckListBoxes(Deck[] decks)
        {
            if (decks.Length == 1)
            {
                this.CreateDeckListBox(decks[0], true);
            }
            else
            {
                foreach (Deck deck in decks)
                {
                    this.CreateDeckListBox(deck, false);
                }
            }
        }
        
        // Creates a List Box with all of its Children by using a Deck Class
        private ListBox CreateDeckListBox(Deck deck, bool onlyDeck)
        {
            // Deck Label
            TextBlock textblock_Label = new TextBlock();
            textblock_Label.Name = "labelDeck_" + deck.Name;
            textblock_Label.Text = deck.Label + ": (0)";
            this.deckLabels.Add(deck.Name, textblock_Label);

            // Uniform Grid
            System.Windows.Controls.Primitives.UniformGrid uniform = new System.Windows.Controls.Primitives.UniformGrid();
            var templatePanel = new FrameworkElementFactory(typeof(System.Windows.Controls.Primitives.UniformGrid));
            templatePanel.SetValue(System.Windows.Controls.Primitives.UniformGrid.ColumnsProperty, 10);
            templatePanel.SetValue(System.Windows.Controls.Primitives.UniformGrid.HeightProperty, double.NaN);
            templatePanel.SetValue(System.Windows.Controls.Primitives.UniformGrid.VerticalAlignmentProperty, VerticalAlignment.Top);
            Binding panelBinding = new Binding("ViewportWidth");
            panelBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
            panelBinding.RelativeSource.AncestorType = typeof(ScrollViewer);
            templatePanel.SetValue(System.Windows.Controls.Primitives.UniformGrid.WidthProperty, panelBinding);

            // ListBox Panel Template
            var panelTemplate = new ItemsPanelTemplate();
            panelTemplate.VisualTree = templatePanel;

            // Deck List Box
            ListBox listBox_Deck = new ListBox();
            listBox_Deck.Name = "listBoxDeck_" + deck.Name;
            listBox_Deck.Tag = deck;
            listBox_Deck.HorizontalAlignment = HorizontalAlignment.Stretch;
            listBox_Deck.VerticalAlignment = VerticalAlignment.Top;
            listBox_Deck.Width = double.NaN;
            listBox_Deck.MinHeight = 40;
            // listBox_Deck.MinHeight = Math.Ceiling(deck.ExpectedDeckSize / 10.0d) * 40;
            if (onlyDeck)
            {
                listBox_Deck.VerticalAlignment = VerticalAlignment.Stretch;
                listBox_Deck.Height = double.NaN;
                listBox_Deck.Margin = new Thickness(0, 15, 0, 0);
            }
            else
            {
                listBox_Deck.VerticalAlignment = VerticalAlignment.Top;
            }
            listBox_Deck.ItemTemplate = this.FindResource("ImageControl_Deck") as DataTemplate;
            listBox_Deck.ItemsPanel = panelTemplate;
            listBox_Deck.AllowDrop = true;
            listBox_Deck.DragOver += this.ListBox_Deck_DragOver;
            listBox_Deck.Drop += this.ListBox_Deck_Drop;
            listBox_Deck.GotFocus += this.listBox_GotFocus;
            listBox_Deck.SelectionChanged += this.listBox_SelectionChanged;

            if (onlyDeck)
            {
                grid_Decks.Children.Add(textblock_Label);
                grid_Decks.Children.Add(listBox_Deck);
            }
            else
            {
                panel_Decks.Children.Add(textblock_Label);
                panel_Decks.Children.Add(listBox_Deck);
            }

            this.deckListBoxes.Add(deck.Name, listBox_Deck);

            return listBox_Deck;
        }

        // Sub-Routine for Adding a Card to a ListBox Item Collection
        private bool AddCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            Deck? deck = listbox_Deck.Tag as Deck;
            TextBlock? label = deckLabels.GetValueOrDefault(deck != null ? deck.Name : "");
            if (deck != null && label != null)
            {
                // Convert ListBox Items to Cardlist Format
                Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = new Dictionary<string, IEnumerable<DeckBuilderCard>>();
                foreach (KeyValuePair<string, ListBox> valuePair in this.deckListBoxes)
                {
                    allDecks.Add(valuePair.Key, valuePair.Value.Items.Cast<DeckBuilderCardArt>());
                }

                // Verify whether card has not reached its maximum allowable copies and can be added to the Deck
                if (!format.ValidateMaximum(card, allDecks) && deck.ValidateAdd(card, listbox_Deck.Items.Cast<DeckBuilderCardArt>()))
                {
                    listbox_Deck.Items.Add(card);
                    label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\)$", string.Format("({0})", listbox_Deck.Items.Count));
                    return true;
                }
            }
            return false;
        }

        // Sub-Routine for Removing a Card from a ListBox Item Collection
        private bool RemoveCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            Deck? deck = listbox_Deck.Tag as Deck;
            TextBlock? label = deckLabels.GetValueOrDefault(deck != null ? deck.Name : "");
            if (label != null)
            {
                listbox_Deck.Items.Remove(card);
                label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\)$", string.Format("({0})", listbox_Deck.Items.Count));
                return true;
            }
            return false;
        }

        // Sub-Routine for Moving a Card from a ListBox Item Collection to Another
        private bool MoveCard(DeckBuilderCardArt card, ListBox listbox_From, ListBox listbox_To)
        {
            Deck? deckTo = listbox_To.Tag as Deck;
            TextBlock? labelTo = deckLabels.GetValueOrDefault(deckTo != null ? deckTo.Name : "");
            Deck? deckFrom = listbox_From.Tag as Deck;
            TextBlock? labelFrom = deckLabels.GetValueOrDefault(deckFrom != null ? deckFrom.Name : "");

            if (deckTo != null && labelFrom != null && labelTo != null
                && deckTo.ValidateAdd(card, listbox_To.Items.Cast<DeckBuilderCardArt>()))
            {
                listbox_To.Items.Add(card);
                listbox_From.Items.Remove(card);

                labelTo.Text = Regex.Replace(labelTo.Text, "\\(([0-9]+)\\)$", string.Format("({0})", listbox_To.Items.Count));
                labelFrom.Text = Regex.Replace(labelFrom.Text, "\\(([0-9]+)\\)$", string.Format("({0})", listbox_From.Items.Count));
                return true;
            }
            return false;
        }

        // Remove Placeholder Text
        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;
            if (searchBox.Foreground == SystemColors.GrayTextBrush)
            {
                searchBox.Text = "";
                searchBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        // Create Placeholder Text
        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox searchBox = (TextBox)sender;
            if (searchBox.Text == "")
            {
                searchBox.Foreground = SystemColors.GrayTextBrush;
                searchBox.Text = "Search";
            }
        }

        // Search For Card Function
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBox_SearchText.Text;
            if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush || this.advancedSearchList == null || searchText.Length < 4)
            {
                return;
            }
            this.searchList = this.advancedSearchList.Where(item => item.ViewDetails.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            this.listBox_CardResults.ItemsSource = this.searchList;
        }

        // Filter Button Clicked
        private void button_AdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSearch searchWindow = new AdvancedSearch(this.game.SearchFields);
            if (searchWindow.ShowDialog() == true)
            {
                this.advancedSearchList = game.AdvancedFilterSearchList(this.fullList.Cast<DeckBuilderCard>(), game.SearchFields).Cast<DeckBuilderCardArt>().ToList();

                string searchText = textBox_SearchText.Text;
                if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush || this.fullList == null || searchText.Length < 4)
                {
                    this.searchList = this.advancedSearchList;
                    this.listBox_CardResults.ItemsSource = this.searchList;
                }
                else
                {
                    this.searchList = this.advancedSearchList.Where(item => item.ViewDetails.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    this.listBox_CardResults.ItemsSource = this.searchList;
                }
            }
        }

        private void button_ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            this.advancedSearchList = this.fullList;
            if (this.textBox_SearchText.Foreground != SystemColors.GrayTextBrush)
            {
                this.textBox_SearchText.Foreground = SystemColors.GrayTextBrush;
                this.textBox_SearchText.Text = "Search";
            }
            this.listBox_CardResults.ItemsSource = null;
        }

        // Add Card to Default Deck
        private void ImageCardSearch_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image) sender;
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            ListBox? deck = card != null ? this.deckListBoxes.GetValueOrDefault(this.format.DefaultDeckName(card)) : null;
            if (card != null && deck != null)
            {
                this.AddCard(card, deck);
            }
        }

        // Initiate Drag Event for Card Data
        private void ImageCardSearch_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Image imageControl = (Image)sender;
                DataObject dragData = new DataObject();
                dragData.SetData("myFormat", imageControl.DataContext);
                DragDrop.DoDragDrop(imageControl, dragData, DragDropEffects.Copy);
            }
        }

        // Remove Card from Deck
        private void ImageCardDeck_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            Deck deck = (Deck)image.Tag;
            ListBox? listbox_Deck = this.deckListBoxes.GetValueOrDefault(deck.Name);
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            if (card != null && listbox_Deck != null)
            {
                RemoveCard(card, listbox_Deck);
            }
        }

        // Initiate Drag Event for Card in Deck Data
        private void ImageCardDeck_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Image imageControl = (Image)sender;
                DataObject dragData = new DataObject();
                dragData.SetData("myFormat", imageControl.DataContext);
                dragData.SetData("listTag", (Deck) imageControl.Tag);
                DragDrop.DoDragDrop(imageControl, dragData, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        // ListBox Focused
        private void listBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            if (listBox != null)
            {
                grid_ViewCard.DataContext = listBox.SelectedItem;
            }
        }

        // ListBox Selection Changed
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            if (listBox != null)
            {
                grid_ViewCard.DataContext = listBox.SelectedItem;
            }
        }

        // Determine Valid Drop for ListBox Deck
        private void ListBox_Deck_DragOver(object sender, DragEventArgs e)
        {
            Deck? imageTag = e.Data.GetData("listTag") as Deck;
            if (!e.Data.GetDataPresent("myFormat") || sender == e.OriginalSource ||
                (!e.KeyStates.HasFlag(DragDropKeyStates.ControlKey) && imageTag != null && imageTag.Name == ((Deck)((ListBox)sender).Tag).Name))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        // Drop Event for ListBox Deck
        private void ListBox_Deck_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                DeckBuilderCardArt? card = e.Data.GetData("myFormat") as DeckBuilderCardArt;
                ListBox? listbox = sender as ListBox;
                if (card != null && listbox != null)
                {
                    if (e.Effects.HasFlag(DragDropEffects.Move) && !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey)
                        && e.Data.GetDataPresent("listTag"))
                    {
                        Deck? originalDeck = e.Data.GetData("listTag") as Deck;
                        ListBox? originalList = this.deckListBoxes.GetValueOrDefault(originalDeck != null ? originalDeck.Name : "");
                        if (originalList != null)
                        {
                            MoveCard(card, originalList, listbox);
                        }
                    }
                    else
                    {
                        AddCard(card, listbox);
                    }
                }
            }
            e.Handled = true;
        }

        // Determine Valid Drop for Window
        private void Window_DragOver(object sender, DragEventArgs e)
        {
            string[]? filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (sender == e.OriginalSource || (!e.Data.GetDataPresent("listTag") && filePaths == null) ||
                (filePaths != null && (filePaths.Length > 1 || !filePaths[0].EndsWith(".mtdk"))))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else if (e.Data.GetDataPresent("listTag"))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            else if (filePaths != null)
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
        }

        // Drop Event for Window
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("listTag") && e.Data.GetDataPresent("myFormat"))
            {
                Deck? deck = e.Data.GetData("listTag") as Deck;
                ListBox? originalList = deck != null ? this.deckListBoxes.GetValueOrDefault(deck.Name) : null;
                DeckBuilderCardArt? card = e.Data.GetData("myFormat") as DeckBuilderCardArt;
                if (originalList != null && card != null)
                {
                    RemoveCard(card, originalList);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file;

                if (filePaths.Length == 1 && (file = filePaths[0]).EndsWith(".mtdk"))
                {
                    
                }
            }
        }

        // New Deck
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            foreach (ListBox listbox_Deck in this.deckListBoxes.Values)
            {
                listbox_Deck.Items.Clear();
            }

            foreach (TextBlock label in this.deckLabels.Values)
            {
                label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\)$", "(0)");
            }
        }
    }
}
