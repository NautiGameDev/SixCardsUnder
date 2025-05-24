using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class InfoBox : UIElement
    {
        TextElement infoText { get; set; }
        string infoTextMessage { get; set; }
        bool hasLifeSpan { get; set; } = false;
        UICanvas canvas { get; set; }

        DateTime startTime { get; set; }
        double endTime { get; set; }

        public InfoBox(float x, float y, float rotation, string infoTextMessage) : base(x, y, rotation)
        {
            this.infoTextMessage = infoTextMessage;
            
            LoadGraphics();            
        }

        public void SetLifeSpan(UICanvas uiCanvas)
        {
            this.canvas = uiCanvas;
            hasLifeSpan = true;
            startTime = DateTime.Now;

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/deny.wav", effectsVolume, false);
        }

        private void LoadGraphics()
        {
            sprite.SetImage("Assets/UI/InfoBox.png");
            sprite.SetDimensions(new Vector2(180, 48));
            sprite.SetScale(new Vector2(2f, 2f));
                       

            infoText = new TextElement(position.X, position.Y + 13, 0, infoTextMessage, "#a88d75", "#a88d75", "Elv Pixel", "36px");
        }

        public void ChangeMessage(string message)
        {
            infoText.UpdateMessage(message);
        }

        public async override Task Render()
        {
            await RenderingController.Draw(sprite.image, position - (sprite.dimensions * sprite.scale)/2, sprite.dimensions * sprite.scale);
            await infoText.Render();

            if (hasLifeSpan)
            {
                DateTime currentTime = DateTime.Now;

                double timeElapsed = (currentTime - startTime).TotalSeconds;

                if (timeElapsed > 2)
                {
                    canvas.RemoveElement(this);
                }
            }
        }
    }
}
