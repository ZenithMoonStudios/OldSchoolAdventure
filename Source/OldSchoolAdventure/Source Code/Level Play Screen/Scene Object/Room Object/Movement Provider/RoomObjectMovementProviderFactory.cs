using System;

namespace OSA
{
    /// <summary>
    /// Room object movement provider factory.
    /// </summary>
    public class RoomObjectMovementProviderFactory
    {
        private StringToXmlRoomObjectMovementProviderReaderDictionary m_NameToReader = new StringToXmlRoomObjectMovementProviderReaderDictionary();

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObjectMovementProviderFactory()
        {
            this.Register("RunAndJump", new XmlRunAndJumpMovementProviderReader());
            this.Register("Linear", new XmlLinearProjectileMovementProviderReader());
            this.Register("Circular", new XmlCircularMovementProviderReader());
            this.Register("Directed", new XmlDirectedMovementProviderReader());
        }

        public RoomObjectMovementProvider GetProvider(OSATypes.TemplateMovementProvider p_XmlObjectProviderNode, OSATypes.TemplateMovementProvider p_XmlTemplateProviderNode)
        {
            RoomObjectMovementProvider result = null;
            if (p_XmlTemplateProviderNode != null)
            {
                string name = p_XmlTemplateProviderNode.Name != null ? p_XmlTemplateProviderNode.Name : String.Empty;
                result = this.GetProvider(name, p_XmlObjectProviderNode, p_XmlTemplateProviderNode);
            }
            return result;
        }

        public RoomObjectMovementProvider GetProvider(string p_Name, OSATypes.TemplateMovementProvider p_XmlObjectProviderNode, OSATypes.TemplateMovementProvider p_XmlTemplateProviderNode)
        {
            RoomObjectMovementProvider result = null;
            if (m_NameToReader.ContainsKey(p_Name))
            {
                result = m_NameToReader[p_Name].CreateInstance(p_XmlObjectProviderNode, p_XmlTemplateProviderNode);
            }
            return result;
        }

        private void Register(string p_Name, XmlRoomObjectMovementProviderReader p_ProviderFactory)
        {
            m_NameToReader.Add(p_Name, p_ProviderFactory);
        }

        #region Singleton instance

        private static RoomObjectMovementProviderFactory s_Instance;

        public static RoomObjectMovementProviderFactory Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new RoomObjectMovementProviderFactory();
                }
                return s_Instance;
            }
        }

        #endregion
    }
}
