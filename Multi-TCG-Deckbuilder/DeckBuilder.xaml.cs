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
using Multi_TCG_Deckbuilder.ViewModels;

namespace Multi_TCG_Deckbuilder
{

    /// <summary>
    /// Interaction logic for DeckBuilder.xaml
    /// </summary>
    public partial class DeckBuilder : Window
    {
        public DeckBuilderViewModel ViewModel { get; }

        public DeckBuilder(IGamePlugIn gamePlugIn, IFormat format, DeckBuilderDeckFile? deckFile = null, string openFile = "")
        {
            ViewModel = new DeckBuilderViewModel(gamePlugIn, format);

            InitializeComponent();

            if (deckFile != null)
            {
                // Automatically Open Deck From File
                LoadFromDeckFile(deckFile);
                ViewModel.OpenedFilePath = openFile;
            }
        }

        // Load Deck from Deck File
        private void LoadFromDeckFile(DeckBuilderDeckFile? deckFile)
        {
            if (deckFile == null || deckFile.Game != ViewModel.GameName)
            {
                MessageBox.Show("There was a problem loading this Deck File. Make sure the Game is correct.",
                    "Problem Loading Deck File", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (deckFile.Format != ViewModel.Format.Name && ViewModel.GetGameFormat(deckFile.Format) != null &&
                MessageBox.Show("You are about to change formats, are you sure?", "Change Formats?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
            {
                return;
            }

            ViewModel.LoadFromDeckFile(deckFile);
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
            if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush ||  searchText.Length < 4)
            {
                ViewModel.BasicSearchText = "";
                return;
            }

            ViewModel.BasicSearchText = searchText;
        }

        // Filter Button Clicked
        private void button_AdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdvancedSearch searchWindow = new AdvancedSearch(ViewModel.DeckBuilderService.SearchFields);
                if (searchWindow.ShowDialog() == true)
                {
                    ViewModel.AdvancedSearchCriteria = searchWindow.SearchFields;
                }
            }
            catch (NotImplementedException error)
            {
                Console.WriteLine("{0}\n{1}", error.Message, error.StackTrace);
                MessageBox.Show("Advanced Search is not implemented for this Plug-In. Disabling button.", "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Error);
                button_AdvancedSearch.IsEnabled = false;
            }
        }

        // Clear Search Terms and Filters
        private void button_ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearFilters();
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
            CardModel? card = image.DataContext as CardModel;
            if (card != null)
            {
                string toDeck = ViewModel.DeckBuilderService.DefaultDeckName(card);
                if (toDeck == "")
                {
                    ViewModel.AddCardToFirst(card);
                }
                else
                {
                    ViewModel.AddCard(card, toDeck);
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
            CardModel? card = image.DataContext as CardModel;
            if (card != null && deck != null)
            {
                ViewModel.RemoveCard(card, deck);
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
                var card = e.Data.GetData("myFormat") as CardModel;
                ListBox? listbox = sender as ListBox;

                if (card != null && listbox != null)
                {
                    string? deckTo = listbox.Tag as string;

                    // Determine if Card is being Moved
                    if (e.Effects.HasFlag(DragDropEffects.Move) && !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey)
                        && e.Data.GetDataPresent("listTag"))
                    {
                        string? originalDeck = e.Data.GetData("listTag") as string;
                        if (originalDeck != null && deckTo != null)
                        {
                            ViewModel.MoveCard(card, originalDeck, deckTo);
                        }
                    }
                    else if (deckTo != null)
                    {
                        ViewModel.AddCard(card, deckTo);
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
                CardModel? card = e.Data.GetData("myFormat") as CardModel;
                if (deck != null && card != null)
                {
                    ViewModel.RemoveCard(card, deck);
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

                    LoadFromDeckFile(deckFile);
                }
            }
        }

        // New Deck
        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ClearDecks();
            ViewModel.OpenedFilePath = "";
        }

        // Open Deck File
        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(ViewModel.OpenedFilePath);
            openDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.OpenedFilePath);
            openDialog.DefaultExt = ".mtdk";
            openDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

            if (openDialog.ShowDialog() != true) { return; }

            string? jsonText = Contexts.FileLoadContext.ReadFromFile(openDialog.FileName);

            if (jsonText == null) { return; }

            DeckBuilderDeckFile? deckFile = Contexts.FileLoadContext.ConvertFromJson(jsonText);

            ViewModel.OpenedFilePath = openDialog.FileName;
            LoadFromDeckFile(deckFile);
        }

        // Can Save Deck
        private void CommandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = ViewModel.DecksValid;
        }
        
        // Save Deck
        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ViewModel.DecksValid) { return; }

            string filePath;
            if (ViewModel.OpenedFilePath == "")
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.FileName = "New Deck";
                saveDialog.DefaultExt = ".mtdk";
                saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.OpenedFilePath);
                saveDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

                bool? result = saveDialog.ShowDialog();

                if (result == false) { return; }

                filePath = saveDialog.FileName;
                ViewModel.OpenedFilePath = filePath;
            }
            else
            {
                filePath = ViewModel.OpenedFilePath;
            }

            string jsonText = ViewModel.GetDeckBuilderDeckFileAsJSON();
            Contexts.FileLoadContext.WriteToFile(jsonText, filePath);
        }

        // Save As
        private void CommandBinding_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ViewModel.DecksValid) { return; }

            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(ViewModel.OpenedFilePath);
            saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.OpenedFilePath);
            saveDialog.DefaultExt = ".mtdk";
            saveDialog.Filter = "Multi-TCG Deck Builder File (.mtdk)|*.mtdk";

            bool? result = saveDialog.ShowDialog();

            if (result != true) { return; }

            string filePath = saveDialog.FileName;
            ViewModel.OpenedFilePath = filePath;

            string jsonText = ViewModel.GetDeckBuilderDeckFileAsJSON();
            Contexts.FileLoadContext.WriteToFile(jsonText, filePath);
        }

        // Check Deck is Valid
        private void MenuItem_CheckValid_Click(object sender, RoutedEventArgs e)
        {
            var errorMessages = ViewModel.CheckDecksValid();
            if (errorMessages.Count() == 0)
            {
                MessageBox.Show(string.Format("This Deck is allowed for {0}!", ViewModel.Format.LongName), "Pass!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(string.Join("\n", errorMessages), "Invalid Deck!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sort Deck
        private void MenuItem_Sort_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SortDecks();
        }

        // Open View Stats
        private void MenuItem_ViewStats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ViewModel.AdvancedDetailsText, "Deck Statistics", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        // Export to Image
        private void MenuItem_ExportImage_Click(object sender, RoutedEventArgs e)
        {
            var exportWindow = new ExportImage(
                ViewModel.GameLongName,
                ViewModel.FormatLongName,
                ViewModel.Decks,
                ViewModel.OpenedFilePath
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
            foreach (var deck in ViewModel.Decks)
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

            Contexts.FileLoadContext.ExportImageDialog(bmp, ViewModel.OpenedFilePath);
        }

        // Set Preferences for the Application
        private void CommandBinding_Preferences_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        // Open About Window for Plug-In
        private void MenuItem_PlugInAbout_Click(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About(ViewModel.PlugInAbout);
            aboutWindow.ShowDialog();
        }

        // Open About Window for Program
        private void MenuItem_ProgramAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void MenuItem_File_Opening(object sender, RoutedEventArgs e)
        {
            MenuItem_Export.IsEnabled = ViewModel.DecksValid;
        }

        // Generic ImportMenuItem Click
        void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IImportMenuItem? importMenu = ((MenuItem)sender).DataContext as IImportMenuItem;

            if (importMenu == null)
            {
                return;
            }

            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(ViewModel.OpenedFilePath);
            openDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.OpenedFilePath);
            openDialog.DefaultExt = importMenu.DefaultExtension;
            openDialog.Filter = importMenu.FileFilter;

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    var deckFile = importMenu.Import(openDialog.FileName, ViewModel.FormatName);

                    if (deckFile.Game != ViewModel.GameName)
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

        // Generic ExportMenuItem Click
        void exportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IExportMenuItem? exportMenu = ((MenuItem)sender).DataContext as IExportMenuItem;

            if (exportMenu == null || !ViewModel.DecksValid)
            {
                return;
            }

            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(ViewModel.OpenedFilePath);
            saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.OpenedFilePath);
            saveDialog.DefaultExt = exportMenu.DefaultExtension;
            saveDialog.Filter = exportMenu.FileFilter;

            if (saveDialog.ShowDialog() == true)
            {
                exportMenu.Export(saveDialog.FileName, Contexts.FileLoadContext.CreateDeckFile(ViewModel.GameName, ViewModel.FormatName, ViewModel.Decks));
                MessageBox.Show("Export Completed!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
