using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Director class.
    /// </summary>
    public class Director : RoomObject
    {
        public DirectorInstruction Instruction { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Director(DirectorInstruction p_Instruction, RoomObjectInitialization p_Init) : base(p_Init)
        {
            this.Instruction = p_Instruction;
        }
    }

    /// <summary>
    /// Director list class.
    /// </summary>
    public class DirectorList : List<Director> { }

    /// <summary>
    /// String to director dictionary class.
    /// </summary>
    public class StringToDirectorDictionary : Dictionary<string, Director> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlDirectorReader : XmlRoomObjectReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlDirectorReader()
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            DirectorInstruction instruction = null;
            string type = p_RoomTemplateObject.Type != null ? p_RoomTemplateObject.Type : "Linear";
            if (type == "Linear")
            {
                Vector2 velocity = p_RoomTemplateObject.Velocity != null && p_RoomTemplateObject.Velocity.Length > 0 ? OSATypes.Functions.LoadVector(p_RoomTemplateObject.Velocity[0].X, p_RoomTemplateObject.Velocity[0].Y) : Vector2.Zero;
                instruction = new DirectorInstruction(velocity);
            }
            else if (type == "Circular")
            {
                string direction = p_RoomTemplateObject.Direction != null ? p_RoomTemplateObject.Direction : "Clockwise";
                int timePerRevolution = int.TryParse(p_RoomTemplateObject.TimePerRevolution, out timePerRevolution) ? int.Parse(p_RoomTemplateObject.TimePerRevolution) : 60;
                Vector2 rotationGridOffset = p_RoomTemplateObject.RotationGridOffset != null && p_RoomTemplateObject.RotationGridOffset.Length > 0 ? OSATypes.Functions.LoadVector(p_RoomTemplateObject.RotationGridOffset[0].X, p_RoomTemplateObject.RotationGridOffset[0].Y) : Vector2.Zero;
                instruction = new DirectorInstruction(direction == "Clockwise", timePerRevolution, rotationGridOffset);
            }

            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new Director(instruction, initializationInfo);
        }
    }

}
