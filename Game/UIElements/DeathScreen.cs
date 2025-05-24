using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class DeathScreen : UIElement
    {
        Dungeon dungeonRef {  get; set; }
        Sprite skullSprite {  get; set; }
        TextElement youDiedText { get; set; }
        Button confirmButton { get; set; }

        FadeEffect fadeEffect { get; set; }

        float animIndex = 0;
        float animStep = 3f;
        Vector2 skullPosition { get; set; }

        public DeathScreen(float x, float y, float rotation, Dungeon dungeonRef) : base(x, y, rotation)
        {
            LoadGraphics();
            this.dungeonRef = dungeonRef;

        }

        private void LoadGraphics()
        {
            skullSprite = new Sprite();
            skullSprite.SetImage("Assets/Abilities/Ability_Specter.png");
            skullSprite.SetDimensions(new Vector2(256, 256));
            skullSprite.SetScale(new Vector2(5f, 5f));
            skullPosition = new Vector2((float)CanvasController.width / 2 - (skullSprite.dimensions.X * skullSprite.scale.X) /2, (float)CanvasController.height / 2 - (skullSprite.dimensions.Y * skullSprite.scale.Y) / 2 - 100);

            youDiedText = new TextElement((float)CanvasController.width / 2, (float)CanvasController.height / 2 + 100, 0, "You died...", "#a88d75", "#a88d75", "Elv Pixel", "48px");

            confirmButton = new Button((float)CanvasController.width / 2, (float)(CanvasController.height / 2) + 200, 0, "Well, shit...", OnClick_ConfirmButton);    
            
        }

        public async void OnClick_ConfirmButton()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            confirmButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/footstep.wav", effectsVolume, false);

            await Task.Delay(1000);
            dungeonRef.ReturnToMainMenu();
        }

        public void HandleAnimations(float deltaTime)
        {
            animIndex += (animStep * deltaTime);

            if (animIndex > 3)
            {
                animIndex = 0;
            }
        }

        public async override Task Render()
        {
            await RenderingController.DrawRectangles("rgba(0, 0, 0, 0.75)", 0, 0, (float)CanvasController.width, CanvasController.height);
            await RenderingController.DrawRectangles("rgba(220, 20, 60, 0.5)", 0, 0, (float)CanvasController.width, CanvasController.height);
            

            await RenderingController.Draw(skullSprite.image, (int)animIndex, 0, skullSprite.dimensions, skullSprite.dimensions * skullSprite.scale, skullPosition);
            await youDiedText.Render();
            await confirmButton.Render();

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
