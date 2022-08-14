using IGamePlugInBase;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FECipher
{
    internal class LackeyCCGImport : ImportMenuItem
    {
        IEnumerable<FECard> CardList;

        public string Header { get => "LackeyCCG"; }

        public string DefaultExtension { get => ".dek"; }

        public string FileFilter { get => "LackeyCCG Deck File (.dek)|*.dek"; }

        public LackeyCCGImport(IEnumerable<FECard> cardList)
        {
            this.CardList = cardList;
        }

        public DeckBuilderDeckFile Import(string filePath, string currentFormat)
        {
            DeckBuilderDeck[] decks = new DeckBuilderDeck[2];
            decks[0] = new DeckBuilderDeck("maincharacter", new List<DeckBuilderCard>());
            decks[1] = new DeckBuilderDeck("main", new List<DeckBuilderCard>());

            // Load XML File
            XmlDocument lackeyCCGDeck = new XmlDocument();
            lackeyCCGDeck.Load(filePath);

            // Change Format
            string changeFormat = currentFormat;

            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                XmlNodeList? nodeList = lackeyCCGDeck.DocumentElement.SelectNodes("superzone");

                foreach (XmlNode node in nodeList)
                {
                    XmlAttribute? nameAttribute = node.Attributes["name"];
                    string nameText = nameAttribute.InnerText;

                    XmlNodeList? decklist = node.SelectNodes("card");

                    List<DeckBuilderCard> cards = new List<DeckBuilderCard>();

                    foreach (XmlNode card in decklist)
                    {
                        string lackeyID = card.SelectSingleNode("name").Attributes["id"].InnerText;

                        foreach (FECard feCard in this.CardList)
                        {
                            FEAlternateArts? altArt = feCard.altArts.FirstOrDefault(art => art.LackeyCCGId == lackeyID);

                            if (altArt != null)
                            {
                                if (changeFormat == "standard" && feCard.seriesNo < 5)
                                {
                                    changeFormat = "unlimited";
                                }

                                cards.Add(new DeckBuilderCard(feCard.ID, altArt.Id));
                                break;
                            }
                        }
                    }

                    if (nameText == "MC")
                    {
                        decks[0] = new DeckBuilderDeck("maincharacter", cards);
                    }
                    else if (nameText == "Deck")
                    {
                        decks[1] = new DeckBuilderDeck("main", cards);
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            catch (Exception e)
            {
                throw new FileLoadException("File is in the incorrect Format.", filePath, e.InnerException);
            }

            DeckBuilderDeckFile file = new DeckBuilderDeckFile("FECipher", changeFormat, decks);
            return file;
        }
    }

    internal class LackeyCCGExport : ExportMenuItem
    {
        // Private Variables
        IEnumerable<FECard> CardList;

        // Accessors
        public string Header { get => "LackeyCCG"; }

        public string DefaultExtension { get => ".dek"; }

        public string FileFilter { get => "LackeyCCG Deck File (.dek)|*.dek"; }

        // Constructor
        public LackeyCCGExport(IEnumerable<FECard> cardList)
        {
            this.CardList = cardList;
        }

        public void Export(string filePath, DeckBuilderDeckFile decks)
        {
            var xmlWriter = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
            xmlWriter.WriteStartDocument(true);
            xmlWriter.Formatting = Formatting.None;
            xmlWriter.Indentation = 1;

            // Create First Element
            xmlWriter.WriteStartElement("deck");
            xmlWriter.WriteAttributeString("version", "0.8");

            // First Node: Meta
            xmlWriter.WriteStartElement("meta");
            xmlWriter.WriteStartElement("game");
            xmlWriter.WriteString("FECipher0");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            // Write Decks in a Particular Order
            WriteDeck(xmlWriter, decks.Decks.First(deck => deck.DeckName == "main"));
            WriteDeck(xmlWriter, decks.Decks.First(deck => deck.DeckName == "maincharacter"));


            // Close Document
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void WriteDeck(XmlTextWriter xmlWriter, DeckBuilderDeck deck)
        {
            // SuperZone Element
            xmlWriter.WriteStartElement("superzone");
            // SuperZone Deck Attribute
            if (deck.DeckName == "maincharacter")
            {
                xmlWriter.WriteAttributeString("deck", "MC");
            }
            else if (deck.DeckName == "main")
            {
                xmlWriter.WriteAttributeString("deck", "Deck");
            }

            // Write All Cards
            foreach (DeckBuilderCard deckCard in deck.Cards)
            {
                FECard feCard = this.CardList.First(card => card.ID == deckCard.CardID);
                FEAlternateArts altArt = feCard.altArts.First(art => art.Id == deckCard.ArtID);

                // Write Card
                xmlWriter.WriteStartElement("card");
                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteAttributeString("id", altArt.LackeyCCGId);
                xmlWriter.WriteString(altArt.LackeyCCGName);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("set");
                xmlWriter.WriteString(altArt.SetCode);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}
