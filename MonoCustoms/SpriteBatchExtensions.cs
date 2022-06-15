using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCustoms
{
    public static class SpriteBatchExtensions
    {
        public static void DrawStringOffset(this SpriteBatch spriteBatch,
            SpriteFont spriteFont,
            string text,
            Color color,
            Vector2 position,
            Vector2? offsetTimesStringSize = null,
            Vector2? origin = null,
            float scale = 1.0F,
            float rotation = 0.0F,
            float layerDepth = 0.0F,
            SpriteEffects effects = SpriteEffects.None)
        {
            var textDim = spriteFont.MeasureStringScaled(text, scale);

            if (offsetTimesStringSize != null)
            {
                position.X += offsetTimesStringSize?.X * textDim.X ?? 0.0F;
                position.Y += offsetTimesStringSize?.Y * textDim.Y ?? 0.0F;
            }

            spriteBatch.DrawString(spriteFont: spriteFont,
                text: text,
                position: position,
                color: color,
                effects: effects,
                layerDepth: layerDepth,
                rotation: rotation,
                scale: scale,
                origin: origin ?? Vector2.Zero);
        }

        public static void DrawScaled(this SpriteBatch spriteBatch,
            Texture2D texture,
            Rectangle destinationRectangle,
            Color color,
            double scale,
            float rotation = 0.0F,
            Vector2? origin = null)
        {
            var r = destinationRectangle;
            spriteBatch.Draw(texture, 
                new Rectangle(r.X.Scale(scale), r.Y.Scale(scale), r.Width.Scale(scale), r.Height.Scale(scale)), 
                null, 
                color,
                rotation,
                origin ?? Vector2.Zero,
                SpriteEffects.None,
                0.0F);
        }
    }
}
