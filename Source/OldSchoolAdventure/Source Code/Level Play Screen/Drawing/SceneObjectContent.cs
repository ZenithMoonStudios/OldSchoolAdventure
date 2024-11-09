using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Scene object content class.
    /// </summary>
    public class SceneObjectContent
    {
        Dictionary<string, SpriteAnimation> m_KeyToSpriteAnimation = new Dictionary<string, SpriteAnimation>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public SceneObjectContent()
        {
        }

        /// <summary>
        /// Register a sprite animation.
        /// </summary>
        public void RegisterSpriteAnimation(string p_Key, SpriteAnimation p_SpriteAnimation)
        {
            if (!m_KeyToSpriteAnimation.ContainsKey(p_Key))
            {
                m_KeyToSpriteAnimation.Add(p_Key, p_SpriteAnimation);
            }
        }

        /// <summary>
        /// Get the sprite animation for the specified animation key.
        /// </summary>
        public SpriteAnimation GetSpriteAnimation(string p_Key)
        {
            bool keyFound = !string.IsNullOrEmpty(p_Key) && m_KeyToSpriteAnimation.ContainsKey(p_Key);
            return m_KeyToSpriteAnimation[keyFound ? p_Key : "Default"];
        }
    }
}
