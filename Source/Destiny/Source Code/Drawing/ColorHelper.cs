using Microsoft.Xna.Framework;

namespace Destiny
{
    public static class ColorHelper
    {
        public static Color AddAlpha(Color p_Color, byte p_Alpha)
        {
            p_Color.A = p_Alpha;
            return p_Color;
        }
    }
}
