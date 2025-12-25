using System.Net.Http.Headers;
using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Abilities
{
    public class Firewall : Ability
    {
        public Firewall(Projectile.ProjSource projSource) : base(projSource)
        {
            sprite.SetImage("Assets/Abilities/Ability_Firewall_Center.png");
            sprite.SetDimensions(new Vector2(256f, 256f));
            sprite.SetScale(new Vector2(2f, 2f));

            cost = 6;
            damage = 10;
            absorb = 0;
            stunFactor = 0;
            fearFactor = 0;
            burnFactor = 0;
        }

        public override void Cast(Dungeon dungeonReference, Vector2 position, float projectileRotation)
        {
            Vector2 worldPosition = new Vector2(
                position.X * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2),
                position.Y * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2));

            Console.WriteLine($"Firewall center: {projectileRotation}");

            Projectile newProjectile = new Projectile(
                worldPosition,
                projSource,
                damage,
                absorb,
                stunFactor,
                fearFactor,
                burnFactor,
                projectileRotation,
                dungeonReference,
                "Assets/Audio/projectileHit.wav");

            newProjectile.sprite.SetImage("Assets/Abilities/Ability_Firewall_Center.png");
            newProjectile.sprite.SetDimensions(new Vector2(256f, 256f));
            newProjectile.sprite.SetScale(new Vector2(1f, 1f));
            newProjectile.sprite.SetShadow("Assets/Abilities/Ability_Firewall_Center_Shadow.png");
            newProjectile.SetVerticalOffset(50);

            dungeonReference.AddProjectileToDungeon(newProjectile, position);

            CastLeft(dungeonReference, position, projectileRotation);
            CastRight(dungeonReference, position, projectileRotation);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/projectileCast.wav", effectsVolume, false);
        }

        private void CastLeft(Dungeon dungeonReference, Vector2 position, float projectileRotation)
        {
            float angle = projectileRotation - 90;

            if (angle == -180)
            {
                angle = 180;
            }

            double angleRadians = angle * Math.PI / 180;

            float dirX = -1 * (float)Math.Floor(Math.Cos(angleRadians));
            float dirY = -1 * (float)Math.Floor(Math.Sin(angleRadians));
            
            Vector2 targetGridSpace = position + new Vector2(dirX, dirY);

            if (dungeonReference.TestGridSpaceEmpty(targetGridSpace))
            {
                Vector2 worldPosition = new Vector2(
                    targetGridSpace.X * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2),
                    targetGridSpace.Y * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2)
                    );

                Projectile newProjectile = new Projectile(
                worldPosition,
                projSource,
                damage,
                absorb,
                stunFactor,
                fearFactor,
                burnFactor,
                projectileRotation,
                dungeonReference,
                "Assets/Audio/projectileHit.wav");

                newProjectile.sprite.SetImage("Assets/Abilities/Ability_Firewall_Left.png");
                newProjectile.sprite.SetDimensions(new Vector2(256f, 256f));
                newProjectile.sprite.SetScale(new Vector2(1f, 1f));
                newProjectile.sprite.SetShadow("Assets/Abilities/Ability_Firewall_Left_Shadow.png");
                newProjectile.SetVerticalOffset(50);

                dungeonReference.AddProjectileToDungeon(newProjectile, targetGridSpace);
            }
        }

        private void CastRight(Dungeon dungeonReference, Vector2 position, float projectileRotation)
        {
            float angle = projectileRotation + 90;
            
            if (angle > 180)
            {
                angle = -90;
            }

            double angleRadians = angle * Math.PI / 180;

            float dirX = -1 * (float)Math.Floor(Math.Cos(angleRadians));
            float dirY = -1 * (float)Math.Floor(Math.Sin(angleRadians));

            Vector2 targetGridSpace = position + new Vector2(dirX, dirY);

            if (dungeonReference.TestGridSpaceEmpty(targetGridSpace))
            {
                Vector2 worldPosition = new Vector2(
                    targetGridSpace.X * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2),
                    targetGridSpace.Y * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2)
                    );

                Projectile newProjectile = new Projectile(
                worldPosition,
                projSource,
                damage,
                absorb,
                stunFactor,
                fearFactor,
                burnFactor,
                projectileRotation,
                dungeonReference,
                "Assets/Audio/projectileHit.wav");

                newProjectile.sprite.SetImage("Assets/Abilities/Ability_Firewall_Right.png");
                newProjectile.sprite.SetDimensions(new Vector2(256f, 256f));
                newProjectile.sprite.SetScale(new Vector2(1f, 1f));
                newProjectile.sprite.SetShadow("Assets/Abilities/Ability_Firewall_Right_Shadow.png");
                newProjectile.SetVerticalOffset(50);

                dungeonReference.AddProjectileToDungeon(newProjectile, targetGridSpace);
            }
        }

    }
}
