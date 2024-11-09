using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Sprite animation class.
    /// </summary>
    public class SpriteAnimation
    {
        /// <summary>
        /// Sprite animation frame class.
        /// </summary>
        private class SpriteAnimationFrame
        {
            public Sprite Sprite { get; private set; }
            public int Duration { get; private set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            public SpriteAnimationFrame(Sprite p_Sprite, int p_Duration)
            {
                this.Sprite = p_Sprite;
                this.Duration = p_Duration;
            }
        }

        /// <summary>
        /// Sprite animation frame collection class.
        /// </summary>
        private class SpriteAnimationFrameList : List<SpriteAnimationFrame> { }

        /// <summary>
        /// Animation frames.
        /// </summary>
        SpriteAnimationFrameList m_Frames = new SpriteAnimationFrameList();

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpriteAnimation()
        {
        }

        /// <summary>
        /// Add a frame to the animation.
        /// </summary>
        /// <param name="p_Sprite">Sprite.</param>
        /// <param name="p_Duration">Duration.</param>
        public void AddFrame(Sprite p_Sprite, int p_Duration)
        {
            m_Frames.Add(new SpriteAnimationFrame(p_Sprite, p_Duration));
        }

        /// <summary>
        /// Get the sprite for the specified elapsed time.
        /// </summary>
        /// <param name="p_Elapsed">Elapsed time.</param>
        /// <returns>Sprite for the specified elapsed time.</returns>
        public Sprite GetSprite(int p_Elapsed)
        {
            SpriteAnimationFrame activeFrame = null;
            bool isNewActiveFrame = false;
            if (m_Frames.Count == 1)
            {
                activeFrame = m_Frames[0];
                isNewActiveFrame = (p_Elapsed == 0);
            }
            else
            {
                // Determine the total duration of all frames
                int totalDuration = 0;
                for (int frameIndex = 0; frameIndex < m_Frames.Count; frameIndex++)
                {
                    totalDuration += m_Frames[frameIndex].Duration;
                }

                // Find the appropriate sprite for the elapsed time
                p_Elapsed = totalDuration > 0 ? p_Elapsed %= totalDuration : 0;
                int compoundDuration = 0;
                for (int frameIndex = 0; frameIndex < m_Frames.Count; frameIndex++)
                {
                    SpriteAnimationFrame frame = m_Frames[frameIndex];
                    compoundDuration += frame.Duration;
                    if (p_Elapsed < compoundDuration)
                    {
                        activeFrame = frame;
                        isNewActiveFrame = (p_Elapsed == compoundDuration - frame.Duration);
                        break;
                    }
                }
            }

            return activeFrame.Sprite;
        }
    }
}
