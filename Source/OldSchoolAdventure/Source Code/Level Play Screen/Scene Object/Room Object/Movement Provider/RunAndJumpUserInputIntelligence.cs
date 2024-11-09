using Destiny;

namespace OSA
{
    public class RunAndJumpUserInputIntelligence : RunAndJumpMovementIntelligence
    {
        const int c_MaximumStartJumpLatency = 10;

        bool m_CanUseCurrentStartJump;
        int m_CurrentStartJumpLatency;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpUserInputIntelligence()
        {
        }

        /// <summary>
        /// Update movement actions.
        /// </summary>
        public override void Update(
            RoomObject p_RoomObject,
            RunAndJumpMovementProvider.Orientation p_Orientation,
            RunAndJumpMovementProvider.VerticalStates p_VerticalState,
            SurfaceInformation p_LeftSurroundings,
            SurfaceInformation p_TopSurroundings,
            SurfaceInformation p_RightSurroundings,
            SurfaceInformation p_BottomSurroundings,
            bool p_IsGroundInFrontOfFeetSolid
            )
        {
            // Get power
            CollectiblePower power = null;
            MainCharacter mainCharacter = p_RoomObject as MainCharacter;
            if (mainCharacter != null)
            {
                power = mainCharacter.Power;
            }

            InputState inputState = p_RoomObject.InputState;
            if (power != null && power.RunRight)
            {
                this.IsMoveLeft = false;
                this.IsMoveRight = true;
            }
            else
            {
                this.IsMoveLeft = inputState.IsMoveLeft();
                this.IsMoveRight = inputState.IsMoveRight();
            }
            this.IsStartJump = inputState.IsJump();
            this.IsStartOrContinueJump = inputState.IsContinueJump();
            this.IsStartAction = inputState.IsStartAction();
            this.IsFly = false;

            // Assess whether character can fly
            if (inputState.IsContinueJump())
            {
                this.IsFly = p_RoomObject.CanFly;
            }

            if (this.IsFly)
            {
                // If flying, cannot jump
                this.IsStartJump = false;
                this.IsStartOrContinueJump = false;
            }
            else
            {
                // If player pressed start jump previously, but their character wasn't in a position
                // to be able to jump, then start jump now
                if (this.IsStartJump)
                {
                    m_CurrentStartJumpLatency = 0;
                    m_CanUseCurrentStartJump = true;
                }
                if (this.IsStartOrContinueJump && !this.IsStartJump)
                {
                    m_CanUseCurrentStartJump =
                        m_CanUseCurrentStartJump &&
                        p_VerticalState != RunAndJumpMovementProvider.VerticalStates.Jumping &&
                        (++m_CurrentStartJumpLatency < c_MaximumStartJumpLatency);
                    this.IsStartJump = m_CanUseCurrentStartJump;
                }
            }
        }

        //private static bool GetIsMoveLeft(InputState p_InputState)
        //{
        //    return (
        //            p_InputState.IsKeyDown(Keys.Left) ||
        //            p_InputState.IsButtonDown(Buttons.DPadLeft) ||
        //            p_InputState.IsButtonDown(Buttons.LeftThumbstickLeft)
        //        ) &&
        //        !(
        //            p_InputState.IsKeyDown(Keys.Right) ||
        //            p_InputState.IsButtonDown(Buttons.DPadRight) ||
        //            p_InputState.IsButtonDown(Buttons.LeftThumbstickRight)
        //        );
        //}

        //private static bool GetIsMoveRight(InputState p_InputState)
        //{
        //    return (
        //            p_InputState.IsKeyDown(Keys.Right) ||
        //            p_InputState.IsButtonDown(Buttons.DPadRight) ||
        //            p_InputState.IsButtonDown(Buttons.LeftThumbstickRight)
        //        ) &&
        //        !(
        //            p_InputState.IsKeyDown(Keys.Left) ||
        //            p_InputState.IsButtonDown(Buttons.DPadLeft) ||
        //            p_InputState.IsButtonDown(Buttons.LeftThumbstickLeft)
        //        );
        //}

        //static bool GetIsStartJump(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) || p_InputState.IsNewButtonPress(Buttons.A);
        //}

        //static bool GetIsStartOrContinueJump(InputState p_InputState)
        //{
        //    return p_InputState.IsKeyDown(Keys.Space) || p_InputState.IsButtonDown(Buttons.A);
        //}

        //static bool GetIsStartAction(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Up) || p_InputState.IsNewButtonPress(Buttons.X);
        //}

        //static bool GetIsFly(InputState p_InputState)
        //{
        //    return p_InputState.IsKeyDown(Keys.Space) || p_InputState.IsButtonDown(Buttons.A);
        //}
    }
}
