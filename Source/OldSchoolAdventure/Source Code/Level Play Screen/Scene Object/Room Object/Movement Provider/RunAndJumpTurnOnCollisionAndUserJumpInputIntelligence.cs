using Destiny;

namespace OSA
{
    public class RunAndJumpTurnOnCollisionAndUserJumpInputIntelligence : RunAndJumpMovementIntelligence
    {
        private const int c_MaximumStartJumpLatency = 10;

        private bool m_CanUseCurrentStartJump = false;
        private int m_CurrentStartJumpLatency = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpTurnOnCollisionAndUserJumpInputIntelligence()
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
            InputState inputState = p_RoomObject.InputState;
            this.IsStartJump = inputState.IsJump();
            this.IsStartOrContinueJump = inputState.IsContinueJump();

            // Turn on collision
            bool isColliding = this.IsColliding(p_Orientation, p_LeftSurroundings, p_RightSurroundings);
            bool isStanding = (p_VerticalState == RunAndJumpMovementProvider.VerticalStates.Standing);
            bool isTurnAround = isColliding && (isStanding || this.IsStartJump);
            if (p_Orientation == RunAndJumpMovementProvider.Orientation.Left)
            {
                this.IsMoveLeft = !isTurnAround;
                this.IsMoveRight = isTurnAround;
            }
            else
            {
                this.IsMoveLeft = isTurnAround;
                this.IsMoveRight = !isTurnAround;
            }

            // Jump according to user input
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

        //static bool GetIsStartJump(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) || p_InputState.IsNewButtonPress(Buttons.A);
        //}

        //static bool GetIsStartOrContinueJump(InputState p_InputState)
        //{
        //    return p_InputState.IsKeyDown(Keys.Space) || p_InputState.IsButtonDown(Buttons.A);
        //}
    }
}
