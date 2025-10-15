namespace KlondikeSolitaire.Core.Models
{
    public class GameStatistics
    {
        public int TotalGamesPlayed { get; set; }
        public int TotalGamesWon { get; set; }
        public int HighestScore { get; set; }
        public TimeSpan FastestWin { get; set; } = TimeSpan.MaxValue;
        public Dictionary<string, int> ConfigUsageCount { get; set; } = new();
        public DateTime? LastPlayed { get; set; }
        public string? FastestWinConfig { get; set; }
        public string? HighestScoreConfig { get; set; }

        public void RecordGame(GameOptions options, bool won, int score, TimeSpan duration)
        {
            TotalGamesPlayed++;
            if (won) TotalGamesWon++;

            var configKey = options.GetConfigKey();
            if (!ConfigUsageCount.ContainsKey(configKey))
                ConfigUsageCount[configKey] = 0;
            ConfigUsageCount[configKey]++;

            if (won && options.EnableScoring)
            {
                if (score > HighestScore)
                {
                    HighestScore = score;
                    HighestScoreConfig = configKey;
                }

                if (duration < FastestWin)
                {
                    FastestWin = duration;
                    FastestWinConfig = configKey;
                }
            }

            LastPlayed = DateTime.Now;
        }

        public string GetMostUsedConfig()
        {
            if (ConfigUsageCount.Count == 0) return "None yet";
            return ConfigUsageCount.OrderByDescending(x => x.Value).First().Key;
        }

        public double WinRate => TotalGamesPlayed > 0 ? (double)TotalGamesWon / TotalGamesPlayed * 100 : 0;
    }
}