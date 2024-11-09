namespace OSA
{
    /// <summary>
    /// Vertical alignment helper class.
    /// </summary>
    public static class VerticalAlignmentHelper
    {
        public const string Top = "Top";
        public const string Center = "Center";
        public const string Bottom = "Bottom";

        public static VerticalAlignment FromString(string p_String)
        {
            VerticalAlignment result = VerticalAlignment.Center;
            switch (p_String)
            {
                case VerticalAlignmentHelper.Top: result = VerticalAlignment.Top; break;
                case VerticalAlignmentHelper.Center: result = VerticalAlignment.Center; break;
                case VerticalAlignmentHelper.Bottom: result = VerticalAlignment.Bottom; break;
            }
            return result;
        }
    }
}
