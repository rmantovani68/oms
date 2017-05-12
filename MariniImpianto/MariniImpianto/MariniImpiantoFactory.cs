using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;

namespace MariniImpianti
{
    class MariniImpiantoFactory
    {

    }

    public class MariniObjectCreator
    {

        private static MariniObjectCreator _mariniCreator = new MariniObjectCreator();
        public static MariniObjectCreator MariniCreator
        {
            get { return _mariniCreator; }
        }

        public static MariniGenericObject CreateMariniObject(MariniGenericObject parent, XmlNode node)
        {

            MariniGenericObject mgo;

            switch (node.Name)
            {
                case "impianto":
                    mgo = new MariniImpianto(parent, node);
                    break;
                case "zona":
                    mgo = new MariniZona(parent, node);
                    break;
                case "predosatore":
                    mgo = new MariniPredosatore(parent, node);
                    break;
                case "plctag":
                    mgo = new MariniPlctag(parent, node);
                    break;
                case "bilancia":
                    mgo = new MariniBilancia(parent, node);
                    break;
                case "motore":
                    mgo = new MariniMotore(parent, node);
                    break;
                case "nastro":
                    mgo = new MariniNastro(parent, node);
                    break;
                case "amperometro":
                    mgo = new MariniAmperometro(parent, node);
                    break;
                default:
                    mgo = new MariniOggettoBase(parent, node);
                    break;
                //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
            }

            /* Riempio la lista oggetti con i nodi figli */
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (children.Count > 0)
                {
                    //Console.WriteLine("Parent id: {0} node.childnodes = {1}", mgo.id, node.ChildNodes.Count);
                    mgo.ListaGenericObject.Add(CreateMariniObject(mgo, child));
                }
            }

            /* restituisco l'oggetto creato: GOF factory pattern */
            return mgo;
        }

        public static MariniGenericObject CreateMariniObject(XmlNode node)
        {
            return CreateMariniObject(null, node);
        }

    }
}