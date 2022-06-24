using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class Deck
    {
        [JsonPropertyName("name")]
        string name;
        [JsonPropertyName("longName")]
        string fullName;
        [JsonPropertyName("defaultMaxCopies")]
        int defaultMaxCopies;
        [JsonPropertyName("minSize")]
        int minSize;
        [JsonPropertyName("maxSize")]
        int maxSize;
        [JsonPropertyName("validateAdd")]
        string? validateAdd;
        [JsonPropertyName("validateDeck")]
        string? validateDeck;
    }
}
