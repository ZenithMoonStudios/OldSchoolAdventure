using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Door class.
    /// </summary>
    public class Door : RoomObject
    {
        public DoorTarget Target { get; private set; }
        DoorLockType m_LockType;
        string m_RequiredItem;
        bool m_RequiresInteraction;
        string m_LockedMessage;

        public bool IsValidRespawnLocation { get; private set; }
        public bool IsSpawnPointOnly { get; private set; }

        DoorState m_State;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Door(
            DoorTarget p_Target,
            DoorLockType p_LockType,
            string p_RequiredItem,
            bool p_RequiresInteraction,
            string p_LockedMessage,
            bool p_IsValidRespawnLocation,
            bool p_IsSpawnPointOnly,
            RoomObjectInitialization p_Init
            ) : base(p_Init)
        {
            this.Target = new DoorTarget();
            this.Target.Set(p_Target);

            m_LockType = p_LockType;
            m_RequiredItem = p_RequiredItem;
            m_RequiresInteraction = p_RequiresInteraction;
            m_LockedMessage = p_LockedMessage;

            this.IsValidRespawnLocation = p_IsValidRespawnLocation;
            this.IsSpawnPointOnly = p_IsSpawnPointOnly;

            // Update state
            m_State = (m_LockType == DoorLockType.None) ? DoorState.Closed : DoorState.Locked;
            this.CollisionActionState = DoorStateHelper.ToString(m_State);
        }

        public void ForceUnlock()
        {
            m_State = DoorState.Closed;
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
                if (mainCharacter.IsAlive)
                {
                    // Work out whether door should be locked
                    bool isLocked = false;
                    if (m_LockType != DoorLockType.None)
                    {
                        // Unlock door if possible
                        bool hasRequiredItem = mainCharacter.Store.Has(m_RequiredItem);
                        if (m_LockType == DoorLockType.Switch)
                        {
                            isLocked = !hasRequiredItem;
                        }
                        else if (m_LockType == DoorLockType.Key)
                        {
                            isLocked = (m_State == DoorState.Locked) && (!p_IsCollision || !hasRequiredItem);
                        }
                    }

                    // Use key, if required
                    if (m_LockType == DoorLockType.Key && m_State == DoorState.Locked && !isLocked)
                    {
                        mainCharacter.Store.Use(m_RequiredItem);
                        mainCharacter.Store.UnlockDoor(this);
                        this.AudioEvent("OnUnlock");
                    }

                    // Handle interaction
                    if (p_IsCollision && (!m_RequiresInteraction || mainCharacter.DoInteract))
                    {
                        if (isLocked)
                        {
                            if (m_RequiresInteraction && !string.IsNullOrEmpty(m_LockedMessage))
                            {
                                SpeakerConversationSentence sentence = new SpeakerConversationSentence(this.ObjectTypePath, m_LockedMessage);
                                SpeakerConversation conversation = new SpeakerConversation(sentence);
                                mainCharacter.Speak(conversation);
                            }
                        }
                        else
                        {
                            if (this.IsSpawnPointOnly)
                            {
                                mainCharacter.SetSpawnPoint(this.Target);
                            }
                            else
                            {
                                mainCharacter.RequestNavigation(this.Target);
                            }
                        }
                    }

                    // Play audio
                    if (m_State != DoorState.Open && !isLocked && p_IsCollision)
                    {
                        this.AudioEvent("OnOpen");
                    }
                    else if (m_State != DoorState.Closed && !isLocked && !p_IsCollision)
                    {
                        this.AudioEvent("OnClose");
                    }

                    // Update state
                    if (isLocked)
                    {
                        m_State = DoorState.Locked;
                    }
                    else
                    {
                        m_State = p_IsCollision ? DoorState.Open : DoorState.Closed;
                    }
                }

                // Update action state
                this.CollisionActionState = DoorStateHelper.ToString(m_State);
            }

            // Do default processing
            base.HandleCollision(p_Object, p_IsCollision);
        }
    }

    /// <summary>
    /// Door list class.
    /// </summary>
    public class DoorList : List<Door> { }

    /// <summary>
    /// String to door dictionary class.
    /// </summary>
    public class StringToDoorDictionary : Dictionary<string, Door> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlDoorReader : XmlRoomObjectReader
    {
        public XmlDoorReader() { }

        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            bool isTemplateNode = (p_RoomTemplateObject != null);

            // Door target
            string targetRoom = string.Empty;
            string targetObjectName = string.Empty;
            if (item.Target != null && item.Target.Count > 0)
            {
                OSATypes.RoomTarget roomTarget = item.Target[0];
                targetRoom = roomTarget.Room;
                targetObjectName = roomTarget.ObjectName;
            }

            DoorTarget target = new DoorTarget();
            target.Set(targetRoom, targetObjectName);

            bool requiresInteraction = bool.TryParse(p_RoomTemplateObject.RequiresInteraction, out requiresInteraction) ? bool.Parse(p_RoomTemplateObject.RequiresInteraction) : true;
            bool isSpawnPointOnly = bool.TryParse(p_RoomTemplateObject.IsSpawnPointOnly, out isSpawnPointOnly) ? bool.Parse(p_RoomTemplateObject.IsSpawnPointOnly) : false;

            // Load lock properties
            bool TemplateLock = (isTemplateNode && p_RoomTemplateObject.Lock != null && p_RoomTemplateObject.Lock.Length > 0);
            string requiredItem = string.Empty;
            string lockedMessage = string.Empty;
            DoorLockType lockType = (TemplateLock) ? DoorLockType.Key : DoorLockType.None;
            if (TemplateLock)
            {
                lockType = DoorLockTypeHelper.FromString(p_RoomTemplateObject.Lock[0].Type != null ? p_RoomTemplateObject.Lock[0].Type : DoorLockTypeHelper.c_Key);
                requiredItem = p_RoomTemplateObject.Lock[0].RequiredItem != null ? p_RoomTemplateObject.Lock[0].RequiredItem : requiredItem;
                lockedMessage = p_RoomTemplateObject.Lock[0].Message != null ? p_RoomTemplateObject.Lock[0].Message : lockedMessage;
            }

            bool isValidRespawnLocation = item.IsValidRespawnLocation; // XmlLoadHelper.LoadBoolean(p_XmlObjectNode, "IsValidRespawnLocation", true);

            // Create door
            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new Door(target, lockType, requiredItem, requiresInteraction, lockedMessage, isValidRespawnLocation, isSpawnPointOnly, initializationInfo);
        }
    }

}
