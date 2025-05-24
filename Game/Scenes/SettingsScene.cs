using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class SettingsScene : Scene
    {
        DungeonCrawler dCrawlerRef { get; set; }
        
        FadeEffect fadeEffect { get; set; }

        SettingsCanvas settingsCanvas { get; set; }
        Sprite background { get; set; }
        Button backButton { get; set; }
        Button applyButton { get; set; }

        bool settingsModified { get; set; } = false;

        public SettingsScene(DungeonCrawler dCrawlerRef)
        {
            this.dCrawlerRef = dCrawlerRef;
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            LoadGraphics();
        }

        private void LoadGraphics()
        {
            settingsCanvas = new SettingsCanvas(SetModifiedSettings);

            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            float xIncrement = (float)CanvasController.width / 3;
            float yPos = (float)CanvasController.height - 50;

            backButton = new Button(xIncrement, yPos, 0, "Back", OnClickBackButon);
            applyButton = new Button(xIncrement * 2, yPos, 0, "Apply", OnClickApplyButton);
            applyButton.SetButtonDisabled(true);
        }

        public async void OnClickBackButon()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            applyButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);

            MainMenu newScene = new MainMenu(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
        }

        public async void OnClickApplyButton()
        {
            
            settingsCanvas.CommitChanges();
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            applyButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);

            MainMenu newScene = new MainMenu(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
            
        }

        public void SetModifiedSettings(bool value)
        {
            settingsModified = value;
            applyButton.SetButtonDisabled(false);
        }

        public async override Task Update(float deltaTime)
        {
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            await settingsCanvas.Render();

            await backButton.Render();
            await applyButton.Render();


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
