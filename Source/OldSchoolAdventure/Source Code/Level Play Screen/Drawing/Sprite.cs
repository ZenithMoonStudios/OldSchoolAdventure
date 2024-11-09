using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OSA
{
    /// <summary>
    /// Sprite class.
    /// </summary>
    public class Sprite
    {
        public Texture2D Texture { get; private set; }
        public Rectangle TextureRegion { get; private set; }
        public Size DisplayOffset { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_Texture">Texture.</param>
        /// <param name="p_TextureRegion">Texture region.</param>
        /// <param name="p_DisplayOffset">Display offset.</param>
        public Sprite(Texture2D p_Texture, Rectangle p_TextureRegion, Size p_DisplayOffset)
        {
            this.Texture = p_Texture;
            this.TextureRegion = p_TextureRegion;
            this.DisplayOffset = p_DisplayOffset;
        }
    }
}
