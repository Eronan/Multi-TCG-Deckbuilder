using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Models
{
    public class CardModel : DeckBuilderCardArt
    {
        public CardModel(string cardID, string artID, string name, string fileLocation, CardArtOrientation orientation, string viewDetails = "") : base(cardID, artID, name, fileLocation, orientation, viewDetails)
        {
        }

        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(new Uri(FullPath));
            }
        }

        public string FullPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + FileLocation; }
        }
    }
}
