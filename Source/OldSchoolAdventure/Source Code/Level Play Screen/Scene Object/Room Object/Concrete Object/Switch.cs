using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Switch class.
    /// </summary>
    public class Switch : RoomObject
    {
        private bool m_MainCharacterHasSwitchObject = false;

        private const string c_CollisionActionStateOff = "Off";
        private const string c_CollisionActionStateOn = "On";

        public string SwitchObjectPath { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Switch(string p_SwitchObjectPath, RoomObjectInitialization p_Init) : base(p_Init)
        {
            this.SwitchObjectPath = p_SwitchObjectPath;
            this.CollisionActionState = c_CollisionActionStateOff;
        }

        /// <summary>
        /// Handle collision.
        /// </summary>
        /// <param name="p_Object">Object that is being collided with.</param>
        /// <param name="p_IsCollision">Whether there is a collision.</param>
        protected override void HandleCollision(SceneObject p_Object, bool p_IsCollision)
        {
            if (this.IsAlive && p_Object is MainCharacter)
            {
                MainCharacter mainCharacter = p_Object as MainCharacter;
                if (p_IsCollision && mainCharacter.IsAlive && mainCharacter.DoInteract)
                {
                    if (m_MainCharacterHasSwitchObject)
                    {
                        mainCharacter.Store.Use(this.SwitchObjectPath);
                    }
                    else
                    {
                        mainCharacter.Store.Collect(this.SwitchObjectPath, 1);
                    }
                    this.AudioEvent("OnToggle");
                }

                m_MainCharacterHasSwitchObject = mainCharacter.Store.Has(this.SwitchObjectPath);
            }

            // Update action state
            this.CollisionActionState = m_MainCharacterHasSwitchObject ? c_CollisionActionStateOn : c_CollisionActionStateOff;

            // Do default processing
            base.HandleCollision(p_Object, p_IsCollision);
        }
    }

    /// <summary>
    /// Switch list class.
    /// </summary>
    public class SwitchList : List<Switch> { }

    /// <summary>
    /// String to switch dictionary class.
    /// </summary>
    public class StringToSwitchDictionary : Dictionary<string, Switch> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlSwitchReader : XmlRoomObjectReader
    {
        public XmlSwitchReader() { }

        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            string switchObjectPath = item.SwitchObject != null ? item.SwitchObject : string.Empty;

            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new Switch(switchObjectPath, initializationInfo);
        }
    }

}
