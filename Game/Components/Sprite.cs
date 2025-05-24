using System.Numerics;
using Microsoft.AspNetCore.Components;
using SeaLegs.Data;

namespace PixelArtGameJam.Game.Components
{
    public class Sprite
    {
        public ElementReference image { get; private set; }
        public ElementReference shadowImage { get; private set; }
        public Vector2 dimensions { get; private set; }
        public Vector2 scale { get; private set; }

        public void SetImage(string path)
        {
            image = GraphicsData.FindSprite(path);
        }

        public void SetShadow(string path)
        {
            shadowImage = GraphicsData.FindSprite(path);
        }

        public ElementReference GetImage()
        {
            return image;
        }

        public void SetDimensions(Vector2 dimensions)
        {
            this.dimensions = dimensions;
        }

        public Vector2 GetDimensions()
        {
            return dimensions;
        }

        public void SetScale(Vector2 scale)
        {
            this.scale = scale;
        }

        public Vector2 GetScale()
        {
            return scale;
        }
    }
}
