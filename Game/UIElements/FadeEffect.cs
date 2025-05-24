using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class FadeEffect : UIElement
    {
        public enum EffectDir { FADEIN, FADEOUT }
        public EffectDir currentDir { get; set; }
        float opacity { get; set; }

        DateTime startTime { get; set; }
        double fadeTimer { get; set; }

        public bool readyForDeletion = false;

        public FadeEffect(float x, float y, float rotation, EffectDir currentDir) : base(x, y, rotation)
        {
            this.currentDir = currentDir;
            opacity = 0f;
            fadeTimer = 1.0;

            startTime = DateTime.Now;
        }

        public async override Task Render()
        {
            TimeSpan elapsedTimeSpan = DateTime.Now - startTime;
            double elapsedTime = elapsedTimeSpan.TotalSeconds;

            if (currentDir == EffectDir.FADEIN)
            {
                opacity = 1f - (float)(elapsedTime / fadeTimer);
            }
            else
            {
                opacity = (float)(elapsedTime / fadeTimer);
            }

            opacity = Math.Clamp(opacity, 0f, 1f);

            await CanvasController.context.SetGlobalAlphaAsync(opacity);
            await RenderingController.DrawRectangles("black", 0, 0, CanvasController.width, CanvasController.height);

            await CanvasController.context.SetGlobalAlphaAsync(1f);

            if (opacity == 0f || opacity == 1f)
            {
                readyForDeletion = true;
            }
        }
    }
}
