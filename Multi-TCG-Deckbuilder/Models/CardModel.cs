using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Models
{
    public class CardModel : DeckBuilderCardArt
    {
        public CardModel(string cardID, string artID, string name, string fileLocation, string downloadUrl, CardArtOrientation orientation, string viewDetails = "") : base(cardID, artID, name, fileLocation, downloadUrl, orientation, viewDetails)
        {
        }

        public BitmapImage Image
        {
            get
            {
                /*
                if (false)
                {
                    return new BitmapImage(new Uri(DownloadLocation));
                }
                */

                if (!File.Exists(FullPath))
                {
                    //_ = MTCGHttpClientFactory.DownloadFile(DownloadLocation, FullPath);
                    return new BitmapImage(new Uri(DownloadLocation));
                }
                else
                {
                    return new BitmapImage(new Uri(FullPath));
                }
                
            }
        }

        public string FullPath
        {
            get
            {
                var returnPath = AppDomain.CurrentDomain.BaseDirectory + FileLocation;
                if (!File.Exists(returnPath))
                {
                    _ = MTCGHttpClientFactory.DownloadFile(DownloadLocation, returnPath);
                    return DownloadLocation;
                }
                else
                {
                    return returnPath;
                }
            }
        }
    }
}
