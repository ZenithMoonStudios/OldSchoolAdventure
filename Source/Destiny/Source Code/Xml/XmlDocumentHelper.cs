namespace Destiny
{
    //    public static class XmlDocumentHelper
    //    {
    //        #if WINDOWS || XBOX
    //        public static XmlDocument LoadFromTitleContainer(ContentManager p_ContentManager, string p_FileName)
    //        {

    //            return LoadFromTitleContainer(Path.Combine(p_ContentManager.RootDirectory, p_FileName));

    //        }

    //        public static XmlDocument LoadFromTitleContainer(string p_FileName)
    //        {
    //            XmlDocument result = null;
    //            try
    //            {
    //                Stream stream = TitleContainer.OpenStream(p_FileName);
    //                result = new XmlDocument();
    //                result.Load(stream);
    //                stream.Close();
    //            }
    //            catch
    //            {
    //                result = null;
    //            }
    //            return result;
    //        }
    //#else
    //        public static XDocument LoadFromTitleContainer(ContentManager p_ContentManager, string p_FileName)
    //        {

    //            return LoadFromTitleContainer(Path.Combine(p_ContentManager.RootDirectory, p_FileName));

    //        }

    //        public static XDocument LoadFromTitleContainer(string p_FileName)
    //        {
    //            XDocument result = null;
    //            try
    //            {
    //                using (Stream stream = TitleContainer.OpenStream(p_FileName))
    //                {
    //                    result = XDocument.Load(stream);
    //               }
    //            }
    //            catch
    //            {
    //                result = null;
    //            }
    //            return result;
    //        }
    //#endif
    //    }
}
