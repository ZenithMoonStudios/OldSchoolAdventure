using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#if(MOBILE)
using AccelerometerHelper;
#endif

namespace Destiny
{
    /// <summary>
    /// Input state class.
    /// </summary>
    public class InputState
    {

#if (!MOBILE)
        const int c_MaximumNumberOfPlayers = 4;
#else
		const int c_MaximumNumberOfPlayers = 1;
#endif
        const float c_ThumbStickButtonDownThreshold = 0.4f;

        KeyboardState m_KeyboardState = new KeyboardState();
        KeyboardState m_PreviousKeyboardState = new KeyboardState();
        GamePadState[] m_GamePadStates = new GamePadState[c_MaximumNumberOfPlayers];
        GamePadState[] m_PreviousGamePadStates = new GamePadState[c_MaximumNumberOfPlayers];

        public readonly List<GestureSample> Gestures = new List<GestureSample>();
        public TouchCollection TouchState;
        public Vector3 Prev_AccelerometerReading;
        public Vector3 AccelerometerReading;

        public PlayerIndex? ActivePlayerIndex { get; set; }
        public bool IsActivePlayerUsingKeyboard { get; set; }
        public bool IsActivePlayerUsingSignedInProfile { get; set; }
        public bool IsActivePlayerUsingGuestProfile { get; set; }
        public bool IsActivePlayerUsingPhone { get; set; }
        public bool IsActivePlayerConnected { get { return this.IsActivePlayerUsingKeyboard || this.IsActivePlayerUsingPhone || this.ActivePlayerIndex == null || m_GamePadStates[(int)this.ActivePlayerIndex].IsConnected; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputState()
        {
            this.ActivePlayerIndex = PlayerIndex.One;
            this.IsActivePlayerUsingKeyboard = false;
            this.IsActivePlayerUsingSignedInProfile = false;
            this.IsActivePlayerUsingGuestProfile = false;
#if (MOBILE)
			this.IsActivePlayerUsingPhone = true;
			this.ActivePlayerIndex = PlayerIndex.One;
			AccelerometerHelper.AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
			AccelerometerHelper.AccelerometerHelper.Instance.Active = true;
#endif
        }
#if (MOBILE)
		private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
		{
			AccelerometerReading.X = e.OptimalyFilteredAcceleration.X;
			AccelerometerReading.Y = e.OptimalyFilteredAcceleration.Y;
			AccelerometerReading.Z = e.OptimalyFilteredAcceleration.Z;
		}
#endif
        /// <summary>
        /// Update state.
        /// </summary>
        public void Update()
        {
#if (MOBILE)
			// update our virtual thumbsticks
			VirtualThumbsticks.Update();
#endif
            m_PreviousKeyboardState = m_KeyboardState;
            m_KeyboardState = Keyboard.GetState();

            for (int i = 0; i < c_MaximumNumberOfPlayers; i++)
            {
                m_PreviousGamePadStates[i] = m_GamePadStates[i];
                m_GamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

#if (MOBILE)

			Prev_AccelerometerReading = AccelerometerReading;

			TouchState = TouchPanel.GetState();

			Gestures.Clear();
			while (TouchPanel.IsGestureAvailable)
			{
				Gestures.Add(TouchPanel.ReadGesture());
			}

			// if the user is moving the right thumbstick a bit
			if (VirtualThumbsticks.RightThumbstick.Length() > .1f)
			{
				AccelerometerReading.X = VirtualThumbsticks.RightThumbstick.X;
				AccelerometerReading.Y = VirtualThumbsticks.RightThumbstick.Y;

			}
			// if the user isn't moving the right thumbstick enough, update our
			// rotation based on the left thumstick
			else if (VirtualThumbsticks.LeftThumbstick.Length() > .1f)
			{
				AccelerometerReading.X = VirtualThumbsticks.LeftThumbstick.X;
				AccelerometerReading.Y = VirtualThumbsticks.LeftThumbstick.Y;
			}
#endif
        }

        /// <summary>
        /// Determine whether the Shift key is down.
        /// </summary>
        public bool IsShiftKeyDown()
        {
            return (m_KeyboardState.IsKeyDown(Keys.LeftShift) || m_KeyboardState.IsKeyDown(Keys.RightShift));
        }

        /// <summary>
        /// Get all new key presses.
        /// </summary>
        public KeysList GetNewKeyPresses()
        {
            KeysList result = new KeysList();
            foreach (Keys key in m_KeyboardState.GetPressedKeys())
            {
                if (m_PreviousKeyboardState.IsKeyUp(key))
                {
                    result.Add(key);
                }
            }
            return result;
        }

        /// <summary>
        /// Determine whether the specified key was pressed by any player.
        /// </summary>
        private bool IsNewKeyPress(Keys p_Key)
        {
            return m_KeyboardState.IsKeyDown(p_Key) &&
                               m_PreviousKeyboardState.IsKeyUp(p_Key);
        }

        /// <summary>
        /// Determine whether the specified key is being pressed by any player.
        /// </summary>
        private bool IsKeyDown(Keys p_Key)
        {
            return m_KeyboardState.IsKeyDown(p_Key);
        }

        /// <summary>
        /// Determine whether the specified button was pressed by any player.
        /// </summary>
        private bool IsNewButtonPress(Buttons p_Button) { return this.IsNewButtonPress(p_Button, true); }
        private bool IsNewButtonPress(Buttons p_Button, bool p_ActivePlayerOnly)
        {
            bool result = false;
            for (int i = 0; i < c_MaximumNumberOfPlayers; i++)
            {
                if (!p_ActivePlayerOnly || i == (int)this.ActivePlayerIndex)
                {
                    if (this.IsNewButtonPress(p_Button, (PlayerIndex)i))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determine whether the specified button was pressed by the specified player.
        /// </summary>
        private bool IsNewButtonPress(Buttons p_Button, PlayerIndex p_PlayerIndex)
        {
            int playerIndex = (int)p_PlayerIndex;
            return this.IsButtonDown(p_Button, m_GamePadStates[playerIndex]) &&
                   !this.IsButtonDown(p_Button, m_PreviousGamePadStates[playerIndex]);
        }

        /// <summary>
        /// Determine whether the specified button is being pressed by any player.
        /// </summary>
        private bool IsButtonDown(Buttons p_Button) { return this.IsButtonDown(p_Button, true); }
        private bool IsButtonDown(Buttons p_Button, bool p_ActivePlayerOnly)
        {
            bool result = false;
            for (int i = 0; i < c_MaximumNumberOfPlayers; i++)
            {
                if (!p_ActivePlayerOnly || i == (int)this.ActivePlayerIndex)
                {
                    if (this.IsButtonDown(p_Button, (PlayerIndex)i))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determine whether the specified button is being pressed by the specified player.
        /// </summary>
        private bool IsButtonDown(Buttons p_Button, PlayerIndex p_PlayerIndex)
        {
            int playerIndex = (int)p_PlayerIndex;
            return this.IsButtonDown(p_Button, m_GamePadStates[playerIndex]);
        }

        private bool IsButtonDown(Buttons p_Button, GamePadState p_GamePadState)
        {
            bool result = false;
            switch (p_Button)
            {
                case Buttons.LeftThumbstickLeft: result = (p_GamePadState.ThumbSticks.Left.X < -c_ThumbStickButtonDownThreshold); break;
                case Buttons.LeftThumbstickRight: result = (p_GamePadState.ThumbSticks.Left.X > c_ThumbStickButtonDownThreshold); break;
                case Buttons.LeftThumbstickUp: result = (p_GamePadState.ThumbSticks.Left.Y > c_ThumbStickButtonDownThreshold); break;
                case Buttons.LeftThumbstickDown: result = (p_GamePadState.ThumbSticks.Left.Y < -c_ThumbStickButtonDownThreshold); break;
                case Buttons.RightThumbstickLeft: result = (p_GamePadState.ThumbSticks.Right.X < -c_ThumbStickButtonDownThreshold); break;
                case Buttons.RightThumbstickRight: result = (p_GamePadState.ThumbSticks.Right.X > c_ThumbStickButtonDownThreshold); break;
                case Buttons.RightThumbstickUp: result = (p_GamePadState.ThumbSticks.Right.Y > c_ThumbStickButtonDownThreshold); break;
                case Buttons.RightThumbstickDown: result = (p_GamePadState.ThumbSticks.Right.Y < -c_ThumbStickButtonDownThreshold); break;
                default: result = p_GamePadState.IsButtonDown(p_Button); break;
            }
            return result;
        }

        /// <summary>
        /// Determine whether the input state is requesting to cancel the dialog.
        /// </summary>
        public bool IsCancel()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.B) || IsNewButtonPress(Buttons.Back);
#else
            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Tap)
				{
					return true;
				}
			}
			return false;
#endif
        }

        public bool IsKeyboardActive()
        {
            return IsActivePlayerUsingKeyboard;
        }

        /// <summary>
        /// Determine whether the input state is requesting to cancel the dialog.
        /// </summary>
        public bool IsAccept()
        {
#if (!MOBILE)
            if (!IsActivePlayerUsingKeyboard)
            {
                IsActivePlayerUsingKeyboard = IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter);
            }

            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Tap)
				{
					return true;
				}
			}
			return false;
#endif
        }

        /// <summary>
        /// Determine whether the input state is requesting to cancel the dialog.
        /// </summary>
        public bool IsAcceptAll()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Space) ||
                    IsButtonDown(Buttons.Start, false) ||
                    IsButtonDown(Buttons.A, false);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Tap)
				{
					return true;
				}
			}
			return false;
#endif
        }



        public bool IsEnd()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Escape)
                || IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.B);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Tap)
				{
					return true;
				}
			}
			return false;
#endif
        }

        public bool IsMoveRight()
        {
#if (!MOBILE)
            return (
                    IsKeyDown(Keys.Right) ||
                    IsButtonDown(Buttons.DPadRight) ||
                    IsButtonDown(Buttons.LeftThumbstickRight)
                ) &&
                !(
                    IsKeyDown(Keys.Left) ||
                    IsButtonDown(Buttons.DPadLeft) ||
                    IsButtonDown(Buttons.LeftThumbstickLeft)
                );
#else

			if (AccelerometerReading.X > 0f)
				return true;

			//foreach (var item in TouchState)
			//{
			//    TouchLocation prevtouch;
			//    item.TryGetPreviousLocation(out prevtouch);
			//    if (item.Position.X > prevtouch.Position.X) return true;
			//}
			return false;
#endif
        }


        public bool IsMoveLeft()
        {
#if (!MOBILE)
            return (
                IsKeyDown(Keys.Left) ||
                IsButtonDown(Buttons.DPadLeft) ||
                IsButtonDown(Buttons.LeftThumbstickLeft)
            ) &&
            !(
                IsKeyDown(Keys.Right) ||
                IsButtonDown(Buttons.DPadRight) ||
                IsButtonDown(Buttons.LeftThumbstickRight)
            );
#else
			if (AccelerometerReading.X < 0f)
				return true;
			//foreach (var item in TouchState)
			//{
			//    TouchLocation prevtouch;
			//    item.TryGetPreviousLocation(out prevtouch);
			//    if (item.Position.X < prevtouch.Position.X) return true;
			//}
			return false;
#endif
        }

        public bool IsJump()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Space) || IsNewButtonPress(Buttons.A);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Flick)
				{
					return true;
				}
			}
			return false;
#endif

        }

        public bool IsContinueJump()
        {
#if (!MOBILE)
            return IsKeyDown(Keys.Space) || IsButtonDown(Buttons.A);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Hold)
				{
					return true;
				}
			}
			return false;
#endif

        }


        public bool IsStartAction()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.X);
#else
			// look for any taps that occurred and select any entries that were tapped
			foreach (GestureSample gesture in Gestures)
			{
				if (gesture.GestureType == GestureType.Tap)
				{
					return true;
				}
			}
			return false;
#endif

        }

        public bool IsMenuUp()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Up) ||
                   IsNewButtonPress(Buttons.DPadUp) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp);
#else
			return false;
#endif
        }

        public bool IsMenuDown()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Down) ||
                   IsNewButtonPress(Buttons.DPadDown) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown);
#else
			return false;
#endif
        }

        public bool IsPause()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.Escape) ||
                    IsNewButtonPress(Buttons.Start);
#else
			return IsNewButtonPress(Buttons.Back);
			
#endif
        }

        public bool IsOpenCommandConsole()
        {
#if (!MOBILE)
            return IsNewKeyPress(Keys.F1);
#else
			//// look for any taps that occurred and select any entries that were tapped
			//foreach (GestureSample gesture in Gestures)
			//{
			//    if (gesture.GestureType == GestureType.DoubleTap)
			//    {
			//        return true;
			//    }
			//}
			return false;

#endif
        }
    }
}
