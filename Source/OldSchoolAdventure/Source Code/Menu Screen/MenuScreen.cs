using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace OSA
{
    /// <summary>
    /// Menu screen base class.
    /// </summary>
    public abstract class MenuScreen : Screen
    {
        /// <summary>
        /// Resources.
        /// </summary>
        Texture2D m_MenuStartTexture;
        Texture2D m_MenuItemTexture;
        Texture2D m_MenuSelectedItemTexture;
        Texture2D m_MenuFinishTexture;

        /// <summary>
        /// State.
        /// </summary>
        string m_Title;
        int m_SelectedIndex;

        // the number of pixels to pad above and below menu entries for touch input
        const int menuEntryPadding = 10;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_Title">Title of the menu.</param>
        public MenuScreen(string p_Title)
        {
            m_Title = p_Title;
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Menu Screen";
            this.CanCancel = true;

            this.Entries = new MenuEntryList();
#if (MOBILE)
			m_SelectedIndex = -1;
#endif
        }

        /// <summary>
        /// Entries.
        /// </summary>
        protected MenuEntryList Entries { get; private set; }

        /// <summary>
        /// Fonts.
        /// </summary>
        public SpriteFont EntryFont { get; private set; }

        /// <summary>
        /// Colors.
        /// </summary>
        public virtual Color MenuEntryColor { get { return Color.White; } }
        public virtual Color SelectedMenuEntryColor { get { return Color.White; } }
        public virtual Color DisabledMenuEntryColor { get { return new Color(127, 127, 127); } }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load textures
            m_MenuStartTexture = this.ContentManager.Load<Texture2D>("MenuBackground_Start");
            m_MenuItemTexture = this.ContentManager.Load<Texture2D>("MenuBackground_Item");
            m_MenuSelectedItemTexture = this.ContentManager.Load<Texture2D>("MenuBackground_ItemSelected");
            m_MenuFinishTexture = this.ContentManager.Load<Texture2D>("MenuBackground_Finish");

            // Load fonts
            this.EntryFont = this.ContentManager.Load<SpriteFont>("OptionFont");
        }

        #region Input

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                0,
                (int)entry.Position.Y - menuEntryPadding,
                ScreenManager.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this) + (menuEntryPadding * 2));
        }

        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        /// <param name="p_InputState">Input state.</param>
        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in p_InputState.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // iterate the entries to see if any were tapped
                    for (int i = 0; i < this.Entries.Count; i++)
                    {
                        MenuEntry menuEntry = this.Entries[i];

                        if (GetMenuEntryHitBounds(menuEntry).Contains(tapLocation))
                        {
                            // select the entry. since gestures are only available on Windows Phone,
                            // we can safely pass PlayerIndex.One to all entries since there is only
                            // one player on Windows Phone.
                            if (this.Entries[i].IsValid) this.OnSelection(i);
                        }
                    }
                }
            }
#if (!MOBILE)
            // If currently on an invalid record, go to the next one
            while (!this.Entries[m_SelectedIndex].IsValid)
            {
                m_SelectedIndex++;
                if (m_SelectedIndex > this.LastEntryIndex)
                {
                    m_SelectedIndex = 0;
                }
            }

            // Move up and down
            bool isUp = p_InputState.IsMenuUp();
            bool isDown = p_InputState.IsMenuDown();
            if (isUp && !isDown)
            {
                do
                {
                    m_SelectedIndex--;
                    if (m_SelectedIndex < 0)
                    {
                        m_SelectedIndex = this.LastEntryIndex;
                    }
                } while (!this.Entries[m_SelectedIndex].IsValid);

                // Play audio
                this.AudioManager.PlayCue("MenuChange");
            }
            if (isDown && !isUp)
            {
                do
                {
                    m_SelectedIndex++;
                    if (m_SelectedIndex > this.LastEntryIndex)
                    {
                        m_SelectedIndex = 0;
                    }
                } while (!this.Entries[m_SelectedIndex].IsValid);

                // Play audio
                this.AudioManager.PlayCue("MenuChange");
            }

            // Proceed or cancel
            if (p_InputState.IsAccept())
            {
                this.OnSelection(m_SelectedIndex);

            }

            else
#endif
            if (this.CanCancel && p_InputState.IsCancel())
            {
                this.OnCancel();

                // Play audio
                this.AudioManager.PlayCue("MenuBack");
            }
        }

        ///// <summary>
        ///// Determine whether the input state is requesting to move up the menu.
        ///// </summary>
        ///// <param name="p_InputState">Input state.</param>
        ///// <returns>Whether the input state is requesting to move up the menu.</returns>
        //private bool IsUp(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Up) ||
        //           p_InputState.IsNewButtonPress(Buttons.DPadUp) ||
        //           p_InputState.IsNewButtonPress(Buttons.LeftThumbstickUp);
        //}

        ///// <summary>
        ///// Determine whether the input state is requesting to move down the menu.
        ///// </summary>
        ///// <param name="p_InputState">Input state.</param>
        ///// <returns>Whether the input state is requesting to move down the menu.</returns>
        //private bool IsDown(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Down) ||
        //           p_InputState.IsNewButtonPress(Buttons.DPadDown) ||
        //           p_InputState.IsNewButtonPress(Buttons.LeftThumbstickDown);
        //}

        ///// <summary>
        ///// Determine whether the input state is requesting to select the current menu item.
        ///// </summary>
        ///// <param name="p_InputState">Input state.</param>
        ///// <returns>Whether the input state is requesting to select the current menu item.</returns>
        //private bool IsSelect(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) ||
        //           p_InputState.IsNewKeyPress(Keys.Enter) ||
        //           p_InputState.IsNewButtonPress(Buttons.A) ||
        //           p_InputState.IsNewButtonPress(Buttons.Start);
        //}

        ///// <summary>
        ///// Determine whether the input state is requesting to cancel the current menu.
        ///// </summary>
        ///// <param name="p_InputState">Input state.</param>
        ///// <returns>Whether the input state is requesting to cancel the current menu.</returns>
        //private bool IsCancel(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) ||
        //           p_InputState.IsNewButtonPress(Buttons.B) ||
        //           p_InputState.IsNewButtonPress(Buttons.Back);
        //}

        #endregion

        /// <summary>
        /// On selection handler.
        /// </summary>
        protected virtual void OnSelection(int p_SelectedIndex)
        {
            this.Entries[p_SelectedIndex].OnSelection();
            // Play audio
            this.AudioManager.PlayCue("MenuSelect");

        }

        /// <summary>
        /// Whether can cancel.
        /// </summary>
        protected virtual bool CanCancel { get; set; }

        ///// <summary>
        ///// On cancel helper.
        ///// </summary>
        //protected void OnCancel(object p_Tag)
        //{
        //    this.OnCancel();
        //}

        /// <summary>
        /// Update screen.
        /// Called regardless of screen state.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_HasFocus">Whether screen has focus.</param>
        /// <param name="p_IsCoveredByOtherScreen">Whether screen is covered by another screen.</param>
        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            // Call base class to handle transitions
            base.Update(p_GameTime, p_HasFocus, p_IsCoveredByOtherScreen);

            // Update profile state
            UserProfile profile = PlatformerGame.Instance.UserProfile;

            // Update menu entries
            int lastCanDrawIndex = -1;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                MenuEntry entry = this.Entries[i];

                bool isSelected = (i == m_SelectedIndex);
                bool canDraw = entry.CanDraw;

                entry.Update(isSelected, p_GameTime);

                if (canDraw)
                {
                    lastCanDrawIndex = i;
                }
                else if (isSelected)
                {
                    if (lastCanDrawIndex >= 0)
                    {
                        m_SelectedIndex = lastCanDrawIndex;
                    }
                    else
                    {
                        m_SelectedIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Draw screen.
        /// </summary>
        Rectangle m_DrawBackgroundRegion;
        Vector2 m_DrawItemPosition;
        public override void Draw(GameTime p_GameTime)
        {
            // Start drawing sprites
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // Define basic colours
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            // Determine the number of entries
            int numberOfVisibleItems = 0;
            int MaxLineWidth = 0;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (this.Entries[i].CanDraw)
                {
                    numberOfVisibleItems++;

                }
                MaxLineWidth = this.Entries[i].GetWidth(this) > MaxLineWidth ? this.Entries[i].GetWidth(this) : MaxLineWidth;
            }

            // Determine draw start position
#if MOBILE

			m_DrawBackgroundRegion.Y = (this.Viewport.Height - (numberOfVisibleItems * EntryFont.LineSpacing) - m_MenuStartTexture.Height - m_MenuFinishTexture.Height) - EntryFont.LineSpacing ;
#else
            m_DrawBackgroundRegion.Y = Convert.ToInt32(0.9f * this.Viewport.Height) - (numberOfVisibleItems * m_MenuItemTexture.Height) - m_MenuStartTexture.Height - m_MenuFinishTexture.Height;
#endif

            // Draw menu top
            m_DrawBackgroundRegion.X = (this.Viewport.Width - (MaxLineWidth + 40)) / 2;
            m_DrawBackgroundRegion.Width = MaxLineWidth + 40;
            m_DrawBackgroundRegion.Height = numberOfVisibleItems * EntryFont.LineSpacing;
            spriteBatch.Draw(m_MenuStartTexture, m_DrawBackgroundRegion, color);

            m_DrawBackgroundRegion.Y += m_MenuStartTexture.Height;

            // Draw entries
            for (int i = 0; i < this.Entries.Count; i++)
            {
                MenuEntry entry = this.Entries[i];
                if (entry.CanDraw)
                {
#if (!MOBILE)
                    bool isSelected = (i == m_SelectedIndex);
#else
					bool isSelected = false;
#endif

                    // Draw item background
#if MOBILE
					m_DrawBackgroundRegion.Height = EntryFont.LineSpacing;
#else
                    m_DrawBackgroundRegion.Height = m_MenuItemTexture.Height;
#endif
                    spriteBatch.Draw(
                        isSelected ? m_MenuSelectedItemTexture : m_MenuItemTexture,
                        m_DrawBackgroundRegion,
                        color
                        );

                    // Draw item text, centered on the middle of the line
                    m_DrawItemPosition.Y = m_DrawBackgroundRegion.Y + 5 - (m_DrawBackgroundRegion.Height / 5);
                    m_DrawItemPosition.X = (this.Viewport.Width - entry.GetSize(this).X) / 2;
                    entry.Draw(m_DrawItemPosition, isSelected, p_GameTime);

                    // Move to the next item
                    m_DrawBackgroundRegion.Y += m_DrawBackgroundRegion.Height;
                }
            }

            // Draw menu bottom
            m_DrawBackgroundRegion.Height = m_MenuFinishTexture.Height;
            spriteBatch.Draw(m_MenuFinishTexture, m_DrawBackgroundRegion, color);

            // Finish drawing sprites
            spriteBatch.End();
        }

        /// <summary>
        /// Last entry index.
        /// </summary>
        int LastEntryIndex { get { return this.Entries.Count - 1; } }
    }
}
