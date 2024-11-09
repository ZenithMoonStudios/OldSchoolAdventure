using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
	/// Credits screen class.
    /// </summary>
    public class CreditsScreen : Screen
    {
        public class CreditType
        {
            public string Title { get; private set; }
            public StringList Credits { get; private set; }
            public CreditType(string p_Title)
            {
                this.Title = p_Title;
                this.Credits = new StringList();
            }
        }

        SpriteFont m_Font;

        /// <summary>
        /// Constraints.
        /// </summary>
        int m_CreditTypeSpacing = 60;
        int m_CreditTypeHeight = 60;
        int m_CreditHeight = 60;
        const float c_DrawScale = 1f;

        /// <summary>
		/// State.
		/// </summary>
		bool m_IsLoadMainMenu;
        List<CreditType> m_Credits = null;
        int m_TotalHeight;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreditsScreen(bool p_IsLoadMainMenu)
        {
            this.m_IsLoadMainMenu = p_IsLoadMainMenu;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            this.ContentRelativeFolder = "Credits Screen";

            m_CreditTypeSpacing = Convert.ToInt32(m_CreditTypeSpacing * c_DrawScale);
            m_CreditTypeHeight = Convert.ToInt32(m_CreditTypeHeight * c_DrawScale);
            m_CreditHeight = Convert.ToInt32(m_CreditHeight * c_DrawScale);
        }

        #region Load and Unload content

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load fonts
            m_Font = this.ContentManager.Load<SpriteFont>("Font");

            // Initialize credits
            this.m_Credits = new List<CreditType>();
            CreditType creditMusic = new CreditType("music by");
            creditMusic.Credits.Add("Ryan Gilchrist");
            this.m_Credits.Add(creditMusic);
            CreditType creditSounds = new CreditType("sounds from");
            creditSounds.Credits.Add("http://soundjay.com");
            this.m_Credits.Add(creditSounds);
            CreditType creditThirdParties = new CreditType("3rd party components");
            creditThirdParties.Credits.Add("EasyStorage");
            this.m_Credits.Add(creditThirdParties);
            CreditType creditArt = new CreditType("title screen and box art");
            creditArt.Credits.Add("Woltz Media");
            this.m_Credits.Add(creditArt);
            CreditType creditIdeas = new CreditType("game ideas");
            creditIdeas.Credits.Add("Palmer Middlekauff");
            this.m_Credits.Add(creditIdeas);
            CreditType creditThanks = new CreditType("special thanks");
            creditThanks.Credits.Add("Da Voodoo Chief");
            creditThanks.Credits.Add("Luke Quinn");
            creditThanks.Credits.Add("Michael B. McLaughlin");
            creditThanks.Credits.Add("Robert Boyd");
            this.m_Credits.Add(creditThanks);
            CreditType creditPort = new CreditType("phone port by");
            creditPort.Credits.Add("Dark Omen Games");
            this.m_Credits.Add(creditPort);
            CreditType creditProduction = new CreditType("developed by");
            creditProduction.Credits.Add("Chris Hughes Games");
            this.m_Credits.Add(creditProduction);

            // Determine total height of the credits
            this.m_TotalHeight = 0;
            foreach (CreditType creditType in this.m_Credits)
            {
                if (this.m_TotalHeight > 0)
                {
                    this.m_TotalHeight += m_CreditTypeSpacing;
                }
                this.m_TotalHeight += m_CreditTypeHeight;
                this.m_TotalHeight += (creditType.Credits.Count * m_CreditHeight);
            }

            // Once the load has finished, we need to tell the game that we have just
            // finished a very long frame, and that it should not try to catch up
            this.ScreenManager.Game.ResetElapsedTime();
        }

        #endregion

        #region Input

        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            if (p_InputState.IsEnd() || this.ActiveFrames > (this.Viewport.Height + this.m_TotalHeight))
            {
                if (this.m_IsLoadMainMenu)
                {
                    LoadingScreen.Load(
                        this.ScreenManager,
                        false,
                        new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
                        new StartScreen()
                        );
                }
                else
                {
                    this.AudioManager.PlayCue("MenuBack");
                    this.ExitScreen();
                }
            }
        }

        //static bool IsEnd(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) || p_InputState.IsNewKeyPress(Keys.Escape)
        //        || p_InputState.IsNewButtonPress(Buttons.A) || p_InputState.IsNewButtonPress(Buttons.B);
        //}

        #endregion

        /// <summary>
        /// Draw screen.
        /// </summary>
        static Vector2 s_TempVector;
        public override void Draw(GameTime p_GameTime)
        {
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Fade everything with screen transitions
            Color typeColor = ColorHelper.AddAlpha(Color.White, Convert.ToByte(0.5f * this.TransitionAlpha));
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            // Darken everything behind this screen
            this.ScreenManager.DarkenScreen(this.TransitionAlpha, spriteBatch);

            // Draw credits
            s_TempVector.Y = this.Viewport.Height - this.ActiveFrames;
            foreach (CreditType creditType in this.m_Credits)
            {
                // Draw credit type string
                s_TempVector.X = (this.Viewport.Width - (c_DrawScale * m_Font.MeasureString(creditType.Title)).X) / 2;
                spriteBatch.DrawString(m_Font, creditType.Title, s_TempVector, typeColor, 0f, Vector2.Zero, c_DrawScale, SpriteEffects.None, 0f);
                s_TempVector.Y += m_CreditTypeHeight;

                foreach (string credit in creditType.Credits)
                {
                    // Draw credit string
                    s_TempVector.X = (this.Viewport.Width - (c_DrawScale * m_Font.MeasureString(credit).X)) / 2;
                    spriteBatch.DrawString(m_Font, credit, s_TempVector, color, 0f, Vector2.Zero, c_DrawScale, SpriteEffects.None, 0f);
                    s_TempVector.Y += m_CreditHeight;
                }

                // Apply spacing
                s_TempVector.Y += m_CreditTypeSpacing;
            }

            spriteBatch.End();
        }
    }
}
