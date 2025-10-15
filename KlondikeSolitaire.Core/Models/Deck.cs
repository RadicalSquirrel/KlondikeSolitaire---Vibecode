namespace KlondikeSolitaire.Core.Game
{
    using KlondikeSolitaire.Core.Models;

    public class Deck
    {
        private List<Card> cards = new();

        public Deck()
        {
            foreach (Suit suit in Enum.GetValues<Suit>())
            {
                foreach (Rank rank in Enum.GetValues<Rank>())
                {
                    cards.Add(new Card(suit, rank));
                }
            }
        }

        public void Shuffle()
        {
            var rng = new Random();
            cards = cards.OrderBy(x => rng.Next()).ToList();
        }

        public Card? Draw()
        {
            if (cards.Count == 0) return null;
            var card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        public int Count => cards.Count;
    }
}