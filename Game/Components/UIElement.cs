using System.Numerics;

namespace PixelArtGameJam.Game.Components
{
    public abstract class UIElement
    {
        public Vector2 position {  get; protected set; }
        public float rotation { get; protected set; }

        public Sprite sprite { get; private set; }

        public UIElement(float x, float y, float rotation)
        {
            position = new Vector2(x, y);
            sprite = new Sprite();
            this.rotation = rotation;
        }

        public abstract Task Render();
    }
}
