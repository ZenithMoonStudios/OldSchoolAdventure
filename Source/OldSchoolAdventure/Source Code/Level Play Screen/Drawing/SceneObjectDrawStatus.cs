namespace OSA
{
    /// <summary>
    /// Scene object draw status class.
    /// </summary>
    public class SceneObjectDrawStatus
    {
        public string Action { get; private set; }
        public int ActionStartFrame { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SceneObjectDrawStatus()
        {
        }

        /// <summary>
        /// Update the action.
        /// </summary>
        /// <param name="p_Action">Action.</param>
        /// <param name="p_Frame">Frame number.</param>
        public void Update(string p_Action, int p_Frame)
        {
            if (this.Action != p_Action)
            {
                this.Action = p_Action;
                this.ActionStartFrame = p_Frame;
            }
        }

        /// <summary>
        /// Get elapsed time for the current action.
        /// </summary>
        /// <param name="p_Frame">Frame number.</param>
        /// <returns>Elapsed time for the current action.</returns>
        public int GetElapsed(int p_Frame)
        {
            return p_Frame - this.ActionStartFrame;
        }
    }
}
