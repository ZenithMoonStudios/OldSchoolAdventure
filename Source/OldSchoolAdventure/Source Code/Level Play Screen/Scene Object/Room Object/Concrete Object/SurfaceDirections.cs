namespace OSA
{
    public enum SurfaceDirections { None, Left, Right, Top, Bottom }

    public static class SurfaceDirectionsHelper
    {
        public static SurfaceDirections Opposite(SurfaceDirections p_Direction)
        {
            SurfaceDirections result = SurfaceDirections.None;
            switch (p_Direction)
            {
                case SurfaceDirections.Left: result = SurfaceDirections.Right; break;
                case SurfaceDirections.Right: result = SurfaceDirections.Left; break;
                case SurfaceDirections.Top: result = SurfaceDirections.Bottom; break;
                case SurfaceDirections.Bottom: result = SurfaceDirections.Top; break;
            }
            return result;
        }
    }
}
