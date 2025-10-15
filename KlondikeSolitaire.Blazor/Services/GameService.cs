using KlondikeSolitaire.Core.Game;
using KlondikeSolitaire.Core.Models;

namespace KlondikeSolitaire.Blazor.Services
{
    public class GameService
    {
        public KlondikeSolitaireGame? CurrentGame { get; private set; }
        public event Action? OnGameStateChanged;

        public void StartNewGame(GameOptions options)
        {
            CurrentGame = new KlondikeSolitaireGame(options);
            NotifyStateChanged();
        }

        public bool DrawFromStock()
        {
            if (CurrentGame == null) return false;
            var result = CurrentGame.DrawFromStock();
            NotifyStateChanged();
            return result.Success;
        }

        public bool MoveWasteToTableau(int tableauCol)
        {
            if (CurrentGame == null) return false;
            var success = CurrentGame.MoveWasteToTableau(tableauCol);
            NotifyStateChanged();
            return success;
        }

        public bool MoveWasteToFoundation(int foundationIndex)
        {
            if (CurrentGame == null) return false;
            var success = CurrentGame.MoveWasteToFoundation(foundationIndex);
            NotifyStateChanged();
            return success;
        }

        public bool MoveTableauToFoundation(int fromCol, int foundationIndex)
        {
            if (CurrentGame == null) return false;
            var success = CurrentGame.MoveTableauToFoundation(fromCol, foundationIndex);
            NotifyStateChanged();
            return success;
        }

        public bool MoveTableauToTableau(int fromCol, int toCol, int cardIndex)
        {
            if (CurrentGame == null) return false;
            var success = CurrentGame.MoveTableauToTableau(fromCol, toCol, cardIndex);
            NotifyStateChanged();
            return success;
        }

        public bool MoveFoundationToTableau(int foundationIndex, int tableauCol)
        {
            if (CurrentGame == null) return false;
            var result = CurrentGame.MoveFoundationToTableau(foundationIndex, tableauCol);
            NotifyStateChanged();
            return result.Success;
        }

        public bool UndoLastMove()
        {
            if (CurrentGame == null) return false;
            var result = CurrentGame.UndoLastMove();
            NotifyStateChanged();
            return result.Success;
        }

        private void NotifyStateChanged() => OnGameStateChanged?.Invoke();
    }
}
