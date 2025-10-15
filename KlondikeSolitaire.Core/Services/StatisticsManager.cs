namespace KlondikeSolitaire.Core.Services
{
    using System.Text.Json;
    using KlondikeSolitaire.Core.Models;

    public static class StatisticsManager
    {
        private static readonly string StatsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "KlondikeSolitaire",
            "stats.json"
        );

        public static GameStatistics LoadStatistics()
        {
            try
            {
                if (File.Exists(StatsFilePath))
                {
                    var json = File.ReadAllText(StatsFilePath);
                    return JsonSerializer.Deserialize<GameStatistics>(json) ?? new GameStatistics();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }

            return new GameStatistics();
        }

        public static void SaveStatistics(GameStatistics stats)
        {
            try
            {
                var directory = Path.GetDirectoryName(StatsFilePath);
                if (directory != null && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StatsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving statistics: {ex.Message}");
            }
        }

        public static string FormatConfigName(string configKey)
        {
            if (configKey == "None yet") return configKey;

            var parts = configKey.Split('_');
            var formatted = new List<string>();

            foreach (var part in parts)
            {
                if (part == "Any") formatted.Add("Any Card in Empty");
                else if (part == "King") formatted.Add("King Only");
                else if (part.EndsWith("Draw")) formatted.Add($"{part[0]} Card Draw");
                else if (part == "FT") formatted.Add("Foundation→Tableau");
                else if (part == "NoFT") formatted.Add("No Foundation→Tableau");
                else if (part.EndsWith("Redeals"))
                {
                    var num = part.Replace("Redeals", "");
                    if (num == "-1") formatted.Add("Unlimited Redeals");
                    else if (num == "0") formatted.Add("No Redeals");
                    else formatted.Add($"{num} Redeal(s)");
                }
            }

            return string.Join(", ", formatted);
        }
    }
}