﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IGamePlugInBase;

namespace FECipher
{
    public class FECard : Card
    {
        [JsonPropertyName("Character")]
        [JsonPropertyOrder(0)]
        public string characterName { get; set; }
        [JsonPropertyName("Title")]
        [JsonPropertyOrder(1)]
        public string characterTitle { get; set; }
        [JsonPropertyName("Color")]
        [JsonPropertyOrder(2)]
        public string[] colors { get; set; }
        [JsonPropertyName("Cost")]
        [JsonPropertyOrder(3)]
        public string cost { get; set; }
        [JsonPropertyName("ClassChangeCost")]
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? classChangeCost { get; set; }
        [JsonPropertyName("Class")]
        [JsonPropertyOrder(5)]
        public string cardClass { get; set; }
        [JsonPropertyName("Type")]
        [JsonPropertyOrder(6)]
        public string[] types { get; set; }
        [JsonPropertyName("MinRange")]
        [JsonPropertyOrder(7)]
        public int minRange { get; set; }
        [JsonPropertyName("MaxRange")]
        [JsonPropertyOrder(8)]
        public int maxRange { get; set; }
        [JsonPropertyName("Attack")]
        [JsonPropertyOrder(9)]
        public string attack { get; set; }
        [JsonPropertyName("Support")]
        [JsonPropertyOrder(10)]
        public string support { get; set; }
        [JsonPropertyName("Skill")]
        [JsonPropertyOrder(11)]
        public string skill { get; set; }
        [JsonPropertyName("SupportSkill")]
        [JsonPropertyOrder(12)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? supportSkill { get; set; }
        [JsonPropertyName("Rarity")]
        [JsonPropertyOrder(13)]
        public string rarity { get; set; }
        [JsonPropertyName("SeriesNumber")]
        [JsonPropertyOrder(14)]
        public int seriesNo { get; set; }
        [JsonPropertyName("AlternateArts")]
        [JsonPropertyOrder(15)]
        public List<FEAlternateArts> altArts { get; set; }

        public FECard(string character, string title, string[] colors, string rarity, string cost, string? cccost, string feclass,
            string[] types, int minRange, int maxRange, string attack, string support, string skill, string? supportSkill, int seriesNo)
        {
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
        public FECard(string Character, string Title, string[] Color, string Cost, string? ClassChangeCost, string Class,
            string[] Type, int MinRange, int MaxRange, string Attack, string Support, string Skill, string? SupportSkill,
            string Rarity, int SeriesNumber, List<FEAlternateArts> AlternateArts)
        {
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
        public string ID
        {
            get { return this.Name; }
        }

        [JsonIgnore]
        public string Name
        {
            get { return this.characterName + ": " + this.characterTitle; }
        }

        [JsonIgnore]
        public AlternateArt[] AltArts
        {
            get { return this.altArts.ToArray(); }
        }
    }
}