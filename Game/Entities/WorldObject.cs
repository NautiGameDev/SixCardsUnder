using System.Numerics;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;
using SeaLegs.Data;

namespace PixelArtGameJam.Game.Entities
{
    public class WorldObject : Entity
    {
        public enum ObjectType { AIR, PLAYER, WALL, WALLSTART, WALLFINISH, ENEMY, ENT, PROJECTILE, COLLECTABLE }
        public ObjectType objType { get; set; } = ObjectType.AIR;

        public Sprite sprite {  get; set; }

        //Rendering variables
        public Vector2 castedPosition { get; private set; }
        public Vector2 castedDimensions { get; private set;}
        public double castedDistance { get; set; }
        protected double verticalOffset { get; set; } = 0;

        //Passed references
        private int maxDepth { get; set; } = 1500;

        //Used to find the center raycast to position sprite in world
        protected List<int> rayIndices { get; set; } = new List<int>();
        protected Dictionary<int, double> indexTODistance { get; set; } = new Dictionary<int, double>();

        protected WorldObject(Vector2 position, ObjectType objType) : base(position)
        {
            sprite = new Sprite();
            this.objType = objType;
        }

        public void SetVerticalOffset(double offset)
        {
            verticalOffset = offset;
        }

        public void UpdateRenderingData(double hitDistance, int rayIndex, double rayGap, double FOV, int gridSize, int maxDepth)
        {
            

            if (!rayIndices.Contains(rayIndex))
            {
                rayIndices.Add(rayIndex);
                indexTODistance[rayIndex] = hitDistance;
            }

            this.maxDepth = maxDepth;

            int indexToUse = (rayIndices.FirstOrDefault() + rayIndices.Last()) / 2;
            double distanceToUse = indexTODistance[indexToUse];
            castedDistance = distanceToUse;

            double spriteHeight = CalculateSpriteHeight(distanceToUse, FOV, gridSize);
            double sizeScale = spriteHeight / sprite.dimensions.Y;
            float spriteWidth = sprite.dimensions.X * (float)sizeScale;
            castedDimensions = new Vector2(spriteWidth, (float)spriteHeight);
           
            double xPos = indexToUse * rayGap - spriteWidth / 2;
            double yPos = (CanvasController.height / 2) - (spriteHeight/2) + (verticalOffset * sizeScale); //Might need to implement yoffset for each object
            castedPosition = new Vector2((float)xPos, (float)yPos);
        }

        public double CalculateSpriteHeight(double distance, double FOV, int gridSize)
        {
            double projectionDistance = (CanvasController.height / 2) / Math.Tan(FOV / 2);
            double spriteHeightOnScreen = (gridSize / distance) * projectionDistance;
            double maxSpriteHeight = CanvasController.height * 2;
            if (spriteHeightOnScreen > maxSpriteHeight)
            {
                spriteHeightOnScreen = maxSpriteHeight;
            }

            return spriteHeightOnScreen;
        }

        protected float CalculateBrightness()
        {
            float brightness = 1f - (float)(castedDistance / maxDepth);
            brightness = Math.Clamp(brightness, 0f, 1f);
            float adjustedBrightness = (1f - brightness) * 1.5f; // <-- Brightness scale in settings

            return adjustedBrightness;
        }

        public virtual async Task Render()
        {
            await RenderingController.DrawNoScale(sprite.image, castedPosition, castedDimensions);

            float brightness = CalculateBrightness();

            await CanvasController.context.SetGlobalAlphaAsync(brightness);
            await RenderingController.DrawNoScale(sprite.shadowImage, castedPosition, castedDimensions);
            await CanvasController.context.SetGlobalAlphaAsync(1f);

            //Reset arrays for next calculation
            rayIndices.Clear();
            indexTODistance.Clear();
        }

        public virtual async Task RenderShadows()
        {
            
        }
        
        public virtual void ResolveDamage(int damage, int stunFactor, int fearFactor, int burnFactor)
        {

        }

    }
}
