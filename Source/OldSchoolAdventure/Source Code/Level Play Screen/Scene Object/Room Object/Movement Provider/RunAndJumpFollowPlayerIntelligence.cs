namespace OSA
{
    public class RunAndJumpFollowPlayerIntelligence : RunAndJumpMovementIntelligence
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpFollowPlayerIntelligence()
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
            this.IsMoveLeft = false;
            this.IsMoveRight = false;

            float margin = 20f;
            float mainCharacterMidX = p_RoomObject.Room.MainCharacter.MidX;
            this.IsMoveLeft = (p_RoomObject.MidX > mainCharacterMidX + margin);
            this.IsMoveRight = (p_RoomObject.MidX < mainCharacterMidX - margin);
        }
    }
}
