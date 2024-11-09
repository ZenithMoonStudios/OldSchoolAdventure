using Destiny;
using Microsoft.Xna.Framework;
using System;

namespace OSA
{
    /// <summary>
    /// Camera class.
    /// </summary>
    public class Camera
    {
        public delegate Rectangle RectangleGetter();
        public delegate Room RoomGetter();

        Rectangle m_ScreenRegion;
        int m_PercentageNoScroll = 0;
        RectangleGetter m_GetObjectRegionDelegate;
        RoomGetter m_GetRoomDelegate;

        float m_DisplayScaleX = 1f;
        float m_DisplayScaleY = 1f;
        Size m_DisplayOffset = Size.Zero;

        /// <summary>
        /// World region.
        /// </summary>
        Rectangle m_WorldRegion = Rectangle.Empty;
        public Rectangle WorldRegion
        {
            get { return m_WorldRegion; }
            set { m_WorldRegion = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Camera(
            Rectangle p_ScreenRegion,
            int p_PercentageNoScroll,
            RectangleGetter p_GetObjectRegionDelegate,
            RoomGetter p_GetRoomDelegate
            )
        {
            m_GetObjectRegionDelegate = p_GetObjectRegionDelegate;
            m_GetRoomDelegate = p_GetRoomDelegate;
            m_PercentageNoScroll = p_PercentageNoScroll;
            m_ScreenRegion = p_ScreenRegion;
            m_WorldRegion = new Rectangle(0, 0, m_ScreenRegion.Width, m_ScreenRegion.Height);
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update()
        {
            // Adjust world region based on room and object position
            Rectangle objectRegion = m_GetObjectRegionDelegate();
            Room room = m_GetRoomDelegate();
            m_WorldRegion.X = this.GetPosition(this.WorldRegion.X, this.WorldRegion.Width, (int)objectRegion.Width, (int)objectRegion.X, (int)room.Left, (int)room.Width);
            m_WorldRegion.Y = this.GetPosition(this.WorldRegion.Y, this.WorldRegion.Height, (int)objectRegion.Height, (int)objectRegion.Y, (int)room.Top, (int)room.Height);

            // Calculate draw parameters, to save them being calculated each time a sprite is drawn
            m_DisplayScaleX = m_ScreenRegion.Width / (float)this.WorldRegion.Width;
            m_DisplayScaleY = m_ScreenRegion.Height / (float)this.WorldRegion.Height;
            m_DisplayOffset.Width = m_ScreenRegion.X - this.WorldRegion.X;
            m_DisplayOffset.Height = m_ScreenRegion.Y - this.WorldRegion.Y;
        }

        /// <summary>
        /// Get the position based on character position and no scroll region.
        /// </summary>
        /// <param name="p_StartPosition">Position of the camera.</param>
        /// <param name="p_Size">Size of the camera in the world.</param>
        /// <param name="p_CharacterSize">Size of the character in the world.</param>
        /// <param name="p_CharacterPosition">Position of the character in the world.</param>
        /// <param name="p_RoomPosition">Position of the room.</param>
        /// <param name="p_RoomSize">Size of the room.</param>
        /// <returns>Position.</returns>
        private int GetPosition(
            int p_Position,
            int p_Size,
            int p_CharacterSize,
            int p_CharacterPosition,
            int p_RoomPosition,
            int p_RoomSize
            )
        {
            int position = p_Position;

            int center = position + (p_Size / 2) - (p_CharacterSize / 2);
            int minimum = center - (int)(m_PercentageNoScroll / 100f * ((p_Size - p_CharacterSize) / 2f));
            int maximum = center + (int)(m_PercentageNoScroll / 100f * ((p_Size - p_CharacterSize) / 2f));
            if (p_CharacterPosition < minimum)
            {
                position -= minimum - p_CharacterPosition;
            }
            else if (p_CharacterPosition > maximum)
            {
                position += p_CharacterPosition - maximum;
            }
            position = Math.Max(position, p_RoomPosition);
            position = Math.Min(position, p_RoomPosition + p_RoomSize - p_Size);

            return position;
        }

        /// <summary>
        /// Transform the specified world region into a screen region.
        /// </summary>
        public void WorldToScreen(Rectangle p_WorldRegion, ref Rectangle p_ScreenRegion)
        {
            p_ScreenRegion.X = (int)((p_WorldRegion.X + m_DisplayOffset.Width) * m_DisplayScaleX);
            p_ScreenRegion.Y = (int)((p_WorldRegion.Y + m_DisplayOffset.Height) * m_DisplayScaleY);
            p_ScreenRegion.Width = (int)(p_WorldRegion.Width * m_DisplayScaleX);
            p_ScreenRegion.Height = (int)(p_WorldRegion.Height * m_DisplayScaleY);
        }

        /// <summary>
        /// Transform the specified world position into a screen position.
        /// </summary>
        public void WorldToScreen(Point p_WorldPosition, ref Point p_ScreenPosition)
        {
            p_ScreenPosition.X = (int)((p_WorldPosition.X + m_DisplayOffset.Width) * m_DisplayScaleX);
            p_ScreenPosition.Y = (int)((p_WorldPosition.Y + m_DisplayOffset.Height) * m_DisplayScaleY);
        }
    }
}
