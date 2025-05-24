namespace PixelArtGameJam.Game.Data
{
    public class CardData
    {
        public static Dictionary<string, string> cardImagePaths {  get; private set; } = new Dictionary<string, string>()
        {
            {"Energize", "Assets/Cards/Card_Energize.png"},
            {"Spark", "Assets/Cards/Card_Spark.png"},
            {"Siphon", "Assets/Cards/Card_Siphon.png"},
            {"Burst", "Assets/Cards/Card_Burst.png"},
            {"Fear", "Assets/Cards/Card_Fear.png"},
            {"Spikes", "Assets/Cards/Card_Spikes.png"},
            {"Fireball", "Assets/Cards/Card_Fireball.png"},
            {"Heal", "Assets/Cards/Card_Heal.png"},
            {"Block", "Assets/Cards/Card_Block.png"},
            {"Bolt", "Assets/Cards/Card_Bolt.png"},
            {"Surge", "Assets/Cards/Card_Surge.png"},
            {"Specter", "Assets/Cards/Card_Specter.png"},
            {"Firewall", "Assets/Cards/Card_Firewall.png"},
            {"Comet", "Assets/Cards/Card_Comet.png"},
            {"Frost", "Assets/Cards/Card_Frost.png"}            
        };

        public static Dictionary<string, string> GetCardData()
        {
            return cardImagePaths.ToDictionary(KeyValuePair => KeyValuePair.Key, KeyValuePair => KeyValuePair.Value);
        }
    }
}
