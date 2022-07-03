using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Multi_TCG_Deckbuilder.Models;
using IGamePlugInBase;

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
        List<DeckBuilderCardArt> searchList;
        Dictionary<string, ListBox> deckListBoxes;

        public DeckBuilder(IGamePlugIn gamePlugIn, Format format)
        {
            InitializeComponent();
            this.Title = gamePlugIn.LongName + " Deck Builder";
            this.game = gamePlugIn;
            this.format = format;
            this.fullList = DeckBuilderCardArt.GetFromCards(format.CardList, AppDomain.CurrentDomain.BaseDirectory);
            this.searchList = new List<DeckBuilderCardArt>();

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

            // ListBox ItemsSource
            List<DeckBuilderCardArt> itemsSource = new List<DeckBuilderCardArt>();

            // Deck List Box
            ListBox listBox_Deck = new ListBox();
            listBox_Deck.Name = "listBoxDeck_" + deck.Name;
            listBox_Deck.Tag = deck;
            listBox_Deck.HorizontalAlignment = HorizontalAlignment.Stretch;
            listBox_Deck.VerticalAlignment = VerticalAlignment.Top;
            listBox_Deck.Width = double.NaN;
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
        private void AddCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            Deck? deck = listbox_Deck.Tag as Deck;
            if (deck != null && deck.ValidateAdd(card, listbox_Deck.Items.Cast<DeckBuilderCardArt>()))
            {
                listbox_Deck.Items.Add(card);
            }
        }

        // Sub-Routine for Removing a Card from a ListBox Item Collection
        private void RemoveCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            listbox_Deck.Items.Remove(card);
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
            if (textBox_SearchText.Foreground == SystemColors.GrayTextBrush || this.fullList == null || searchText.Length < 4)
            {
                return;
            }
            this.searchList = this.fullList.Where(item => item.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || item.ViewDetails.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            this.listBox_CardResults.ItemsSource = this.searchList;
        }

        // Filter Button Clicked
        private void button_AdvancedSearch_Click(object sender, RoutedEventArgs e)
        {

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
                    AddCard(card, listbox);

                    if (e.Effects.HasFlag(DragDropEffects.Move) && !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        ListBox? originalList = e.Source as ListBox;
                        if (originalList != null)
                        {
                            RemoveCard(card, originalList);
                        }
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
    }
}
