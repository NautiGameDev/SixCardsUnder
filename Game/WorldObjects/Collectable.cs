using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Entities;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.WorldObjects
{  

    public class Collectable : WorldObject
    {
        float animIndex { get; set; } = 0;
        float animStep { get; set; } = 3f;

        public Ability ability { get; set; }

        public Collectable(Vector2 position, ObjectType objType, Ability ability) : base(position, objType)
        {
            this.ability = ability;
        }

        private void HandleAnimations(float deltaTime)
        {
            animIndex += (animStep * deltaTime);

            if (animIndex > 3)
            {
                animIndex = 0;
            }
        }

        public void Update(float deltaTime)
        {
            HandleAnimations(deltaTime);
        }

        public override async Task Render()
        {
            await RenderingController.Draw(sprite.image, (int)animIndex, 0, sprite.dimensions, castedDimensions * sprite.scale, castedPosition);

            float brightness = CalculateBrightness();

            await CanvasController.context.SetGlobalAlphaAsync(brightness);
            await RenderingController.Draw(sprite.shadowImage, (int)animIndex, 0, sprite.dimensions, castedDimensions * sprite.scale, castedPosition);
            await CanvasController.context.SetGlobalAlphaAsync(1f);

            rayIndices.Clear();
            indexTODistance.Clear();
        }
    }
}
