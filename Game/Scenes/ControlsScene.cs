using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class ControlsScene : Scene
    {
        FadeEffect fadeEffect {  get; set; }
        DungeonCrawler dCrawlerRef { get; set; }
        ControlsCanvas controlsCanvas { get; set; }

        Sprite background { get; set; }
        Button backButton { get; set; }

        public ControlsScene(DungeonCrawler dCrawlerRef)
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            this.dCrawlerRef = dCrawlerRef;
            
            LoadGraphics();
        }

        private void LoadGraphics()
        {
            controlsCanvas = new ControlsCanvas();

            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            backButton = new Button((float)CanvasController.width / 2, (float)CanvasController.height - 50, 0, "Back", OnClick_BackButton);
        }

        public async void OnClick_BackButton()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);

            MainMenu newScene = new MainMenu(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
        }

        public async override Task Update(float deltaTime)
        {
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            await controlsCanvas.Render();
            await backButton.Render();

            if (fadeEffect != null)
            {
                await fadeEffect.Render();

                if (fadeEffect.readyForDeletion)
                {
                    fadeEffect = null;
                }
            }
        }
    }
}
