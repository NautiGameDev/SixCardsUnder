namespace PixelArtGameJam.Game.Data
{
    public class Stats
    {
        public static DateTime floorStart { get; set; } = DateTime.Now;
        public static int EnemiesKilled { get; set; } = 0;
        public static int FloorReached {  get; set; } = 0;
        public static int DamageTaken { get; set; } = 0;
        public static int CardsCasted { get; set; } = 0;
        public static int CardsDrawn { get; set; } = 0;
        public static int DamageDealt { get; set; } = 0;


    }
}
