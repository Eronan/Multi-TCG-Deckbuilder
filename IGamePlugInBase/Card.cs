using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    public interface Card
    {
        /// <summary>
        /// ID of the Card to be saved in the Deck File.
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// Name of the Card to be shown on the Deck Builder.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A set of Alternate Arts the card has. All cards must have at least 1 art.
        /// </summary>
        public AlternateArt[] AltArts { get; }
    }
}
