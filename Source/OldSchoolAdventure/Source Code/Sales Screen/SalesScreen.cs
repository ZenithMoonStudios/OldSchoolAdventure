using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    public class SalesScreen : Screen
    {
        Texture2D m_BackgroundTexture;
        Rectangle m_BackgroundTextureRegion;

        public SalesScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            this.ContentRelativeFolder = "Sales Screen";
            this.IsPopup = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            m_BackgroundTexture = this.ContentManager.Load<Texture2D>("Background");
            m_BackgroundTextureRegion = RectangleHelper.FitToRegion(
                new Size(m_BackgroundTexture.Width, m_BackgroundTexture.Height),
                RectangleHelper.FromViewport(this.Viewport),
                true
                );
        }

        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            if (p_InputState.IsAccept() || p_InputState.IsCancel())
            {
                this.AudioManager.PlayCue("MenuBack");
                LoadingScreen.Load(
                    this.ScreenManager,
                    false,
                    new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
                    new StartScreen()
                    );
            }
        }

        //static bool IsClose(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) || p_InputState.IsNewKeyPress(Keys.Space) ||
        //           p_InputState.IsNewButtonPress(Buttons.A) || p_InputState.IsNewButtonPress(Buttons.B) || p_InputState.IsNewButtonPress(Buttons.Back);
        //}

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
