using System.Numerics;
using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;
using SeaLegs.Data;

namespace PixelArtGameJam.Game.Components
{
    public class Raycast
    {
        private int index { get; set; }
        private double rayAngle { get; set; }
        private double rayAngleRadians { get; set; }
        private float xStep { get; set; }
        private float yStep { get; set; }
        private WorldObject[][] worldObjects { get; set; }
        private WorldObject[][] woLayerTwo { get; set; }
        private List<WorldObject> objectsToRender { get; set; }
        private double FOV { get; set; }
        private int numRays { get; set; }
        private int maxDepth { get; set; }
        private int gridSize { get; set; }

        private WallStrip strip { get; set; }


        public Raycast(int index, WorldObject[][] worldObjects, WorldObject[][] woLayerTwo, List<WorldObject> objectsToRender, double FOV, int numRays, int maxDepth, int gridSize)
        {
            this.index = index;
            this.worldObjects = worldObjects;
            this.woLayerTwo = woLayerTwo;
            this.objectsToRender = objectsToRender;
            this.FOV = FOV;
            this.numRays = numRays;
            this.maxDepth = maxDepth;
            this.gridSize = gridSize;

            strip = new WallStrip(this.FOV, this.gridSize, this.maxDepth);
        }

        //Method to update angle when player rotates
        public void UpdateRayAngle(float playerRotationDegrees)
        {
            double deltaAngle = FOV / numRays;
            rayAngle = ((playerRotationDegrees - (FOV / 2)) + deltaAngle * index);
            rayAngleRadians = rayAngle * Math.PI / 180;
            xStep = -(float)Math.Cos(rayAngleRadians);
            yStep = -(float)Math.Sin(rayAngleRadians);
        }

        public async Task Cast(float playerRotationDegrees, Vector2 playerPosition)
        {
            double playerRotRads = playerRotationDegrees * 180 / Math.PI;
            
            double dx = Math.Round(Math.Cos(playerRotRads));
            double dy = Math.Round(Math.Sin(playerRotRads));

            float xPos = playerPosition.X;
            float yPos = playerPosition.Y;

            //Console.WriteLine(xPos / gridSize + " " + yPos / gridSize);

            for (int depth = 0; depth < maxDepth; depth++)
            {
                int gridX = (int)Math.Floor(xPos / gridSize);
                int gridY = (int)Math.Floor(yPos / gridSize);

                if (gridX >= 0 && gridX < worldObjects[0].Length && gridY >= 0 && gridY < worldObjects.Length)
                {

                    if (woLayerTwo[gridY][gridX] != null)
                    {
                        WorldObject currentObject = woLayerTwo[gridY][gridX];

                        Vector2 hitPosition = new Vector2(xPos, yPos);
                        Vector2 hitGridPosition = new Vector2(gridX, gridY);

                        double angleDifference = rayAngleRadians - (playerRotationDegrees * Math.PI / 180);

                        while (angleDifference > Math.PI) angleDifference -= 2 * Math.PI;
                        while (angleDifference < -Math.PI) angleDifference += 2 * Math.PI;

                        double hitDistance = Vector2.Distance(playerPosition, hitPosition) * (double)Math.Cos(angleDifference);

                        double sliceWidth = (double)(CanvasController.width / numRays);

                        currentObject.UpdateRenderingData(hitDistance, index, sliceWidth, FOV, gridSize, maxDepth);

                        if (!objectsToRender.Contains(currentObject))
                        {
                            objectsToRender.Add(currentObject);
                            Console.WriteLine($"Added {currentObject} to objects to render list.");
                        }
                        
                    }


                    if (worldObjects[gridY][gridX] != null)
                    {
                        WorldObject currentObject = worldObjects[gridY][gridX];

                        Vector2 hitPosition = new Vector2(xPos, yPos);
                        Vector2 hitGridPosition = new Vector2(gridX, gridY);

                        double angleDifference = rayAngleRadians - (playerRotationDegrees * Math.PI / 180);

                        while (angleDifference > Math.PI) angleDifference -= 2 * Math.PI;
                        while (angleDifference < -Math.PI) angleDifference += 2 * Math.PI;

                        double hitDistance = Vector2.Distance(playerPosition, hitPosition) * (double)Math.Cos(angleDifference);

                        double sliceWidth = (double)(CanvasController.width / numRays);

                        if (currentObject.objType == WorldObject.ObjectType.ENT ||
                            currentObject.objType == WorldObject.ObjectType.PROJECTILE ||
                            currentObject.objType == WorldObject.ObjectType.ENEMY ||
                            currentObject.objType == WorldObject.ObjectType.COLLECTABLE)
                        {
                            currentObject.UpdateRenderingData(hitDistance, index, sliceWidth, FOV, gridSize, maxDepth);

                            if (!objectsToRender.Contains(currentObject))
                            {
                                objectsToRender.Add(currentObject);
                                
                            }
                        }
                        if (currentObject.objType == WorldObject.ObjectType.WALL ||
                            currentObject.objType == WorldObject.ObjectType.WALLSTART ||
                            currentObject.objType == WorldObject.ObjectType.WALLFINISH)
                        {
                            ElementReference wallSprite = currentObject.sprite.GetImage();
                            Vector2 spriteDimensions = currentObject.sprite.GetDimensions();

                            Wall wallAtPosition = (Wall)worldObjects[gridY][gridX];
                            wallAtPosition.MarkHasBeenSeen();

                            strip.SetWallData(wallSprite, spriteDimensions, hitPosition, hitDistance, sliceWidth, index);
                            objectsToRender.Add(strip);

                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
                    xPos += xStep;
                    yPos += yStep;
                }
            }
        }
    }

