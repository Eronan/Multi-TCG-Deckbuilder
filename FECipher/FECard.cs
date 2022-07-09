using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IGamePlugInBase;

namespace FECipher
{
    public class FECard : ICard
    {
        [JsonPropertyName("CardID")]
        [JsonPropertyOrder(0)]
        public string ID { get; set; }
        [JsonPropertyName("Character")]
        [JsonPropertyOrder(1)]
        public string characterName { get; set; }
        [JsonPropertyName("Title")]
        [JsonPropertyOrder(2)]
        public string characterTitle { get; set; }
        [JsonPropertyName("Color")]
        [JsonPropertyOrder(3)]
        public string[] colors { get; set; }
        [JsonPropertyName("Cost")]
        [JsonPropertyOrder(4)]
        public string cost { get; set; }
        [JsonPropertyName("ClassChangeCost")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? classChangeCost { get; set; }
        [JsonPropertyName("Class")]
        [JsonPropertyOrder(6)]
        public string cardClass { get; set; }
        [JsonPropertyName("Type")]
        [JsonPropertyOrder(7)]
        public string[] types { get; set; }
        [JsonPropertyName("MinRange")]
        [JsonPropertyOrder(8)]
        public int minRange { get; set; }
        [JsonPropertyName("MaxRange")]
        [JsonPropertyOrder(9)]
        public int maxRange { get; set; }
        [JsonPropertyName("Attack")]
        [JsonPropertyOrder(10)]
        public string attack { get; set; }
        [JsonPropertyName("Support")]
        [JsonPropertyOrder(11)]
        public string support { get; set; }
        [JsonPropertyName("Skill")]
        [JsonPropertyOrder(12)]
        public string skill { get; set; }
        [JsonPropertyName("SupportSkill")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? supportSkill { get; set; }
        [JsonPropertyName("Rarity")]
        [JsonPropertyOrder(14)]
        public string rarity { get; set; }
        [JsonPropertyName("SeriesNumber")]
        [JsonPropertyOrder(15)]
        public int seriesNo { get; set; }
        [JsonPropertyName("AlternateArts")]
        [JsonPropertyOrder(16)]
        public List<FEAlternateArts> altArts { get; set; }

        public FECard(string id, string character, string title, string[] colors, string rarity, string cost, string? cccost, string feclass,
            string[] types, int minRange, int maxRange, string attack, string support, string skill, string? supportSkill, int seriesNo)
        {
            this.ID = id;
            this.characterName = character;
            this.characterTitle = title;
            this.colors = colors;
            this.rarity = rarity;
            this.cost = cost;
            this.classChangeCost = cccost;
            this.cardClass = feclass;
            this.types = types;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.attack = attack;
            this.support = support;
            this.skill = skill;
            this.supportSkill = supportSkill;
            this.seriesNo = seriesNo;
            this.altArts = new List<FEAlternateArts>();
        }

        [JsonConstructor]
        public FECard(string ID, string Character, string Title, string[] Color, string Cost, string? ClassChangeCost, string Class,
            string[] Type, int MinRange, int MaxRange, string Attack, string Support, string Skill, string? SupportSkill,
            string Rarity, int SeriesNumber, List<FEAlternateArts> AlternateArts)
        {
            this.ID = ID;
            this.characterName = Character;
            this.characterTitle = Title;
            this.colors = Color;
            this.rarity = Rarity;
            this.cost = Cost;
            this.classChangeCost = ClassChangeCost;
            this.cardClass = Class;
            this.types = Type;
            this.minRange = MinRange;
            this.maxRange = MaxRange;
            this.attack = Attack;
            this.support = Support;
            this.skill = Skill;
            this.supportSkill = SupportSkill;
            this.seriesNo = SeriesNumber;
            this.altArts = AlternateArts;
        }

        [JsonIgnore]
        public string Name
        {
            get { return this.characterName + ": " + this.characterTitle; }
        }

        [JsonIgnore]
        public Dictionary<string, AlternateArt> AltArts
        {
            get { return this.altArts.ToDictionary(keySelector: m => m.Id, elementSelector: m => m as AlternateArt); }
        }

        [JsonIgnore]
        public string ViewDetails
        {
            get
            {
                string fullDetails = this.Name;
                fullDetails += string.Format("\nClass: {0}/Cost: {1}", this.cardClass, this.cost);
                if (classChangeCost != null)
                {
                    fullDetails += "(" + this.classChangeCost + ")";
                }

                fullDetails += string.Format("\nColors: {0}\nTypes: {1}\nAttack: {2}/Support: {3}/Range: {4}-{5}", string.Join('/', this.colors), string.Join('/', this.types), this.attack, this.support, this.minRange, this.maxRange);
                fullDetails += "\n---\nSkills:\n" + skill;

                if (supportSkill != null)
                {
                    fullDetails += "\n---\nSupport:\n" + supportSkill;
                }
                return fullDetails;
            }
        }
    }
}
