using System.Numerics;
using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Entities;
using SeaLegs.Controllers;
using System.Globalization;


/*This class is attached to each raycast and used for rendering walls*/

namespace PixelArtGameJam.Game.WorldObjects
{
    public class WallStrip : WorldObject
    {
        double FOV { get; set; }
        int maxDepth { get; set; }
        int gridSize { get; set; }

        private ElementReference sprite { get; set; }
        private Vector2 spriteDimensions { get; set; }
        private Vector2 hitPosition { get; set; }

        private double sliceWidth {  get; set; }

        private double wallHeight { get; set; }
        private Vector2 position { get; set; }
        string wallBrightness { get; set; }
        int textureStartPoint { get; set; }

        public WallStrip(double FOV, int gridSize, int maxDepth) : base(Vector2.Zero, ObjectType.WALL)
        {
            this.FOV = FOV;
            this.gridSize = gridSize;
            this.maxDepth = maxDepth;
        }

        public void SetWallData(ElementReference sprite, Vector2 spriteDimensions, Vector2 hitPosition, double hitDistance, double sliceWidth, int rayIndex)
        {
            castedDistance = hitDistance;

            //Data xfer
            this.sprite = sprite;
            this.spriteDimensions = spriteDimensions;
            this.sliceWidth = sliceWidth;

            //Strip calculations
            wallHeight = CalculateWallHeight(hitDistance);
            double xPos = rayIndex * sliceWidth;
            double yPos = (CanvasController.height / 2) - (wallHeight / 2);
            position = new Vector2((float)xPos, (float)yPos);
            wallBrightness = CalculateWallBrightness(castedDistance);


            //Texture mapping
            Vector2 hitPosGrid = new Vector2(hitPosition.X / gridSize, hitPosition.Y / gridSize);
            int roundedHitPosX = (int)Math.Round(hitPosGrid.X);
            int roundedHitPosY = (int)Math.Round(hitPosGrid.Y);

            double remainingX = Math.Abs(hitPosGrid.X - roundedHitPosX);
            double remainingY = Math.Abs(hitPosGrid.Y - roundedHitPosY);
            double mappingScale = Math.Max(remainingX, remainingY);

            textureStartPoint = (int)(spriteDimensions.X * mappingScale);
            
        }

        private double CalculateWallHeight(double distance)
        {
            double projectionPlaneDistance = (CanvasController.width / 2f) / Math.Tan(FOV / 2);
            double wallHeightOnScreen = (gridSize / distance) * projectionPlaneDistance;
            double maxWallHeight = CanvasController.height * 2;
            if(wallHeightOnScreen > maxWallHeight)
            {
                wallHeightOnScreen = maxWallHeight;
            }
            return wallHeightOnScreen;
        }

        private string CalculateWallBrightness(double distance)
        {
            float brightness = 1f - ((float)distance / maxDepth);
            brightness = Math.Clamp(brightness, 0f, 1f);
            float adjustedBrightness = (1f - brightness) * 1.2f; // <-- Brightness scale in settings

            return $"rgba(0, 0, 0, {adjustedBrightness.ToString(CultureInfo.InvariantCulture)}";
        }

        public override async Task Render()
        {
            await RenderingController.DrawWall(
                sprite,
                textureStartPoint,
                0,
                sliceWidth,
                spriteDimensions.Y,
                position.X,
                position.Y,
                sliceWidth,
                wallHeight);
        }

        public override async Task RenderShadows()
        {
            await CanvasController.context.SetFillStyleAsync(wallBrightness);
            await CanvasController.context.FillRectAsync(position.X, position.Y - 1f, sliceWidth, wallHeight + 2);
        }
    }
}
