using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Destiny
{
    /// <summary>
    /// Rectangle helper class.
    /// </summary>
    public static class RectangleHelper
    {
        public static int Left(Rectangle p_Rectangle) { return p_Rectangle.X; }
        public static int Top(Rectangle p_Rectangle) { return p_Rectangle.Y; }
        public static int InnerRight(Rectangle p_Rectangle) { return p_Rectangle.X + p_Rectangle.Width - 1; }
        public static int OuterRight(Rectangle p_Rectangle) { return p_Rectangle.X + p_Rectangle.Width; }
        public static int InnerBottom(Rectangle p_Rectangle) { return p_Rectangle.Y + p_Rectangle.Height - 1; }
        public static int OuterBottom(Rectangle p_Rectangle) { return p_Rectangle.Y + p_Rectangle.Height; }
        public static Size Size(Rectangle p_Rectangle) { return new Size(p_Rectangle.Width, p_Rectangle.Height); }
        public static float CenterX(Rectangle p_Rectangle) { return p_Rectangle.X + (p_Rectangle.Width / 2f); }
        public static float CenterY(Rectangle p_Rectangle) { return p_Rectangle.Y + (p_Rectangle.Height / 2f); }
        public static Vector2 Center(Rectangle p_Rectangle) { return new Vector2(CenterX(p_Rectangle), CenterX(p_Rectangle)); }

        static Rectangle s_Rectangle;
        public static Rectangle FitToRegion(Size p_Size, Rectangle p_Region, bool p_IsAllowOverflow)
        {
            // Determine scale factor
            float xScaleFactor = p_Region.Width / (float)p_Size.Width;
            float yScaleFactor = p_Region.Height / (float)p_Size.Height;
            float scaleFactor = p_IsAllowOverflow ? Math.Max(xScaleFactor, yScaleFactor) : Math.Min(xScaleFactor, yScaleFactor);

            // Calculate new size
            s_Rectangle.Width = Convert.ToInt32(p_Size.Width * scaleFactor);
            s_Rectangle.Height = Convert.ToInt32(p_Size.Height * scaleFactor);

            // Center result
            s_Rectangle.X = p_Region.X + (p_Region.Width - s_Rectangle.Width) / 2;
            s_Rectangle.Y = p_Region.Y + (p_Region.Height - s_Rectangle.Height) / 2;
            return s_Rectangle;
        }

        public static Rectangle FromViewport(Viewport p_Viewport)
        {
            s_Rectangle.X = p_Viewport.X;
            s_Rectangle.Y = p_Viewport.Y;
            s_Rectangle.Width = p_Viewport.Width;
            s_Rectangle.Height = p_Viewport.Height;
            return s_Rectangle;
        }
    }
}
