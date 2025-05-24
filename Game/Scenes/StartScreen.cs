using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Blazor.Extensions;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class StartScreen : Scene
    {
        public enum ScreenState { NAUTIFADEIN, NAUTISIT, NAUTIFADEOUT, STARTSCREENFADEIN, STARTSCREEN}
        public ScreenState currentState = ScreenState.NAUTIFADEIN;

        DungeonCrawler dCrawlerRef {  get; set; }

        Sprite nautiLogo {  get; set; }
        Sprite gameLogo { get; set; }
        Sprite background { get; set; }

        Button startButton { get; set; }
        
        float opacity { get; set; }

        float stateTimer { get; set; }
        float stateTimeToDisplay { get; set; }

        FadeEffect fadeEffect { get; set; }

        public StartScreen(DungeonCrawler dCrawlerRef) 
        {
            this.dCrawlerRef = dCrawlerRef;
            LoadGraphics();
            opacity = 0f;
            stateTimeToDisplay = 1.5f;
            stateTimer = 0f;
        }

        private void LoadGraphics()
        {
            nautiLogo = new Sprite();
            nautiLogo.SetImage("Assets/UI/Nauti.png");
            nautiLogo.SetDimensions(new Vector2(1489, 1139));
            nautiLogo.SetScale(new Vector2(0.25f, 0.25f));

            gameLogo = new Sprite();
            gameLogo.SetImage("Assets/UI/Logo.png");
            gameLogo.SetDimensions(new Vector2(698, 425));
            gameLogo.SetScale(new Vector2(1, 1));

            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4, 4));

            Vector2 buttonPos = new Vector2((float)(CanvasController.width / 2), (float)(CanvasController.height / 2) + 100);

            startButton = new Button(buttonPos.X, buttonPos.Y, 0, "Start", StartGame);            
        }

        public async void StartGame()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            PlayMusic();

            MainMenu newScene = new MainMenu(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
        }


        private async void PlayMusic()
        {
            await AudioController.PlaySound("Assets/Audio/MainMenuMusic.ogg", 1f, true);
        }

        public async override Task Update(float deltaTime)
        {
            stateTimer += deltaTime;
            if (stateTimer > stateTimeToDisplay)
            {
                stateTimer = stateTimeToDisplay;
            }


            await RenderingController.DrawRectangles("black", 0, 0, CanvasController.width, CanvasController.height);

            switch (currentState)
            {
                case ScreenState.NAUTIFADEIN:
                    await NautiLogoFadeIn(deltaTime);
                    break;

                case ScreenState.NAUTISIT:
                    await NautiLogoSit(deltaTime);
                    break;

                case ScreenState.NAUTIFADEOUT:
                    await NautiLogoFadeOut(deltaTime);
                    break;

                case ScreenState.STARTSCREENFADEIN:
                    await StartScreenFadeIn(deltaTime);
                    break;

                case ScreenState.STARTSCREEN:
                    await StartScreenSit(deltaTime);
                    break;
            }
        }

        private async Task NautiLogoFadeIn(float deltaTime)
        {
            opacity = stateTimer / stateTimeToDisplay;

            await CanvasController.context.SetGlobalAlphaAsync(opacity);

            float logoX = (float)((CanvasController.width / 2) - (nautiLogo.dimensions.X * nautiLogo.scale.X)/2);
            float logoY = (float)((CanvasController.height / 2) - (nautiLogo.dimensions.Y * nautiLogo.scale.Y) / 2);

            Vector2 logoPos = new Vector2(logoX, logoY);

            await RenderingController.Draw(nautiLogo.image, logoPos, nautiLogo.dimensions * nautiLogo.scale);
            await CanvasController.context.SetGlobalAlphaAsync(1f);

            if (stateTimer >= stateTimeToDisplay)
            {
                stateTimer = 0f;
                currentState = ScreenState.NAUTISIT;
            }
        }

        private async Task NautiLogoSit(float deltaTime)
        {
            float logoX = (float)((CanvasController.width / 2) - (nautiLogo.dimensions.X * nautiLogo.scale.X) / 2);
            float logoY = (float)((CanvasController.height / 2) - (nautiLogo.dimensions.Y * nautiLogo.scale.Y) / 2);

            Vector2 logoPos = new Vector2(logoX, logoY);
            await RenderingController.Draw(nautiLogo.image, logoPos, nautiLogo.dimensions * nautiLogo.scale);

            if (stateTimer >= stateTimeToDisplay)
            {
                stateTimer = 0f;
                currentState = ScreenState.NAUTIFADEOUT;
            }
        }

        private async Task NautiLogoFadeOut(float deltaTime)
        {
            opacity = 1 - (stateTimer / stateTimeToDisplay);

            await CanvasController.context.SetGlobalAlphaAsync(opacity);

            float logoX = (float)((CanvasController.width / 2) - (nautiLogo.dimensions.X * nautiLogo.scale.X) / 2);
            float logoY = (float)((CanvasController.height / 2) - (nautiLogo.dimensions.Y * nautiLogo.scale.Y) / 2);

            Vector2 logoPos = new Vector2(logoX, logoY);

            await RenderingController.Draw(nautiLogo.image, logoPos, nautiLogo.dimensions * nautiLogo.scale);
            await CanvasController.context.SetGlobalAlphaAsync(1f);

            if (stateTimer >= stateTimeToDisplay)
            {
                stateTimer = 0f;
                currentState = ScreenState.STARTSCREENFADEIN;
            }
        }

        private async Task StartScreenFadeIn(float deltaTime)
        {
            opacity = 1 - (stateTimer / stateTimeToDisplay);

            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            float logoX = (float)((CanvasController.width / 2) - (gameLogo.dimensions.X * gameLogo.scale.X) / 2 + 90);
            float logoY = (float)((CanvasController.height / 2) - (gameLogo.dimensions.Y * gameLogo.scale.Y) / 2 - 100);

            Vector2 logoPos = new Vector2( logoX, logoY);

            await RenderingController.Draw(gameLogo.image, logoPos, gameLogo.dimensions * gameLogo.scale);

            await CanvasController.context.SetGlobalAlphaAsync(opacity);
            await RenderingController.DrawRectangles("black", 0, 0, CanvasController.width, CanvasController.height);
            await CanvasController.context.SetGlobalAlphaAsync(1f);

            if (stateTimer >= stateTimeToDisplay)
            {
                stateTimer = 0f;
                currentState = ScreenState.STARTSCREEN;
            }
        }

        private async Task StartScreenSit(float deltaTime)
        {           

            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            float logoX = (float)((CanvasController.width / 2) - (gameLogo.dimensions.X * gameLogo.scale.X) / 2 + 90);
            float logoY = (float)((CanvasController.height / 2) - (gameLogo.dimensions.Y * gameLogo.scale.Y) / 2 - 100);
            Vector2 logoPos = new Vector2(logoX, logoY);

            await RenderingController.Draw(gameLogo.image, logoPos, gameLogo.dimensions * gameLogo.scale);

            if (fadeEffect == null)
            {
                await startButton.Render();
            }
            else
            {
                await fadeEffect.Render();
            }
                        
        }

    }
}
