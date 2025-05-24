using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class DungeonMap : UIElement
    {
        Sprite mapBorder { get; set; }
        Sprite background { get; set; }
        Sprite character { get; set; }
        Sprite floorBG { get; set; }

        TextElement floorText { get; set; }
        
        private Dungeon dungeonRef { get; set; }

        public DungeonMap(float x, float y, float rotation, Dungeon dungeonRef) : base(x, y, rotation)
        {
            this.dungeonRef = dungeonRef;

            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            mapBorder = new Sprite();
            mapBorder.SetImage("Assets/UI/Map_Border.png");
            mapBorder.SetDimensions(new Vector2(64, 64));
            mapBorder.SetScale(new Vector2(9, 9));

            character = new Sprite();
            character.SetImage("Assets/UI/Character_Elf.png");
            character.SetDimensions(new Vector2(91, 289));
            character.SetScale(new Vector2(3f, 3f));

            floorBG = new Sprite();
            floorBG.SetImage("Assets/UI/InfoBox.png");
            floorBG.SetDimensions(new Vector2(180, 48));
            floorBG.SetScale(new Vector2(2f, 2f));

            int dungeonFloor = dungeonRef.floor;
            string floorMsg = "Floor: -" + dungeonFloor;
            floorText = new TextElement((int)(CanvasController.width / 2), 60, 0, floorMsg, "#a88d75", "#ffffff", "Elv Pixel", "36px");
        }

        public async Task DrawWalls()
        {
            const int cellSize = 4;

            WorldObject[][] worldObjects = dungeonRef.worldObjects;

            int startX = (int)((CanvasController.width) / 2 - (worldObjects[0].Length * cellSize) / 2) + 10;
            int startY = (int)((CanvasController.height) / 2 - (worldObjects.Length * cellSize) / 2) + 10;

            Vector2 startDoorPos = Vector2.Zero;
            Vector2 finishDoorPos = Vector2.Zero;
            Vector2 playerPos = Vector2.Zero;

            bool startDoorSeen = false;
            bool finishDoorSeen = false;

            for (int Y = 0; Y < dungeonRef.worldObjects.Length; Y++)
            {
                for (int X = 0; X < dungeonRef.worldObjects[0].Length; X++)
                {
                    float rectX = startX + (X * cellSize);
                    float rectY = startY + (Y * cellSize);

                    
                        if (worldObjects[Y][X] != null)
                        {
                            if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALL)
                            {

                                Wall wallObj = (Wall)worldObjects[Y][X];
                                if (wallObj.hasBeenSeen)
                                {
                                    await RenderingController.DrawRectangles("White", rectX, rectY, cellSize, cellSize);
                                }
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALLSTART)
                            {
                                Wall wallObj = (Wall)worldObjects[Y][X];
                                if (wallObj.hasBeenSeen)
                                {
                                    startDoorSeen = true;
                                    startDoorPos = new Vector2(rectX, rectY);
                                }
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.WALLFINISH)
                            {
                                Wall wallObj = (Wall)worldObjects[Y][X];
                                if (wallObj.hasBeenSeen)
                                {
                                    finishDoorSeen = true;
                                    finishDoorPos = new Vector2(rectX, rectY);
                                }
                            }
                            else if (worldObjects[Y][X].objType == WorldObject.ObjectType.PLAYER)
                            {
                                playerPos = new Vector2(rectX, rectY);


                            }
                        }
                    


                }
            }

            if (startDoorSeen)
            {
                await RenderingController.DrawRectangles("Gold", startDoorPos.X, startDoorPos.Y, cellSize * 2, cellSize * 2);
            }

            if (finishDoorSeen)
            {
                await RenderingController.DrawRectangles("Green", finishDoorPos.X, finishDoorPos.Y, cellSize * 2, cellSize * 2);
            }


            await RenderingController.DrawRectangles("Blue", playerPos.X, playerPos.Y, cellSize * 2, cellSize * 2);
        }

        public async override Task Render()
        {
            //Background image
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);

            

            //Map BG and Walls
            float borderX = (float)((CanvasController.width) / 2 - (mapBorder.dimensions.X * mapBorder.scale.X) / 2);
            float borderY = (float)((CanvasController.height) / 2 - (mapBorder.dimensions.Y * mapBorder.scale.Y) / 2);
                        
            await RenderingController.DrawRectangles("rgba(0,0,0,0.7", borderX, borderY, 576, 576);

            try
            {
                await DrawWalls();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Rendering map stuff");

            //Map Border
            Vector2 borderPos = new Vector2(borderX, borderY);
            await RenderingController.Draw(mapBorder.image, borderPos, mapBorder.dimensions * mapBorder.scale);

            //Floor info box background
            float fBGX = (float)((CanvasController.width / 2) - (floorBG.dimensions.X * floorBG.scale.X) / 2);
            float fBGY = 0f;
            Vector2 fBGPos = new Vector2(fBGX, fBGY);
            await RenderingController.Draw(floorBG.image, fBGPos, floorBG.dimensions * floorBG.scale);

            await floorText.Render();

            //Elf Character
            await RenderingController.Draw(character.image, new Vector2(10, 10), character.dimensions * character.scale);
        }
    }
}
