using PixelArtGameJam.Game.Components;

namespace PixelArtGameJam.Game.Data
{
    public class PlayerData
    {
        public static Hand storedHand { get; set; } = null;
        public static int currentHealth { get; set; }
        public static int currentShield { get; set; }
        public static int currentEnergy { get; set; }

    }
}
