using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace BingoGame.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;

        public SettingsService(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _settingsPath = Path.Combine(env.ContentRootPath, "settings.json");
        }

        public List<string> GetBackgroundImages()
        {
            var result = new List<string>();
            if (File.Exists(_settingsPath))
            {
                try {
                    var json = File.ReadAllText(_settingsPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null && dict.TryGetValue("BackgroundImage", out var bgUrl))
                    {
                        if (string.IsNullOrEmpty(bgUrl)) return result;
                        var parts = bgUrl.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        result.AddRange(parts);
                    }
                } catch {
                }
            }
            return result;
        }

        public List<BingoGame.Models.AudioTrack> GetBackgroundAudios()
        {
            var result = new List<BingoGame.Models.AudioTrack>();
            if (File.Exists(_settingsPath))
            {
                try {
                    var json = File.ReadAllText(_settingsPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null && dict.TryGetValue("BackgroundAudio", out var audioUrl))
                    {
                        if (string.IsNullOrEmpty(audioUrl)) return result;
                        var parts = audioUrl.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var part in parts)
                        {
                            var subparts = part.Split(';');
                            if (subparts.Length >= 2 && int.TryParse(subparts[1], out int count))
                            {
                                result.Add(new BingoGame.Models.AudioTrack { Url = subparts[0], RepeatCount = count });
                            }
                            else
                            {
                                result.Add(new BingoGame.Models.AudioTrack { Url = subparts[0], RepeatCount = 1 });
                            }
                        }
                    }
                } catch {
                }
            }
            return result;
        }

        public void SetBackgroundImages(List<string> urls)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (File.Exists(_settingsPath))
            {
                try {
                    dict = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_settingsPath)) ?? new Dictionary<string, string>();
                } catch {
                    dict = new Dictionary<string, string>();
                }
            }
            dict["BackgroundImage"] = string.Join("|", urls);
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(dict, options));
        }

        public void SetBackgroundAudios(List<BingoGame.Models.AudioTrack> tracks)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (File.Exists(_settingsPath))
            {
                try {
                    dict = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_settingsPath)) ?? new Dictionary<string, string>();
                } catch {
                    dict = new Dictionary<string, string>();
                }
            }
            var stringParts = System.Linq.Enumerable.Select(tracks, t => $"{t.Url};{t.RepeatCount}");
            dict["BackgroundAudio"] = string.Join("|", stringParts);
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(dict, options));
        }
    }
}
