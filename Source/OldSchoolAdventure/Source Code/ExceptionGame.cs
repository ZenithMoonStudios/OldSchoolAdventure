using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OSA
{
    /// <summary>
    /// Exception game.
    /// </summary>
    public class ExceptionGame : Game
    {
        private SpriteBatch m_SpriteBatch;
        private SpriteFont m_ExceptionFont;
        private readonly Exception m_Exception;

        public ExceptionGame(Exception p_Exception)
        {
            m_Exception = p_Exception;

            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            m_ExceptionFont = this.Content.Load<SpriteFont>("GameExceptionFont");
            m_SpriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        protected override void Update(GameTime p_GameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            base.Update(p_GameTime);
        }

        protected override void Draw(GameTime p_GameTime)
        {
            this.GraphicsDevice.Clear(Color.Black);
            m_SpriteBatch.Begin();

            string message = string.Format("Exception: {0}", m_Exception.Message);
            string stackTrace = string.Format("Stack Trace:\n{0}", m_Exception.StackTrace);

            m_SpriteBatch.DrawString(m_ExceptionFont, "**** CRASH LOG ****", new Vector2(0f, 0f), Color.White);
            m_SpriteBatch.DrawString(m_ExceptionFont, "Press Back to Exit", new Vector2(0f, 20f), Color.White);
            m_SpriteBatch.DrawString(m_ExceptionFont, message, new Vector2(0f, 40f), Color.White);
            m_SpriteBatch.DrawString(m_ExceptionFont, stackTrace, new Vector2(0f, 60f), Color.White);

            m_SpriteBatch.End();
            base.Draw(p_GameTime);
        }
    }
}
