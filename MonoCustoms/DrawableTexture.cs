using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCustoms;

public class DrawableTexture : RenderTarget2D
{
    public DrawableTexture(GraphicsDevice gd, int w, int h, Action<SpriteBatch> drawingCode)
        : base(gd, w, h, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.Depth24)
    {
        var sb = new SpriteBatch(gd);

        gd.SetRenderTarget(this);
        gd.Clear(Color.Transparent);
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        drawingCode(sb);

        sb.End();
        gd.SetRenderTarget(null);
    }
}
