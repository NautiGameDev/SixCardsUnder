using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Abilities
{
    public class Energize : Ability
    {
        public Energize(Projectile.ProjSource projSource) : base(projSource)
        {
            sprite.SetImage("Assets/Abilities/Ability_Energize.png");
            sprite.SetDimensions(new Vector2(256, 256));
            sprite.SetScale(new Vector2(2f, 2f));

            cost = 0;
            damage = 0;
            absorb = 0;
            stunFactor = 0;
            fearFactor = 0;
            burnFactor = 0;
        }

        public override void Cast(Dungeon dungeonReference, Vector2 position, float projectileRotation)
        {
            dungeonReference.player.ResolveMana(3);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/heal.wav", effectsVolume, false);
        }
    }
}
