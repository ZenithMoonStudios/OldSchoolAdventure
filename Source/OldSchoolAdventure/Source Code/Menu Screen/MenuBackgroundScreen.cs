using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Menu background screen class.
    /// 
    /// This screen sits behind all other menu screens, providing a fixed
    /// background including between menu screen transitions.
    /// </summary>
    public class MenuBackgroundScreen : Screen
    {
        public enum Modes { MainMenu, PauseMenu }
        Modes m_Mode;

        Texture2D m_BackgroundTexture;
        Rectangle m_BackgroundTextureRegion;

        Texture2D m_TitleTexture;
        Vector2 m_TitlePosition;

        public MenuBackgroundScreen(Modes p_Mode)
        {
            m_Mode = p_Mode;
            this.IsPopup = (m_Mode == Modes.PauseMenu);
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Menu Screen/Menu Background Screen";
        }

        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load title
            m_TitleTexture = this.ContentManager.Load<Texture2D>("Title");
            m_TitlePosition.X = (this.Viewport.Width - m_TitleTexture.Width) / 2;
            m_TitlePosition.Y = 80;

            // If main menu, load background and music
            if (m_Mode == Modes.MainMenu)
            {
                m_BackgroundTexture = this.ContentManager.Load<Texture2D>("Background");
                m_BackgroundTextureRegion = RectangleHelper.FitToRegion(
                    Size.FromTexture(m_BackgroundTexture),
                    RectangleHelper.FromViewport(this.Viewport),
                    true
                    );
                this.AudioManager.PlayMusic("MenuScreen");
            }
        }

        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            base.Update(p_GameTime, p_HasFocus, false);
        }

        public override void Draw(GameTime p_GameTime)
        {
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Define basic colours
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            // Draw background
            if (m_Mode == Modes.MainMenu)
            {
                // Draw main menu background
                spriteBatch.Draw(m_BackgroundTexture, m_BackgroundTextureRegion, color);
            }
            else if (m_Mode == Modes.PauseMenu)
            {
                // Darken everything behind this screen
                this.ScreenManager.DarkenScreen(Convert.ToByte(0.5f * this.TransitionAlpha), spriteBatch);

                // Draw title
                spriteBatch.Draw(m_TitleTexture, m_TitlePosition, color);
            }

            spriteBatch.End();
        }
    }
}
