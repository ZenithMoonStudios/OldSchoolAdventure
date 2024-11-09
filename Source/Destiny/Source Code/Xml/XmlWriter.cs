using System.IO;
using System.Xml.Serialization;

namespace Destiny
{
    public class XmlWriter<T>
    {
        public void Write(T p_Object, string p_FilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter writer = new StreamWriter(p_FilePath);
            serializer.Serialize(writer, p_Object);
            writer.Close();
        }

        public void Write(T p_Object, Stream p_Stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamWriter writer = new StreamWriter(p_Stream);
            serializer.Serialize(writer, p_Object);
            writer.Close();
        }

        public void Write(T p_Object, StreamWriter p_Writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(p_Writer, p_Object);
        }
    }
}
