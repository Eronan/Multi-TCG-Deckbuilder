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
using Multi_TCG_Deckbuilder.Dialogs;

namespace Multi_TCG_Deckbuilder
{

    /// <summary>
    /// Interaction logic for DeckBuilder.xaml
    /// </summary>
    public partial class DeckBuilder : Window
    {
        IGamePlugIn game;
        IFormat format;
        List<DeckBuilderCardArt> fullList;
        List<DeckBuilderCardArt> advancedSearchList;
        List<DeckBuilderCardArt> searchList;
        Dictionary<string, Tuple<TextBlock, ListBox, IDeck>> deckControls;
        string openedFile;

        public DeckBuilder(IGamePlugIn gamePlugIn, IFormat format, DeckBuilderDeckFile? deckFile = null, string openFile = "")
        {
            InitializeComponent();

            // Set Up Variables
            this.game = gamePlugIn;
            this.format = format;

            // Create Necessary Lists
            this.fullList = new List<DeckBuilderCardArt>();
            this.searchList = new List<DeckBuilderCardArt>();
            this.deckControls = new Dictionary<string, Tuple<TextBlock, ListBox, IDeck>>();
            this.advancedSearchList = new List<DeckBuilderCardArt>();

            // Create Import Menu Items
            if (this.game.ImportFunctions.Length == 0)
            {
                MenuItem_Import.IsEnabled = false;
            }
            else
            {
                foreach (ImportMenuItem importMenu in this.game.ImportFunctions)
                {
                    AddImportMenuItem(importMenu);
                }
            }

            // Create Export Menu Items
            foreach (ExportMenuItem exportMenu in this.game.ExportFunctions)
            {
                AddExportMenuItem(exportMenu);
            }

            if (deckFile != null)
            {
                // Automatically Open Deck From File
                ChangeFormat(format);
                LoadFromDeckFile(deckFile);
                this.openedFile = openFile;
            }
            else
            {
                // Set Up Format Decks and Card Lists
                this.openedFile = "";
                ChangeFormat(format);
            }
        }

        // Change Format
        private void ChangeFormat(IFormat format)
        {
            this.format = format;

            this.Title = string.Format("Multi-TCG Deck Builder: {0} - {1}", this.game.LongName, this.format.LongName);

            this.fullList = DeckBuilderCardArt.GetFromCards(format.CardList, AppDomain.CurrentDomain.BaseDirectory);
            this.advancedSearchList = this.fullList;
            this.searchList.Clear();

            this.deckControls.Clear();

            panel_Decks.Children.Clear();

            this.CreateDeckListBoxes(this.format.Decks);
            button_ViewStats.Content = this.format.GetStats(this.GetAllDecks());
        }

        // Clear Decks
        private void ClearDecks()
        {
            foreach (Tuple<TextBlock, ListBox, IDeck> controls_Deck in this.deckControls.Values)
            {
                controls_Deck.Item1.Text = Regex.Replace(controls_Deck.Item1.Text, "\\(([0-9]+)\\) [✔❌]$", "(0) ❌");
                controls_Deck.Item2.Items.Clear();

                button_ViewStats.Content = this.format.GetStats(this.GetAllDecks());
            }
        }

        // Load Deck from Deck File
        private void LoadFromDeckFile(DeckBuilderDeckFile deckFile)
        {
            if (deckFile.Format != this.format.Name)
            {
                if (MessageBoxResult.No == MessageBox.Show("You are about to change formats, are you sure?",
                    "Change Formats?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No))
                {
                    return;
                }

                // Change Format
                IFormat newFormat = this.game.Formats.Where(item => item.Name == deckFile.Format).First();
                ChangeFormat(newFormat);
            }
            else
            {
                // Clear Decks
                ClearDecks();
            }

            foreach (DeckBuilderDeck deck in deckFile.Decks)
            {
                Tuple<TextBlock, ListBox, IDeck>? controls_Deck = this.deckControls.GetValueOrDefault(deck.DeckName);

                if (controls_Deck != null)
                {
                    TextBlock label = controls_Deck.Item1;
                    ListBox listBox = controls_Deck.Item2;
                    IDeck deckTag = controls_Deck.Item3;
                    foreach (DeckBuilderCard card in deck.Cards)
                    {
                        DeckBuilderCardArt cardArt = this.fullList.Where(item => item.CardID == card.CardID && item.ArtID == card.ArtID).First();
                        listBox.Items.Add(cardArt);
                        label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\) [✔❌]$", string.Format("({0}) {1}", listBox.Items.Count, deckTag.ValidateDeck(listBox.Items.Cast<DeckBuilderCardArt>()).Length == 0 ? "✔" : "❌"));
                    }
                }
            }

            button_ViewStats.Content = this.format.GetStats(this.GetAllDecks());
        }

        // Creates all the Deck List Boxes from an Array of Decks
        private void CreateDeckListBoxes(IDeck[] decks)
        {
            if (decks.Length == 1)
            {
                this.CreateDeckListBox(decks[0], true);
            }
            else
            {
                foreach (IDeck deck in decks)
                {
                    this.CreateDeckListBox(deck, false);
                }
            }
        }
        
        // Creates a List Box with all of its Children by using a Deck Class
        private ListBox CreateDeckListBox(IDeck deck, bool onlyDeck)
        {
            // Deck Label
            TextBlock textblock_Label = new TextBlock();
            textblock_Label.Name = "labelDeck_" + deck.Name;
            textblock_Label.Text = deck.Label + ": (0) ❌";

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
            listBox_Deck.Tag = deck.Name;
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

            this.deckControls.Add(deck.Name, new Tuple<TextBlock, ListBox, IDeck>(textblock_Label, listBox_Deck, deck));

            return listBox_Deck;
        }

        // Sub-Routine for getting Decks in a Format for being parsed to Plug-In Functions
        private Dictionary<string, IEnumerable<DeckBuilderCard>> GetAllDecks()
        {
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = new Dictionary<string, IEnumerable<DeckBuilderCard>>();
            foreach (KeyValuePair<string, Tuple<TextBlock, ListBox, IDeck>> valuePair in this.deckControls)
            {
                allDecks.Add(valuePair.Key, valuePair.Value.Item2.Items.Cast<DeckBuilderCardArt>());
            }

            return allDecks;
        }

        // Sub-Routine for Adding a Card to a ListBox Item Collection
        private bool AddCard(DeckBuilderCardArt card, Tuple<TextBlock, ListBox, IDeck> controls_Deck)
        {
            // Get Controls and Variables
            TextBlock label = controls_Deck.Item1;
            ListBox listbox_Deck = controls_Deck.Item2;
            IDeck deck = controls_Deck.Item3;

            // Convert ListBox Items to Cardlist Format
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = this.GetAllDecks();

            // Verify whether card has not reached its maximum allowable copies and can be added to the Deck
            if (!format.ValidateMaximum(card, allDecks) && deck.ValidateAdd(card, listbox_Deck.Items.Cast<DeckBuilderCardArt>()))
            {
                listbox_Deck.Items.Add(card);
                label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\) [✔❌]$", string.Format("({0}) {1}", listbox_Deck.Items.Count, deck.ValidateDeck(listbox_Deck.Items.Cast<DeckBuilderCardArt>()).Length == 0 ? "✔" : "❌"));
                button_ViewStats.Content = this.format.GetStats(allDecks);
                return true;
            }

            return false;
        }

        // Sub-Routine for Removing a Card from a ListBox Item Collection
        private bool RemoveCard(DeckBuilderCardArt card, Tuple<TextBlock, ListBox, IDeck> controls_Deck)
        {
            // Get Controls
            TextBlock label = controls_Deck.Item1;
            ListBox listBox_Deck = controls_Deck.Item2;
            IDeck deck = controls_Deck.Item3;

            // Remove Item
            int previousCount = listBox_Deck.Items.Count;
            listBox_Deck.Items.Remove(card);

            // Check if Item was Removed
            if (previousCount == listBox_Deck.Items.Count + 1)
            {
                // Update Text
                label.Text = Regex.Replace(label.Text, "\\(([0-9]+)\\) [✔❌]$", string.Format("({0}) {1}", listBox_Deck.Items.Count, deck.ValidateDeck(listBox_Deck.Items.Cast<DeckBuilderCardArt>()).Length == 0 ? "✔" : "❌"));
                button_ViewStats.Content = this.format.GetStats(GetAllDecks());
                return true;
            }

            return false;
        }

        // Sub-Routine for Moving a Card from a ListBox Item Collection to Another
        private bool MoveCard(DeckBuilderCardArt card, Tuple<TextBlock, ListBox, IDeck> controls_DeckFrom, Tuple<TextBlock, ListBox, IDeck> controls_DeckTo)
        {
            TextBlock labelFrom = controls_DeckFrom.Item1;
            ListBox listbox_From = controls_DeckFrom.Item2;
            IDeck deckFrom = controls_DeckFrom.Item3;
            TextBlock labelTo = controls_DeckTo.Item1;
            ListBox listbox_To = controls_DeckTo.Item2;
            IDeck deckTo = controls_DeckTo.Item3;

            if (deckFrom != null && deckTo != null && labelFrom != null && labelTo != null
                && deckTo.ValidateAdd(card, listbox_To.Items.Cast<DeckBuilderCardArt>()))
            {
                listbox_To.Items.Add(card);
                listbox_From.Items.Remove(card);

                labelTo.Text = Regex.Replace(labelTo.Text, "\\(([0-9]+)\\) [✔❌]$", string.Format("({0}) {1}", listbox_To.Items.Count, deckTo.ValidateDeck(listbox_To.Items.Cast<DeckBuilderCardArt>()).Length == 0 ? "✔" : "❌"));
                labelFrom.Text = Regex.Replace(labelFrom.Text, "\\(([0-9]+)\\) [✔❌]$", string.Format("({0}) {1}", listbox_From.Items.Count, deckFrom.ValidateDeck(listbox_From.Items.Cast<DeckBuilderCardArt>()).Length == 0 ? "✔" : "❌"));

                button_ViewStats.Content = this.format.GetStats(GetAllDecks());
                return true;
            }
            return false;
        }

        // Check Deck is Valid
        private bool CheckDecksValid(bool showMessages = true)
        {
            List<string> errorMessages = new List<string>();
            foreach (Tuple<TextBlock, ListBox, IDeck> controls_Deck in this.deckControls.Values)
            {
                errorMessages.AddRange(controls_Deck.Item3.ValidateDeck(controls_Deck.Item2.Items.Cast<DeckBuilderCardArt>()));
            }

            if (errorMessages.Count > 0)
            {
                if (showMessages)
                {
                    MessageBox.Show(string.Join("\n", errorMessages), "Invalid Deck!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }

            return true;
        }

        // Sort ListBox Items based on DeckBuilderCard
        private ItemCollection SortListBoxDeck(ItemCollection array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = (DeckBuilderCardArt) array[leftIndex];

            while (i <= j)
            {
                while (this.game.CompareCards((DeckBuilderCardArt) array[i], pivot) < 0)
                {
                    i++;
                }

                while (this.game.CompareCards((DeckBuilderCardArt)array[j], pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    DeckBuilderCardArt temp = (DeckBuilderCardArt) array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortListBoxDeck(array, leftIndex, j);

            if (i < rightIndex)
                SortListBoxDeck(array, i, rightIndex);

            return array;
        }

        // Add Import MenuItem
        private void AddImportMenuItem(ImportMenuItem importMenu)
        {
            // Instantiate Function
            void importMenuItem_Click(object sender, RoutedEventArgs e)
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.openedFile);
                openDialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.openedFile);
                openDialog.DefaultExt = importMenu.DefaultExtension;
                openDialog.Filter = importMenu.FileFilter;

                if (openDialog.ShowDialog() == true)
                {
                    var deckFile = importMenu.Import(openDialog.FileName);

                    if (deckFile.Game != this.game.Name)
                    {
                        MessageBox.Show("There was a problem loading this Deck File. Make sure the Game is correct.",
                            "Problem Loading Deck File", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    LoadFromDeckFile(deckFile);
                }
            }

            // Create Menu Item
            MenuItem menuItem = new MenuItem();
            menuItem.Header = importMenu.Header;
            menuItem.Click += importMenuItem_Click;
            MenuItem_Import.Items.Add(menuItem);
        }

        // Add Export MenuItem
        private void AddExportMenuItem(ExportMenuItem exportMenu)
        {
            // Instantiate Function
            void exportMenuItem_Click(object sender, RoutedEventArgs e)
            {
                if (!CheckDecksValid()) { return; }

                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.openedFile);
                saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.openedFile);
                saveDialog.DefaultExt = exportMenu.DefaultExtension;
                saveDialog.Filter = exportMenu.FileFilter;

                if (saveDialog.ShowDialog() == true)
                {
                    exportMenu.Export(saveDialog.FileName, Contexts.FileLoadContext.CreateDeckFile(this.game.Name, this.format.Name, this.deckControls));
                }
            }

            // Create Menu Item
            MenuItem menuItem = new MenuItem();
            menuItem.Header = exportMenu.Header;
            menuItem.Click += exportMenuItem_Click;
            MenuItem_Export.Items.Add(menuItem);
        }

        // Control Events

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

        // Clear Search Terms and Filters
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
            if (card != null)
            {
                string defaultDeckName = this.format.DefaultDeckName(card);
                Tuple<TextBlock, ListBox, IDeck>? controls_Deck = this.deckControls.GetValueOrDefault(defaultDeckName);
                if (controls_Deck != null)
                {
                    this.AddCard(card, controls_Deck);
                }
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
            string deck = (string)image.Tag;
            Tuple<TextBlock, ListBox, IDeck>? controls_Deck = this.deckControls.GetValueOrDefault(deck);
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            if (card != null && controls_Deck != null)
            {
                RemoveCard(card, controls_Deck);
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
                dragData.SetData("listTag", (string) imageControl.Tag);
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
            string? imageTag = e.Data.GetData("listTag") as string;
            if (!e.Data.GetDataPresent("myFormat") || sender == e.OriginalSource ||
                (!e.KeyStates.HasFlag(DragDropKeyStates.ControlKey) && imageTag != null && imageTag == ((string)((ListBox)sender).Tag)))
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
                    Tuple<TextBlock, ListBox, IDeck>? controlsTo = this.deckControls.GetValueOrDefault((string) listbox.Tag);

                    // Determine if Card is being Moved
                    if (e.Effects.HasFlag(DragDropEffects.Move) && !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey)
                        && e.Data.GetDataPresent("listTag"))
                    {
                        string? originalDeck = e.Data.GetData("listTag") as string;
                        Tuple<TextBlock, ListBox, IDeck>? originalControls = this.deckControls.GetValueOrDefault(originalDeck != null ? originalDeck : "");
                        if (originalControls != null && controlsTo != null)
                        {
                            MoveCard(card, originalControls, controlsTo);
                        }
                    }
                    else if (controlsTo != null)
                    {
                        AddCard(card, controlsTo);
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
                string? deck = e.Data.GetData("listTag") as string;
                Tuple<TextBlock, ListBox, IDeck>? controls_Deck = deck != null ? this.deckControls.GetValueOrDefault(deck) : null;
                DeckBuilderCardArt? card = e.Data.GetData("myFormat") as DeckBuilderCardArt;
                if (controls_Deck != null && card != null)
                {
                    RemoveCard(card, controls_Deck);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file;

                if (filePaths.Length == 1 && (file = filePaths[0]).EndsWith(".mtdk"))
                {
                    string? jsonText = Contexts.FileLoadContext.ReadFromFile(file);

                    if (jsonText == null) { return; }

                    DeckBuilderDeckFile? deckFile = Contexts.FileLoadContext.ConvertFromJson(jsonText);

                    if (deckFile == null || deckFile.Game != this.game.Name)
                    {
                        MessageBox.Show("There was a problem loading this Deck File. Make sure the Game is correct.",
                            "Problem Loading Deck File", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoadFromDeckFile(deckFile);
                }
            }
        }

        // New Deck
        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearDecks();

            this.openedFile = "";
        }

        // Open Deck File
        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.openedFile);
            openDialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.openedFile);
            openDialog.DefaultExt = ".mtdk";
            openDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

            if (openDialog.ShowDialog() != true) { return; }

            string? jsonText = Contexts.FileLoadContext.ReadFromFile(openDialog.FileName);

            if (jsonText == null) { return; }

            DeckBuilderDeckFile? deckFile = Contexts.FileLoadContext.ConvertFromJson(jsonText);

            if (deckFile == null || deckFile.Game != this.game.Name)
            {
                MessageBox.Show("There was a problem loading this Deck File. Make sure the Game is correct.",
                    "Problem Loading Deck File", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.openedFile = openDialog.FileName;
            LoadFromDeckFile(deckFile);
        }

        // Can Save Deck
        private void CommandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CheckDecksValid(false);
        }
        
        // Save Deck
        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!CheckDecksValid()) { return; }

            string filePath;
            if (this.openedFile == "")
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.FileName = "New Deck";
                saveDialog.DefaultExt = ".mtdk";
                saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.openedFile);
                saveDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

                bool? result = saveDialog.ShowDialog();

                if (result == false) { return; }

                filePath = saveDialog.FileName;
                this.openedFile = filePath;
            }
            else
            {
                filePath = this.openedFile;
            }

            string jsonText = Contexts.FileLoadContext.ConvertToJSON(game.Name, format.Name, this.deckControls);
            Contexts.FileLoadContext.WriteToFile(jsonText, filePath);
        }

        // Save As
        private void CommandBinding_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!CheckDecksValid()) { return; }

            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.openedFile);
            saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.openedFile);
            saveDialog.DefaultExt = ".mtdk";
            saveDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

            bool? result = saveDialog.ShowDialog();

            if (result != true) { return; }

            string filePath = saveDialog.FileName;
            this.openedFile = filePath;

            string jsonText = Contexts.FileLoadContext.ConvertToJSON(game.Name, format.Name, this.deckControls);
            Contexts.FileLoadContext.WriteToFile(jsonText, filePath);
        }

        // Check Deck is Valid
        private void MenuItem_CheckValid_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckDecksValid()) { return; }
            MessageBox.Show(string.Format("This Deck is allowed for {0}!", this.format.LongName), "Pass!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Sort Deck
        private void MenuItem_Sort_Click(object sender, RoutedEventArgs e)
        {
            foreach (var controls_Deck in this.deckControls.Values)
            {
                ListBox listBox = controls_Deck.Item2;

                if (listBox.Items.Count > 1)
                {
                    SortListBoxDeck(listBox.Items, 0, listBox.Items.Count - 1);
                }
            }
        }

        // Open View Stats
        private void MenuItem_ViewStats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.format.GetDetailedStats(this.GetAllDecks()), "Deck Statistics", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        // Export to Image
        private void MenuItem_ExportImage_Click(object sender, RoutedEventArgs e)
        {
            var exportWindow = new ExportImage(
                this.game.LongName,
                this.format.LongName,
                this.deckControls.Select(x => new KeyValuePair<string, IEnumerable<DeckBuilderCardArt>>(
                    this.format.Decks.Where(deck => deck.Name == x.Key).First().Label, //Get Label instead of Key
                    x.Value.Item2.Items.Cast<DeckBuilderCardArt>()) // Get Items
                )
            );
            exportWindow.Show();
        }

        // Export to Image in Tabletop Simulator Custom Deck Format
        private void MenuItem_TableTop_Click(object sender, RoutedEventArgs e)
        {
            //Contexts.FileLoadContext.ExportImageDialog(this.openedFile);
        }

        // Set Preferences for the Application
        private void CommandBinding_Preferences_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        // Open About Window for Plug-In
        private void MenuItem_PlugInAbout_Click(object sender, RoutedEventArgs e)
        {
            string programAbout = this.game.AboutInformation;
            About aboutWindow = new About(programAbout);
            aboutWindow.ShowDialog();
        }

        // Open About Window for Program
        private void MenuItem_ProgramAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }
    }
}
