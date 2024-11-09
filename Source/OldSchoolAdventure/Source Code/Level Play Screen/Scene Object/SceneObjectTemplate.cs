using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Scene object template class.
    /// </summary>
    public class SceneObjectTemplate
    {
        public string ObjectTypePath { get; protected set; }
        public Vector2 Size { get; protected set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SceneObjectTemplate(string p_ObjectTypePath, Vector2 p_Size)
        {
            this.ObjectTypePath = p_ObjectTypePath;
            this.Size = p_Size;
        }
    }
}
