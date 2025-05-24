

using System.Numerics;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.WorldObjects;

namespace PixelArtGameJam.Game.Components
{
    public abstract class Ability
    {
        public Sprite sprite { get; protected set; }

        public int cost { get; set; }
        public int damage { get; set; }
        public int absorb { get; set; }
        public int stunFactor { get; set; }
        public int fearFactor { get; set; }
        public int burnFactor { get; set; }

        protected Projectile.ProjSource projSource { get; set; }

        public Ability(Projectile.ProjSource projSource)
        {
            sprite = new Sprite();
            this.projSource = projSource;
        }

        public abstract void Cast(Dungeon dungeonReference, Vector2 position, float projectileRotation);
    }
}
