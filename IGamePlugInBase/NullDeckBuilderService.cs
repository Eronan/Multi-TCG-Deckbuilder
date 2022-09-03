using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGamePlugInBase
{
    /// <summary>
    /// Used as the Default Deck Builder Service when not specfied
    /// </summary>
    internal class NullDeckBuilderService : IDeckBuilderService
    {
        public IEnumerable<SearchField> SearchFields => Array.Empty<SearchField>();

        public IEnumerable<DeckBuilderCardArt> CardList => Array.Empty<DeckBuilderCardArt>();
    }
}
