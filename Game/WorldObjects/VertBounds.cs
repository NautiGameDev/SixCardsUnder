using System.Numerics;
using PixelArtGameJam.Game.Entities;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.WorldObjects
{
    /*
        Class is used for floor and ceiling rendering
        Basic rendering with a single image and overlapping horizontal rectangles to create depth
     */
    public class VertBounds : WorldObject
    {
        public enum BoundsType { FLOOR, CEILING }
        private BoundsType currentBounds = BoundsType.FLOOR;

        private int maxDepth { get; set; }
        private int totalHorizontalGradients { get; set; }
        private int horizontalStripSize { get; set; }

        const int numGradientLines = 15;
        const int stripOffset = 2;
        const float darknessRate  = 1.3f;

        public VertBounds(Vector2 position, ObjectType objType, BoundsType boundsType, int maxDepth) : base(position, objType)
        {
            currentBounds = boundsType;
            this.maxDepth = maxDepth;

            
            totalHorizontalGradients = (int)(CanvasController.height / 2) / numGradientLines;
            horizontalStripSize = (int)(CanvasController.height / 2) / totalHorizontalGradients;
            
        }

        private string CalculateBrightness(double distance)
        {
            float brightness = 1f - (float)distance / ((float)CanvasController.height/2);
            brightness = Math.Clamp(brightness, 0f, 1f);
            float adjustedbrightness = 1f - brightness;
            return $"rgba(0, 0, 0, {adjustedbrightness}";
        }

        public override async Task Render()
        {

            await RenderingController.Draw(sprite.image, position, sprite.dimensions);

        }

        public override async Task RenderShadows()
        {
            if (currentBounds == BoundsType.FLOOR)
            {
                for (int i = 0; i < totalHorizontalGradients + stripOffset; i++) //2 is a buffer to ensure floor is completely covered
                {
                    

                    double distance = i * horizontalStripSize * darknessRate; //1.5f is a modifier to adjust darkness levels

                    string brightness = CalculateBrightness(distance);
                    float x = 0;
                    float y = ((float)CanvasController.height) - (horizontalStripSize * (i + 1));

                    await CanvasController.context.SetFillStyleAsync(brightness);
                    await CanvasController.context.FillRectAsync(x, y, CanvasController.width, horizontalStripSize);
                }
            }
            else
            {
                for (int i = 0; i < totalHorizontalGradients + stripOffset; i++) //2 is a buffer to ensure floor is completely covered
                {
                    

                    double distance = i * horizontalStripSize * darknessRate; //Magic number = the rate at which gradient darkens

                    string brightness = CalculateBrightness(distance);
                    float x = 0;
                    float y = (horizontalStripSize * (i + 1));

                    await CanvasController.context.SetFillStyleAsync(brightness);
                    await CanvasController.context.FillRectAsync(x, y, CanvasController.width, horizontalStripSize);
                }
            }
        }
    }
}
