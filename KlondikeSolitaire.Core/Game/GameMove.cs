namespace KlondikeSolitaire.Core.Models
{
    public enum MoveType
    {
        DrawStock,
        WasteToTableau,
        WasteToFoundation,
        TableauToFoundation,
        TableauToTableau,
        FoundationToTableau,
        RecycleWaste,
        FlipTableauCard
    }

    public class GameMove
    {
        public MoveType Type { get; set; }
        public List<Card> MovedCards { get; set; } = new();
        public int SourceIndex { get; set; }
        public int DestinationIndex { get; set; }
        public bool WasCardFlipped { get; set; }
        public int WasteIndexBefore { get; set; }
        public int ScoreChange { get; set; }
        public List<Card>? RecycledCards { get; set; }
    }
}