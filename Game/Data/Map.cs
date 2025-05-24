using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.MapGenerator;

namespace PixelArtGameJam.Game.Data
{
    public class Map
    {
        new string[][] mapData =
                {
                [".", ".", ".", ".", ".", ".", ".", ".", "x", "x", "x", ".", ".", ".", ".", ".", ".", ".", ".", "."],
                [".", "x", "x", "x", "x", "x", "x", "x", "x", " ", "x", "x", "x", "x", "x", "x", "x", "x", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", "x", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", "x", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", "x", " ", " ", " ", " ", "x", " ", " ", "x", " ", "x", " ", " ", "x", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                ["x", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                ["x", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                ["x", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", "."],
                [" ", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", "x"],
                [" ", "x", " ", "x", " ", " ", " ", " ", "x", " ", " ", "x", " ", " ", " ", " ", " ", " ", " ", "x"],
                [" ", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "x"],
                [".", "x", "x", "x", " ", " ", "x", "x", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", "x", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", "x", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", "x", " ", " ", " ", "x", " ", "x", " ", " ", "x", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", "x", " ", "p", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                [".", "x", " ", " ", " ", " ", " ", "x", " ", " ", " ", "x", " ", " ", " ", " ", " ", " ", "x", "."],
                [".", "x", "x", "x", "x", "x", "x", "x", "x", " ", "x", "x", "x", "x", "x", "x", "x", "x", "x", "."],
                [".", ".", ".", ".", ".", ".", ".", ".", "x", "x", "x", ".", ".", ".", ".", ".", ".", ".", ".", "."],
                };

        public LevelGenerator levelGen { get; set; }

        public string wallPath = "Assets/Textures/HighRes/MudBricks.png";
        public string floorPath = "Assets/Textures/HighRes/MudBricksFloor.png";
        public string ceilingPath = "Assets/Textures/HighRes/MudBricksCeiling.png";

        Dictionary<string, string> wallTextures = new Dictionary<string, string>()
        {
            {"MudBrick", "Assets/Textures/HighRes/MudBricks.png"},
            {"TwoToneBrick", "Assets/Textures/HighRes/TwoToneBricks.png"},
            {"CrackedBricks", "Assets/Textures/HighRes/CrackedBricks.png"},
            {"PolishedFlag", "Assets/Textures/HighRes/PolishedBrickFlag.png"},
            {"TwoToneFinish", "Assets/Textures/HighRes/TwoToneFinish.png"},
            {"TwoToneStart", "Assets/Textures/HighRes/TwoToneStart.png"}
        };

        public int gridSize { get; private set; } = 128;

        public Map()
        {
            
        }

        public async Task LoadLevelGenerator(int floorNumb)
        {
            levelGen = new LevelGenerator();
            await levelGen.LoadRoomData();
            await levelGen.SetEnemyNumber(floorNumb);
        }

        public async Task GenerateMapData()
        {
            mapData = await levelGen.GenerateMap();
        }

        public string[][] GetMapData()
        {
            return mapData;
        }

        public string GetTexturePath(string texture)
        {
            return wallTextures[texture];
        }
    }
}
