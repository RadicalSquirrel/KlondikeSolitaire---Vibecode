namespace KlondikeSolitaire.UI.Renderers
{
    using KlondikeSolitaire.Core.Game;
    using KlondikeSolitaire.Core.Models;
    using KlondikeSolitaire.Core.Services;

    public static class GameRenderer
    {
        public static void DisplayGame(KlondikeSolitaireGame game)
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                          KLONDIKE SOLITAIRE                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"Rules: {(game.Options.AllowAnyCardInEmptyTableau ? "Any card" : "King only")} | Draw {game.Options.DrawCount} | " +
                $"F→T: {(game.Options.AllowFoundationToTableau ? "Yes" : "No")} | " +
                $"Redeals: {(game.Options.MaxStockRedeals < 0 ? "∞" : game.Options.MaxStockRedeals.ToString())} (Used: {game.StockRedeals})");

            if (game.Options.EnableScoring)
            {
                Console.WriteLine($"Score: {game.Score} | Time: {game.ElapsedTime:mm\\:ss}");
            }
            Console.WriteLine();

            Console.WriteLine("┌─────────────────────────────────────┬─────────────────────────────────────┐");
            Console.WriteLine("│ STOCK & WASTE                       │ FOUNDATIONS                         │");
            Console.WriteLine("├─────────────────────────────────────┼─────────────────────────────────────┤");

            Console.Write("│ ");
            if (game.StockCount > 0)
                Console.Write($"[##]({game.StockCount,2})");
            else
                Console.Write("[    ]  ");

            Console.Write(" → ");

            if (game.WasteCount > 0 && game.WasteIndex >= 0)
            {
                var visibleCards = game.Waste.Skip(Math.Max(0, game.WasteIndex - game.Options.DrawCount + 1)).Take(game.Options.DrawCount).ToList();
                for (int i = 0; i < visibleCards.Count; i++)
                {
                    Console.Write(visibleCards[i].ToString());
                    if (i < visibleCards.Count - 1) Console.Write(" ");
                }
                for (int i = visibleCards.Count; i < 3; i++)
                    Console.Write("     ");
            }
            else
            {
                Console.Write("[    ]               ");
            }

            Console.Write(" │ ");

            for (int i = 0; i < 4; i++)
            {
                if (game.Foundations[i].Count > 0)
                    Console.Write(game.Foundations[i][^1].ToString());
                else
                    Console.Write($"[{i + 1}:_]");

                if (i < 3) Console.Write(" ");
            }
            Console.WriteLine(" │");

            Console.WriteLine("└─────────────────────────────────────┴─────────────────────────────────────┘");
            Console.WriteLine();

            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                  TABLEAU                                      ║");
            Console.WriteLine("╠═════╦═════╦═════╦═════╦═════╦═════╦═══════════════════════════════════════════╣");
            Console.Write("║");
            for (int col = 0; col < 7; col++)
            {
                Console.Write($"  {col + 1}  ");
                if (col < 6) Console.Write("║");
            }
            Console.WriteLine("║                             ║");
            Console.WriteLine("╠═════╬═════╬═════╬═════╬═════╬═════╬═══════════════════════════════════════════╣");

            int maxHeight = game.Tableau.Any(t => t.Count > 0) ? game.Tableau.Max(pile => pile.Count) : 0;

            var commands = new[]
            {
                "Commands:",
                "D - Draw from stock",
                "WT# - Waste to Tableau #",
                "WF# - Waste to Foundation #",
                "TF#,# - Tableau to Found.",
                "TT#,#,idx - Tableau move",
                "FT#,# - Foundation to Tab.",
                "U - Undo last move",
                "S - View statistics",
                "Q - Quit game"
            };

            for (int row = 0; row < Math.Max(maxHeight, commands.Length); row++)
            {
                Console.Write("║");
                for (int col = 0; col < 7; col++)
                {
                    if (row < game.Tableau[col].Count)
                    {
                        Console.Write(game.Tableau[col][row].ToString());
                    }
                    else
                    {
                        if (row == 0 && game.Tableau[col].Count == 0)
                            Console.Write("[   ]");
                        else
                            Console.Write("     ");
                    }

                    if (col < 6) Console.Write("║");
                }

                if (row < commands.Length)
                    Console.WriteLine($"║ {commands[row],-27} ║");
                else
                    Console.WriteLine("║                             ║");
            }

            Console.WriteLine("╚═════╩═════╩═════╩═════╩═════╩═════╩═══════════════════════════════════════════╝");
        }

        public static void DisplayStatistics(GameStatistics stats)
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║         KLONDIKE SOLITAIRE - STATISTICS           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine($"  Total Games Played: {stats.TotalGamesPlayed}");
            Console.WriteLine($"  Total Games Won: {stats.TotalGamesWon}");
            Console.WriteLine($"  Win Rate: {stats.WinRate:F1}%");
            Console.WriteLine();

            if (stats.HighestScore > 0)
            {
                Console.WriteLine($"  Highest Score: {stats.HighestScore}");
                if (!string.IsNullOrEmpty(stats.HighestScoreConfig))
                    Console.WriteLine($"    Configuration: {StatisticsManager.FormatConfigName(stats.HighestScoreConfig)}");
            }
            else
            {
                Console.WriteLine("  Highest Score: No wins yet");
            }
            Console.WriteLine();

            if (stats.FastestWin != TimeSpan.MaxValue)
            {
                Console.WriteLine($"  Fastest Win: {stats.FastestWin:mm\\:ss}");
                if (!string.IsNullOrEmpty(stats.FastestWinConfig))
                    Console.WriteLine($"    Configuration: {StatisticsManager.FormatConfigName(stats.FastestWinConfig)}");
            }
            else
            {
                Console.WriteLine("  Fastest Win: No wins yet");
            }
            Console.WriteLine();

            Console.WriteLine($"  Most Used Configuration: {StatisticsManager.FormatConfigName(stats.GetMostUsedConfig())}");
            if (stats.ConfigUsageCount.Count > 0)
            {
                var mostUsed = stats.ConfigUsageCount.OrderByDescending(x => x.Value).First();
                Console.WriteLine($"    Used {mostUsed.Value} time(s)");
            }
            Console.WriteLine();

            if (stats.ConfigUsageCount.Count > 1)
            {
                Console.WriteLine("  Configuration Usage:");
                foreach (var config in stats.ConfigUsageCount.OrderByDescending(x => x.Value).Take(5))
                {
                    Console.WriteLine($"    • {StatisticsManager.FormatConfigName(config.Key)}: {config.Value} game(s)");
                }
                Console.WriteLine();
            }

            if (stats.LastPlayed.HasValue)
            {
                Console.WriteLine($"  Last Played: {stats.LastPlayed.Value:g}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║          KLONDIKE SOLITAIRE - MAIN MENU           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1. New Game");
            Console.WriteLine("  2. View Statistics");
            Console.WriteLine("  3. Exit");
            Console.WriteLine();
            Console.Write("Select option: ");
        }

        public static void DisplayGameSetup()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║          KLONDIKE SOLITAIRE - SETUP               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");
            Console.WriteLine();
        }
    }
}