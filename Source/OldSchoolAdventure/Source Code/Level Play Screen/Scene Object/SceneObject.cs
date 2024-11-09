using Destiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Scene object base class.
    /// </summary>
    public class SceneObject
    {
        public int ObjectGuid { get; private set; }
        public string ObjectTypePath { get; private set; }

        protected Vector2 m_Size;
        protected Vector2 m_PreviousPosition;
        protected Vector2 m_Position;
        protected Vector2 m_Velocity;
        protected DirectorInstruction m_LastDirectorInstruction;
        public Vector2 Size { get { return m_Size; } }
        public Vector2 PreviousPosition { get { return m_PreviousPosition; } }
        public Vector2 Position { get { return m_Position; } }
        public Vector2 Velocity { get { return m_Velocity; } }
        public DirectorInstruction LastDirectorInstruction { get { return m_LastDirectorInstruction; } }

        public SceneObjectList SceneObjects { get; protected set; }

        public InputState InputState { get; protected set; }

        public string ActionState { get; protected set; }

        protected bool CanDraw { get; set; }

        #region Size and Position helpers

        /// <summary>
        /// Size helpers.
        /// </summary>
        public float Width { get { return m_Size.X; } }
        public float Height { get { return m_Size.Y; } }

        /// <summary>
        /// Position helpers.
        /// </summary>
        public float Left { get { return m_Position.X; } }
        public float Top { get { return m_Position.Y; } }
        public float Right { get { return m_Position.X + m_Size.X; } }
        public float Bottom { get { return m_Position.Y + m_Size.Y; } }
        public float MidX { get { return m_Position.X + (m_Size.X / 2); } }
        public float MidY { get { return m_Position.Y + (m_Size.Y / 2); } }
        public Vector2 Mid { get { return new Vector2(m_Position.X + (m_Size.X / 2), m_Position.Y + (m_Size.Y / 2)); } }

        /// <summary>
        /// Previous position helpers.
        /// </summary>
        public float PreviousLeft { get { return m_PreviousPosition.X; } }
        public float PreviousTop { get { return m_PreviousPosition.Y; } }
        public float PreviousRight { get { return m_PreviousPosition.X + m_Size.X; } }
        public float PreviousBottom { get { return m_PreviousPosition.Y + m_Size.Y; } }
        public float PreviousMidX { get { return m_PreviousPosition.X + (m_Size.X / 2); } }
        public float PreviousMidY { get { return m_PreviousPosition.Y + (m_Size.Y / 2); } }
        public Vector2 PreviousMid { get { return new Vector2(m_PreviousPosition.X + (m_Size.X / 2), m_PreviousPosition.Y + (m_Size.Y / 2)); } }

        #endregion

        /// <summary>
        /// Constructors.
        /// </summary>
        public SceneObject() : this(null, Vector2.Zero) { }
        public SceneObject(string p_ObjectTypePath, Vector2 p_Size) : this(p_ObjectTypePath, p_Size, Vector2.Zero) { }
        public SceneObject(string p_ObjectTypePath, Vector2 p_Size, Vector2 p_Position)
        {
            this.ObjectGuid = SceneObjectGuidFactory.Instance.Create();
            this.ObjectTypePath = p_ObjectTypePath;

            m_Size = p_Size;
            m_Position = p_Position;
            if (m_PreviousPosition == Vector2.Zero)
            {
                m_PreviousPosition = m_Position;
            }
            this.SceneObjects = new SceneObjectList();

            this.CanDraw = true;
            this.ActionState = string.Empty;
        }

        /// <summary>
        /// Load content of children and this scene object.
        /// </summary>
        /// <param name="p_DrawManager">Draw manager.</param>
        public virtual void LoadContentAndChildren(DrawManager p_DrawManager)
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].LoadContentAndChildren(p_DrawManager);
                }
            }
            this.LoadContent(p_DrawManager);
        }

        /// <summary>
        /// Load content.
        /// </summary>
        /// <param name="p_DrawManager">Draw manager.</param>
        protected virtual void LoadContent(DrawManager p_DrawManager)
        {
            if (this.CanDraw)
            {
                p_DrawManager.Register(this);
            }
        }

        /// <summary>
        /// Unload content of children and this scene object.
        /// </summary>
        public virtual void UnloadContentAndChildren()
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].UnloadContentAndChildren();
                }
            }
            this.UnloadContent();
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        protected virtual void UnloadContent()
        {
            this.InputState = null;
        }

        /// <summary>
        /// Reset children and this scene object.
        /// </summary>
        public virtual void ResetAndChildren()
        {
            for (int i = 0; i < this.SceneObjects.Count; i++)
            {
                if (this.SceneObjects[i] != null)
                {
                    this.SceneObjects[i].ResetAndChildren();
                }
            }

            // Reset
            this.Reset();
        }

        /// <summary>
        /// Reset state.
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Handle input for children and this scene object.
        /// </summary>
        public virtual void HandleInputAndChildren(InputState p_InputState)
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].HandleInputAndChildren(p_InputState);
                }
            }

            // Handle input
            this.HandleInput(p_InputState);
        }

        /// <summary>
        /// Handle input.
        /// </summary>
        protected virtual void HandleInput(InputState p_InputState)
        {
            this.InputState = p_InputState;
        }

        /// <summary>
        /// Update children and this scene object.
        /// </summary>
        Vector2 m_TempVector;
        public virtual void UpdateAndChildren()
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].UpdateAndChildren();
                }
            }

            // Store position before update
            // Previous position is not immediately set, because it doesn't make sense for the current
            // and previous positions to be the same, and some updates might rely on them being different
            // and knowing where an object came from
            m_TempVector.X = m_Position.X;
            m_TempVector.Y = m_Position.Y;

            // Update
            this.Update();

            // Now that position has changed, set previous position to be the position before update
            m_PreviousPosition.X = m_TempVector.X;
            m_PreviousPosition.Y = m_TempVector.Y;
            if (m_PreviousPosition == Vector2.Zero)
            {
                m_PreviousPosition.X = m_Position.X;
                m_PreviousPosition.Y = m_Position.Y;
            }
            m_Velocity.X = m_Position.X - m_PreviousPosition.X;
            m_Velocity.Y = m_Position.Y - m_PreviousPosition.Y;
        }

        /// <summary>
        /// Update.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Draw children and this scene object.
        /// </summary>
        /// <param name="p_DrawManager">Draw manager.</param>
        public virtual void DrawAndChildren(DrawManager p_DrawManager)
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].DrawAndChildren(p_DrawManager);
                }
            }
            this.Draw(p_DrawManager);
        }

        /// <summary>
        /// Draw.
        /// </summary>
        protected void Draw(DrawManager p_DrawManager)
        {
            if (this.CanDraw)
            {
                this.DoDraw(p_DrawManager);
            }
        }

        protected virtual void DoDraw(DrawManager p_DrawManager)
        {
            p_DrawManager.Draw(this);
        }

        /// <summary>
        /// Evaluate and handle collision for children and this scene object.
        /// </summary>
        public virtual void EvaluateAndHandleCollisionAndChildren(SceneObject p_SceneObject)
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].EvaluateAndHandleCollisionAndChildren(p_SceneObject);
                }
            }
            bool isCollision =
                this.Left < p_SceneObject.Right && this.Right > p_SceneObject.Left &&
                this.Top < p_SceneObject.Bottom && this.Bottom > p_SceneObject.Top;
            this.HandleCollision(p_SceneObject, isCollision);
        }

        /// <summary>
        /// Handle collision.
        /// </summary>
        /// <param name="p_Object">Scene object.</param>
        /// <param name="p_IsCollision">Whether there is a collision.</param>
        protected virtual void HandleCollision(SceneObject p_Object, bool p_IsCollision)
        {
            if (p_IsCollision && p_Object is Director)
            {
                Director director = p_Object as Director;
                m_LastDirectorInstruction = director.Instruction;
            }
        }

        /// <summary>
        /// Update action state of children and this scene object.
        /// </summary>
        public virtual void UpdateActionStateAndChildren()
        {
            for (int sceneObjectIndex = 0; sceneObjectIndex < this.SceneObjects.Count; sceneObjectIndex++)
            {
                if (this.SceneObjects[sceneObjectIndex] != null)
                {
                    this.SceneObjects[sceneObjectIndex].UpdateActionStateAndChildren();
                }
            }
            this.UpdateActionState();
        }

        /// <summary>
        /// Update action state.
        /// </summary>
        protected virtual void UpdateActionState()
        {
        }
    }

    /// <summary>
    /// Scene object collection class.
    /// </summary>
    public class SceneObjectList : List<SceneObject> { }
}
