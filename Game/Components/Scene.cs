using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.WorldObjects;

namespace PixelArtGameJam.Game.Components
{
    public abstract class Scene
    {
        public abstract Task Update(float deltaTime);
    }
}
