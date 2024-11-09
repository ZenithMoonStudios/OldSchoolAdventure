namespace OSA
{
    /// <summary>
    /// Run and jump movement intelligence.
    /// </summary>
    public abstract class RunAndJumpMovementIntelligence
    {
        public bool IsMoveLeft { get; protected set; }
        public bool IsMoveRight { get; protected set; }
        public bool IsStartJump { get; protected set; }
        public bool IsStartOrContinueJump { get; protected set; }
        public bool IsStartAction { get; protected set; }
        public bool IsFly { get; protected set; }

        /// <summary>
        /// Update movement actions.
        /// </summary>
        public abstract void Update(
            RoomObject p_RoomObject,
            RunAndJumpMovementProvider.Orientation p_Orientation,
            RunAndJumpMovementProvider.VerticalStates p_VerticalState,
            SurfaceInformation p_LeftSurroundings,
            SurfaceInformation p_TopSurroundings,
            SurfaceInformation p_RightSurroundings,
            SurfaceInformation p_BottomSurroundings,
            bool p_IsGroundInFrontOfFeetSolid
            );

        public bool IsColliding(
            RunAndJumpMovementProvider.Orientation p_Orientation,
            SurfaceInformation p_LeftSurroundings,
            SurfaceInformation p_RightSurroundings
            )
        {
            return (p_Orientation == RunAndJumpMovementProvider.Orientation.Left && p_LeftSurroundings.IsSolid)
                || (p_Orientation == RunAndJumpMovementProvider.Orientation.Right && p_RightSurroundings.IsSolid);
        }
    }
}
