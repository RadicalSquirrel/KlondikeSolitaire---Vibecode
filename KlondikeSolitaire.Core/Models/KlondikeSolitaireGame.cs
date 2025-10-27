namespace KlondikeSolitaire.Core.Game
{
    using KlondikeSolitaire.Core.Models;

    public class KlondikeSolitaireGame
    {
        private readonly GameOptions options;
        private List<Card>[] tableau = new List<Card>[7];
        private List<Card>[] foundations = new List<Card>[4];
        private List<Card> stock = new();
        private List<Card> waste = new();
        private int wasteIndex = -1;
        private int score = 0;
        private int stockRedeals = 0;
        private Stack<GameMove> moveHistory = new();
        private DateTime gameStartTime;

        public GameOptions Options => options;
        public int Score => score;
        public int StockRedeals => stockRedeals;
        public int StockCount => stock.Count;
        public int WasteCount => waste.Count;
        public int WasteIndex => wasteIndex;
        public TimeSpan ElapsedTime => DateTime.Now - gameStartTime;
        public IReadOnlyList<Card>[] Tableau => tableau.Select(t => t.AsReadOnly()).ToArray();
        public IReadOnlyList<Card>[] Foundations => foundations.Select(f => f.AsReadOnly()).ToArray();
        public IReadOnlyList<Card> Stock => stock.AsReadOnly();
        public IReadOnlyList<Card> Waste => waste.AsReadOnly();

        public KlondikeSolitaireGame(GameOptions options)
        {
            this.options = options;

            for (int i = 0; i < 7; i++)
                tableau[i] = new List<Card>();

            for (int i = 0; i < 4; i++)
                foundations[i] = new List<Card>();

            gameStartTime = DateTime.Now;
            InitializeGame();
        }

        private void InitializeGame()
        {
            var deck = new Deck();
            deck.Shuffle();

            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row <= col; row++)
                {
                    var card = deck.Draw()!;
                    if (row == col)
                        card.IsFaceUp = true;
                    tableau[col].Add(card);
                }
            }

            while (deck.Count > 0)
            {
                stock.Add(deck.Draw()!);
            }
        }

        private void AddScore(int points)
        {
            if (options.EnableScoring)
            {
                // Apply difficulty multiplier to all score changes
                double multiplier = options.GetScoreMultiplier();
                int adjustedPoints = (int)Math.Round(points * multiplier);
                score += adjustedPoints;
            }
        }

        public (bool Success, string? Message) DrawFromStock()
        {
            if (stock.Count == 0)
            {
                if (options.MaxStockRedeals >= 0 && stockRedeals >= options.MaxStockRedeals)
                {
                    return (false, $"Maximum redeals ({options.MaxStockRedeals}) reached!");
                }

                var move = new GameMove
                {
                    Type = MoveType.RecycleWaste,
                    RecycledCards = new List<Card>(waste),
                    WasteIndexBefore = wasteIndex,
                    ScoreChange = -100
                };

                // Add cards to stock - reverse if option is enabled
                if (options.ReverseWasteOnRecycle)
                {
                    // Reverse the order when moving back to stock
                    var reversedWaste = new List<Card>(waste);
                    reversedWaste.Reverse();
                    stock.AddRange(reversedWaste);
                }
                else
                {
                    // Keep the same order (current behavior)
                    stock.AddRange(waste);
                }

                waste.Clear();
                wasteIndex = -1;
                stockRedeals++;
                AddScore(-100);

                moveHistory.Push(move);
                return (true, $"Stock recycled. Redeals: {stockRedeals}" +
                    (options.MaxStockRedeals >= 0 ? $"/{options.MaxStockRedeals}" : ""));
            }

            var move2 = new GameMove
            {
                Type = MoveType.DrawStock,
                MovedCards = new List<Card>(),
                WasteIndexBefore = wasteIndex,
                ScoreChange = 0
            };

            for (int i = 0; i < options.DrawCount && stock.Count > 0; i++)
            {
                var card = stock[^1];
                stock.RemoveAt(stock.Count - 1);
                card.IsFaceUp = true;
                waste.Add(card);
                move2.MovedCards.Add(card);
            }

            wasteIndex = waste.Count - 1;
            moveHistory.Push(move2);
            return (true, null);
        }

        public bool MoveWasteToTableau(int tableauCol)
        {
            if (wasteIndex < 0 || wasteIndex >= waste.Count) return false;

            var card = waste[wasteIndex];

            if (CanPlaceOnTableau(card, tableauCol))
            {
                var move = new GameMove
                {
                    Type = MoveType.WasteToTableau,
                    MovedCards = new List<Card> { card },
                    SourceIndex = wasteIndex,
                    DestinationIndex = tableauCol,
                    WasteIndexBefore = wasteIndex,
                    ScoreChange = 5
                };

                tableau[tableauCol].Add(card);
                waste.RemoveAt(wasteIndex);
                wasteIndex = waste.Count - 1;
                AddScore(5);
                moveHistory.Push(move);
                return true;
            }

            return false;
        }

        public bool MoveWasteToFoundation(int foundationIndex)
        {
            if (wasteIndex < 0 || wasteIndex >= waste.Count) return false;

            var card = waste[wasteIndex];

            if (CanPlaceOnFoundation(card, foundationIndex))
            {
                var move = new GameMove
                {
                    Type = MoveType.WasteToFoundation,
                    MovedCards = new List<Card> { card },
                    SourceIndex = wasteIndex,
                    DestinationIndex = foundationIndex,
                    WasteIndexBefore = wasteIndex,
                    ScoreChange = 10
                };

                foundations[foundationIndex].Add(card);
                waste.RemoveAt(wasteIndex);
                wasteIndex = waste.Count - 1;
                AddScore(10);
                moveHistory.Push(move);
                return true;
            }

            return false;
        }

        public bool MoveTableauToFoundation(int fromCol, int foundationIndex)
        {
            if (tableau[fromCol].Count == 0) return false;

            var card = tableau[fromCol][^1];

            if (CanPlaceOnFoundation(card, foundationIndex))
            {
                bool willFlipCard = tableau[fromCol].Count > 1 && !tableau[fromCol][^2].IsFaceUp;

                var move = new GameMove
                {
                    Type = MoveType.TableauToFoundation,
                    MovedCards = new List<Card> { card },
                    SourceIndex = fromCol,
                    DestinationIndex = foundationIndex,
                    WasCardFlipped = willFlipCard,
                    ScoreChange = willFlipCard ? 15 : 10
                };

                foundations[foundationIndex].Add(card);
                tableau[fromCol].RemoveAt(tableau[fromCol].Count - 1);

                if (tableau[fromCol].Count > 0 && !tableau[fromCol][^1].IsFaceUp)
                {
                    tableau[fromCol][^1].IsFaceUp = true;
                    AddScore(5);
                }

                AddScore(10);
                moveHistory.Push(move);
                return true;
            }

            return false;
        }

        public bool MoveTableauToTableau(int fromCol, int toCol, int cardIndex)
        {
            if (cardIndex >= tableau[fromCol].Count) return false;

            var cardsToMove = tableau[fromCol].Skip(cardIndex).ToList();
            if (cardsToMove.Count == 0 || !cardsToMove[0].IsFaceUp) return false;

            if (CanPlaceOnTableau(cardsToMove[0], toCol))
            {
                bool willFlipCard = cardIndex > 0 && !tableau[fromCol][cardIndex - 1].IsFaceUp;

                var move = new GameMove
                {
                    Type = MoveType.TableauToTableau,
                    MovedCards = new List<Card>(cardsToMove),
                    SourceIndex = fromCol,
                    DestinationIndex = toCol,
                    WasCardFlipped = willFlipCard,
                    ScoreChange = willFlipCard ? 5 : 0
                };

                tableau[toCol].AddRange(cardsToMove);
                tableau[fromCol].RemoveRange(cardIndex, cardsToMove.Count);

                if (tableau[fromCol].Count > 0 && !tableau[fromCol][^1].IsFaceUp)
                {
                    tableau[fromCol][^1].IsFaceUp = true;
                    AddScore(5);
                }

                moveHistory.Push(move);
                return true;
            }

            return false;
        }

        public (bool Success, string? Message) MoveFoundationToTableau(int foundationIndex, int tableauCol)
        {
            if (!options.AllowFoundationToTableau)
            {
                return (false, "Foundation to tableau moves are disabled!");
            }

            if (foundations[foundationIndex].Count == 0) return (false, null);

            var card = foundations[foundationIndex][^1];

            if (CanPlaceOnTableau(card, tableauCol))
            {
                var move = new GameMove
                {
                    Type = MoveType.FoundationToTableau,
                    MovedCards = new List<Card> { card },
                    SourceIndex = foundationIndex,
                    DestinationIndex = tableauCol,
                    ScoreChange = -15
                };

                tableau[tableauCol].Add(card);
                foundations[foundationIndex].RemoveAt(foundations[foundationIndex].Count - 1);
                AddScore(-15);
                moveHistory.Push(move);
                return (true, null);
            }

            return (false, null);
        }

        public (bool Success, string? Message) UndoLastMove()
        {
            if (moveHistory.Count == 0)
            {
                return (false, "No moves to undo!");
            }

            var move = moveHistory.Pop();
            score -= move.ScoreChange;

            switch (move.Type)
            {
                case MoveType.DrawStock:
                    for (int i = move.MovedCards.Count - 1; i >= 0; i--)
                    {
                        var card = move.MovedCards[i];
                        waste.Remove(card);
                        stock.Add(card);
                    }
                    wasteIndex = move.WasteIndexBefore;
                    break;

                case MoveType.RecycleWaste:
                    if (move.RecycledCards != null)
                    {
                        foreach (var card in move.RecycledCards)
                        {
                            stock.Remove(card);
                        }
                        waste.AddRange(move.RecycledCards);
                        wasteIndex = move.WasteIndexBefore;
                        stockRedeals--;
                    }
                    break;

                case MoveType.WasteToTableau:
                    var card1 = tableau[move.DestinationIndex][^1];
                    tableau[move.DestinationIndex].RemoveAt(tableau[move.DestinationIndex].Count - 1);
                    waste.Insert(move.SourceIndex, card1);
                    wasteIndex = move.WasteIndexBefore;
                    break;

                case MoveType.WasteToFoundation:
                    var card2 = foundations[move.DestinationIndex][^1];
                    foundations[move.DestinationIndex].RemoveAt(foundations[move.DestinationIndex].Count - 1);
                    waste.Insert(move.SourceIndex, card2);
                    wasteIndex = move.WasteIndexBefore;
                    break;

                case MoveType.TableauToFoundation:
                    var card3 = foundations[move.DestinationIndex][^1];
                    foundations[move.DestinationIndex].RemoveAt(foundations[move.DestinationIndex].Count - 1);
                    tableau[move.SourceIndex].Add(card3);

                    if (move.WasCardFlipped && tableau[move.SourceIndex].Count > 1)
                    {
                        tableau[move.SourceIndex][^2].IsFaceUp = false;
                    }
                    break;

                case MoveType.TableauToTableau:
                    var cardsToMoveBack = tableau[move.DestinationIndex]
                        .Skip(tableau[move.DestinationIndex].Count - move.MovedCards.Count)
                        .ToList();
                    tableau[move.DestinationIndex].RemoveRange(
                        tableau[move.DestinationIndex].Count - move.MovedCards.Count,
                        move.MovedCards.Count);
                    tableau[move.SourceIndex].AddRange(cardsToMoveBack);

                    if (move.WasCardFlipped && tableau[move.SourceIndex].Count > move.MovedCards.Count)
                    {
                        tableau[move.SourceIndex][^(move.MovedCards.Count + 1)].IsFaceUp = false;
                    }
                    break;

                case MoveType.FoundationToTableau:
                    var card4 = tableau[move.DestinationIndex][^1];
                    tableau[move.DestinationIndex].RemoveAt(tableau[move.DestinationIndex].Count - 1);
                    foundations[move.SourceIndex].Add(card4);
                    break;
            }

            return (true, "Move undone!");
        }

        private bool CanPlaceOnTableau(Card card, int col)
        {
            if (tableau[col].Count == 0)
            {
                return options.AllowAnyCardInEmptyTableau || card.Rank == Rank.King;
            }

            var topCard = tableau[col][^1];
            return card.Color != topCard.Color && (int)card.Rank == (int)topCard.Rank - 1;
        }

        private bool CanPlaceOnFoundation(Card card, int foundationIndex)
        {
            var foundation = foundations[foundationIndex];

            if (foundation.Count == 0)
                return card.Rank == Rank.Ace;

            var topCard = foundation[^1];
            return card.Suit == topCard.Suit && (int)card.Rank == (int)topCard.Rank + 1;
        }

        public bool IsGameWon()
        {
            return foundations.All(f => f.Count == 13);
        }

        public bool HasAvailableMoves()
        {
            // Check if we can draw from stock or recycle waste
            if (stock.Count > 0)
                return true;

            if (stock.Count == 0 && waste.Count > 0)
            {
                // Can we recycle?
                if (options.MaxStockRedeals < 0 || stockRedeals < options.MaxStockRedeals)
                    return true;
            }

            // Check if the current waste card can move anywhere
            if (wasteIndex >= 0 && wasteIndex < waste.Count)
            {
                var wasteCard = waste[wasteIndex];

                // Can waste card go to any tableau?
                for (int col = 0; col < 7; col++)
                {
                    if (CanPlaceOnTableau(wasteCard, col))
                        return true;
                }

                // Can waste card go to any foundation?
                for (int f = 0; f < 4; f++)
                {
                    if (CanPlaceOnFoundation(wasteCard, f))
                        return true;
                }
            }

            // Check all tableau cards
            for (int fromCol = 0; fromCol < 7; fromCol++)
            {
                if (tableau[fromCol].Count == 0)
                    continue;

                // Can the top card go to a foundation?
                var topCard = tableau[fromCol][^1];
                for (int f = 0; f < 4; f++)
                {
                    if (CanPlaceOnFoundation(topCard, f))
                        return true;
                }

                // Can any face-up cards in this column move to another tableau column?
                for (int cardIdx = 0; cardIdx < tableau[fromCol].Count; cardIdx++)
                {
                    if (!tableau[fromCol][cardIdx].IsFaceUp)
                        continue;

                    var card = tableau[fromCol][cardIdx];
                    for (int toCol = 0; toCol < 7; toCol++)
                    {
                        if (toCol == fromCol)
                            continue;

                        if (CanPlaceOnTableau(card, toCol))
                            return true;
                    }
                }
            }

            // Check if any foundation cards can move to tableau (if allowed)
            if (options.AllowFoundationToTableau)
            {
                for (int f = 0; f < 4; f++)
                {
                    if (foundations[f].Count == 0)
                        continue;

                    var foundationCard = foundations[f][^1];
                    for (int col = 0; col < 7; col++)
                    {
                        if (CanPlaceOnTableau(foundationCard, col))
                            return true;
                    }
                }
            }

            // No moves available
            return false;
        }

        public bool CanAutoComplete()
        {
            // Auto-complete is only possible when:
            // 1. Stock is empty
            // 2. Waste is empty
            // 3. All cards in tableau are face-up

            if (stock.Count > 0 || waste.Count > 0)
                return false;

            // Check if all tableau cards are face-up
            for (int col = 0; col < 7; col++)
            {
                foreach (var card in tableau[col])
                {
                    if (!card.IsFaceUp)
                        return false;
                }
            }

            return true;
        }

        public bool AutoComplete()
        {
            if (!CanAutoComplete())
                return false;

            bool madeMoves = true;
            int maxIterations = 1000; // Safety limit to prevent infinite loops
            int iterations = 0;

            // Keep moving cards to foundations until no more moves possible
            while (madeMoves && iterations < maxIterations)
            {
                madeMoves = false;
                iterations++;

                // Try to move each tableau column's top card to a foundation
                for (int col = 0; col < 7; col++)
                {
                    if (tableau[col].Count == 0)
                        continue;

                    var topCard = tableau[col][^1];

                    // Find the appropriate foundation for this card
                    for (int f = 0; f < 4; f++)
                    {
                        if (CanPlaceOnFoundation(topCard, f))
                        {
                            MoveTableauToFoundation(col, f);
                            madeMoves = true;
                            break; // Move to next column
                        }
                    }

                    if (madeMoves)
                        break; // Start over from first column
                }
            }

            return IsGameWon();
        }
    }
}