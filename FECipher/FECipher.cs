using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGamePlugInBase;
using System.Text.Json;

namespace FECipher
{
    public class FECipher : IGamePlugIn
    {
        //Variables
        Dictionary<string, Format> formatList;
        dynamic[] cardList;
        
        public FECipher()
        {
            formatList = new Dictionary<string, Format>();
            formatList.Add("standard", new Format("standard", "Standard", Properties.Resources.StandardIcon,
                "The last Official Format of Fire Emblem Cipher, cards from Series 1 to Series 4 are not allowed in this format."));
            formatList.Add("unlimited", new Format("unlimited", "Unlimited", Properties.Resources.UnlimitedIcon,
                "All cards are allowed in this format from Series 1 to Series 22."));

            string jsonText = File.ReadAllText("./cardlist.json");
            var jsonArray = JsonSerializer.Deserialize<dynamic[]>(jsonText);
            if (jsonArray != null)
            {
                cardList = jsonArray;
            }
            else
            {
                cardList = new dynamic[0] { };
            }
        }

        //Accessors
        public string Name { get => "FECipher"; }
        public string LongName { get => "Fire Emblem Cipher"; }
        public string IconLocation { get => "./cardback.jpg"; }
        public byte[] IconImage { get => Properties.Resources.Icon; }
        public Dictionary<string, Format> Formats
        {
            get { return this.formatList; }
        }
        public dynamic[] CardList
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
