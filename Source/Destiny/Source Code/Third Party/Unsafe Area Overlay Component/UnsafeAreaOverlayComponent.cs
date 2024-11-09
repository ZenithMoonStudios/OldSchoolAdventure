using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Destiny
{
    /// <summary>
    /// Component that renders the unsafe screen areas for any content (10%)
    /// and for action or text (20%).
    /// </summary>
    public class UnsafeAreaOverlayComponent : DrawableGameComponent
    {
        /// <summary>
        /// Color for the area unsafe for game action or text.
        /// </summary>
        public Color NoActionAreaColor
        {
            get { return noActionAreaColor; }
            set { noActionAreaColor = value; }
        }

        /// <summary>
        /// Color for the area unsafe for game action or text.
        /// </summary>
        private Color noActionAreaColor;

        /// <summary>
        /// Color for the area unsafe for any content.
        /// </summary>
        public Color UnsafeAreaColor
        {
            get { return unsafeAreaColor; }
            set { unsafeAreaColor = value; }
        }

        /// <summary>
        /// Color for the area unsafe for any content.
        /// </summary>
        private Color unsafeAreaColor;

        /// <summary>
        /// Renders safe-zone overlay.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Texture to render (1x1).
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Defines the screen area unsafe for action.
        /// </summary>
        private Rectangle[] noActionAreaParts;

        /// <summary>
        /// Defines the screen area unsafe for any content.
        /// </summary>
        private Rectangle[] unsafeAreaParts;

        /// <summary>
        /// Create a new <c>UnsafeAreaOverlayComponent</c>.
        /// </summary>
        /// <param name="game">Containing game.</param>
        public UnsafeAreaOverlayComponent(Game game)
            : base(game)
        {
            // draw this last
            DrawOrder = Int32.MaxValue;

            // default the unsafe area colors, semi-transparent
            noActionAreaColor = new Color(255, 0, 0, 63);
            unsafeAreaColor = new Color(255, 255, 0, 63);
        }

        /// <summary>
        /// Load component content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // generate a 1x1 texture
            texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] textureData = new Color[1];
            textureData[0] = Color.White;
            texture.SetData<Color>(textureData);

            // get viewport size and the offset percentage
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;
            int dw = (int)(width * 0.05);
            int dh = (int)(height * 0.05);

            // generate the area unsafe for game action or text
            noActionAreaParts = new Rectangle[4];
            noActionAreaParts[0] = new Rectangle(0, 0, width, dh);
            noActionAreaParts[1] = new Rectangle(0, height - dh, width, dh);
            noActionAreaParts[2] = new Rectangle(0, dh, dw, height - 2 * dh);
            noActionAreaParts[3] = new Rectangle(width - dw, dh, dw, height - 2 * dh);

            // generate the area not safe for anything
            unsafeAreaParts = new Rectangle[4];
            unsafeAreaParts[0] = new Rectangle(dw, dh, width - 2 * dw, dh);
            unsafeAreaParts[1] = new Rectangle(dw, height - 2 * dh, width - 2 * dw, dh);
            unsafeAreaParts[2] = new Rectangle(dw, 2 * dh, dw, height - 4 * dh);
            unsafeAreaParts[3] = new Rectangle(width - 2 * dw, 2 * dh, dw, height - 4 * dh);
        }

        /// <summary>
        /// Draw the component.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // render the area unsafe for action or game text
            for (int i = 0; i < noActionAreaParts.Length; i++)
            {
                spriteBatch.Draw(texture, noActionAreaParts[i], noActionAreaColor);
            }

            // render the area unsafe for any game content
            for (int i = 0; i < unsafeAreaParts.Length; i++)
            {
                spriteBatch.Draw(texture, unsafeAreaParts[i], unsafeAreaColor);
            }

            spriteBatch.End();
        }
    }
}
