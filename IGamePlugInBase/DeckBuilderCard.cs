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

        /// <summary>
        /// Constructor for the Card that is used by the Deck Builder
        /// </summary>
        /// <param name="cardID">Card ID</param>
        /// <param name="artID">Art ID</param>
        /// <param name="markSpecial">Is the Special Card in the Deck</param>
        [JsonConstructor]
        public DeckBuilderCard(string cardID, string artID)
        {
            this.CardID = cardID;
            this.ArtID = artID;
        }
    }
}
