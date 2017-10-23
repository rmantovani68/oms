using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;
// Libreria per usare oggetti XmlSerializer
using System.Xml.Serialization;
// Libreria per usare StringWriter
using System.IO;
// Libreria per usare WebUtility
using System.Net;

namespace DataModel
{
    public class MariniStandardXmlSerializer : IMariniSerializer
    {
        private XmlSerializer _xmlSerializer;

        public MariniStandardXmlSerializer()
        {

        }

        /// <summary>
        /// Serialize a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">the path of the object.</param>
        /// <returns>A string that contains the serialized object</returns>
        public string Serialize(MariniGenericObject mgo)
        {
            
            if (mgo == null)
            {
                Console.WriteLine("\nOggetto null!!!");
                return "NN";
            }
            else
            {
                _xmlSerializer = new XmlSerializer(mgo.GetType());
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true
                        , ConformanceLevel = ConformanceLevel.Auto
                            //, ConformanceLevel = ConformanceLevel.Document
                            //, NewLineOnAttributes = true
                        , Indent = true
                    }))
                    {
                        // Build Xml with xw.
                        _xmlSerializer.Serialize(xmlWriter, mgo);
                    }
                    // rielaboro lo streaming per convertire in un formato compatibile con HTTP/HTML
                    // ad esempio la codifica di < e > diventa &lt; and &gt;
                    return WebUtility.HtmlDecode(stringWriter.ToString());
                }
            }
        }
    }
}
