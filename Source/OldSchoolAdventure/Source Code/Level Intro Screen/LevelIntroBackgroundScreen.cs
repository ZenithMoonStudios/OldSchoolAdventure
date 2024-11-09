using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Level intro background screen class.
    /// 
    /// This screen sits behind all other level intro screens, providing a fixed
    /// background including between intro page screen transitions.
    /// </summary>
    public class LevelIntroBackgroundScreen : Screen
    {
        Level m_Level;

        Texture2D m_BackgroundTexture;
        Rectangle m_BackgroundTextureRegion;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelIntroBackgroundScreen(Level p_Level)
        {
            m_Level = p_Level;

            this.ContentRelativeFolder = "Level Intro Screen";
            this.TransitionOnTime = TimeSpan.FromSeconds(2);
            this.TransitionOffTime = TimeSpan.FromSeconds(2);
        }

        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>("Backgrounds/" + m_Level.Name);
            m_BackgroundTextureRegion = RectangleHelper.FitToRegion(
                new Size(m_BackgroundTexture.Width, m_BackgroundTexture.Height),
                RectangleHelper.FromViewport(this.Viewport),
                true
                );

            // Load music
            this.AudioManager.PlayMusic("LevelIntroScreen");
        }

        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            base.Update(p_GameTime, p_HasFocus, false);
        }

        public override void Draw(GameTime p_GameTime)
        {
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(m_BackgroundTexture, m_BackgroundTextureRegion, color);
            spriteBatch.End();
        }
    }
}
