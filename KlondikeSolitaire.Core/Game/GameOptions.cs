namespace KlondikeSolitaire.Core.Models
{
    public class GameOptions
    {
        public bool AllowAnyCardInEmptyTableau { get; set; } = false;
        public int DrawCount { get; set; } = 1;
        public bool AllowFoundationToTableau { get; set; } = false;
        public int MaxStockRedeals { get; set; } = -1;
        public bool EnableScoring { get; set; } = true;
        public bool NotifyWhenNoMovesAvailable { get; set; } = true;
        public bool AutoCompleteWhenPossible { get; set; } = true;
        public bool ReverseWasteOnRecycle { get; set; } = false;

        /// <summary>
        /// Calculates the score multiplier based on game difficulty options.
        /// Returns 1.0 when scoring is disabled.
        /// </summary>
        public double GetScoreMultiplier()
        {
            if (!EnableScoring)
                return 1.0;

            double multiplier = 1.0;

            // Draw count: 3-card draw is harder, gets 1.5x multiplier
            if (DrawCount == 3)
                multiplier *= 1.5;
            else if (DrawCount == 1)
                multiplier *= 1.0; // Default, no change

            // Any card in empty tableau makes it easier
            if (AllowAnyCardInEmptyTableau)
                multiplier *= 0.9;

            // Foundation to tableau makes it easier
            if (AllowFoundationToTableau)
                multiplier *= 0.9;

            // Limited redeals makes it harder
            if (MaxStockRedeals == 0)
                multiplier *= 1.3;
            else if (MaxStockRedeals == 1)
                multiplier *= 1.2;
            else if (MaxStockRedeals == 2)
                multiplier *= 1.1;
            // Unlimited redeals (< 0) or 3+ get no bonus

            return multiplier;
        }

        /// <summary>
        /// Gets a formatted string representation of the score multiplier for display.
        /// </summary>
        public string GetScoreMultiplierDisplay()
        {
            if (!EnableScoring)
                return "";

            double multiplier = GetScoreMultiplier();
            return $"{multiplier:0.0#}x";
        }

        public string GetConfigKey()
        {
            return $"{(AllowAnyCardInEmptyTableau ? "Any" : "King")}_{DrawCount}Draw_{(AllowFoundationToTableau ? "FT" : "NoFT")}_{MaxStockRedeals}Redeals";
        }
    }
}