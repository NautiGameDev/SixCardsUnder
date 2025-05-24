using System.Numerics;
using System.Threading.Tasks;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class MainMenu : Scene
    {
        DungeonCrawler dgCrawlerRef { get; set; }
        
        Sprite background { get; set; }
        Sprite character { get; set; }
        Sprite logo { get; set; }

        Button playButton { get; set; }
        Button settingsButton { get; set; }
        Button controlsButton { get; set; }

        FadeEffect? fadeEffect { get; set; }

        public MainMenu(DungeonCrawler dgCrawlerRef)
        {
            this.dgCrawlerRef = dgCrawlerRef;
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);

            LoadSprites();
            InstantiateButtons();
            
        }

        private void LoadSprites()
        {
            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            character = new Sprite();
            character.SetImage("Assets/UI/Character_Elf.png");
            character.SetDimensions(new Vector2(91, 289));
            character.SetScale(new Vector2(3f, 3f));

            logo = new Sprite();
            logo.SetImage("Assets/UI/Logo.png");
            logo.SetDimensions(new Vector2(698, 425));
            logo.SetScale(new Vector2(1, 1));
        }

        private void InstantiateButtons()
        {
            float buttonX = (float)(CanvasController.width / 2);
            float buttonY = (float)(CanvasController.height / 2);

            playButton = new Button(buttonX, buttonY, 0, "Play", PlayButtonClicked);
            controlsButton = new Button(buttonX, buttonY + 75, 0, "Controls", ControlsButtonClicked);
            settingsButton = new Button(buttonX, buttonY + 150, 0, "Settings", SettingsButtonClicked);
            
        }


        public async void PlayButtonClicked()
        {
            if (fadeEffect == null)
            {
                fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

                await Task.Delay(1000);

                DraftingMenu newScene = new DraftingMenu(dgCrawlerRef);
                dgCrawlerRef.LoadNewScene(newScene);
            }
        }


        public async void SettingsButtonClicked()
        {
            if (fadeEffect == null)
            {
                fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

                await Task.Delay(1000);

                SettingsScene newScene = new SettingsScene(dgCrawlerRef);
                dgCrawlerRef.LoadNewScene(newScene);
            }

        }

        public async void ControlsButtonClicked()
        {
            if (fadeEffect == null)
            {
                fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

                await Task.Delay(1000);
                ControlsScene newScene = new ControlsScene(dgCrawlerRef);
                dgCrawlerRef.LoadNewScene(newScene);
            }
        }

        public async override Task Update(float deltaTime)
        {
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            float logoX = (float)(CanvasController.width / 2) - (logo.dimensions.X/2) + 85;
            float logoY = (float)(CanvasController.height / 2) - (logo.dimensions.Y/2) - 200;
            Vector2 logoPos = new Vector2(logoX, logoY);

            await RenderingController.Draw(logo.image, logoPos, logo.dimensions * logo.scale);

            await playButton.Render();
            await settingsButton.Render();
            await controlsButton.Render();

            await RenderingController.Draw(character.image, Vector2.Zero, character.dimensions * character.scale);

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
