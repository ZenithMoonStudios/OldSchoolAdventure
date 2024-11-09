using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Terrain class.
    /// </summary>
    public class Terrain : SceneObject
    {
        private TerrainTemplate m_Template;
        private TerrainSegment[] m_Segments;

        public TerrainPosition TerrainPosition { get { return m_Template.TerrainPosition; } }
        public int SegmentLength { get { return m_Template.SegmentLength; } }
        public float Friction { get { return m_Template.Friction; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Terrain(TerrainTemplate p_Template, TerrainSegmentList p_Segments) : base(p_Template.ObjectTypePath, Vector2.Zero)
        {
            m_Template = p_Template;
            m_Segments = new TerrainSegment[p_Segments.Count];
            for (int i = 0; i < p_Segments.Count; i++)
            {
                m_Segments[i] = p_Segments[i];
            }
        }

        /// <summary>
        /// Dock terrain to side of room.
        /// </summary>
        /// <param name="p_Room">Room.</param>
        public void Dock(Room p_Room)
        {
            // Dock to side of room
            switch (this.TerrainPosition)
            {
                case TerrainPosition.Top:
                    m_Position = p_Room.Position;
                    break;
                case TerrainPosition.Bottom:
                    m_Position = new Vector2(p_Room.Left, p_Room.Bottom);
                    break;
            }
        }

        /// <summary>
        /// Get the index of the segment in which the specified position resides.
        /// </summary>
        /// <param name="p_StartPosition">Position.</param>
        /// <returns>Index of the segment in which the specified position resides.</returns>
        public int GetSegmentIndex(float p_Position)
        {
            int result = 0;
            if (this.TerrainPosition != TerrainPosition.None)
            {
                result = (int)(p_Position - this.Left) / this.SegmentLength;
            }
            return result;
        }

        /// <summary>
        /// Get the specified segment.
        /// </summary>
        /// <param name="p_SegmentIndex">Index of the segment.</param>
        /// <returns>Segment.</returns>
        public TerrainSegment GetSegment(int p_SegmentIndex)
        {
            TerrainSegment segment = null;
            if (p_SegmentIndex >= 0 && p_SegmentIndex < m_Segments.Length)
            {
                segment = m_Segments[p_SegmentIndex];
            }
            return segment;
        }

        /// <summary>
        /// Constrain a scene object within the limits of the terrain.
        /// </summary>
        /// <returns>Whether the scene object was constrained.</returns>
        public bool Constrain(Vector2 p_Size, ref Vector2 p_Position)
        {
            float midX = p_Position.X + (p_Size.X / 2);
            float terrainHeight = this.GetTerrainHeight(midX);

            // If terrain is at the bottom, height must be subtracted from the terrain position
            float constraint = terrainHeight;
            if (this.TerrainPosition == TerrainPosition.Bottom)
            {
                constraint = this.Bottom - constraint;
            }

            // Apply the constraint
            bool isChanged = false;
            if (this.TerrainPosition == TerrainPosition.Top && p_Position.Y < constraint)
            {
                p_Position.Y = constraint;
                isChanged = true;
            }
            else if (this.TerrainPosition == TerrainPosition.Bottom && (p_Position.Y + p_Size.Y) > constraint)
            {
                p_Position.Y = constraint - p_Size.Y;
                isChanged = true;
            }
            return isChanged;
        }

        /// <summary>
        /// Get the height of the terrain at the specified X position.
        /// </summary>
        private float GetTerrainHeight(float p_X)
        {
            int segmentIndex = this.GetSegmentIndex(p_X);
            TerrainSegment segment = this.GetSegment(segmentIndex);
            float partialSegmentLength = (p_X - this.Left) - (segmentIndex * this.SegmentLength);
            float partialSegmentPercent = partialSegmentLength / Convert.ToSingle(this.SegmentLength);
            return segment.StartHeight + (partialSegmentPercent * Convert.ToSingle(segment.IncrementSize));
        }
    }

    /// <summary>
    /// Terrain list class.
    /// </summary>
    public class TerrainList : List<Terrain> { }
}
