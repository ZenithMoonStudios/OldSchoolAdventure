using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace Destiny
{
    /// <summary>
    /// Xml load helper class.
    /// </summary>
    public static class XmlLoadHelper
    {
#if WINDOWS || XBOX
		/// <summary>
		/// Load size.
		/// </summary>
		public static Size LoadSize(XmlNode p_XmlSizeNode)
		{
			// Width
			int width = LoadInteger(p_XmlSizeNode, "Width", 0);
			width = LoadInteger(p_XmlSizeNode, "X", width);

			// Height
			int height = LoadInteger(p_XmlSizeNode, "Height", 0);
			height = LoadInteger(p_XmlSizeNode, "Y", height);

			return new Size(width, height);
		}

		/// <summary>
		/// Load point.
		/// </summary>
		public static Point LoadPoint(XmlNode p_XmlPointNode)
		{
			// X
			int x = LoadInteger(p_XmlPointNode, "Left", 0);
			x = LoadInteger(p_XmlPointNode, "X", x);

			// Y
			int y = LoadInteger(p_XmlPointNode, "Top", 0);
			y = LoadInteger(p_XmlPointNode, "Y", y);

			return new Point(x, y);
		}

		/// <summary>
		/// Load vector.
		/// </summary>
		public static Vector2 LoadVector(XmlNode p_XmlVectorNode)
		{
			// X
			float x = LoadFloat(p_XmlVectorNode, "X", 0f);
			x = LoadFloat(p_XmlVectorNode, "Left", x);
			x = LoadFloat(p_XmlVectorNode, "Width", x);

			// Y
			float y = LoadFloat(p_XmlVectorNode, "Y", 0f);
			y = LoadFloat(p_XmlVectorNode, "Top", y);
			y = LoadFloat(p_XmlVectorNode, "Height", y);

			return new Vector2(x, y);
		}


		/// <summary>
		/// Load rectangle.
		/// </summary>
		public static Rectangle LoadRectangle(XmlNode p_XmlRectangleNode)
		{
			// X
			int x = LoadInteger(p_XmlRectangleNode, "X", 0);
			x = LoadInteger(p_XmlRectangleNode, "Left", x);

			// Y
			int y = LoadInteger(p_XmlRectangleNode, "Y", 0);
			y = LoadInteger(p_XmlRectangleNode, "Top", y);

			// Width and height
			int width = LoadInteger(p_XmlRectangleNode, "Width", 0);
			int height = LoadInteger(p_XmlRectangleNode, "Height", 0);

			return new Rectangle(x, y, width, height);
		}

		/// <summary>
		/// Load color.
		/// </summary>
		public static Color LoadColor(XmlNode p_XmlNode, Color p_DefaultValue)
		{
			Color result = p_DefaultValue;
			result.R = LoadByte(p_XmlNode, "Red", 255);
			result.G = LoadByte(p_XmlNode, "Green", 255);
			result.B = LoadByte(p_XmlNode, "Blue", 255);
			result.A = LoadByte(p_XmlNode, "Alpha", 255);
			return result;
		}

		/// <summary>
		/// Load boolean.
		/// </summary>
		public static bool LoadBoolean(XmlNode p_XmlNode, string p_AttributeName, bool p_DefaultValue)
		{
			bool result = p_DefaultValue;
			if (p_XmlNode != null && p_XmlNode.Attributes[p_AttributeName] != null)
			{
				result = Convert.ToBoolean(p_XmlNode.Attributes[p_AttributeName].Value);
			}
			return result;
		}

		/// <summary>
		/// Load string.
		/// </summary>
		public static string LoadString(XmlNode p_XmlNode, string p_AttributeName, string p_DefaultValue)
		{
			string result = p_DefaultValue;
			if (p_XmlNode != null && p_XmlNode.Attributes[p_AttributeName] != null)
			{
				result = p_XmlNode.Attributes[p_AttributeName].Value;
			}
			return result;
		}

		/// <summary>
		/// Load float.
		/// </summary>
		public static float LoadFloat(XmlNode p_XmlNode, string p_AttributeName, float p_DefaultValue)
		{
			float result = p_DefaultValue;
			if (p_XmlNode != null && p_XmlNode.Attributes[p_AttributeName] != null)
			{
				result = float.Parse(p_XmlNode.Attributes[p_AttributeName].Value, CultureInfo.InvariantCulture);
			}
			return result;
		}

		/// <summary>
		/// Load integer.
		/// </summary>
		public static int LoadInteger(XmlNode p_XmlNode, string p_AttributeName, int p_DefaultValue)
		{
			int result = p_DefaultValue;
			if (p_XmlNode != null && p_XmlNode.Attributes[p_AttributeName] != null)
			{
				result = Convert.ToInt32(p_XmlNode.Attributes[p_AttributeName].Value);
			}
			return result;
		}

		/// <summary>
		/// Load byte.
		/// </summary>
		public static byte LoadByte(XmlNode p_XmlNode, string p_AttributeName, byte p_DefaultValue)
		{
			byte result = p_DefaultValue;
			if (p_XmlNode != null && p_XmlNode.Attributes[p_AttributeName] != null)
			{
				result = Convert.ToByte(p_XmlNode.Attributes[p_AttributeName].Value);
			}
			return result;
		}
#else
        /// <summary>
        /// Load size.
        /// </summary>
        public static Size LoadSize(XElement p_XmlSizeNode)
        {
            // Width
            int width = LoadInteger(p_XmlSizeNode, "Width", 0);
            width = LoadInteger(p_XmlSizeNode, "X", width);

            // Height
            int height = LoadInteger(p_XmlSizeNode, "Height", 0);
            height = LoadInteger(p_XmlSizeNode, "Y", height);

            return new Size(width, height);
        }

        /// <summary>
        /// Load point.
        /// </summary>
        public static Point LoadPoint(XElement p_XmlPointNode)
        {
            // X
            int x = LoadInteger(p_XmlPointNode, "Left", 0);
            x = LoadInteger(p_XmlPointNode, "X", x);

            // Y
            int y = LoadInteger(p_XmlPointNode, "Top", 0);
            y = LoadInteger(p_XmlPointNode, "Y", y);

            return new Point(x, y);
        }

        /// <summary>
        /// Load vector.
        /// </summary>
        public static Vector2 LoadVector(XElement p_XmlVectorNode)
        {
            // X
            float x = LoadFloat(p_XmlVectorNode, "X", 0f);
            x = LoadFloat(p_XmlVectorNode, "Left", x);
            x = LoadFloat(p_XmlVectorNode, "Width", x);

            // Y
            float y = LoadFloat(p_XmlVectorNode, "Y", 0f);
            y = LoadFloat(p_XmlVectorNode, "Top", y);
            y = LoadFloat(p_XmlVectorNode, "Height", y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Load rectangle.
        /// </summary>
        public static Rectangle LoadRectangle(XElement p_XmlRectangleNode)
        {
            // X
            int x = LoadInteger(p_XmlRectangleNode, "X", 0);
            x = LoadInteger(p_XmlRectangleNode, "Left", x);

            // Y
            int y = LoadInteger(p_XmlRectangleNode, "Y", 0);
            y = LoadInteger(p_XmlRectangleNode, "Top", y);

            // Width and height
            int width = LoadInteger(p_XmlRectangleNode, "Width", 0);
            int height = LoadInteger(p_XmlRectangleNode, "Height", 0);

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Load color.
        /// </summary>
        public static Color LoadColor(XElement p_XmlNode, Color p_DefaultValue)
        {
            Color result = p_DefaultValue;
            result.R = LoadByte(p_XmlNode, "Red", 255);
            result.G = LoadByte(p_XmlNode, "Green", 255);
            result.B = LoadByte(p_XmlNode, "Blue", 255);
            result.A = LoadByte(p_XmlNode, "Alpha", 255);
            return result;
        }

        /// <summary>
        /// Load boolean.
        /// </summary>
        public static bool LoadBoolean(XElement p_XmlNode, string p_AttributeName, bool p_DefaultValue)
        {
            bool result = p_DefaultValue;

            if (p_XmlNode != null && p_XmlNode.Attribute(XName.Get(p_AttributeName)) != null)
            {
                result = Convert.ToBoolean(p_XmlNode.Attribute(XName.Get(p_AttributeName)).Value);
            }
            return result;
        }

        /// <summary>
        /// Load string.
        /// </summary>
        public static string LoadString(XElement p_XmlNode, string p_AttributeName, string p_DefaultValue)
        {
            string result = p_DefaultValue;
            if (p_XmlNode != null && p_XmlNode.Attribute(XName.Get(p_AttributeName)) != null)
            {
                result = p_XmlNode.Attribute(XName.Get(p_AttributeName)).Value;
            }
            return result;
        }

        /// <summary>
        /// Load float.
        /// </summary>
        public static float LoadFloat(XElement p_XmlNode, string p_AttributeName, float p_DefaultValue)
        {
            float result = p_DefaultValue;
            if (p_XmlNode != null && p_XmlNode.Attribute(XName.Get(p_AttributeName)) != null)
            {
                result = float.Parse(p_XmlNode.Attribute(XName.Get(p_AttributeName)).Value, CultureInfo.InvariantCulture);
            }
            return result;
        }

        /// <summary>
        /// Load integer.
        /// </summary>
        public static int LoadInteger(XElement p_XmlNode, string p_AttributeName, int p_DefaultValue)
        {
            int result = p_DefaultValue;

            if (p_XmlNode != null && p_XmlNode.Attribute(XName.Get(p_AttributeName)) != null)
            {
                result = Convert.ToInt32(p_XmlNode.Attribute(XName.Get(p_AttributeName)).Value);
            }
            return result;
        }

        /// <summary>
        /// Load byte.
        /// </summary>
        public static byte LoadByte(XElement p_XmlNode, string p_AttributeName, byte p_DefaultValue)
        {
            byte result = p_DefaultValue;
            if (p_XmlNode != null && p_XmlNode.Attribute(XName.Get(p_AttributeName)) != null)
            {
                result = Convert.ToByte(p_XmlNode.Attribute(XName.Get(p_AttributeName)).Value);
            }
            return result;
        }
#endif
    }
}
