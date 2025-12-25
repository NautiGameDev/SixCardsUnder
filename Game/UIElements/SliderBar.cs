using System.Numerics;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class SliderBar : UIElement
    {
        Action<int> callback { get; set; }
        
        Sprite sliderBarGraphic {  get; set; }
        Sprite sliderBarNotch {  get; set; }
        TextElement valueText { get; set; }

        int value { get; set; }
        double increment { get; set; }

        Dictionary<int, Vector2> valuePoints { get; set; } = new Dictionary<int, Vector2>();
        bool isChanging { get; set; } = false;


        public SliderBar(float x, float y, float rotation, Action<int> callback, int value) : base(x, y, rotation)
        {
            this.value = value;
            this.callback = callback;
            LoadGraphics();
            FillPointsDictionary();
        }

        private void LoadGraphics()
        {
            sliderBarGraphic = new Sprite();
            sliderBarGraphic.SetImage("Assets/UI/Slider_Bar.png");
            sliderBarGraphic.SetDimensions(new Vector2(79, 13));
            sliderBarGraphic.SetScale(new Vector2(6f, 6f));

            sliderBarNotch = new Sprite();
            sliderBarNotch.SetImage("Assets/UI/Slider_Notch.png");
            sliderBarNotch.SetDimensions(new Vector2(13, 9));
            sliderBarNotch.SetScale(new Vector2(6f, 6f));

            valueText = new TextElement(
                position.X + (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2 + 25,
                position.Y + (sliderBarGraphic.dimensions.Y * sliderBarGraphic.scale.Y) / 2 - 25,
                0,
                value.ToString(),
                "#a88d75",
                "#ffffff",
                "Elv Pixel",
                "24px"
                );
        }

        private Vector2 CalculateSliderBarPosition()
        {
            float xPos = position.X - (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2;
            float yPos = position.Y - (sliderBarGraphic.dimensions.Y * sliderBarGraphic.scale.Y) / 2;

            return new Vector2(xPos, yPos);
        }

        private Vector2 CalculateSliderBarNotchPosition()
        {
            float xMinRange = position.X - (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2 + 25;
            float xMaxRange = position.X + (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2 - 150;
            float xIncrements = (xMaxRange - xMinRange) / 10;

            float xPos = xMinRange + (xIncrements * value);
            float yPos = position.Y - (sliderBarNotch.dimensions.Y * sliderBarNotch.scale.Y) / 2;
                        

            return new Vector2(xPos, yPos);
        }

        private void FillPointsDictionary()
        {
            float xMinRange = position.X - (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2 + 25;
            float xMaxRange = position.X + (sliderBarGraphic.dimensions.X * sliderBarGraphic.scale.X) / 2 - 150;
            float xIncrements = (xMaxRange - xMinRange) / 10;
                        
            float yPos = position.Y - 10;// - (sliderBarNotch.dimensions.Y * sliderBarNotch.scale.Y) / 2;


            for (int i = 1; i < 11; i++)
            {
                Vector2 pointPos = new Vector2(xMinRange + xIncrements * i + (sliderBarNotch.dimensions.X * sliderBarNotch.scale.X) / 2, yPos);
                valuePoints[i] = pointPos;
                
            }
        }

        private bool TestClickDown()
        {
            Vector2 mousePos = InputController.GetMousePosition();
            Vector2 touchPos = InputController.GetTouchPosition();
            Vector2 notchPos = CalculateSliderBarNotchPosition();

            if (mousePos.X > notchPos.X &&
                mousePos.X < notchPos.X + (sliderBarNotch.dimensions.X * sliderBarNotch.scale.X) &&
                mousePos.Y > notchPos.Y &&
                mousePos.Y < notchPos.Y + (sliderBarNotch.dimensions.Y * sliderBarNotch.scale.Y) &&
                InputController.OnMouseDown())
            {
                
                return true;
            }
            else if (touchPos.X > notchPos.X &&
                touchPos.X < notchPos.X + (sliderBarNotch.dimensions.X * sliderBarNotch.scale.X) &&
                touchPos.Y > notchPos.Y &&
                touchPos.Y < notchPos.Y + (sliderBarNotch.dimensions.Y * sliderBarNotch.scale.Y) &&
                InputController.OnTouchDown())
            {
                return true;
            }

            return false;
        }

        private void TestChangeValue()
        {
            Vector2 mousePosition = InputController.GetMousePosition();
            Vector2 touchPosition = InputController.GetTouchPosition();
                        

            foreach (int sliderValue in valuePoints.Keys)
            {
                Vector2 valueToTest = valuePoints[sliderValue];

                float mouseDistance = Vector2.Distance(mousePosition, valueToTest);
                float touchDistance = Vector2.Distance(touchPosition, valueToTest);

                float distance = Math.Max(mouseDistance, touchDistance);

                if (distance < 25)
                {
                    value = sliderValue;
                    valueText.UpdateMessage(value.ToString());
                    callback?.Invoke(value);
                    break;
                }
            }
        }

        public async override Task Render()
        {
            if (!isChanging)
            {
                isChanging = TestClickDown();

            }
            else if (isChanging && InputController.OnMouseUp())
            {
                isChanging = false;
            }

            if (isChanging)
            {
                TestChangeValue();
            }

            
            await RenderingController.Draw(sliderBarGraphic.image, CalculateSliderBarPosition(), sliderBarGraphic.dimensions * sliderBarGraphic.scale);
            await RenderingController.Draw(sliderBarNotch.image, CalculateSliderBarNotchPosition(), sliderBarNotch.dimensions * sliderBarNotch.scale);
            await valueText.Render();
        }
    }
}
