using IGamePlugInBase;
using System.Text.Json;

namespace FECipher
{
    public class FECipher : IGamePlugIn
    {
        //Variables
        IFormat[] formatList;
        Dictionary<string, FECard> cardList;
        SearchField[] searchFieldList;

        public FECipher()
        {
            string jsonText = File.ReadAllText("./plug-ins/fe-cipher/cardlist_New.json");
            JsonElement jsonDeserialize = JsonSerializer.Deserialize<dynamic>(jsonText);
            var jsonEnumerator = jsonDeserialize.EnumerateArray();

            // Get Card Data
            Dictionary<string, FECard> feCards = new Dictionary<string, FECard>();
            foreach (var jsonCard in jsonEnumerator)
            {
                string? id = jsonCard.GetProperty("CardID").GetString();
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
                string[]? types = jsonCard.GetProperty("Type").Deserialize<string[]>();
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

                if (id == null || character == null || title == null || colors == null || cost == null || cardClass == null || types == null ||
                    attack == null || support == null || skill == null || rarity == null)
                {
                    throw new ArgumentException(String.Format("JSON Object {0}: {1} is missing a Non-Nullabe Property.", character, title));
                }

                FECard card = new FECard(id, character, title, colors, cost, cccost, cardClass, types, minRange,
                    maxRange, attack, support, skill, supportSkill, rarity, seriesNo, altArts);

                feCards.Add(card.ID, card);
            }

            this.cardList = feCards;

            // Create Valid Formats
            formatList = new IFormat[2]
            {
                new FEFormats("unlimited", "Unlimited", Properties.Resources.UnlimitedIcon, "All cards are allowed in this format from Series 1 to Series 22.", this.cardList.Values.ToArray()),
                new FEFormats("standard", "Standard", Properties.Resources.StandardIcon,
                "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format.",
                this.cardList.Values.ToArray().Where(item => item.seriesNo > 4).ToArray()),
            };

            // Create Search Fields
            string[] colours = new string[10] { "", "Red", "Blue", "Yellow", "Purple", "Green", "Black", "White", "Brown", "Colorless" };
            this.searchFieldList = new SearchField[12]
            {
                new SearchField("character", "Character"),
                new SearchField("title", "Title"),
                new SearchField("color1", "Color", colours, ""),
                new SearchField("color2", "Color", colours, ""),
                new SearchField("cost", "Cost", 1),
                new SearchField("cccost", "Class Change Cost", 1),
                new SearchField("class", "Class"),
                new SearchField("type", "Type"),
                new SearchField("range", "Range", 0, 3),
                new SearchField("attack", "Attack", 3),
                new SearchField("support", "Support", 3),
                new SearchField("series", "Series", 0, 12),
            };
        }

        // Functions
        private bool MatchFields(FECard card, SearchField[] searchFields)
        {
            foreach (SearchField field in searchFields)
            {
                if (string.IsNullOrEmpty(field.Value)) continue;

                switch (field.Id)
                {
                    case "character":
                        // Comparison is Equals or Name Contains Value
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.characterName.Contains(field.Value)) { return false; }
                        break;
                    case "title":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.characterTitle.Contains(field.Value)) return false;
                        break;
                    case "color1":
                    case "color2":
                        if (field.Comparison == SearchFieldComparison.Equals ^ Array.IndexOf(card.colors, field.Value) != -1) return false;
                        break;
                    case "cost":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.cost == field.Value) return false;
                        break;
                    case "cccost":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.classChangeCost == field.Value) return false;
                        break;
                    case "class":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.cardClass.Contains(field.Value)) return false;
                        break;
                    case "type":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.types.Any(item => item.Contains(field.Value))) return false;
                        break;
                    case "range":
                        int fieldValue = int.Parse(field.Value);
                        switch (field.Comparison)
                        {
                            case SearchFieldComparison.Equals:
                                if (card.minRange < fieldValue || card.maxRange > fieldValue) return false;
                                break;
                            case SearchFieldComparison.NotEquals:
                                if (card.minRange > fieldValue || card.maxRange < fieldValue) return false;
                                break;
                            case SearchFieldComparison.LessThan:
                                if (card.minRange > fieldValue) return false;
                                break;
                            case SearchFieldComparison.GreaterThan:
                                if (card.maxRange < fieldValue) return false;
                                break;
                        }
                        break;
                    case "attack":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.attack == field.Value) return false;
                        break;
                    case "support":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.support == field.Value) return false;
                        break;
                    case "series":
                        if (field.Comparison == SearchFieldComparison.Equals ^ card.seriesNo == int.Parse(field.Value)) return false;
                        break;
                }
            }
            return true;
        }

        //Public Accessors
        public string Name { get => "FECipher"; }
        public string LongName { get => "Fire Emblem Cipher"; }
        public byte[] IconImage { get => Properties.Resources.Icon; }
        public IFormat[] Formats { get => this.formatList; }
        public ICard[] CardList { get => this.cardList.Values.ToArray(); }
        public SearchField[] SearchFields { get => this.searchFieldList; }

        // Public Functions
        public List<DeckBuilderCard> AdvancedFilterSearchList(IEnumerable<DeckBuilderCard> cards, SearchField[] searchFields)
        {
            List<DeckBuilderCard> returnList = new List<DeckBuilderCard>();
            foreach (DeckBuilderCard card in cards)
            {
                FECard? feCard = this.cardList.GetValueOrDefault(card.CardID);
                if (feCard != null)
                {
                    if (this.MatchFields(feCard, searchFields)) returnList.Add(card);
                }
            }
            return returnList;
        }

        public int CompareCards(DeckBuilderCard x, DeckBuilderCard y)
        {
            if (x.CardID == y.CardID)
            {
                return string.Compare(x.ArtID, y.ArtID);
            }

            // Get FECards
            FECard? feCardX = this.cardList.GetValueOrDefault(x.CardID);
            FECard? feCardY = this.cardList.GetValueOrDefault(y.CardID);

            if (feCardX == null || feCardY == null)
            {
                return 0;
            }

            if (feCardX.characterName != feCardY.characterName)
            {
                // First Comparison: Character Name
                return string.Compare(feCardX.characterName, feCardY.characterName);
            }
            else if (feCardX.cost != feCardY.cost)
            {
                // Third Comparison: Deployment Cost
                if (feCardX.cost == "X") { return 1; }
                if (feCardY.cost == "X") { return -1; }

                return string.Compare(feCardX.cost, feCardY.cost);
            }
            else if (feCardX.classChangeCost != feCardY.classChangeCost)
            {
                // Third Comparison: Class Change Cost
                return string.Compare(feCardX.classChangeCost, feCardY.classChangeCost);
            }
            else if (feCardX.attack != feCardY.attack)
            {
                // Fourth Comparison: Attack
                return string.Compare(feCardX.attack, feCardY.attack);
            }
            else if (feCardX.support != feCardY.support)
            {
                // Fourth Comparison: Attack
                return string.Compare(feCardX.support, feCardY.support);
            }
            else if (feCardX.characterTitle != feCardY.characterTitle)
            {
                // Fifth Comparison: Character Title
                return string.Compare(feCardX.characterTitle, feCardY.characterTitle);
            }
            else
            {
                // Sixth Comparison: Card ID
                return string.Compare(feCardX.ID, feCardY.ID);
            }
        }
    }
}
