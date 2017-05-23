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
            // uso il ToLower percui mettere tutto a minuscolo qui e come si vuole nel file xml
            switch (node.Name)
            {
                case "Impianto":
                    mgo = new MariniImpianto(parent, node);
                    break;
                case "ZonaPredosaggio":
                    mgo = new MariniZonaPredosaggio(parent, node);
                    break;
                case "Predosatore":
                    mgo = new MariniPredosatore(parent, node);
                    break;
                case "Property":
                    mgo = new MariniProperty(parent, node);
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