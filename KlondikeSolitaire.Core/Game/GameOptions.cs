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

        public string GetConfigKey()
        {
            return $"{(AllowAnyCardInEmptyTableau ? "Any" : "King")}_{DrawCount}Draw_{(AllowFoundationToTableau ? "FT" : "NoFT")}_{MaxStockRedeals}Redeals";
        }
    }
}