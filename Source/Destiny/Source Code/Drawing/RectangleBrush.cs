using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Destiny
{
    /// <summary>
    /// Rectangle brush class.
    /// </summary>
    public class RectangleBrush
    {
        int m_Width;
        int m_Height;
        Color m_BorderColor;
        Texture2D m_Texture;

        Vector2 m_DrawPosition;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RectangleBrush(int p_Width, int p_Height, Color p_BorderColor, GraphicsDevice p_GraphicsDevice)
        {
            m_Width = p_Width;
            m_Height = p_Height;
            m_BorderColor = p_BorderColor;
            m_Texture = this.CreateTexture(p_GraphicsDevice);
        }

        /// <summary>
        /// Draw rectangle.
        /// </summary>
        public void Draw(Point p_Position, SpriteBatch p_SpriteBatch)
        {
            m_DrawPosition.X = p_Position.X;
            m_DrawPosition.Y = p_Position.Y;
            p_SpriteBatch.Draw(m_Texture, m_DrawPosition, Color.White);
        }

        /// <summary>
        /// Create texture.
        /// </summary>
        Texture2D CreateTexture(GraphicsDevice p_GraphicsDevice)
        {
            int textureDataSize = m_Width * m_Height;
            Color[] textureData = new Color[textureDataSize];
            for (int x = 0; x < m_Width; x++)
            {
                textureData[x] = m_BorderColor;
                textureData[textureDataSize - m_Width + x] = m_BorderColor;
            }
            for (int y = 1; y < m_Height - 1; y++)
            {
                textureData[y * m_Width] = m_BorderColor;
                textureData[(y + 1) * m_Width - 1] = m_BorderColor;
            }
            Texture2D texture = new Texture2D(p_GraphicsDevice, m_Width, m_Height);
            texture.SetData(textureData);
            return texture;
        }
    }
}
