using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCustoms
{
    public static class SpriteFontExtensions
    {
        public static Vector2 MeasureStringScaled(this SpriteFont font, string text, float scale)
        {
            return font.MeasureString(text: text) * scale;
        }
    }
}
