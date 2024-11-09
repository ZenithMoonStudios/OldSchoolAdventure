namespace OSA
{
    public class SceneObjectGuidFactory
    {
        private int m_NextGuid = 0;

        public SceneObjectGuidFactory()
        {
        }

        public int Create()
        {
            return m_NextGuid++;
        }

        #region Singleton

        private static SceneObjectGuidFactory s_Instance = null;
        public static SceneObjectGuidFactory Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new SceneObjectGuidFactory();
                }
                return s_Instance;
            }
        }

        #endregion
    }
}
