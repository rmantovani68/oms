using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;

namespace DataModel
{
    
    public class ObjectCreator
    {

        private static ObjectCreator _creator = new ObjectCreator();
        public static ObjectCreator MariniCreator
        {
            get { return _creator; }
        }

        public static GenericObject CreateObject(GenericObject parent, XmlNode node)
        {

            GenericObject mgo;
            /* uso il ToLower percui mettere tutto a minuscolo qui e come si vuole nel file xml */
            switch (node.Name)
            {
                case "Property":
                    mgo = new PropertyObject(parent, node);
                    break;
                default:
                    mgo = new BaseObject(parent, node);
                    break;
                //    throw new ApplicationException(string.Format("Object '{0}' cannot be created", mgo));
            }

            /* Riempio la lista oggetti con i nodi figli */
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (children.Count > 0)
                {
                    //Console.WriteLine("Parent id: {0} node.childnodes = {1}", mgo.id, node.ChildNodes.Count);
                    mgo.ChildList.Add(CreateObject(mgo, child));
                }
            }

            /* restituisco l'oggetto creato: GOF factory pattern */
            return mgo;
        }

        public static GenericObject CreateMariniObject(XmlNode node)
        {
            return CreateObject(null, node);
        }

    }
}
