using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OSA
{
    public class StartScreen : Screen
    {
        Texture2D m_MessageTexture;
        Vector2 m_MessagePosition;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StartScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Menu Screen/Start Screen";
            this.IsPopup = true;

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }

        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_MessageTexture = this.ContentManager.Load<Texture2D>("Message");
#if (MOBILE)
			m_MessagePosition.Y = (this.Viewport.Height - m_MessageTexture.Height) / 2;
#else
            // Position prompt tightly within 80% overscan (10% on each side)
            m_MessagePosition.Y = (0.9f * this.Viewport.Height) - m_MessageTexture.Height;
#endif
            m_MessagePosition.X = (this.Viewport.Width - m_MessageTexture.Width) / 2;
        }

        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            if (p_InputState.IsAcceptAll())
            {
                // Get active player index
                PlayerIndex activePlayerIndex = PlayerIndex.One;
                for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                {
                    if (p_InputState.IsAccept())
                    {
                        activePlayerIndex = index;
                        break;
                    }
                }

                // Set active player index
                p_InputState.ActivePlayerIndex = activePlayerIndex;

                // Play audio
                this.AudioManager.PlayCue("MenuSelect");

                // Navigate to main menu
                this.ScreenManager.AddScreen(new ProfileCheckScreen());
                this.ExitScreen();
            }
        }

        public override void Draw(GameTime p_GameTime)
        {
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Draw start message
            float sine = (float)Math.Abs(Math.Sin(this.ActiveFrames / 20f));
            Color startMessageColor = ColorHelper.AddAlpha(Color.Yellow, Convert.ToByte((float)this.TransitionAlpha * sine));
            spriteBatch.Draw(m_MessageTexture, m_MessagePosition, startMessageColor);

            spriteBatch.End();
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }
    }
}
