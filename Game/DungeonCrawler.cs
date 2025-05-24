using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;
using SeaLegs.Data;

namespace PixelArtGameJam.Game
{
    public class DungeonCrawler
    {
        private bool hasLoaded { get; set; } = false;
        Scene currentScene { get; set; }

        public DungeonCrawler()
        {
            CanvasController.SetCallback(this.Update);
        }

        public void LoadNewScene(Scene scene)
        {
            currentScene = scene;
        }

        public async Task Update(float deltaTime)
        {
            if (!hasLoaded)
            {
                currentScene = new StartScreen(this);
                hasLoaded = true;
            }

            if (currentScene != null)
            {
                await currentScene.Update(deltaTime);
            }
        }
    }
}
