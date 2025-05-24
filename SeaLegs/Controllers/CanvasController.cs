using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions;
using Microsoft.JSInterop;
using SeaLegs.Core;
using SeaLegs.Data;
using System.Numerics;
using System.Runtime.CompilerServices;


namespace SeaLegs.Controllers
{
    public class CanvasController : Controller
    {
        public static BECanvasComponent? canvas { get; private set; }
        public static Canvas2DContext? context { get; private set; }
        public static IJSObjectReference? JSModule { get; private set; }

        public static Func<float, Task>? UpdateAction { get; set; }

        public static double width { get; private set; }
        public static double height { get; private set; }
        public static Vector2 scale { get; private set; }
        public static double canvasScale { get; private set; }
        


        public static void SetCoreComponents(BECanvasComponent canvas, Canvas2DContext context, IJSObjectReference JSModule)
        {
            CanvasController.canvas = canvas;
            CanvasController.context = context;
            CanvasController.JSModule = JSModule;

            
        }

        public static void SetDimensions(long width, long height)
        {
            CanvasController.width = width;
            CanvasController.height = height;

            Console.WriteLine($"Canvas Dimensions set: {width} {height}");

            //Scale ensures consistent look and feel regardless of canvas size.
            //Design width/height can be updated in GameSettings class
            //These dimensions should be set to the target dimensions the game was designed around.
            double horizontalScale = width / GameSettings.DesignWidth;
            double verticalScale = height / GameSettings.DesignHeight;
            CanvasController.canvasScale = horizontalScale;
            CanvasController.scale = new Vector2((float)horizontalScale, (float)verticalScale);

            Console.WriteLine($"Scale Calculations: {horizontalScale}, {verticalScale}");

        }

        

        public static void SetCallback(Func<float, Task> callback)
        {
            UpdateAction = callback;
            Console.WriteLine("Setting up callback method");
        }

        public static async Task Update(float deltaTime)
        {
            if (CanvasController.context != null && UpdateAction != null && CanvasController.JSModule != null)
            {
                await RenderingController.DrawBlackBackground();
                await UpdateAction.Invoke(deltaTime);
            }
            
        }
    }
}
