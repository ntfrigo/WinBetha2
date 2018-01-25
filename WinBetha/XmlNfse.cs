using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WinBetha
{
    public static class XmlNfse
    {
        public static string ToXml<T>(this T objeto)
        {
            XElement xml;
            var ser = new XmlSerializer(typeof(T));

            using (var memory = new MemoryStream())
            {
                using (TextReader tr = new StreamReader(memory, Encoding.UTF8))
                {
                    ser.Serialize(memory, objeto);
                    memory.Position = 0;
                    xml = XElement.Load(tr);
                    //xml.Attributes().Where(x => x.Name.LocalName.Equals("xsd") || x.Name.LocalName.Equals("xsi")).Remove();
                }
            }
            var doc = XElement.Parse(xml.ToString());
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        public static T ToXmlDeserializer<T>(this string input) where T : class
        {
            //string xsi = "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"";
            //if (input.Contains("<Signature")) input = input.Replace(xsi, "<Signature");

            var ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
