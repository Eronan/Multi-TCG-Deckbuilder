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

        public DeckBuilder(IGamePlugIn gamePlugIn, Format format)
        {
            InitializeComponent();
            this.Title = gamePlugIn.LongName + "Deck Builder";
            this.game = gamePlugIn;
            this.format = format;
            fullList = DeckBuilderCardArt.GetFromCards(format.CardList, AppDomain.CurrentDomain.BaseDirectory);
            listBox_CardResults.ItemsSource = fullList;
        }
    }
}
