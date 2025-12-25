using System.ComponentModel.Design;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.WorldObjects
{
    public class Projectile : WorldObject
    {
        public enum ProjSource { PLAYER, ENEMY }

        public ProjSource currentSource = ProjSource.PLAYER;

        private Dungeon dungeonReference { get; set; }

        string audioPath { get; set; }
        private int damage {  get; set; }
        private int absorb { get; set; }
        private int stunFactor {  get; set; }
        private int fearFactor { get; set; }
        private int burnFactor { get; set; }
        private float rotation { get; set; }

        private int woLayer = 1;

        //Animation handling
        private float animStep = 3f;
        private float animIndex { get; set; } = 0;
        private int animRow { get; set; } = 0;
        const float spriteSheetCols = 4;

        public Projectile(Vector2 position, ProjSource projSource, int damage, int absorb, int stunFactor, int fearFactor, int burnFactor, float rotation, Dungeon dungeonRef, string audioPath) : base(position, ObjectType.PROJECTILE)
        {
            currentSource = projSource;
            this.damage = damage;
            this.absorb = absorb;
            this.stunFactor = stunFactor;
            this.fearFactor = fearFactor;
            this.burnFactor = burnFactor;
            this.rotation = rotation;
            this.dungeonReference = dungeonRef;
            this.audioPath = audioPath;
        }

        private void Move()
        {
            double rotRadians = rotation * Math.PI / 180;

            float dirX = -1 * (float)Math.Floor(Math.Cos(rotRadians));
            float dirY = -1 * (float)Math.Floor(Math.Sin(rotRadians));

            Vector2 direction = new Vector2(dirX, dirY);
            Vector2 newPosition;

            if (dungeonReference.TestGridSpaceEmpty((position / dungeonReference.map.gridSize) + direction))
            {
                newPosition = dungeonReference.MoveWorldObject(this, position, direction);

                if (woLayer == 2)
                {
                    dungeonReference.RemoveProjectileFromLayerTwo(this, position);
                    woLayer = 1;
                }
            }
            else
            {
                WorldObject objectBlockingPath = dungeonReference.GetObjectAtGridPos(position, direction);

                if (objectBlockingPath.objType == ObjectType.PROJECTILE)
                {
                    newPosition = dungeonReference.MoveProjectileLayerTwo(this, position, direction);
                    dungeonReference.RemoveProjectileFromLayerTwo(this, position);
                    

                    if (woLayer == 2)
                    {
                        dungeonReference.RemoveProjectileFromLayerTwo(this, position);
                    }

                    woLayer = 2;
                }

                else if (objectBlockingPath.objType == ObjectType.ENEMY && currentSource == ProjSource.PLAYER)
                {
                    objectBlockingPath.ResolveDamage(damage, stunFactor, fearFactor, burnFactor);

                    if (absorb > 0) //Heal player with absorb spells
                    {
                        dungeonReference.player.ResolveHealing(absorb);
                    }

                    //Audio handling
                    Player player = dungeonReference.player;
                    Vector2 playerPosition = player.position;

                    float distance = Vector2.Distance(playerPosition, position);
                    float distanceRate = distance / 1000;

                    distanceRate = Math.Clamp(distanceRate, 0f, 1f);

                    float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                    AudioController.PlaySound(audioPath, effectsVolume * distanceRate, false);                    

                    animRow = 1;
                    newPosition = position;
                }

                else if (objectBlockingPath.objType == ObjectType.PLAYER && currentSource == ProjSource.ENEMY)
                {
                    objectBlockingPath.ResolveDamage(damage, stunFactor, fearFactor, burnFactor);

                    //Audio handling
                    float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                    AudioController.PlaySound(audioPath, effectsVolume, false);

                    animRow = 1;
                    newPosition = position;
                }

                else //Projectile colliding with world object/wall/enemy/etc.
                {
                    //Audio handling
                    Player player = dungeonReference.player;
                    Vector2 playerPosition = player.position;

                    float distance = Vector2.Distance(playerPosition, position);
                    float distanceRate = distance / 1000;

                    distanceRate = Math.Clamp(distanceRate, 0f, 1f);

                    float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                    AudioController.PlaySound(audioPath, effectsVolume * distanceRate, false);

                    animRow = 1;
                    newPosition = position;
                }
                
            }            

            UpdatePosition(newPosition);
        }

        public int GetDamage()
        {
            return damage;
        }

        private void HandleAnimations(float deltaTime)
        {
            animIndex += (animStep * deltaTime);
            
            if (animIndex > spriteSheetCols - 1 && animRow == 0)
            {
                animIndex = 0;
            }

            else if (animIndex > spriteSheetCols - 1 && animRow == 1)
            {
                Vector2 gridPos = new Vector2(
                    (float)Math.Floor(position.X / dungeonReference.map.gridSize),
                    (float)Math.Floor(position.Y / dungeonReference.map.gridSize));

                dungeonReference.RemoveProjectileFromDungeon(this, gridPos);
            }
        }

        public void Update(float deltaTime)
        {
            HandleAnimations(deltaTime);
        }

        public void EnvironmentUpdate()
        {
            for (int i = 0; i < 2; i++)
            {
                Move();
            }
        }

        public async override Task Render()
        {
            await RenderingController.Draw(sprite.image, (int)animIndex, animRow, sprite.dimensions, castedDimensions * sprite.scale, castedPosition);
                       
            rayIndices.Clear();
            indexTODistance.Clear();
        }

        public async override Task RenderShadows()
        {
            float brightness = CalculateBrightness();

            await CanvasController.context.SetGlobalAlphaAsync(brightness);
            await RenderingController.Draw(sprite.shadowImage, (int)animIndex, animRow, sprite.dimensions, castedDimensions * sprite.scale, castedPosition);
            await CanvasController.context.SetGlobalAlphaAsync(1f);
        }
    }
}
