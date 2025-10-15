using KlondikeSolitaire.Core.Game;
using KlondikeSolitaire.Core.Services;
using KlondikeSolitaire.UI.Controllers;
using KlondikeSolitaire.UI.Renderers;

namespace KlondikeSolitaire.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new GameController();

            while (true)
            {
                GameRenderer.DisplayMainMenu();
                var choice = System.Console.ReadLine();

                if (choice == "1")
                {
                    var options = controller.ConfigureNewGame();
                    var game = new KlondikeSolitaireGame(options);
                    controller.Play(game);
                }
                else if (choice == "2")
                {
                    var stats = StatisticsManager.LoadStatistics();
                    GameRenderer.DisplayStatistics(stats);
                }
                else if (choice == "3")
                {
                    break;
                }
            }

            System.Console.WriteLine("\nThanks for playing!");
        }
    }
}