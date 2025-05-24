using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Data
{
    public class PlayerSettings
    {
        public static Dictionary<string, string> settings = new Dictionary<string, string>()
        {
            {"Master Volume", "1" },
            {"Music Volume", "1" },
            {"Effects Volume", "1" },
            {"Rendering", "4" }
        };

        public static string GetSetting(string key)
        {
            return settings[key];
        }

        public static void UpdateSetting(string key, string value)
        {
            settings[key] = value;

            if (key == "Master Volume")
            {
                AudioController.SetMasterVolume(float.Parse(value));
            }
            else if (key == "Music Volume")
            {
                AudioController.SetMusicVolume(float.Parse(value));
            }
        }
    }
}
