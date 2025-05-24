using System.Numerics;
using Blazor.Extensions;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class Card : UIElement
    {
        public Ability ability { get; private set; }

        Vector2 handPosition { get; set; }
        float handRotation { get; set; }

        public Card(float x, float y, float rotation, Ability ability) : base(x, y, rotation)
        {
            this.ability = ability;
        }

        public void Use(Dungeon dungeonReference, Vector2 position, float rotation)
        {
            ability.Cast(dungeonReference, position, rotation);
        }

        public void UpdateCardPosition(Vector2 newPosition, float rotation)
        {
            position = newPosition;
            this.rotation = rotation;
            handPosition = newPosition;
            handRotation = rotation;
        }

        public void MoveCard(Vector2 newPosition)
        {
            position = newPosition;
            rotation = 0;
        }

        public void SnapBackToHand()
        {
            position = handPosition;
            rotation = handRotation;
            sprite.SetScale(new Vector2(1, 1));
        }

        public void ChangeScale(Vector2 newScale)
        {
            sprite.SetScale(newScale);
        }

        public override async Task Render()
        {

            if (position.Y + (sprite.dimensions.Y / 2) > (CanvasController.height * 0.6))
            {
                Vector2 adjustedDimensions = sprite.dimensions * sprite.scale;
                Vector2 rotationOrigin = new Vector2(position.X + (adjustedDimensions.X / 2), position.Y + (adjustedDimensions.Y / 2));

                await RenderingController.Draw(sprite.image, position, adjustedDimensions, rotation, rotationOrigin);
            }
            else
            {
                Vector2 scaledDimensions = ability.sprite.dimensions * ability.sprite.scale;
                Vector2 centerPosition = position - (ability.sprite.dimensions / 2);
                
                await RenderingController.Draw(ability.sprite.image, 0, 0, ability.sprite.dimensions, scaledDimensions, centerPosition);
            }
            
        }
    }
}
