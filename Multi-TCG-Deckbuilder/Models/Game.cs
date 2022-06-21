using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class Game
    {
        string name;
        string longName;
        string version;
        int defaultMaxCopies;
        Format[] formats;
        dynamic cardlist;
        dynamic deckSizes;

        public Game()
        {

        }
    }
}
