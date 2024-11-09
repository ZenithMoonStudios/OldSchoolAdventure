using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Destiny
{
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader p_Reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = p_Reader.IsEmptyElement;
            p_Reader.Read();
            if (wasEmpty)
            {
                return;
            }

            while (p_Reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                p_Reader.ReadStartElement("item");

                p_Reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(p_Reader);
                p_Reader.ReadEndElement();

                p_Reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(p_Reader);
                p_Reader.ReadEndElement();

                this.Add(key, value);

                p_Reader.ReadEndElement();
                p_Reader.MoveToContent();
            }
            p_Reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter p_Writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                p_Writer.WriteStartElement("item");

                p_Writer.WriteStartElement("key");
                keySerializer.Serialize(p_Writer, key);
                p_Writer.WriteEndElement();

                p_Writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(p_Writer, value);
                p_Writer.WriteEndElement();

                p_Writer.WriteEndElement();
            }
        }
    }
}
