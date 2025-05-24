
using System.Numerics;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SeaLegs.Controllers;
using SeaLegs.Data;

namespace SeaLegs.Pages
{
    public class SLDisplayComponent : ComponentBase, IDisposable
    {
        //Core set-up
        public BECanvasComponent? canvas { get; set; }
        private Canvas2DContext? context { get; set; }
        private IJSObjectReference? JSModule {  get; set; }
        [Inject]
        public IJSRuntime? JsRuntime { get; set; }


        //Canvas dimensions
        [Parameter]
        public long Width { get; set; } = 0;
        [Parameter]
        public long Height { get; set; } = 0;
        public string? wrapperWidth { get; set; }
        public string? wrapperHeight { get; set; }

        private int newWidth { get; set; } = 1;
        private int newHeight { get; set; } = 1;

        [Parameter]
        public int AspectRatioWidth { get; set; } = 16;
        [Parameter]
        public int AspectRatioHeight { get; set; } = 9;

        private DotNetObjectReference<SLDisplayComponent>? dotNetReference { get; set; }

        //Gameplay Loop
        private System.Threading.Timer? gameTimer { get; set; }
        private DateTime lastFrameTime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && JsRuntime != null)
            {
                //Set-up key components ---------
                JSModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./SeaLegs/Pages/SLDisplay.razor.js");
                context = await canvas.CreateCanvas2DAsync();
                
                

                //Canvas resizing set-up ---------
                //If dimensions aren't defined in razor page, calculate dimensions according to aspect ratio
                if (Width == 0 && Height == 0)
                {
                    await UpdateCanvasDimensions();
                    StateHasChanged();

                    dotNetReference = DotNetObjectReference.Create(this);
                    await JSModule.InvokeVoidAsync("setupResizeHandler", dotNetReference);
                }
                else
                {
                    wrapperWidth = $"{Width}px";
                    wrapperHeight = $"{Height}px";
                }

                //Disable image smoothing if game is pixel art
                if (GameSettings.isPixelArt)
                {
                    await JSModule.InvokeVoidAsync("disableImageSmoothing");
                }

                await JSModule.InvokeVoidAsync("setupDisplay");
                await UpdateCanvasScaling();
                dotNetReference = DotNetObjectReference.Create(this);
                await JSModule.InvokeVoidAsync("setupResizeHandler", dotNetReference);


                //Transfer components to Lib system classes as needed
                SendComponentsToLib();
                SendDimensionsToLib();

                //Set-up gameplay loop
                lastFrameTime = DateTime.Now;
                gameTimer = new System.Threading.Timer(GameLoop, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
            }
        }

        #region Canvas Sizing
        [JSInvokable]
        public async Task OnBrowserResize()
        {
            await UpdateCanvasScaling();
        }

        private async Task UpdateCanvasDimensions()
        {
            if(AspectRatioWidth != 0 && AspectRatioHeight != 0 && JSModule != null)
            {
                var dimensions = await JSModule.InvokeAsync<BrowserDimensions>("getBrowserDimensions");
                             

                if (AspectRatioWidth > AspectRatioHeight)
                {
                    double horizontalScale = dimensions.width / AspectRatioWidth;
                    Width = (long)dimensions.width;
                    Height = (long)horizontalScale * AspectRatioHeight;
                }
                else
                {
                    double verticleScale = dimensions.height / AspectRatioHeight;
                    Height = (long)dimensions.height;
                    Width = (long)verticleScale * AspectRatioWidth;
                }

                wrapperWidth = $"{Width}px";
                wrapperHeight = $"{Height}px";
            }
        }

        private async Task UpdateCanvasScaling()
        {
            var dimensions = await JSModule.InvokeAsync<BrowserDimensions>("getBrowserDimensions");
            newWidth = (int)dimensions.width;
            newHeight = (int)dimensions.height;
        }
        #endregion

        #region Data Transfer
        private void SendComponentsToLib()
        {
            if (canvas != null && context != null && JSModule != null)
            {
                CanvasController.SetCoreComponents(canvas, context, JSModule);
            }
        }

        private void SendDimensionsToLib()
        {
            CanvasController.SetDimensions(canvas.Width, canvas.Height);
        }

        public void HandleKeyDown(KeyboardEventArgs args)
        {
            InputController.SetKeyPressed(args.Key, true);
            InputController.AddKeyDown(args.Key);
        }

        public void HandleKeyUp(KeyboardEventArgs args)
        {
            InputController.AddKeyUp(args.Key);
        }

        public void HandleMousePosition(MouseEventArgs args)
        {

            float xScale = (float)newWidth / 1080;
            float yScale = (float)newHeight / 720;

            Vector2 mousePos = new Vector2((float)args.ClientX / xScale, (float)args.ClientY / yScale);
            //Vector2 mousePos = new Vector2((float)args.ClientX, (float)args.ClientY);
            InputController.SetMousePosition(mousePos);
        }

        public void HandleMouseDown(MouseEventArgs args)
        {            
            InputController.SetMouseDown(true);
            InputController.SetMouseUp(false);
        }

        public void HandleMouseUp(MouseEventArgs args)
        {
            InputController.SetMouseUp(true);
            InputController.SetMouseDown(false);
        }

        #endregion

        #region Game Loop
        //To set-up game, call root game class Update method from CanvasController
        private async void GameLoop(object state)
        {
            var currentTime = DateTime.Now;
            var deltaTime = (float)(currentTime - lastFrameTime).TotalSeconds;
            lastFrameTime = currentTime;

            await RunGame(deltaTime);
        }

        private async Task RunGame(float deltaTime)
        {
            await CanvasController.Update(deltaTime);
            await JSModule.InvokeVoidAsync("renderDisplayCanvas");
        }

        public void Dispose()
        {
            if (dotNetReference != null && JSModule != null)
            {
                JSModule.InvokeVoidAsync("removeResizeHandler");
                dotNetReference.Dispose();
            }
        }
        #endregion

        //BrowserDimensions class is used to return size of browser from JSInterop
        class BrowserDimensions
        {
            public double width {  get; set; }
            public double height { get; set; }
        }
    }
}
