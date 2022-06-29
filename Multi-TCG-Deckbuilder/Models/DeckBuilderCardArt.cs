using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Models
{
    internal class DeckBuilderCardArt : DeckBuilderCard
    {
        string name = "";
        string fileLocation = "";
        BitmapImage imageFile;
        string viewDetails;

        public DeckBuilderCardArt(string id, string name, string viewDetails, string artID, string fileLocation, string applicationPath) : base(id, artID)
        {
            this.name = name;
            this.viewDetails = viewDetails;
            this.fileLocation = applicationPath + fileLocation;
            this.imageFile = new BitmapImage(new Uri(this.fileLocation));
        }

        public DeckBuilderCardArt(Card card, string altArtID, string applicationPath) : base(card.ID, altArtID)
        {
            this.name = card.Name;
            this.viewDetails = card.ViewDetails;
            AlternateArt? altArt;
            if ((altArt = card.AltArts.GetValueOrDefault(altArtID)) != null)
            {
                this.fileLocation = Path.Combine(applicationPath, altArt.ImageLocation);
                this.imageFile = new BitmapImage(new Uri(this.fileLocation));
            }
            else
            {
                throw new NullReferenceException(String.Format("Card {0} has no Art with the ID: {1}", card.ID, altArtID));
            }
        }

        public DeckBuilderCardArt(Card gameCard, DeckBuilderCard dbCard) : this(gameCard, dbCard.CardID, dbCard.ArtID) {}

        public static List<DeckBuilderCardArt> GetFromCards(IEnumerable<Card> cards, string applicationPath)
        {
            List<DeckBuilderCardArt> allArts = new List<DeckBuilderCardArt>();

            foreach (Card card in cards)
            {
                foreach (AlternateArt art in card.AltArts.Values)
                {
                    allArts.Add(new DeckBuilderCardArt(card.ID, card.Name, card.ViewDetails, art.Id, art.ImageLocation, applicationPath));
                }
            }

            return allArts;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string FileLocation
        {
            get { return this.fileLocation; }
        }

        public BitmapImage ImageFile
        {
            get { return this.imageFile; }
        }

        public string ViewDetails
        {
            get { return this.viewDetails; }
        }
    }
}
