using System.Text.Json.Serialization;
using IGamePlugInBase;

namespace FECipher
{
    public class FEAlternateArts : AlternateArt
    {
        [JsonPropertyName("CardCode")]
        [JsonPropertyOrder(0)]
        public string Id { get; set; }
        [JsonPropertyName("SetCode")]
        [JsonPropertyOrder(1)]
        public string SetCode { get; set; }
        [JsonPropertyName("ImageFile")]
        [JsonPropertyOrder(2)]
        public string ImageLocation { get; set; }
        [JsonPropertyName("LackeyCCGID")]
        [JsonPropertyOrder(3)]
        public string LackeyCCGId { get; set; }
        [JsonPropertyName("LackeyCCGName")]
        [JsonPropertyOrder(4)]
        public string LackeyCCGName { get; set; }
        [JsonIgnore]
        public CardArtOrientation ArtOrientation { get => CardArtOrientation.Portrait; }

        [JsonConstructor]
        public FEAlternateArts(string CardCode, string SetCode, string ImageFile, string LackeyCCGId, string LackeyCCGName)
        {
            this.Id = CardCode;
            this.SetCode = SetCode;
            this.ImageLocation = ImageFile;
            this.LackeyCCGId = LackeyCCGId;
            this.LackeyCCGName = LackeyCCGName;
        }
    }
}
