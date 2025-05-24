using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class Minimap : UIElement
    {
        Sprite borderSprite {  get; set; }
        Dungeon dungeonRef {  get; set; }
        protected WorldObject[][] worldObjects { get; set; }

        public Minimap(float x, float y, float rotation, Dungeon dungeonRef) : base(x, y, rotation)
        {
            position = new Vector2(x, y);
            this.dungeonRef = dungeonRef;
            this.worldObjects = dungeonRef.worldObjects;

            borderSprite = new Sprite();
            borderSprite.SetImage("Assets/UI/Minimap_Border.png");
            borderSprite.SetDimensions(new Vector2(32, 32));
            borderSprite.SetScale(new Vector2(6f, 6f));
        }

        public async Task DrawWalls()
        {
            Player player = dungeonRef.player;
            Vector2 playerWorldPosition = player.position;
            Vector2 playerGridPosition = playerWorldPosition / dungeonRef.map.gridSize;

            int startX = (int)(playerGridPosition.X - 12);
            int startY = (int)(playerGridPosition.Y - 12);

            for (int Y = startY; Y < (startY + 24); Y++)
            {
                for (int X = startX; X < (startX + 24); X++)
                {
                    int mapX = (X - startX) * 8;
                    int mapY = (Y - startY) * 8;

                    float rectX = position.X + mapX;
                    float rectY = position.Y + mapY;

                    try
                    {
                        if (worldObjects[Y][X] != null)
                        {
                            if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALL)
                            {                                
                                await RenderingController.DrawRectangles("White", rectX, rectY, 8, 8);
                                
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALLSTART)
                            {
                                await RenderingController.DrawRectangles("Gold", rectX, rectY, 8, 8);
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALLFINISH)
                            {
                                await RenderingController.DrawRectangles("Magenta", rectX, rectY, 8, 8);
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.PLAYER)
                            {
                                await RenderingController.DrawRectangles("Blue", rectX, rectY, 8, 8);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }
        }

        public async override Task Render()
        {
            await RenderingController.DrawRectangles("black", position.X, position.Y, 192, 192);

            await DrawWalls();
            await RenderingController.Draw(borderSprite.image, position, borderSprite.dimensions * borderSprite.scale);
        }
    }
}
