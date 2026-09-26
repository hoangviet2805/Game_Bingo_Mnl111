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

        public string GetBackgroundImage()
        {
            if (File.Exists(_settingsPath))
            {
                try {
                    var json = File.ReadAllText(_settingsPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null && dict.TryGetValue("BackgroundImage", out var bgUrl))
                    {
                        return bgUrl;
                    }
                } catch {
                    return "";
                }
            }
            return "";
        }

        public string GetBackgroundAudio()
        {
            if (File.Exists(_settingsPath))
            {
                try {
                    var json = File.ReadAllText(_settingsPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null && dict.TryGetValue("BackgroundAudio", out var audioUrl))
                    {
                        return audioUrl;
                    }
                } catch {
                    return "";
                }
            }
            return "";
        }

        public void SetBackgroundImage(string url)
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
            dict["BackgroundImage"] = url;
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(dict, options));
        }

        public void SetBackgroundAudio(string url)
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
            dict["BackgroundAudio"] = url;
            File.WriteAllText(_settingsPath, JsonSerializer.Serialize(dict, options));
        }
    }
}
