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
        DeckBuilderCardArt? dragCard;

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
            textblock_Label.Text = deck.Label + ":";

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
            listBox_Deck.Name = "listBox_" + deck.Name;
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
            listBox_Deck.DragEnter += this.ListBox_Deck_DragEnter;
            listBox_Deck.Drop += this.ListBox_Deck_Drop;
            //listBox_Deck.PreviewMouseLeftButtonUp += ListBox_MouseUp;

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

        //Sub-Routine for Adding a Card to a ListBox Item Collection
        private void AddCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            Deck? deck = listbox_Deck.Tag as Deck;
            if (deck == null || !deck.ValidateAdd(card, listbox_Deck.Items.Cast<DeckBuilderCardArt>()))
            {
                return;
            }
            listbox_Deck.Items.Add(card);
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
            this.searchList = this.fullList.Where(item => item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            this.listBox_CardResults.ItemsSource = this.searchList;
        }

        private void button_AdvancedSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImageCardSearch_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image) sender;
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            ListBox? deck = this.deckListBoxes.GetValueOrDefault(this.format.DefaultDeckName);
            if (card != null && deck != null)
            {
                this.AddCard(card, deck);
            }
        }

        private void ImageCardDeck_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            Deck deck = (Deck)image.Tag;
            ListBox? listbox_Deck = this.deckListBoxes.GetValueOrDefault(deck.Name);
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            if (card != null && listbox_Deck != null)
            {
                listbox_Deck.Items.Remove(image.DataContext);
            }
        }

        /*
        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                ListBox listbox = (ListBox)sender;
                if (dragCard == null)
                {
                    return;
                }
                this.AddCard(dragCard, listbox);
                this.Cursor = Cursors.Arrow;
                this.dragCard = null;
            }
        }
        */

        private void ImageResults_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Image imageControl = (Image)sender;
                DataObject dragData = new DataObject();
                dragData.SetData("myFormat", imageControl.DataContext);
                DragDrop.DoDragDrop(imageControl, dragData, DragDropEffects.Move);
            }
        }

        private void ListBox_Deck_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || 
                sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ListBox_Deck_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                DeckBuilderCardArt? card = e.Data.GetData("myFormat") as DeckBuilderCardArt;
                ListBox? listbox = sender as ListBox;
                if (card != null && listbox != null)
                {
                    AddCard(card, listbox);
                }
            }
        }
    }
}
