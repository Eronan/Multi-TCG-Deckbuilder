using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class Restriction
    {
        [JsonPropertyName("cardId")]
        public string cardId { get; set; }
        [JsonPropertyName("restrictionType")]
        public string restrictionType;
        [JsonPropertyName("maxCopies")]
        public int? maxCopies { get; set; }
        [JsonPropertyName("validateAdd")]
        public string? validateAdd;
        [JsonPropertyName("validateDeck")]
        public string? validateDeck;
    }
}
