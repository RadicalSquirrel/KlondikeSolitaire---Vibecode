using System.Text.Json;
using KlondikeSolitaire.Core.Models;
using Microsoft.JSInterop;

namespace KlondikeSolitaire.Blazor.Services
{
    public class StatisticsService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "klondike_statistics";

        public StatisticsService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<GameStatistics> LoadStatisticsAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<GameStatistics>(json) ?? new GameStatistics();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }

            return new GameStatistics();
        }

        public async Task SaveStatisticsAsync(GameStatistics stats)
        {
            try
            {
                var json = JsonSerializer.Serialize(stats);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
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
