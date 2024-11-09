using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Message box screen.
    /// </summary>
    public class MessageBoxScreen : Screen
    {
        string m_Title;
        string m_Message;
        string m_AcceptText;
        string m_CancelText;

        /// <summary>
        /// Delegates.
        /// </summary>
        public delegate void MessageBoxHandler(bool p_IsAccept);
        MessageBoxHandler m_Handler;

        /// <summary>
        /// Show a message box.
        /// </summary>
        public static void Show(ScreenManager p_ScreenManager, string p_Title, string p_Message, string p_AcceptText, string p_CancelText, MessageBoxHandler p_Handler)
        {
            p_ScreenManager.AddScreen(new MessageBoxScreen(p_Title, p_Message, p_AcceptText, p_CancelText, p_Handler));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageBoxScreen(string p_Title, string p_Message, string p_AcceptText, string p_CancelText, MessageBoxHandler p_Handler)
        {
            m_Title = p_Title;
            m_Message = p_Message;
            m_AcceptText = p_AcceptText;
            m_CancelText = p_CancelText;

            m_Handler = p_Handler;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.2);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.2);
            this.ContentRelativeFolder = "Message Box Screen";
            this.IsPopup = true;

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }

        ///// <summary>
        ///// Message box callback.
        ///// </summary>
        //void MessageBoxCallback(IAsyncResult p_AsyncResult)
        //{
        //    // Get the result of the message box
        //    int? choice = Guide.EndShowMessageBox(p_AsyncResult);
        //    bool isAccept = (choice.HasValue && choice.Value == 0);

        //    // Exit screen and call result handler
        //    this.ExitScreen();
        //    m_Handler(isAccept);
        //}

#if WINDOWS
        SpriteFont m_TitleFont;
        SpriteFont m_MessageFont;
        SpriteFont m_ButtonFont;
        SpriteFont m_ButtonTextFont;

        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_TitleFont = this.ContentManager.Load<SpriteFont>("TitleFont");
            m_MessageFont = this.ContentManager.Load<SpriteFont>("MessageFont");
            m_ButtonFont = this.ContentManager.Load<SpriteFont>("ButtonFont");
            m_ButtonTextFont = this.ContentManager.Load<SpriteFont>("ButtonTextFont");
        }

        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);
            if (p_InputState.IsAccept())
            {
                this.ExitScreen();
                m_Handler(true);
            }
            else if (p_InputState.IsCancel())
            {
                this.ExitScreen();
                m_Handler(false);
            }
        }

        //private static bool IsAccept(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) || p_InputState.IsNewKeyPress(Keys.Enter) ||
        //           p_InputState.IsNewButtonPress(Buttons.A) || p_InputState.IsNewButtonPress(Buttons.Start);
        //}

        //private static bool IsCancel(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) ||
        //           p_InputState.IsNewButtonPress(Buttons.B) || p_InputState.IsNewButtonPress(Buttons.Back);
        //}

        static Vector2 s_TempVector;
        public override void Draw(GameTime p_GameTime)
        {
            // Start drawing sprites
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Fade text with screen transitions
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            // Darken everything behind this screen
            this.ScreenManager.DarkenScreen(Convert.ToByte(0.9f * this.TransitionAlpha), spriteBatch);

            // Draw title
            Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
            s_TempVector.X = 140;
            s_TempVector.Y = 65;
            spriteBatch.DrawString(m_TitleFont, m_Title, s_TempVector, color);

            // Draw message
            s_TempVector.X = 140;
            s_TempVector.Y = 200;
            spriteBatch.DrawString(m_MessageFont, m_Message, s_TempVector, color);

            // Draw buttons
            s_TempVector.X = 140;
            s_TempVector.Y = 485;
            spriteBatch.DrawString(m_ButtonFont, "'", s_TempVector, color, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            if (!string.IsNullOrEmpty(m_CancelText))
            {
                s_TempVector.Y = 545;
                spriteBatch.DrawString(m_ButtonFont, ")", s_TempVector, color, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            }

            // Draw yes and no options
            s_TempVector.X = 205;
            s_TempVector.Y = 520;
            spriteBatch.DrawString(m_ButtonTextFont, m_AcceptText, s_TempVector, color);
            if (!string.IsNullOrEmpty(m_CancelText))
            {
                s_TempVector.Y = 580;
                spriteBatch.DrawString(m_ButtonTextFont, m_CancelText, s_TempVector, color);
            }

            // Finish drawing sprites
            spriteBatch.End();
        }
#else
        bool m_HasShown = false;
		public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
		{
			if (!m_HasShown && !GuideHelper.IsVisible)
			{
				try
				{
					Guide.BeginShowMessageBox(
						(PlayerIndex)PlatformerGame.Instance.ScreenManager.InputState.ActivePlayerIndex,
						m_Title,
						m_Message,
						!string.IsNullOrEmpty(m_CancelText) ? new[] { m_AcceptText, m_CancelText } : new[] { m_AcceptText },
						0,
						MessageBoxIcon.None,
						this.MessageBoxCallback,
						null
						);
					m_HasShown = true;
				}
				catch
				{
					// Just catch the exception, and the message box will be shown next update call
				}
			}
			base.Update(p_GameTime, p_HasFocus, p_IsCoveredByOtherScreen);
		}

        public override void Draw(GameTime p_GameTime) { }
#endif
    }
}
