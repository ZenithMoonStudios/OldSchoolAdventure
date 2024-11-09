using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny
{
    /// <summary>
    /// Screen base class.
    /// </summary>
    public abstract class Screen
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Screen()
        {
            this.IsPopup = false;
            this.CanHaveFocus = true;
            this.IsExiting = false;
            this.TransitionOnTime = TimeSpan.Zero;
            this.TransitionOffTime = TimeSpan.Zero;
            this.TransitionPosition = 0;
            this.ActiveFrames = 0;
        }

        public ScreenManager ScreenManager { get; set; }
        public ScreenState State { get; protected set; }
        public InputState InputState { get { return this.ScreenManager.InputState; } }
        public AudioManager AudioManager { get { return this.ScreenManager.AudioManager; } }

        /// <summary>
        /// Each screen has its own content manager, to minimize memory when
        /// a screen isn't being used.
        /// </summary>
        public ContentManager ContentManager { get; protected set; }
        public string ContentRelativeFolder { get; protected set; }

        public bool IsPopup { get; protected set; }
        public bool IsExiting { get; set; }
        public bool HasFocus { get; protected set; }
        public bool CanHaveFocus { get; protected set; }

        public TimeSpan TransitionOnTime { get; protected set; }
        public TimeSpan TransitionOffTime { get; protected set; }

        /// <summary>
        /// 1 is completely on, 0 is completely off.
        /// </summary>
        public float TransitionPosition { get; protected set; }

        /// <summary>
        /// 255 is completely visible, 0 is completely invisible.
        /// </summary>
        public byte TransitionAlpha { get { return Convert.ToByte(255 * TransitionPosition); } }

        /// <summary>
        /// Number of active frames for this screen.
        /// </summary>
        public int ActiveFrames { get; private set; }

        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set
            {
                enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourself.
                if (State == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        GestureType enabledGestures = GestureType.None;

        /// <summary>
        /// Load content.
        /// </summary>
        public virtual void LoadContent()
        {
            // Load the content manager for this screen
            if (this.ContentManager == null)
            {
                string contentPath = this.Game.Content.RootDirectory;
                if (!string.IsNullOrEmpty(this.ContentRelativeFolder))
                {
                    contentPath = Path.Combine(contentPath, this.ContentRelativeFolder);
                }
                this.ContentManager = new ContentManager(this.Game.Services, contentPath);
            }
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public virtual void UnloadContent()
        {
            this.ContentManager.Unload();
            this.ContentManager.Dispose();
            this.ContentManager = null;
        }

        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        public virtual void HandleInput(InputState p_InputState)
        {
            if (p_InputState.IsCancel())
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Update screen.
        /// Called regardless of screen state.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_HasFocus">Whether screen has focus.</param>
        /// <param name="p_IsCoveredByOtherScreen">Whether screen is covered by another screen.</param>
        public virtual void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            // Store whether screen has focus
            this.HasFocus = p_HasFocus;


            // Update state
            if (this.IsExiting)
            {
                // If exiting, transition off then remove screen
                this.State = ScreenState.TransitionOff;
                if (!this.UpdateTransition(p_GameTime, false))
                {
                    // If transition has finished, remove screen
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (p_IsCoveredByOtherScreen)
            {
                // If screen is covered by another, transition off
                if (this.State != ScreenState.Hidden)
                {
                    this.State = ScreenState.TransitionOff;
                    if (!this.UpdateTransition(p_GameTime, false))
                    {
                        // If transition has finished, set to hidden
                        this.State = ScreenState.Hidden;
                    }
                }
            }
            else
            {
                // Otherwise, transition on and become active
                if (this.State != ScreenState.Active)
                {
                    this.State = ScreenState.TransitionOn;
                    if (!this.UpdateTransition(p_GameTime, true))
                    {
                        // If transition has finished, set to active
                        this.State = ScreenState.Active;
                    }
                }
            }

            // If screen has focus, increment its active frame time
            if (this.HasFocus)
            {
                this.ActiveFrames++;
            }
        }

        /// <summary>
        /// Draw screen.
        /// </summary>
        public abstract void Draw(GameTime p_GameTime);

        /// <summary>
        /// Exit screen.
        /// If screen is active, transitions off first.
        /// </summary>
        public void ExitScreen()
        {
            if (this.TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                this.IsExiting = true;
            }
        }

        /// <summary>
        /// Whether the state is transition on or active.
        /// </summary>
        protected bool IsStateTransitionOnOrActive
        {
            get { return this.State == ScreenState.TransitionOn || this.State == ScreenState.Active; }
        }

        /// <summary>
        /// Public accessors.
        /// </summary>
        public SpriteBatch SpriteBatch { get { return this.ScreenManager.SpriteBatch; } }

        /// <summary>
        /// Protected accessors.
        /// </summary>
        protected Game Game { get { return this.ScreenManager.Game; } }
        protected Viewport Viewport { get { return this.ScreenManager.GraphicsDevice.Viewport; } }

        /// <summary>
        /// Update transition position.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_IsTransitioningOn">Whether the transition is to an active state.</param>
        /// <returns>Whether the transition is still in progress.</returns>
        private bool UpdateTransition(GameTime p_GameTime, bool p_IsTransitioningOn)
        {
            // Whether the transition is complete
            bool isComplete = false;

            // Establish transition-specific parameters
            int direction = p_IsTransitioningOn ? 1 : -1;
            TimeSpan time = p_IsTransitioningOn ? this.TransitionOnTime : this.TransitionOffTime;

            // Determine how much the transition should move
            float transitionDelta = 1;
            if (time != TimeSpan.Zero)
            {
                transitionDelta = Convert.ToSingle(p_GameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
            }

            // Update position
            this.TransitionPosition += (direction * transitionDelta);

            // If transition is complete, ensure that the position stays within the bounds
            if (direction > 0 && this.TransitionPosition >= 1)
            {
                this.TransitionPosition = 1;
                isComplete = true;
            }
            else if (direction < 0 && this.TransitionPosition <= 0)
            {
                this.TransitionPosition = 0;
                isComplete = true;
            }

            // Return whether the transition is still in progress
            return !isComplete;
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }
    }

    /// <summary>
    /// Screen collection class.
    /// </summary>
    public class ScreenList : List<Screen>
    {
    }

    /// <summary>
    /// Screen stack class.
    /// </summary>
    public class ScreenStack : Stack<Screen>
    {
    }
}
