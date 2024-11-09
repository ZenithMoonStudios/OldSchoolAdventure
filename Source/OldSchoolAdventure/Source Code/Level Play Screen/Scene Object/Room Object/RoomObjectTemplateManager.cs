using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Room object template manager.
    /// </summary>
    public class RoomObjectTemplateManager
    {
        string m_ContentRootDirectory;
        Dictionary<string, RoomObjectTemplate> m_Templates = new Dictionary<string, RoomObjectTemplate>();

        public RoomObjectTemplateManager(ContentManager p_ContentManager) : this(p_ContentManager.RootDirectory) { }
        public RoomObjectTemplateManager(string p_ContentRootDirectory)
        {
            m_ContentRootDirectory = p_ContentRootDirectory;
        }

        public RoomObjectTemplate Get(string p_ObjectTypePath)
        {
            if (!m_Templates.ContainsKey(p_ObjectTypePath))
            {
                Type RoomObjectType = OSATypes.Functions.GetTypeFromFolder(p_ObjectTypePath);
                XmlSerializer serializer = new XmlSerializer(RoomObjectType);
#if(MOBILE)
				Stream fs = TitleContainer.OpenStream(Path.Combine(m_ContentRootDirectory, p_ObjectTypePath + ".xml"));
#else
                FileStream fs = new FileStream(Path.Combine(m_ContentRootDirectory, p_ObjectTypePath + ".xml"), FileMode.Open);
#endif
                OSATypes.RoomObjectTemplate roomObjectTemplate;
                roomObjectTemplate = (OSATypes.RoomObjectTemplate)serializer.Deserialize(fs);

                m_Templates.Add(p_ObjectTypePath, new RoomObjectTemplate(p_ObjectTypePath, roomObjectTemplate));
            }
            return m_Templates[p_ObjectTypePath];
        }
    }
}
