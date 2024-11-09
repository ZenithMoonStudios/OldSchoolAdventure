using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Menu entry class.
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// Properties.
        /// </summary>
        MenuScreen m_Screen;
        string m_Text;
        bool m_IsValidInTrialMode;
        bool m_IsValidInProductionMode;
        bool m_IsValidIfNoSavedProfile;
        bool m_IsValidIfNoStorage;
        bool m_IsShowIfNotValid;
        bool m_IsValid;
        object m_Tag;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// Transition position.
        /// This is used to soften the transition between entries.
        /// 0 is deselected, 1 is selected.
        /// </summary>
        float m_TransitionPosition = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuEntry(string p_Text, MenuScreen p_Screen) : this(p_Text, p_Screen, null) { }
        public MenuEntry(string p_Text, MenuScreen p_Screen, object p_Tag) : this(p_Text, true, true, true, true, true, true, p_Screen, p_Tag) { }
        public MenuEntry(string p_Text, bool p_IsValidInTrialMode, bool p_IsValidInProductionMode, bool p_IsValidIfNoSavedProfile, bool p_IsValidIfNoStorage, bool p_IsShowIfNotValid, bool p_IsValid, MenuScreen p_Screen, object p_Tag)
        {
            m_Text = p_Text;
            m_Screen = p_Screen;
            m_IsValidInTrialMode = p_IsValidInTrialMode;
            m_IsValidInProductionMode = p_IsValidInProductionMode;
            m_IsValidIfNoSavedProfile = p_IsValidIfNoSavedProfile;
            m_IsValidIfNoStorage = p_IsValidIfNoStorage;
            m_IsShowIfNotValid = p_IsShowIfNotValid;
            m_IsValid = p_IsValid;
            m_Tag = p_Tag;
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Whether menu entry is valid in the current mode.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isTrialMode = GuideHelper.IsTrialMode;
                return m_IsValid
                    &&
                    (m_IsValidIfNoStorage || (this.IsStorageAvailable && (m_IsValidIfNoSavedProfile || this.IsSavedProfile)))
                    &&
                    ((isTrialMode && m_IsValidInTrialMode) || (!isTrialMode && m_IsValidInProductionMode));
            }
        }

        /// <summary>
        /// Whether storage is available for use.
        /// </summary>
        private bool IsStorageAvailable
        {
            get
            {
#if WINDOWS
                return true;
#else
				UserProfile profile = PlatformerGame.Instance.UserProfile;
				return profile != null && profile.IsLoaded && profile.IsStorageConnected;
#endif
            }
        }

        /// <summary>
        /// Whether there is a saved game.
        /// </summary>
        private bool IsSavedProfile
        {
            get
            {
                UserProfile profile = PlatformerGame.Instance.UserProfile;
                return profile != null && profile.IsLoaded && profile.SavedGameState != null;
            }
        }

        /// <summary>
        /// Whether menu entry should be displayed in the current mode.
        /// </summary>
        public bool CanDraw { get { return m_IsShowIfNotValid || this.IsValid; } }

        /// <summary>
        /// Selected event.
        /// </summary>
        public delegate void SelectionHandler(object p_Tag);
        public event SelectionHandler Selected;

        /// <summary>
        /// Raise selected event.
        /// </summary>
        protected internal virtual void OnSelection()
        {
            if (this.Selected != null)
            {
                this.Selected(m_Tag);
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="p_IsSelected">Whether this entry is selected.</param>
        /// <param name="p_GameTime">Game time.</param>
        public virtual void Update(bool p_IsSelected, GameTime p_GameTime)
        {
            // Update transition position
            // Using this formula, a full transition takes 0.25 seconds
            float delta = (float)p_GameTime.ElapsedGameTime.TotalSeconds * 4;
            if (p_IsSelected)
            {
                m_TransitionPosition += delta;
                if (m_TransitionPosition > 1)
                {
                    m_TransitionPosition = 1;
                }
            }
            else
            {
                m_TransitionPosition -= delta;
                if (m_TransitionPosition < 0)
                {
                    m_TransitionPosition = 0;
                }
            }
        }

        /// <summary>
        /// Draw.
        /// </summary>
        /// <param name="p_Screen">Menu screen.</param>
        /// <param name="p_Position">Position.</param>
        /// <param name="p_IsSelected">Whether the entry is selected.</param>
        /// <param name="p_GameTime">Game time.</param>
        public virtual void Draw(Vector2 p_Position, bool p_IsSelected, GameTime p_GameTime)
        {
            position = p_Position;
            Color deselectedColor = m_Screen.MenuEntryColor;
            Color selectedColor = m_Screen.SelectedMenuEntryColor;
            Color color = m_Screen.DisabledMenuEntryColor;
            if (this.IsValid)
            {
                // Fade text on screen transitions
                color.R = Convert.ToByte(deselectedColor.R + m_TransitionPosition * (selectedColor.R - deselectedColor.R));
                color.G = Convert.ToByte(deselectedColor.G + m_TransitionPosition * (selectedColor.G - deselectedColor.G));
                color.B = Convert.ToByte(deselectedColor.B + m_TransitionPosition * (selectedColor.B - deselectedColor.B));
                color.A = m_Screen.TransitionAlpha;
            }
            m_Screen.ScreenManager.SpriteBatch.DrawString(m_Screen.EntryFont, m_Text, p_Position, color);
        }

        /// <summary>
        /// Get size of this menu entry.
        /// </summary>
        /// <param name="p_Screen">Menu screen.</param>
        /// <returns>Size of this menu entry.</returns>
        public Vector2 GetSize(MenuScreen p_Screen)
        {
            return p_Screen.EntryFont.MeasureString(m_Text);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.EntryFont.LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.EntryFont.MeasureString(m_Text).X;
        }
    }

    /// <summary>
    /// Menu entry collection class.
    /// </summary>
    public class MenuEntryList : List<MenuEntry>
    {
    }
}
