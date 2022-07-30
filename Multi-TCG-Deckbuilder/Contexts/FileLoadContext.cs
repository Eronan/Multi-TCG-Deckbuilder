using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGamePlugInBase;
using Multi_TCG_Deckbuilder.Models;
using System.Text.Json;
using System.IO;

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
    }
}
