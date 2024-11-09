namespace OSA
{
    public class RunAndJumpJumpOverHoleIntelligence : RunAndJumpMovementIntelligence
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpJumpOverHoleIntelligence()
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
            bool isStanding = (p_VerticalState == RunAndJumpMovementProvider.VerticalStates.Standing);
            bool isHole = (p_VerticalState == RunAndJumpMovementProvider.VerticalStates.Standing && !p_IsGroundInFrontOfFeetSolid);
            bool isTurnAround = this.IsColliding(p_Orientation, p_LeftSurroundings, p_RightSurroundings);
            bool isJump = !isStanding || (isStanding && isHole);
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
            this.IsStartJump = isJump;
            this.IsStartOrContinueJump = isJump;
        }
    }
}
