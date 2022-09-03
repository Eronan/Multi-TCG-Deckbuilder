using System.Text.Json.Serialization;

namespace IGamePlugInBase
{
    /// <summary>
    /// Cards within a Deck. Information Necessary to save the File.
    /// </summary>
    public class DeckBuilderCard
    {
        /// <summary>
        /// Card's Identifer, used to determine which card it belongs to.
        /// </summary>
        [JsonPropertyName("CardID")]
        public string CardID { get; }
        /// <summary>
        /// Art Identifier, used to determine which Art is being used from a Card.
        /// </summary>
        [JsonPropertyName("ArtID")]
        public string ArtID { get; }

        /// <summary>
        /// Initializes DeckBuilderCard
        /// </summary>
        /// <param name="cardID">Card's Identifier</param>
        /// <param name="artID">Art Identifier</param>
        [JsonConstructor]
        public DeckBuilderCard(string cardID, string artID)
        {
            this.CardID = cardID;
            this.ArtID = artID;
        }
    }
}
