using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class Format
    {
        [JsonPropertyName("name")]
        string name;
        [JsonPropertyName("longName")]
        string fullName;
        string version;
        [JsonPropertyName("defaultMaxCopies")]
        int defaultMaxCopies;
        [JsonPropertyName("restrictionList")]
        dynamic restrictionList;
        [JsonPropertyName("defaultMaxCopies")]
        dynamic deckSizes;
    }
}
