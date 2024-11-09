using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Room object template class.
    /// </summary>
    public class RoomObjectTemplate
    {
        private string m_ObjectTypePath;
        public string ObjectTypePath { get { return m_ObjectTypePath; } }

        private OSATypes.RoomObjectTemplate m_RoomTemplateObject;
        public OSATypes.RoomObjectTemplate RoomTemplateObject { get { return m_RoomTemplateObject; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObjectTemplate(string p_ObjectTypePath, OSATypes.RoomObjectTemplate p_RoomTemplateObject)
        {
            m_ObjectTypePath = p_ObjectTypePath;
            m_RoomTemplateObject = p_RoomTemplateObject;
        }
    }

    /// <summary>
    /// Room object template list class.
    /// </summary>
    public class RoomObjectTemplateList : List<RoomObjectTemplate> { }

    /// <summary>
    /// String to room object template dictionary class.
    /// </summary>
    public class StringToRoomObjectTemplateDictionary : Dictionary<string, RoomObjectTemplate> { }
}
