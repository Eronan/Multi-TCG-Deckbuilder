using IGamePlugInBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Models
{
    public partial class DeckBuilderCardArt : DeckBuilderCard
    {
        string name = "";
        string fileLocation = "";
        BitmapImage imageFile;
        CardArtOrientation orientation;
        string viewDetails;

        public DeckBuilderCardArt(ICard card, string altArtID, string applicationPath) : base(card.ID, altArtID)
        {
            this.name = card.Name;
            this.viewDetails = card.ViewDetails;
            AlternateArt? altArt;
            if ((altArt = card.AltArts.GetValueOrDefault(altArtID)) != null)
            {
                this.fileLocation = applicationPath + altArt.ImageLocation;
                this.orientation = altArt.ArtOrientation;
                this.imageFile = new BitmapImage(new Uri(this.fileLocation));
            }
            else
            {
                throw new NullReferenceException(String.Format("Card {0} has no Art with the ID: {1}", card.ID, altArtID));
            }
        }

        public DeckBuilderCardArt(ICard gameCard, DeckBuilderCard dbCard) : this(gameCard, dbCard.CardID, dbCard.ArtID) {}

        public static List<DeckBuilderCardArt> GetFromCards(IEnumerable<ICard> cards, string applicationPath)
        {
            List<DeckBuilderCardArt> allArts = new List<DeckBuilderCardArt>();

            foreach (ICard card in cards)
            {
                foreach (AlternateArt art in card.AltArts.Values)
                {
                    allArts.Add(new DeckBuilderCardArt(card, art.Id, applicationPath));
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

        public CardArtOrientation Orientation
        {
            get { return this.orientation; }
        }

        public string ViewDetails
        {
            get { return this.viewDetails; }
        }
    }
}
