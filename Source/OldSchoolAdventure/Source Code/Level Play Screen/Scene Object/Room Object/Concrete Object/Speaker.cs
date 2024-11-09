using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Speaker class.
    /// </summary>
    public class Speaker : RoomObject
    {
        SpeakerConversationList m_Conversations = new SpeakerConversationList();
        bool m_RequiresInteraction;

        public SpeakerConversationList Conversations { get { return m_Conversations; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Speaker(SpeakerConversationList p_Conversations, bool p_RequiresInteraction, RoomObjectInitialization p_Init) : base(p_Init)
        {
            m_RequiresInteraction = p_RequiresInteraction;
            for (int i = 0; i < p_Conversations.Count; i++)
            {
                m_Conversations.Add(p_Conversations[i]);
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
                if (p_IsCollision && mainCharacter.IsAlive && (!m_RequiresInteraction || mainCharacter.DoInteract))
                {
                    // Speak
                    SpeakerConversation conversation = this.GetActiveConversation(mainCharacter.Store);
                    if (conversation != null)
                    {
                        mainCharacter.Speak(conversation);
                    }
                }
            }

            // Do default processing
            base.HandleCollision(p_Object, p_IsCollision);
        }

        /// <summary>
        /// Get the active conversation.
        /// </summary>
        SpeakerConversation GetActiveConversation(CollectibleStore p_Store)
        {
            SpeakerConversation result = null;
            for (int i = 0; i < m_Conversations.Count; i++)
            {
                SpeakerConversation conversation = m_Conversations[i];
                if (conversation.CanSpeak(p_Store))
                {
                    result = conversation;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Speaker list class.
    /// </summary>
    public class SpeakerList : List<Speaker> { }

    /// <summary>
    /// String to speaker dictionary class.
    /// </summary>
    public class StringToSpeakerDictionary : Dictionary<string, Speaker> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlSpeakerReader : XmlRoomObjectReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlSpeakerReader()
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            bool requiresInteraction = bool.TryParse(p_RoomTemplateObject.RequiresInteraction, out requiresInteraction) ? bool.Parse(p_RoomTemplateObject.RequiresInteraction) : true;
            SpeakerConversationList conversations = new SpeakerConversationList();

            if (item != null)
            {
                List<OSATypes.Conversations> xmlConversationsNode = item.Conversations != null && item.Conversations.Count > 0 ? item.Conversations : null;

                foreach (OSATypes.Conversations Conversationitem in xmlConversationsNode)
                    if (Conversationitem.Conversation != null && Conversationitem.Conversation.Count > 0)
                        for (int i = 0; i < Conversationitem.Conversation.Count; i++)
                        {
                            OSATypes.ConversationsConversation xmlConversationNode = Conversationitem.Conversation[i];

                            // Load data
                            GameConditionList conditions = XmlGameConditionListReader.Load(xmlConversationNode.Conditions);
                            SpeakerConversationSentenceList sentences = XmlSpeakerConversationSentenceListReader.Load(xmlConversationNode.Sentences);
                            SpeakerConversationTransactionList transactions = XmlSpeakerConversationTransactionListReader.Load(xmlConversationNode.Transactions);

                            conversations.Add(new SpeakerConversation(conditions, sentences, transactions));
                        }
            }

            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new Speaker(conversations, requiresInteraction, initializationInfo);
        }
    }

}
