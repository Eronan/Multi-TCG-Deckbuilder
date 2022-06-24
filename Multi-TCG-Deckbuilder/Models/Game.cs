using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Drawing;
//using Newtonsoft.Json.Linq;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class Game
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("longName")]
        public string longName { get; set; }
        [JsonPropertyName("version")]
        public int defaultMaxCopies { get; set; }
        [JsonPropertyName("logo")]
        public string logo { get; set; }
        public IEnumerable<JsonValue> cardlist { get; set; }
        
    }
}
