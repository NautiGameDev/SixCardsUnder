using System.Numerics;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class ControlsCanvas : UICanvas
    {        

        InfoBox header { get; set; }
        Sprite canvasFrame { get; set; }

        Button moveForwardButton { get; set; }
        TextElement moveForwardText { get; set; }
        Button moveBackwardButton { get; set; }
        TextElement moveBackwardText { get; set; }
        Button turnLeftButton { get; set; }
        TextElement turnLeftText { get; set; }
        Button turnRightButton { get; set; }
        TextElement turnRightText { get; set; }
        Button strafeLeftButton { get; set; }
        TextElement strafeLeftText { get; set; }
        Button strafeRightButton { get; set; }
        TextElement strafeRightText { get; set; }
        Button cardInteractButton { get; set; }
        TextElement cardInteractText { get; set; }
        Button pauseMenuButton { get; set; }
        TextElement pauseMenutText { get; set; }
        Button mapButton { get; set; }
        TextElement mapText { get; set; }


        public ControlsCanvas()
        {            
            LoadGraphics();
        }

        public void LoadGraphics()
        {
            canvasFrame = new Sprite();
            canvasFrame.SetImage("Assets/UI/Map_Border.png");
            canvasFrame.SetDimensions(new Vector2(80, 64));
            canvasFrame.SetScale(new Vector2(8.5f, 8.5f));

            header = new InfoBox((float)CanvasController.width / 2, 50, 0, "Controls");

            float xIncrement = (float)CanvasController.width / 6 + 25; //Horizontal spacing between controls
            float yIncrement = (float)CanvasController.height / 14; //Vert spacing between controls
            float textOffsetX = 100;
            float yOffset = 100;

            moveForwardButton = new Button(xIncrement * 2, yIncrement + yOffset, 0, "W", null);
            moveForwardButton.SetButtonDisabled(true);
            moveForwardText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement + yOffset, 0, "Move Forward", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            moveBackwardButton = new Button(xIncrement * 2, yIncrement * 2 + yOffset, 0, "S", null);
            moveBackwardButton.SetButtonDisabled(true);
            moveBackwardText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 2 + yOffset, 0, "Move Backward", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            turnLeftButton = new Button(xIncrement * 2, yIncrement * 3 + yOffset, 0, "Q", null);
            turnLeftButton.SetButtonDisabled(true);
            turnLeftText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 3 + yOffset, 0, "Turn Left", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            turnRightButton = new Button(xIncrement * 2, yIncrement * 4 + yOffset, 0, "E", null);
            turnRightButton.SetButtonDisabled(true);
            turnRightText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 4 + yOffset, 0, "Turn Right", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            strafeLeftButton = new Button(xIncrement * 2, yIncrement * 5 + yOffset, 0, "A", null);
            strafeLeftButton.SetButtonDisabled(true);
            strafeLeftText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 5 + yOffset, 0, "Strafe Left", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            strafeRightButton = new Button(xIncrement * 2, yIncrement * 6 + yOffset, 0, "D", null);
            strafeRightButton.SetButtonDisabled(true);
            strafeRightText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 6 + yOffset, 0, "Strafe Right", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            cardInteractButton = new Button(xIncrement * 2, yIncrement * 7 + yOffset, 0, "Left Mouse", null);
            cardInteractButton.SetButtonDisabled(true);
            cardInteractText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 7 + yOffset, 0, "Card Interact", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            pauseMenuButton = new Button(xIncrement * 2, yIncrement * 8 + yOffset, 0, "Esc", null);
            pauseMenuButton.SetButtonDisabled(true);
            pauseMenutText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 8 + yOffset, 0, "Pause Menu", "#a88d75", "#a88d75", "Elv Pixel", "24px");

            mapButton = new Button(xIncrement * 2, yIncrement * 9 + yOffset, 0, "M", null);
            mapButton.SetButtonDisabled(true);
            mapText = new TextElement(xIncrement * 3 + textOffsetX, yIncrement * 9 + yOffset, 0, "Map", "#a88d75", "#a88d75", "Elv Pixel", "24px");
        }

        public async Task Render()
        {
            await header.Render();

            float frameX = (float)(CanvasController.width / 2) - (canvasFrame.dimensions.X * canvasFrame.scale.X) / 2;
            float frameY = (float)(CanvasController.height / 2) - (canvasFrame.dimensions.Y * canvasFrame.scale.Y) / 2;
            Vector2 framePos = new Vector2(frameX, frameY);

            await RenderingController.DrawRectangles("rgba(0, 0, 0, 0.75", frameX, frameY, canvasFrame.dimensions.X * canvasFrame.scale.X, canvasFrame.dimensions.Y * canvasFrame.scale.Y);
            await RenderingController.Draw(canvasFrame.image, framePos, canvasFrame.dimensions * canvasFrame.scale);

            await moveForwardButton.Render();
            await moveForwardText.Render();
            await moveBackwardButton.Render();
            await moveBackwardText.Render();
            await turnLeftButton.Render();
            await turnLeftText.Render();
            await turnRightButton.Render();
            await turnRightText.Render();
            await strafeLeftButton.Render();
            await strafeLeftText.Render();
            await strafeRightButton.Render();
            await strafeRightText.Render();
            await cardInteractButton.Render();
            await cardInteractText.Render();
            await pauseMenuButton.Render();
            await pauseMenutText.Render();
            await mapButton.Render();
            await mapText.Render();


            


        }
    }
}
