using Microsoft.Xna.Framework;

namespace Destiny
{
    public class VectorHelper
    {
        public static Point ToPoint(Vector2 p_Vector)
        {
            return new Point((int)p_Vector.X, (int)p_Vector.Y);
        }
    }
}
