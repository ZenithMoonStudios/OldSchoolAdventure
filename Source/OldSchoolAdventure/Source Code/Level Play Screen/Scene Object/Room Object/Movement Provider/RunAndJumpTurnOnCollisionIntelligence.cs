namespace OSA
{
    public class RunAndJumpTurnOnCollisionIntelligence : RunAndJumpMovementIntelligence
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpTurnOnCollisionIntelligence()
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
            bool isColliding = this.IsColliding(p_Orientation, p_LeftSurroundings, p_RightSurroundings);
            bool isTurnAround = isColliding;

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
        }
    }
}
