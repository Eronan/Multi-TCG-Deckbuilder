using IGamePlugInBase.IO;
using Multi_TCG_Deckbuilder.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Contexts
{
    /// <summary>
    /// Static Methods that are used to Create a (.mtdk) File
    /// </summary>
    internal static class FileLoadContext
    {
        /// <summary>
        /// Initializes a Deck File
        /// </summary>
        /// <param name="gameName">Short Name of the Game Plug-In</param>
        /// <param name="formatName">Short Name of the Format</param>
        /// <param name="deckControls">The Deck Controls including the ListBoxes</param>
        /// <returns></returns>
        public static DeckBuilderDeckFile CreateDeckFile(string gameName, string formatName,
            IEnumerable<DeckModel> deckModels)
        {
            List<DeckBuilderDeck> decks = new List<DeckBuilderDeck>();
            foreach (var deckModel in deckModels)
            {
                decks.Add(new DeckBuilderDeck(deckModel.DeckName, deckModel.Cards));
            }

            DeckBuilderDeckFile deckFile = new DeckBuilderDeckFile(gameName, formatName, decks.ToArray());
            return deckFile;
        }

        /// <summary>
        /// Converts a DeckBuilderDeckFile into JSON Text.
        /// </summary>
        /// <param name="gameName">Short Name of the Game Plug-In</param>
        /// <param name="formatName">Short Name of the Format</param>
        /// <param name="deckControls">The Deck Controls including the ListBoxes</param>
        /// <returns>JSON Text Serialized from the DeckBuilderDeckFile</returns>
        public static string ConvertToJSON(string gameName, string formatName,
            IEnumerable<DeckModel> deckModels)
        {
            DeckBuilderDeckFile deckFile = CreateDeckFile(gameName, formatName, deckModels);
            return JsonSerializer.Serialize<DeckBuilderDeckFile>(deckFile);
        }

        /// <summary>
        /// Writes Text to a File
        /// </summary>
        /// <param name="deckFile">JSON Text of a (.mtdk) File.</param>
        /// <param name="filePath">File Location to save the File to.</param>
        public static void WriteToFile(string deckFile, string filePath)
        {
            // Write To File
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(deckFile);
                writer.Close();
            }
        }

        /// <summary>
        /// Reads an (.mtdk) File.
        /// </summary>
        /// <param name="filePath">File Path of the (.mtdk) File.</param>
        /// <returns>JSON Text of the (.mtdk) File</returns>
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

        /// <summary>
        /// Deserializes a (.mtdk) File's JSON Text
        /// </summary>
        /// <param name="deckFile">JSON Text of the Deck File</param>
        /// <returns>Returns a DeckBuilderDeckFile Instance</returns>
        public static DeckBuilderDeckFile? ConvertFromJson(string deckFile)
        {
            return JsonSerializer.Deserialize<DeckBuilderDeckFile>(deckFile);
        }

        /// <summary>
        /// The SaveFileDialog that is used for Exporting Images and Encodes them.
        /// </summary>
        /// <param name="bitmap">The Image to be Exported.</param>
        /// <param name="openedFile">The File Path of the File to write to.</param>
        /// <returns>Successfully Exported Image</returns>
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
