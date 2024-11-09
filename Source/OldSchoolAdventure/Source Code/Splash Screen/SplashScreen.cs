using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    public class SplashScreen : Screen
    {
        public delegate void SimpleDelegate();
        SimpleDelegate m_OnFinishedDelegate;

        string m_BackgroundTexturePath;
        Texture2D m_BackgroundTexture;
        Vector2 m_BackgroundTexturePosition;

        const float c_DisplayTime = 2f;
        double m_ElapsedTime = 0f;

        public SplashScreen(string p_BackgroundTexturePath, SimpleDelegate p_OnFinishedDelegate)
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Splash Screen";

            m_BackgroundTexturePath = p_BackgroundTexturePath;
            m_OnFinishedDelegate = p_OnFinishedDelegate;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>(m_BackgroundTexturePath);
            m_BackgroundTexturePosition.X = (this.Viewport.Width - m_BackgroundTexture.Width) / 2;
            m_BackgroundTexturePosition.Y = (this.Viewport.Height - m_BackgroundTexture.Height) / 2;
        }

        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            // Force this screen to be shown, even when it should be covered
            base.Update(p_GameTime, p_HasFocus, false);

            // Assess whether can close
            if (Game.IsActive && !this.IsExiting)
            {
                m_ElapsedTime += p_GameTime.ElapsedGameTime.TotalSeconds;
                if (m_ElapsedTime >= c_DisplayTime)
                {
                    m_OnFinishedDelegate();
                }
            }
        }

        public override void Draw(GameTime p_GameTime)
        {
            this.SpriteBatch.Begin();

            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);
            this.SpriteBatch.Draw(m_BackgroundTexture, m_BackgroundTexturePosition, color);

            this.SpriteBatch.End();
        }
    }
}
