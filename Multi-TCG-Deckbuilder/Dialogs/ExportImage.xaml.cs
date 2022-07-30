using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Multi_TCG_Deckbuilder.Dialogs
{
    /// <summary>
    /// Interaction logic for ExportImage.xaml
    /// </summary>
    public partial class ExportImage : Window
    {
        public BitmapImage CreatedImage { get; }

        public ExportImage(string game, string format, IEnumerable<KeyValuePair<string, IEnumerable<DeckBuilderCardArt>>> decks)
        {
            InitializeComponent();

            this.CreatedImage = CreateImage(game, format, decks);
        }

        private BitmapImage CreateImage(string game, string format, IEnumerable<KeyValuePair<string, IEnumerable<DeckBuilderCardArt>>> decks)
        {
            var drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            int height = 0;
            int y = 0;

            foreach (var deck in decks)
            {
                drawingContext.DrawText(new FormattedText(deck.Key,
                        System.Globalization.CultureInfo.GetCultureInfo("en-au"),
                        FlowDirection.LeftToRight,
                        new Typeface(""),
                        20,
                        Brushes.Black,
                        1.0),
                    new Point(0, y));

                int x = 0;
                y += 20;
                foreach (var card in deck.Value)
                {
                    if (card.Orientation == CardArtOrientation.Portrait)
                    {
                        drawingContext.DrawImage(card.ImageFile, new Rect(x, y, 308, height));
                        x += 308;
                    }
                    else
                    {

                    }
                }
            }

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(3080, 0, 72, 72, PixelFormats.Default);
            renderTargetBitmap.Render(drawingVisual);

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
