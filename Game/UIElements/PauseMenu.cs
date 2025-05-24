using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class PauseMenu : UIElement
    {
        public enum MenuStates { PAUSE, CONTROLS, SETTINGS }
        public MenuStates currentState = MenuStates.PAUSE;

        Dungeon dungeonReference {  get; set; }
        FadeEffect fadeEffect { get; set; }

        Sprite background { get; set; }
        Sprite character { get; set; }

        Button resumeButton { get; set; }
        Button controlsButton { get; set; }
        Button settingsButton { get; set; }
        Button mainMenuButton { get; set; }

        SettingsCanvas settingsCanvas { get; set; }
        bool hasSettingsChanged { get; set; }
        Button backButton { get; set; }
        Button applyButton { get; set; }

        ControlsCanvas controlsCanvas { get; set; }

        InfoBox header { get; set; }

        public PauseMenu(float x, float y, float rotation, Dungeon dungeonreference) : base(x, y, rotation)
        {
            this.dungeonReference = dungeonreference;
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            LoadGraphics();
        }

        private void LoadGraphics()
        {
            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            character = new Sprite();
            character.SetImage("Assets/UI/Character_Elf.png");
            character.SetDimensions(new Vector2(91, 289));
            character.SetScale(new Vector2(3f, 3f));

            header = new InfoBox((float)CanvasController.width / 2, 200, 0, "Paused");

            float buttonX = (float)CanvasController.width / 2;
            float buttonY = (float)CanvasController.height / 2 - 50;

            //Pause menu buttons
            resumeButton = new Button(buttonX, buttonY, 0, "Resume", OnClick_Resume);
            controlsButton = new Button(buttonX, buttonY + 75, 0, "Controls", OnClick_Controls);
            settingsButton = new Button(buttonX, buttonY + 150, 0, "Settings", OnClick_Settings);
            mainMenuButton = new Button(buttonX, buttonY + 225, 0, "Main Menu", OnClick_MainMenu);

            //Controls menu
            controlsCanvas = new ControlsCanvas();

            //Settings menu
            settingsCanvas = new SettingsCanvas(SettingsChanged);

            float xIncrement = (float)CanvasController.width / 3;
            float yPos = (float)CanvasController.height - 50;

            backButton = new Button(xIncrement, yPos, 0, "Back", OnClick_Back);
            applyButton = new Button(xIncrement * 2, yPos, 0, "Apply", OnClick_Apply);
            applyButton.SetButtonDisabled(true);
        }

        public async void OnClick_Resume()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            dungeonReference.camera.ChangeState(Entities.Camera.CameraStates.PLAY);
        }

        public async void OnClick_Controls()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            currentState = MenuStates.CONTROLS;
            backButton.SetButtonDisabled(false);
        }

        public async void OnClick_Settings()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            currentState = MenuStates.SETTINGS;
            backButton.SetButtonDisabled(false);
        }

        public async void OnClick_MainMenu()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            resumeButton.SetButtonDisabled(true);
            controlsButton.SetButtonDisabled(true);
            settingsButton.SetButtonDisabled(true);
            mainMenuButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            dungeonReference.ReturnToMainMenu();
        }

        public async void SettingsChanged(bool value)
        {
            hasSettingsChanged = value;
            applyButton.SetButtonDisabled(!value);
        }

        public async void OnClick_Apply()
        {
            settingsCanvas.CommitChanges();
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            applyButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);
            await dungeonReference.camera.InstantiateRayCasts(dungeonReference.worldObjects, dungeonReference.woLayerTwo, dungeonReference.map.gridSize);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            currentState = MenuStates.PAUSE;
            backButton.SetButtonDisabled(false);
        }

        public async void OnClick_Back()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            applyButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            currentState = MenuStates.PAUSE;
            backButton.SetButtonDisabled(false);
        }

        public async override Task Render()
        {
            //Background image
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            //Elf Character
            await RenderingController.Draw(character.image, new Vector2(10, 10), character.dimensions * character.scale);

            if (currentState == MenuStates.PAUSE)
            {
                await header.Render();
                await resumeButton.Render();
                await controlsButton.Render();
                await settingsButton.Render();
                await mainMenuButton.Render();
            }
            else if (currentState == MenuStates.SETTINGS)
            {
                await settingsCanvas.Render();
                await backButton.Render();
                await applyButton.Render();
            }
            else if (currentState == MenuStates.CONTROLS)
            {
                await controlsCanvas.Render();
                await backButton.Render();
            }

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
