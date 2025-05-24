using System.Numerics;

namespace PixelArtGameJam.Game.Components
{
    public abstract class Entity
    {
        public Vector2 position {  get; private set; }

        public Entity(Vector2 position)
        {
            this.position = position;
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            this.position = newPosition;
        }

    }
}
