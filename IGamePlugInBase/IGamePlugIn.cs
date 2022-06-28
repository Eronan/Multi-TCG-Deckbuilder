using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    public interface IGamePlugIn
    {
        //Variables
        /// <summary>
        /// Short Name of the Game Plug-In to be used recognising the Game from the Deck File.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Long Name of the Game Plug-In to be shown on the Load Plugin Window.
        /// </summary>
        public string LongName { get; }
        /// <summary>
        /// Image shown on the Load Plugin Window.
        /// </summary>
        public byte[] IconImage { get; }
        /// <summary>
        /// A List of Formats the Game Plug-In Supports
        /// </summary>
        public Dictionary<string, Format> Formats { get; }
        /// <summary>
        /// The Card List for the Plug-In, can be loaded from an external file or internally created by the plug-in.
        /// </summary>
        Card[] CardList { get; }

        //Methods
        /// <summary>
        /// Determines whether a card is allowed to be added to the Deck.
        /// </summary>
        /// <param name="card">Card to be Added</param>
        /// <param name="deckList">List of cards in the current Deck.</param>
        /// <returns></returns>
        bool ValidateAdd(dynamic card, dynamic[] deckList);

        /// <summary>
        /// Determines whether a Deck is allowed in a specified Format.
        /// </summary>
        /// <param name="deckList">List of cards in the Deck.</param>
        /// <returns></returns>
        bool ValidateDeck(dynamic[] deckList);
    }
}
