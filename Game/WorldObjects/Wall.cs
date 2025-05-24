using System.Numerics;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Entities;

namespace PixelArtGameJam.Game.WorldObjects
{
    public class Wall : WorldObject
    {
        public bool hasBeenSeen { get; private set; } = false;

        public Wall(Vector2 position) : base(position, ObjectType.WALL)
        {
            
        }

        public void SetSpecialWallType(ObjectType wallType)
        {
            objType = wallType;
        }

        public void MarkHasBeenSeen()
        {
            hasBeenSeen = true;
        }

        public override Task Render()
        {
            //Not used for wall objects, nothing to render here
            return null;
        }
    }
}
