using System.Numerics;
using Blazor.Extensions.Canvas.Canvas2D;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class TextElement : UIElement
    {
        string text { get; set; }
        string fontColor {  get; set; }
        string hoverColor { get; set; }
        string disabledColor { get; set; }
        string fontFamily { get; set; }
        string fontSize { get; set; }

        public bool isHovered { get; set; }
        public bool isDisabled { get; set; }

        public TextElement(float x, float y, float rotation, string text, string fontColor, string hoverColor, string fontFamily, string fontSize) : base(x, y, rotation)
        {
            position = new Vector2(x, y);
            rotation = rotation;
            this.text = text;
            this.fontColor = fontColor;
            this.hoverColor = hoverColor;
            this.fontFamily = fontFamily;
            this.fontSize = fontSize;
            this.disabledColor = "#818589";
        }

        public void UpdateMessage(string message)
        {
            text = message;
        }

        public async override Task Render()
        {
            if (isDisabled)
            {
                await CanvasController.context.SetFillStyleAsync(disabledColor);
            }
            else if (isHovered)
            {
                await CanvasController.context.SetFillStyleAsync(hoverColor);
            }
            else
            {
                await CanvasController.context.SetFillStyleAsync(fontColor);
            }

            await CanvasController.context.SetTextAlignAsync(TextAlign.Center);
            await CanvasController.context.SetFontAsync(fontSize + " " + fontFamily);
            await CanvasController.context.FillTextAsync(text, position.X, position.Y);
        }
    }
}
