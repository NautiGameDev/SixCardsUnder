using System.Numerics;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class Button : UIElement
    {
        TextElement buttonText {  get; set; }
        Sprite background {  get; set; }
        Sprite hoveredBackground { get; set; }

        bool isHovered { get; set; } = false;
        bool isDisabled { get; set; } = false;
        Action callbackAction { get; set; }

        public Button(float x, float y, float rotation, string buttonText, Action callback) : base(x, y, rotation)
        {            
            this.background = new Sprite();
            this.background.SetImage("Assets/UI/Button.png");
            this.background.SetDimensions(new Vector2(88, 13));
            this.background.SetScale(new Vector2(4f, 4f));

            this.hoveredBackground = new Sprite();
            this.hoveredBackground.SetImage("Assets/UI/Button_Hovered.png");
            this.hoveredBackground.SetDimensions(new Vector2(88, 13));
            this.hoveredBackground.SetScale(new Vector2(4f, 4f));

            //Center button on position
            float centerX = position.X - ((background.dimensions.X * background.scale.X) / 2);
            float centerY = position.Y - ((background.dimensions.Y * background.scale.Y) / 2);
            position = new Vector2(centerX, centerY);

            float textXPos = (this.background.dimensions.X * this.background.scale.X) / 2 + position.X;
            float textYPos = (this.background.dimensions.Y * this.background.scale.Y) / 2 + position.Y + 10;
            this.buttonText = new TextElement(textXPos, textYPos, 0, buttonText, "#a88d75", "#a88d75", "Elv Pixel", "28px");

            callbackAction = callback;
        }

        public void SetButtonDisabled(bool value)
        {
            isDisabled = value;
            buttonText.isDisabled = value;
        }

        private void TestMouseHover()
        {
            Vector2 mousePos = InputController.GetMousePosition();
            

            if (mousePos.X > position.X &&
                mousePos.X < position.X + background.dimensions.X * background.scale.X &&
                mousePos.Y > position.Y &&
                mousePos.Y < position.Y + background.dimensions.Y * background.scale.Y)
            {
                isHovered = true;
                buttonText.isHovered = true;
            }
            else
            {
                isHovered = false;
                buttonText.isHovered = false;
            }

            
        }

        private void TestMouseClicked()
        {
            if (InputController.OnMouseDown() && !isDisabled)
            {
                callbackAction?.Invoke();
            }
        }

        private void TestTouch()
        {
            Vector2 touchPos = InputController.GetTouchPosition();
            if (touchPos.X > position.X &&
                touchPos.X < position.X + background.dimensions.X * background.scale.X &&
                touchPos.Y > position.Y &&
                touchPos.Y < position.Y + background.dimensions.Y * background.scale.Y &&
                InputController.OnTouchDown())
            {
                callbackAction?.Invoke();
            }
        }

        public async override Task Render()
        {
            if (!isDisabled)
            {
                TestMouseHover();
                TestTouch();
            }
            

            if (isHovered)
            {
                TestMouseClicked();
                await RenderingController.Draw(hoveredBackground.image, position, hoveredBackground.dimensions * hoveredBackground.scale);
            }
            else
            {
                await RenderingController.Draw(background.image, position, background.dimensions * background.scale);
            }

            await buttonText.Render();            
        }
    }
}
