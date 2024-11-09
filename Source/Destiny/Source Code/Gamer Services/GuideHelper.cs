using Microsoft.Xna.Framework;

namespace Destiny
{
    /// <summary>
    /// Guide helper class.
    /// </summary>
    public class GuideHelper
    {
        /// <summary>
        /// Whether the guide is visible.
        /// </summary>
        public static bool IsVisible
        {
            get
            {
                bool result = false;

                // TODO: Not Implemented

                return result;
            }
        }

        /// <summary>
        /// Whether the guide is in trial mode.
        /// </summary>
        public static bool IsTrialMode
        {
            get
            {
                bool result = false;
                return result;
            }
        }

        public static bool CanPurchase(PlayerIndex p_PlayerIndex)
        {
            bool result = false;
            if (!IsVisible && IsTrialMode)
            {
                // TODO: Not Implemented
            }
            return result;
        }

        public static void Purchase(PlayerIndex p_PlayerIndex)
        {
            // TODO: Not Implemented
        }
    }
}
