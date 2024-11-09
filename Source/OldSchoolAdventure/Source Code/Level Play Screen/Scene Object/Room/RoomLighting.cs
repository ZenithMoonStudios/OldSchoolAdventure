using Microsoft.Xna.Framework;

namespace OSA
{
    public class RoomLighting
    {
        public enum Axes { X, Y }

        public Axes Axis { get; set; }

        public int StartPosition { get; set; }
        public Color StartColor { get; set; }

        public int FinishPosition { get; set; }
        public Color FinishColor { get; set; }

        public RoomLighting() { }

        public static Axes AxisFromString(string p_String)
        {
            return (p_String == "Y" ? Axes.Y : Axes.X);
        }
    }
}
