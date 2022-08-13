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
using System.Windows.Interop;
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
        const double cardWidth = 250;
        const double textSize = 50;
        string openedFile;
        public RenderTargetBitmap CreatedImage { get; }

        public ExportImage(string game, string format, IEnumerable<KeyValuePair<string, IEnumerable<DeckBuilderCardArt>>> decks, string openedFile)
        {
            InitializeComponent();

            // Store Created BitmapRender as Global Variable
            this.CreatedImage = CreateImage(game, format, decks);
            this.openedFile = openedFile;
        }

        private RenderTargetBitmap CreateImage(string game, string format, IEnumerable<KeyValuePair<string, IEnumerable<DeckBuilderCardArt>>> decks)
        {
            // Create Drawiong Canvas
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Initialize Variables
            double cardHeight = 0;
            double y = 0;
            foreach (var deck in decks)
            {
                // Draw Deck Name Text
                FormattedText deckName = new FormattedText(deck.Key,
                    System.Globalization.CultureInfo.GetCultureInfo("en-au"),
                    FlowDirection.LeftToRight,
                    new Typeface(""),
                    textSize,
                    Brushes.White,
                    1.0);

                var _textGeometry = deckName.BuildGeometry(new System.Windows.Point(0, y));
                drawingContext.DrawGeometry(Brushes.Black, new System.Windows.Media.Pen(Brushes.White, 1.5), _textGeometry);

                // Increment Y Point by Text Size
                y += textSize;

                double cardInRow = 0;
                foreach (var card in deck.Value)
                {
                    if (card.Orientation == CardArtOrientation.Portrait)
                    {
                        // Initialize Height Variable
                        if (cardHeight == 0)
                        {
                            cardHeight = card.ImageFile.Height * (cardWidth / card.ImageFile.Width);
                        }

                        // Draw Image
                        drawingContext.DrawImage(card.ImageFile, new Rect(cardInRow * cardWidth, y, cardWidth, cardHeight));
                        cardInRow++;
                    }
                    else
                    {
                        // Initialize Height Variable
                        if (cardHeight == 0)
                        {
                            cardHeight = card.ImageFile.Width * (cardWidth / card.ImageFile.Height);
                        }

                        // Rotate Bitmap Image
                        RotateTransform transform = new RotateTransform(90);
                        BitmapImage rotatedImage = card.ImageFile.Clone();
                        rotatedImage.Rotation = Rotation.Rotate90;

                        // Draw Image
                        drawingContext.DrawImage(rotatedImage, new Rect(cardInRow * cardWidth, y, cardWidth, cardHeight));
                        drawingContext.Pop();
                        cardInRow++;
                    }

                    // If 10 Cards have been drawn in the Row, go to the next Row.
                    if (cardInRow >= 10)
                    {
                        cardInRow = 0;
                        y += cardHeight;
                    }
                }

                // Increment Y Point, if Row was not filled
                if (cardInRow != 0)
                {
                    y += cardHeight;
                }
            }

            // Close Drawing Context
            drawingContext.Close();

            // Get Bitmap Width and Height
            int bmpWidth = (int)(cardWidth * 10);
            int bmpHeight = (int)y;

            // Create Bitmap Render and Set to Image
            RenderTargetBitmap bmp = new RenderTargetBitmap(bmpWidth, bmpHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            image_Deck.Source = bmp;

            return bmp;
        }

        private void CommandBinding_Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetImage(this.CreatedImage);
            MessageBox.Show("Image has been copied to your Clipboard!", "Copied!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Contexts.FileLoadContext.ExportImageDialog(this.CreatedImage, this.openedFile);
        }
    }
}
