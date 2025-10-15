namespace KlondikeSolitaire.UI.Controllers
{
    using KlondikeSolitaire.Core.Game;
    using KlondikeSolitaire.Core.Models;
    using KlondikeSolitaire.Core.Services;
    using KlondikeSolitaire.UI.Renderers;
    using System.Xml.Linq;

    public class GameController
    {
        public void Play(KlondikeSolitaireGame game)
        {
            while (!game.IsGameWon())
            {
                GameRenderer.DisplayGame(game);
                Console.Write("\nEnter command: ");
                var input = Console.ReadLine()?.ToUpper().Trim();

                if (input == "Q") break;

                if (input == "D")
                {
                    var result = game.DrawFromStock();
                    if (!result.Success && result.Message != null)
                        Console.WriteLine(result.Message);
                    else if (result.Message != null)
                        Console.WriteLine(result.Message);
                    continue;
                }

                if (input == "U")
                {
                    var result = game.UndoLastMove();
                    if (result.Message != null)
                        Console.WriteLine(result.Message);
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                if (input == "S")
                {
                    var statsCur = StatisticsManager.LoadStatistics();
                    GameRenderer.DisplayStatistics(statsCur);
                    continue;
                }

                var parts = input?.Split(',');
                if (parts == null || parts.Length == 0) continue;

                try
                {
                    if (parts[0].StartsWith("WT") && parts.Length == 1)
                    {
                        int col = int.Parse(parts[0][2..]) - 1;
                        if (!game.MoveWasteToTableau(col))
                            Console.WriteLine("Invalid move!");
                    }
                    else if (parts[0].StartsWith("WF") && parts.Length == 1)
                    {
                        int found = int.Parse(parts[0][2..]) - 1;
                        if (!game.MoveWasteToFoundation(found))
                            Console.WriteLine("Invalid move!");
                    }
                    else if (parts[0].StartsWith("TF") && parts.Length == 2)
                    {
                        int fromCol = int.Parse(parts[0][2..]) - 1;
                        int found = int.Parse(parts[1]) - 1;
                        if (!game.MoveTableauToFoundation(fromCol, found))
                            Console.WriteLine("Invalid move!");
                    }
                    else if (parts[0].StartsWith("TT") && parts.Length == 3)
                    {
                        int fromCol = int.Parse(parts[0][2..]) - 1;
                        int toCol = int.Parse(parts[1]) - 1;
                        int idx = int.Parse(parts[2]);
                        if (!game.MoveTableauToTableau(fromCol, toCol, idx))
                            Console.WriteLine("Invalid move!");
                    }
                    else if (parts[0].StartsWith("FT") && parts.Length == 2)
                    {
                        int found = int.Parse(parts[0][2..]) - 1;
                        int toCol = int.Parse(parts[1]) - 1;
                        var result = game.MoveFoundationToTableau(found, toCol);
                        if (!result.Success && result.Message != null)
                            Console.WriteLine(result.Message);
                    }

                    System.Threading.Thread.Sleep(500);
                }
                catch
                {
                    Console.WriteLine("Invalid command format!");
                    System.Threading.Thread.Sleep(1000);
                }
            }

            var gameDuration = game.ElapsedTime;
            bool won = game.IsGameWon();

            var stats = StatisticsManager.LoadStatistics();
            stats.RecordGame(game.Options, won, game.Score, gameDuration);
            StatisticsManager.SaveStatistics(stats);

            if (won)
            {
                GameRenderer.DisplayGame(game);
                Console.WriteLine("\n🎉 CONGRATULATIONS! You won! 🎉");
                if (game.Options.EnableScoring)
                {
                    Console.WriteLine($"Final Score: {game.Score}");
                    Console.WriteLine($"Time: {gameDuration:mm\\:ss}");

                    if (game.Score == stats.HighestScore)
                        Console.WriteLine("🏆 NEW HIGH SCORE!");
                    if (gameDuration == stats.FastestWin)
                        Console.WriteLine("⚡ NEW FASTEST WIN!");
                }
            }

            Console.WriteLine($"\nTotal Games: {stats.TotalGamesPlayed} | Wins: {stats.TotalGamesWon} | Win Rate: {stats.WinRate:F1}%");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public GameOptions ConfigureNewGame()
        {
            GameRenderer.DisplayGameSetup();

            var options = new GameOptions();

            Console.Write("Allow any card in empty tableau? (Y/N, default=N for King only): ");
            var emptyRule = Console.ReadLine()?.ToUpper();
            options.AllowAnyCardInEmptyTableau = emptyRule == "Y";

            Console.Write("Draw count (1 or 3, default=1): ");
            var drawInput = Console.ReadLine();
            options.DrawCount = drawInput == "3" ? 3 : 1;

            Console.Write("Allow moving cards from foundation back to tableau? (Y/N, default=N): ");
            var foundationMove = Console.ReadLine()?.ToUpper();
            options.AllowFoundationToTableau = foundationMove == "Y";

            Console.Write("Maximum stock redeals (-1=unlimited, 0=none, 1+=limited, default=-1): ");
            var redealInput = Console.ReadLine();
            if (int.TryParse(redealInput, out int redeals))
                options.MaxStockRedeals = redeals;

            Console.Write("Enable scoring? (Y/N, default=Y): ");
            var scoringInput = Console.ReadLine()?.ToUpper();
            options.EnableScoring = scoringInput != "N";

            Console.WriteLine("\nStarting game...\n");
            System.Threading.Thread.Sleep(1000);

            return options;
        }
    }
}