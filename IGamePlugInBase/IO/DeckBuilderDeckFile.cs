namespace IGamePlugInBase.IO
{
    public class DeckBuilderDeckFile
    {
        public string Game { get; set; }
        public string Format { get; set; }
        public DeckBuilderDeck[] Decks { get; set; }

        public DeckBuilderDeckFile(string game, string format, DeckBuilderDeck[] decks)
        {
            Game = game;
            Format = format;
            Decks = decks;
        }
    }
}
