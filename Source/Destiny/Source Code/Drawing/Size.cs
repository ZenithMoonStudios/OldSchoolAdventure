using Microsoft.Xna.Framework.Graphics;

namespace Destiny
{
    public struct Size
    {
        public int Width;
        public int Height;

        public Size(int p_Width, int p_Height)
        {
            this.Width = p_Width;
            this.Height = p_Height;
        }

        public static Size Zero { get { return new Size(); } }
        public static Size FromTexture(Texture2D p_Texture) { return new Size(p_Texture.Width, p_Texture.Height); }
    }
}
