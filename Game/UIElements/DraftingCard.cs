using System.Numerics;
using Blazor.Extensions;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class DraftingCard : UIElement
    {
        public string cardName { get; set; }       
        
        Action<DraftingCard> callback {  get; set; }

        float scaleOffset { get; set; } = 1f;
        bool isHovered = false;

        public bool isSelected = false;

        public DraftingCard(float x, float y, float rotation, string cardName, Action<DraftingCard> callback) : base(x, y, rotation)
        {
            this.cardName = cardName;
            this.callback = callback;
        }

        public void UpdatePosition(Vector2 pos)
        {
            position = pos;
        }

        private bool IsHovered()
        {
            Vector2 mousePos = InputController.GetMousePosition();

            if (mousePos.X > position.X - (sprite.dimensions.X * sprite.scale.X * scaleOffset) / 2 &&
                mousePos.X < position.X + (sprite.dimensions.X * sprite.scale.X * scaleOffset) / 2 &&
                mousePos.Y > position.Y - (sprite.dimensions.Y * sprite.scale.Y * scaleOffset) / 2 &&
                mousePos.Y < position.Y + (sprite.dimensions.Y * sprite.scale.Y * scaleOffset) / 2
                )
            {
                scaleOffset = 1.1f;
                return true;
            }
            else
            {
                scaleOffset = 1f;
                return false;
            }
        }

        private void IsClicked()
        {
            if (InputController.OnMouseDown() && !isSelected)
            {
                callback?.Invoke(this);

                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/card.wav", effectsVolume, false);
            }
        }

        public async override Task Render()
        {
            if (IsHovered())
            {
                IsClicked();
            }

            await RenderingController.Draw(sprite.image, position - (sprite.dimensions * sprite.scale * scaleOffset)/2, sprite.dimensions * sprite.scale * scaleOffset);
        }
    }
}
