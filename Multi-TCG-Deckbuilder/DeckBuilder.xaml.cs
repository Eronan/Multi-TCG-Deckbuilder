using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Multi_TCG_Deckbuilder.Dialogs;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IGamePlugInBase.IO;
using Multi_TCG_Deckbuilder.Models;

namespace Multi_TCG_Deckbuilder
{

    /// <summary>
    /// Interaction logic for DeckBuilder.xaml
    /// </summary>
    public partial class DeckBuilder : Window
    {
        IGamePlugIn game;
        IFormat format;
        IDeckBuilderService deckBuilderService;
        List<CardModel> fullList;
        List<CardModel> advancedSearchList;
        List<CardModel> searchList;
        Dictionary<string, DeckModel> DeckModel;
        string openedFile;

        public DeckBuilder(IGamePlugIn gamePlugIn, IFormat format, DeckBuilderDeckFile? deckFile = null, string openFile = "")
        {
            InitializeComponent();

            // Set Up Variables
            this.game = gamePlugIn;
            this.format = format;
            this.deckBuilderService = format.DeckBuilderService;

            // Create Necessary Lists
            this.fullList = new List<CardModel>();
            this.searchList = new List<CardModel>();
            this.DeckModel = new Dictionary<string, DeckModel>();
            this.advancedSearchList = new List<CardModel>();

            // Create Import Menu Items
            if (gamePlugIn.ImportMenus == null || gamePlugIn.ImportMenus.Count() == 0)
            {
                MenuItem_Import.IsEnabled = false;
            }
            else
            {
                foreach (var importMenu in gamePlugIn.ImportMenus)
                {
                    AddImportMenuItem(importMenu);
                }
            }

            // Create Export Menu Items
            if (gamePlugIn.ExportMenus != null)
            {
                foreach (var exportMenu in gamePlugIn.ExportMenus)
                {
                    AddExportMenuItem(exportMenu);
                }
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

            this.deckBuilderService = format.DeckBuilderService;
            this.deckBuilderService.InitializeService();

            if (this.format.Decks.Count() == 0)
            {
                throw new NotImplementedException("Decks for this Format were not implemented.");
            }

            this.button_AdvancedSearch.IsEnabled = this.deckBuilderService.SearchFields.Count() > 0;
            this.fullList = deckBuilderService.CardList.Select(card => new CardModel(card.CardID, card.ArtID, card.Name, card.FileLocation, card.DownloadLocation, card.Orientation, card.ViewDetails)).ToList();
            this.advancedSearchList = this.fullList;
            this.searchList.Clear();

            this.DeckModel = this.format.Decks.ToDictionary(val => val.Name, val => new DeckModel(val));
            panel_Decks.ItemsSource = this.DeckModel.Values;

            button_ViewStats.Content = this.deckBuilderService.GetStats(this.GetAllDecks());
        }

        // Clear Decks
        private void ClearDecks()
        {
            foreach (var controls_Deck in this.DeckModel.Values)
            {
                controls_Deck.Clear();

                button_ViewStats.Content = this.deckBuilderService.GetStats(this.GetAllDecks());
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
                try
                {
                    IFormat newFormat = this.game.Formats.First(item => item.Name == deckFile.Format);
                    ChangeFormat(newFormat);
                }
                catch (NotImplementedException e)
                {
                    Console.WriteLine(e.Message);
                    MessageBox.Show(string.Format("This Format is not properly implemented and cannot be opened.\n{0}", e.Message), "Format Not Implemented!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                // Clear Decks
                ClearDecks();
            }

            foreach (DeckBuilderDeck deck in deckFile.Decks)
            {
                DeckModel? controls_Deck = this.DeckModel.GetValueOrDefault(deck.DeckName);

                if (controls_Deck != null)
                {
                    foreach (DeckBuilderCard card in deck.Cards)
                    {
                        CardModel cardArt = this.fullList.First(item => item.CardID == card.CardID && item.ArtID == card.ArtID);
                        controls_Deck.Add(cardArt);
                    }
                }
            }

            button_ViewStats.Content = this.deckBuilderService.GetStats(this.GetAllDecks());
        }

        // Sub-Routine for getting Decks in a Format for being parsed to Plug-In Functions
        private Dictionary<string, IEnumerable<DeckBuilderCard>> GetAllDecks()
        {
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = new Dictionary<string, IEnumerable<DeckBuilderCard>>();
            foreach (var controls_Deck in this.DeckModel)
            {
                allDecks.Add(controls_Deck.Key, controls_Deck.Value.Cards);
            }

            return allDecks;
        }

        // Sub-Routine for Adding a Card to a ListBox Item Collection
        private bool AddCard(CardModel card, DeckModel controls_Deck)
        {
            // Convert ListBox Items to Cardlist Format
            Dictionary<string, IEnumerable<DeckBuilderCard>> allDecks = this.GetAllDecks();

            // Verify whether card has not reached its maximum allowable copies and can be added to the Deck
            if (!this.deckBuilderService.ValidateMaximum(card, allDecks) && controls_Deck.ValidateAdd(card))
            {
                controls_Deck.Add(card);
                button_ViewStats.Content = this.deckBuilderService.GetStats(allDecks);
                return true;
            }

            return false;
        }

        private bool RemoveCard(CardModel card, DeckModel controls_Deck)
        {
            if (controls_Deck.Remove(card))
            {
                button_ViewStats.Content = this.deckBuilderService.GetStats(this.GetAllDecks());
                return true;
            }

            return false;
        }

        // Sub-Routine for Moving a Card from a ListBox Item Collection to Another
        private bool MoveCard(CardModel card, DeckModel controls_DeckFrom, DeckModel controls_DeckTo)
        {
            if (controls_DeckTo.ValidateAdd(card) && controls_DeckFrom.Remove(card))
            {
                controls_DeckTo.Add(card);

                button_ViewStats.Content = this.deckBuilderService.GetStats(GetAllDecks());
                return true;
            }
            return false;
        }

        // Check Deck is Valid
        private bool CheckDecksValid(bool showMessages = true)
        {
            List<string> errorMessages = new List<string>();

            int cardsInDeck = 0;
            foreach (var controls_Deck in this.DeckModel.Values)
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

                if (errorMessages.Count > 0 || cardsInDeck == 0)
                {
                    if (showMessages)
                    {
                        if (cardsInDeck == 0)
                        {
                            errorMessages.Add("There are no cards in the Deck.");
                        }
                        MessageBox.Show(string.Join("\n", errorMessages), "Invalid Deck!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return false;
                }
            }


            return true;
        }

        // Add Import MenuItem
        private void AddImportMenuItem(IImportMenuItem importMenu)
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
                    try
                    {
                        var deckFile = importMenu.Import(openDialog.FileName, this.format.Name);

                        if (deckFile.Game != this.game.Name)
                        {
                            MessageBox.Show("There was a problem loading this Deck File. Make sure the Game is correct.",
                                "Problem Loading Deck File", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        LoadFromDeckFile(deckFile);
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Error Importing File. Please check that it is the correct file and game.\n" + error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // Create Menu Item
            MenuItem menuItem = new MenuItem();
            menuItem.Header = importMenu.Header;
            menuItem.Click += importMenuItem_Click;
            MenuItem_Import.Items.Add(menuItem);
        }

        // Add Export MenuItem
        private void AddExportMenuItem(IExportMenuItem exportMenu)
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
                    exportMenu.Export(saveDialog.FileName, Contexts.FileLoadContext.CreateDeckFile(this.game.Name, this.format.Name, this.DeckModel.Values));
                    MessageBox.Show("Export Completed!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
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
            else
            {
                string searchText = textBox_SearchText.Text;
                if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush || this.advancedSearchList == null || searchText.Length < 4)
                {
                    return;
                }

                this.searchList = this.advancedSearchList.Where(item => item.ViewDetails.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
                this.listBox_CardResults.ItemsSource = this.searchList;
            }
        }

        // Search For Card Function
        private void textBox_SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string searchText = textBox_SearchText.Text;
                if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush || this.advancedSearchList == null || searchText.Length < 4)
                {   
                    return;
                }

                this.searchList = this.advancedSearchList.Where(item => item.ViewDetails.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
                this.listBox_CardResults.ItemsSource = this.searchList;
            }
        }

        // Filter Button Clicked
        private void button_AdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            AdvancedSearch searchWindow = new AdvancedSearch(this.deckBuilderService.SearchFields);
            if (searchWindow.ShowDialog() == true)
            {
                this.advancedSearchList = deckBuilderService.AdvancedFilterSearchList(this.fullList.Cast<CardModel>(), deckBuilderService.SearchFields).Cast<CardModel>().ToList();

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
            Image image = (Image)sender;
            CardModel? card = image.DataContext as CardModel;
            if (card != null)
            {
                string toDeck = this.deckBuilderService.DefaultDeckName(card);
                // Add to Default or First Deck
                DeckModel controls_Deck = this.DeckModel.GetValueOrDefault(toDeck) ?? this.DeckModel.Values.First();

                this.AddCard(card, controls_Deck);
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
            DeckModel? controls_Deck = this.DeckModel.GetValueOrDefault(deck);
            CardModel? card = image.DataContext as CardModel;
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
                dragData.SetData("listTag", (string)imageControl.Tag);
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
                CardModel? card = e.Data.GetData("myFormat") as CardModel;
                ListBox? listbox = sender as ListBox;

                if (card != null && listbox != null)
                {
                    DeckModel? controlsTo = this.DeckModel.GetValueOrDefault((string)listbox.Tag);

                    // Determine if Card is being Moved
                    if (e.Effects.HasFlag(DragDropEffects.Move) && !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey)
                        && e.Data.GetDataPresent("listTag"))
                    {
                        string? originalDeck = e.Data.GetData("listTag") as string;
                        DeckModel? originalControls = this.DeckModel.GetValueOrDefault(originalDeck != null ? originalDeck : "");
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
                DeckModel? controls_Deck = deck != null ? this.DeckModel.GetValueOrDefault(deck) : null;
                CardModel? card = e.Data.GetData("myFormat") as CardModel;
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

            string jsonText = Contexts.FileLoadContext.ConvertToJSON(game.Name, format.Name, this.DeckModel.Values);
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

            string jsonText = Contexts.FileLoadContext.ConvertToJSON(game.Name, format.Name, this.DeckModel.Values);
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
            foreach (var controls_Deck in this.DeckModel.Values)
            {
                controls_Deck.Sort(this.deckBuilderService.CompareCards);
            }
        }

        // Open View Stats
        private void MenuItem_ViewStats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.deckBuilderService.GetDetailedStats(this.GetAllDecks()), "Deck Statistics", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        // Export to Image
        private void MenuItem_ExportImage_Click(object sender, RoutedEventArgs e)
        {
            var exportWindow = new ExportImage(
                this.game.LongName,
                this.format.LongName,
                this.DeckModel.Values,
                this.openedFile
            );
            exportWindow.Show();
        }

        // Export to Image in Tabletop Simulator Custom Deck Format
        private void MenuItem_TableTop_Click(object sender, RoutedEventArgs e)
        {
            double cardWidth = 350;

            // Create Drawiong Canvas
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Initialize Variables
            double cardHeight = 0;
            double x = 0;
            double y = 0;
            foreach (var deck in this.DeckModel.Values)
            {
                foreach (var card in deck.Cards)
                {
                    var imageFile = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + card.FileLocation));

                    if (card.Orientation == CardArtOrientation.Portrait)
                    {
                        // Initialize Height Variable
                        if (cardHeight == 0)
                        {
                            cardHeight = imageFile.Height * (cardWidth / imageFile.Width);
                        }

                        // Draw Image
                        drawingContext.DrawImage(imageFile, new Rect(x, y, cardWidth, cardHeight));
                        x += cardWidth;
                    }
                    else
                    {
                        // Initialize Height Variable
                        if (cardHeight == 0)
                        {
                            cardHeight = imageFile.Width * (cardWidth / imageFile.Height);
                        }

                        // Rotate Bitmap Image
                        RotateTransform transform = new RotateTransform(90);
                        System.Windows.Media.Imaging.BitmapImage rotatedImage = imageFile.Clone();
                        rotatedImage.Rotation = System.Windows.Media.Imaging.Rotation.Rotate90;

                        // Draw Image
                        drawingContext.DrawImage(rotatedImage, new Rect(x, y, cardWidth, cardHeight));
                        drawingContext.Pop();
                        x += cardWidth;
                    }

                    // If 10 Cards have been drawn in the Row, go to the next Row.
                    if (x >= cardWidth * 10)
                    {
                        x = 0;
                        y += cardHeight;
                    }
                }
            }

            // Close Drawing Context
            drawingContext.Close();

            // Get Bitmap Width and Height
            int bmpWidth = (int)(cardWidth * 10);
            int bmpHeight = (int)(x == 0 ? y : y + cardHeight);

            // Create Bitmap Render and Set to Image
            System.Windows.Media.Imaging.RenderTargetBitmap bmp = new System.Windows.Media.Imaging.RenderTargetBitmap(bmpWidth, bmpHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            Contexts.FileLoadContext.ExportImageDialog(bmp, this.openedFile);
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

        private void MenuItem_File_Opening(object sender, RoutedEventArgs e)
        {
            if (CheckDecksValid(false))
            {
                MenuItem_Export.IsEnabled = true;
            }
            else
            {
                MenuItem_Export.IsEnabled = false;
            }
        }
    }
}