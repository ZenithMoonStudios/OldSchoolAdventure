using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    public class Collectible : RoomObject
    {
        public bool ShowCarry { get; private set; }
        public CollectiblePower Power { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Collectible(bool p_ShowCarry, CollectiblePower p_Power, RoomObjectInitialization p_Init) : base(p_Init)
        {
            this.ShowCarry = p_ShowCarry;
            this.Power = p_Power;
            if (this.Power != null)
            {
                this.Power.Attach(this);
            }
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
                if (p_IsCollision && mainCharacter.IsAlive)
                {
                    mainCharacter.Store.Collect(this);
                    this.AudioEvent("OnCollect");
                    this.LifeState = LifeStates.Dead;
                }
            }

            base.HandleCollision(p_Object, p_IsCollision);
        }
    }

    public class CollectibleList : List<Collectible> { }
    public class StringToCollectibleDictionary : Dictionary<string, Collectible> { }

    /// <summary>
    /// Xml collectible reader.
    /// </summary>
    public class XmlCollectibleReader : XmlRoomObjectReader
    {
        public XmlCollectibleReader() { }

        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            bool showCarry = bool.TryParse(p_RoomTemplateObject.ShowCarry, out showCarry) ? bool.Parse(p_RoomTemplateObject.ShowCarry) : false;
            CollectiblePower power = p_RoomTemplateObject.Power != null ? new XmlCollectiblePowerReader().CreateInstance(p_RoomTemplateObject.Power) : null;


            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new Collectible(showCarry, power, initializationInfo);
        }
    }

}
