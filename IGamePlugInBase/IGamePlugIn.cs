using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    public interface IGamePlugIn
    {
        //Variables
        public string Name { get; }
        public string LongName { get; }
        public string IconLocation { get; }
        public byte[] IconImage { get; }
        public Dictionary<string, Format> Formats { get; }
        dynamic[] CardList { get; }

        //Methods
        bool ValidateAdd(dynamic card, dynamic[] deckList);
        bool ValidateDeck(dynamic[] deckList);
    }
}
