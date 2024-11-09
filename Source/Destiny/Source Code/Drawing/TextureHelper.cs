using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Destiny
{
    public static class TextureHelper
    {
        public static Texture2D CreateUnitTexture(Color p_Color, GraphicsDevice p_GraphicsDevice)
        {
            Texture2D texture = new Texture2D(p_GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1];
            data[0] = p_Color;
            texture.SetData(data);
            return texture;
        }
    }
}
