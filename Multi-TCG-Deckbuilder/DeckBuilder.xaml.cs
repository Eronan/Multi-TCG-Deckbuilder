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
            this.Title = gamePlugIn.LongName + "Deck Builder";
            this.game = gamePlugIn;
            this.format = format;
            this.fullList = DeckBuilderCardArt.GetFromCards(format.CardList, AppDomain.CurrentDomain.BaseDirectory);
            this.searchList = new List<DeckBuilderCardArt>();

            this.deckListBoxes = new Dictionary<string, ListBox>();
            this.CreateDeckListBoxes(format.Decks);
        }

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

        private void AddCard(DeckBuilderCardArt card, ListBox listbox_Deck)
        {
            //List<DeckBuilderCardArt> cards = (List<DeckBuilderCardArt>)listbox_Deck.ItemsSource;
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

        private void ImageCardSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image) sender;
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            if (card == null) return;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                ListBox? deck = this.deckListBoxes.GetValueOrDefault(this.format.DefaultDeckName);
                if (deck == null) return;
                this.AddCard(card, deck);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragCard = card;
                this.Cursor = Cursors.Cross;
            }
        }

        private void ImageCardDeck_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            Deck deck = (Deck) image.Tag;
            ListBox? listbox_Deck = this.deckListBoxes.GetValueOrDefault(deck.Name);
            DeckBuilderCardArt? card = image.DataContext as DeckBuilderCardArt;
            if (card == null || listbox_Deck == null) return;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                listbox_Deck.Items.Remove(image.DataContext);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragCard = card;
                this.Cursor = Cursors.Cross;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.dragCard = null;
        }

        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox listbox = (ListBox) sender;
            if (dragCard == null)
            {
                return;
            }
            this.AddCard(dragCard, listbox);
            this.Cursor = Cursors.Arrow;
            this.dragCard = null;
        }
    }
}
