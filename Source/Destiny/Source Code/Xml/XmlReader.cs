using System.IO;
using System.Xml.Serialization;

namespace Destiny
{
    public class XmlReader<T>
    {
        public T Read(string p_FilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader reader = new StreamReader(p_FilePath);
            T result = (T)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }

        public T Read(Stream p_Stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(p_Stream);
            T result = (T)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }
    }
}
