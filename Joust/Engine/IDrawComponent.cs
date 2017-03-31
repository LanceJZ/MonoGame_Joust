using Microsoft.Xna.Framework;

namespace Joust.Engine
{
    public interface IDrawComponent
    {
        void Draw(GameTime gametime);
        void LoadContent();
    }
}
