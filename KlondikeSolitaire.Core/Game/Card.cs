namespace KlondikeSolitaire.Core.Models
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }
    public enum CardColor { Red, Black }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }
        public bool IsFaceUp { get; set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            IsFaceUp = false;
        }

        public CardColor Color => Suit == Suit.Hearts || Suit == Suit.Diamonds
            ? CardColor.Red
            : CardColor.Black;

        public override string ToString()
        {
            if (!IsFaceUp) return "[###]";

            var rankStr = Rank switch
            {
                Rank.Ace => "A",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                Rank.Ten => "10",
                _ => ((int)Rank).ToString()
            };

            var suitStr = Suit switch
            {
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                Suit.Spades => "♠",
                _ => ""
            };

            rankStr = rankStr.PadLeft(2);
            return $"[{rankStr}{suitStr}]";
        }
    }
}