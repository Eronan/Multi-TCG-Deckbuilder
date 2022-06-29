using System.Text.Json.Serialization;

namespace IGamePlugInBase
{
    /// <summary>
    /// Used by the Deck Builder to determine what cards exist in what Deck.
    /// </summary>
    public class DeckBuilderCard
    {
        /// <summary>
        /// Card's ID
        /// </summary>
        [JsonPropertyName("CardID")]
        public string CardID { get; }
        /// <summary>
        /// ID of the Art within the Card
        /// </summary>
        [JsonPropertyName("ArtID")]
        public string ArtID { get; }

        [JsonConstructor]
        public DeckBuilderCard(string cardID, string artID)
        {
            this.CardID = cardID;
            this.ArtID = artID;
        }
    }
}
