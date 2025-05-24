using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Abilities
{
    public class Fireball : Ability
    {
        public Fireball(Projectile.ProjSource projSource) : base(projSource)
        {
            sprite.SetImage("Assets/Abilities/Ability_Fireball.png");
            sprite.SetDimensions(new Vector2(256, 256));
            sprite.SetScale(new Vector2(2f, 2f));

            cost = 4;
            damage = 0;
            absorb = 0;
            stunFactor = 0;
            fearFactor = 0;
            burnFactor = 3;
        }

        public override void Cast(Dungeon dungeonReference, Vector2 position, float projectileRotation)
        {
            //Projectile rotation is in degrees here

            Vector2 worldPosition = new Vector2(
                position.X * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2),
                position.Y * dungeonReference.map.gridSize + (dungeonReference.map.gridSize / 2));

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

            newProjectile.sprite.SetImage("Assets/Abilities/Ability_Fireball.png");
            newProjectile.sprite.SetDimensions(new Vector2(256, 256));
            newProjectile.sprite.SetScale(new Vector2(1f, 1f));
            newProjectile.sprite.SetShadow("Assets/Abilities/Ability_Fireball_Shadow.png");
            newProjectile.SetVerticalOffset(25);

            dungeonReference.AddProjectileToDungeon(newProjectile, position);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/projectileCast.wav", effectsVolume, false);
        }
    }
}
