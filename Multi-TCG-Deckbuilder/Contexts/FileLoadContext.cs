using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Models;
using System.Text.Json;
using System.IO;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Contexts
{
    internal class FileLoadContext
    {

        public static DeckBuilderDeckFile CreateDeckFile(string gameName, string formatName,
            Dictionary<string, Tuple<System.Windows.Controls.TextBlock, System.Windows.Controls.ListBox, IDeck>> deckListBoxes)
        {
            List<DeckBuilderDeck> decks = new List<DeckBuilderDeck>();
            foreach (var valuePair in deckListBoxes)
            {
                decks.Add(new DeckBuilderDeck(valuePair.Key, valuePair.Value.Item2.Items.Cast<DeckBuilderCard>()));
            }

            DeckBuilderDeckFile deckFile = new DeckBuilderDeckFile(gameName, formatName, decks.ToArray());
            return deckFile;
        }

        public static string ConvertToJSON(string gameName, string formatName,
            Dictionary<string, Tuple<System.Windows.Controls.TextBlock, System.Windows.Controls.ListBox, IDeck>> deckListBoxes)
        {
            DeckBuilderDeckFile deckFile = CreateDeckFile(gameName, formatName, deckListBoxes);
            return JsonSerializer.Serialize<DeckBuilderDeckFile>(deckFile);
        }

        public static void WriteToFile(string deckFile, string filePath)
        {
            // Write To File
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(deckFile);
                writer.Close();
            }
        }

        public static string? ReadFromFile(string filePath)
        {
            if (!filePath.EndsWith(".mtdk")) { return null; }
            string deckFile;
            using (StreamReader reader = new StreamReader(filePath))
            {
                deckFile = reader.ReadToEnd();
            }
            return deckFile;
        }

        public static DeckBuilderDeckFile? ConvertFromJson(string deckFile)
        {
            return JsonSerializer.Deserialize<DeckBuilderDeckFile>(deckFile);
        }

        public static bool ExportImageDialog(RenderTargetBitmap bitmap, string openedFile = "")
        {
            // Create Save Dialog
            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "PNG Image (.png)|*.png|JPEG Image (.jpg)|*.jpg|Bitmap Image (.bmp)|*.bmp";
            saveDialog.FileName = Path.GetFileNameWithoutExtension(openedFile);
            saveDialog.InitialDirectory = Path.GetDirectoryName(openedFile);

            if (saveDialog.ShowDialog() == true)
            {
                // Save to Image File
                using (var fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    switch (saveDialog.FilterIndex)
                    {
                        case 0:
                            BitmapEncoder pngEncoder = new PngBitmapEncoder();
                            pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                            pngEncoder.Save(fileStream);
                            break;
                        case 1:
                            BitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                            jpgEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                            jpgEncoder.Save(fileStream);
                            break;
                        case 2:
                            BitmapEncoder bmpEncoder = new BmpBitmapEncoder();
                            bmpEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                            bmpEncoder.Save(fileStream);
                            break;
                    }

                    fileStream.Close();
                }

                return true;
            }

            return false;
        }
    }
}
