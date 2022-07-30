using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECipher
{
    internal class LackeyCCGImport : ImportMenuItem
    {
        public string Header { get => "LackeyCCG"; }

        public string DefaultExtension { get => ".dek"; }

        public string FileFilter { get => "LackeyCCG Deck File (.dek)|*.dek"; }

        public DeckBuilderDeckFile Import(string filePath)
        {
            throw new NotImplementedException();
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
