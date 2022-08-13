using IGamePlugInBase;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FECipher
{
    internal class LackeyCCGImport : ImportMenuItem
    {
        public string Header { get => "LackeyCCG"; }

        public string DefaultExtension { get => ".dek"; }

        public string FileFilter { get => "LackeyCCG Deck File (.dek)|*.dek"; }

        public DeckBuilderDeckFile Import(string filePath)
        {
            // Find Installed Plug-Ins
            XmlDocument lackeyCCGDeck = new XmlDocument();
            lackeyCCGDeck.Load(filePath);

            if (lackeyCCGDeck.DocumentElement != null)
            {
                lackeyCCGDeck.DocumentElement.SelectNodes("/superzone");
            }

            DeckBuilderDeckFile file = new DeckBuilderDeckFile("unlimited", "unlimited", new DeckBuilderDeck[0]);
            return file;
        }
    }

    internal class LackeyCCGExport : ExportMenuItem
    {
        public string Header { get => "LackeyCCG"; }

        public string DefaultExtension { get => ".dek"; }

        public string FileFilter { get => "LackeyCCG Deck File (.dek)|*.dek"; }

        public void Export(string filePath, DeckBuilderDeckFile decks)
        {
            throw new NotImplementedException();
        }
    }
}
