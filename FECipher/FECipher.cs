using IGamePlugInBase;
using System.Text.Json;

namespace FECipher
{
    public class FECipher : IGamePlugIn
    {
        //Variables
        Dictionary<string, Format> formatList;
        FECard[] cardList;
        
        public FECipher()
        {
            string jsonText = File.ReadAllText("./plug-ins/fe-cipher/cardlist_New.json");
            JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
            var jsonEnumerator = jsonDeserialize.EnumerateArray();

            List<FECard> feCards = new List<FECard>();
            foreach(var jsonCard in jsonEnumerator)
            {
                string? character = jsonCard.GetProperty("Character").GetString();
                string? title = jsonCard.GetProperty("Title").GetString();
                string[]? colors = jsonCard.GetProperty("Color").Deserialize<string[]>();
                string? cost = jsonCard.GetProperty("Cost").GetString();
                JsonElement classChangeProperty;
                string? cccost = null;
                if (jsonCard.TryGetProperty("ClassChangeCost", out classChangeProperty))
                {
                    cccost = classChangeProperty.GetString();
                }
                string? cardClass = jsonCard.GetProperty("Class").GetString();
                string[]? types = jsonCard.GetProperty("Color").Deserialize<string[]>();
                int minRange = jsonCard.GetProperty("MinRange").GetInt32();
                int maxRange = jsonCard.GetProperty("MaxRange").GetInt32();
                string? attack = jsonCard.GetProperty("Attack").GetString();
                string? support = jsonCard.GetProperty("Support").GetString();
                string? skill = jsonCard.GetProperty("Skill").GetString();
                JsonElement supportSkillProperty;
                string? supportSkill = null;
                if (jsonCard.TryGetProperty("SupportSkill", out supportSkillProperty))
                {
                    supportSkill = supportSkillProperty.GetString();
                }
                
                string? rarity = jsonCard.GetProperty("Rarity").GetString();
                int seriesNo = jsonCard.GetProperty("SeriesNumber").GetInt32();
                var altArtEnumerator = jsonCard.GetProperty("AlternateArts").EnumerateArray();
                List<FEAlternateArts> altArts = new List<FEAlternateArts>();

                foreach (var altArt in altArtEnumerator)
                {
                    string? code = altArt.GetProperty("CardCode").GetString();
                    string? setNo = altArt.GetProperty("SetCode").GetString();
                    string? image = altArt.GetProperty("ImageFile").GetString();
                    string? lackeyID = altArt.GetProperty("LackeyCCGID").GetString();
                    string? lackeyName = altArt.GetProperty("LackeyCCGName").GetString();

                    //Cannot be Null
                    if (code == null || setNo == null || image == null || lackeyID == null || lackeyName == null)
                    {
                        throw new ArgumentException("JSON Field AlternateArts is missing a Non-Nullable Property.");
                    }

                    FEAlternateArts alt = new FEAlternateArts(code, setNo, image, lackeyID, lackeyName);
                    altArts.Add(alt);
                }

                if (character == null || title == null || colors == null || cost == null || cardClass == null || types == null ||
                    attack == null || support == null || skill == null || rarity == null)
                {
                    throw new ArgumentException(String.Format("JSON Object {0}: {1} is missing a Non-Nullabe Property.", character, title));
                }

                FECard card = new FECard(character, title, colors, cost, cccost, cardClass, types, minRange,
                    maxRange, attack, support, skill, supportSkill, rarity, seriesNo, altArts);

                feCards.Add(card);
            }

            this.cardList = feCards.ToArray();

            formatList = new Dictionary<string, Format>();
            formatList.Add("standard", new Format("standard", "Standard", Properties.Resources.StandardIcon,
                "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format.",
                this.cardList.Where(item => item.seriesNo > 4).ToArray()));
            formatList.Add("unlimited", new Format("unlimited", "Unlimited", Properties.Resources.UnlimitedIcon,
                "All cards are allowed in this format from Series 1 to Series 22.", this.cardList));
        }

        //Accessors
        public string Name { get => "FECipher"; }
        public string LongName { get => "Fire Emblem Cipher"; }
        public byte[] IconImage { get => Properties.Resources.Icon; }
        public Dictionary<string, Format> Formats
        {
            get { return this.formatList; }
        }
        public Card[] CardList
        {
            get { return this.cardList; }
        }

        //Public Functions
        public bool ValidateAdd(dynamic card, dynamic[] deckList)
        {
            return false;
        }

        public bool ValidateDeck(dynamic[] deckList)
        {
            return false;
        }
    }
}
