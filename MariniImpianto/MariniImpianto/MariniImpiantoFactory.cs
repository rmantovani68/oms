using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;

namespace MariniImpianto
{
    class MariniImpiantoFactory
    {

    }

    public class MariniObjectCreator
    {

        private static void on_manage_method(object sender, MariniGenericObject.OnManageEventArgs e)
        {
            // Console.WriteLine("Cicio : sto gestendo {0}",sender.ToString());
        }

        private static void impianto_on_manage_method(object sender, MariniGenericObject.OnManageEventArgs e)
        {
            // Console.WriteLine("Cicio : sto gestendo {0}",sender.ToString());
        }

        private static void on_change_method(object sender, MariniGenericObject.OnChangeEventArgs e)
        {
            Console.WriteLine("Pippo : in <{0}> qualcosa e' cambiato {1}", sender.ToString(), e.idImpianto);
        }

        private static void impianto_on_change_method(object sender, MariniGenericObject.OnChangeEventArgs e)
        {
            Console.WriteLine("Impianto : in <{0}> qualcosa e' cambiato {1}", sender.ToString(), e.idImpianto);
        }


        private static MariniObjectCreator _mariniCreator = new MariniObjectCreator();
        public static MariniObjectCreator MariniCreator
        {
            get { return _mariniCreator; }
        }

        public static MariniGenericObject CreateMariniObject(XmlNode node)
        {

            MariniGenericObject mgo;

            switch (node.Name)
            {
                case "impianto":
                    mgo = new MariniImpiantone(node);
                    break;
                case "zona":
                    mgo = new MariniZona(node);
                    break;
                case "predosatore":
                    mgo = new MariniPredosatore(node);
                    break;
                case "plctag":
                    mgo = new MariniPlcTag(node);
                    break;
                case "bilancia":
                    mgo = new MariniBilancia(node);
                    break;
                case "motore":
                    mgo = new MariniMotore(node);
                    break;
                case "nastro":
                    mgo = new MariniNastro(node);
                    break;
                case "amperometro":
                    mgo = new MariniAmperometro(node);
                    break;
                default:
                    mgo = new MariniOggettoBase(node);
                    break;
                //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
            }

            /* tutti gli oggetti creati vengono gestiti dalla stessa ... */
            if (mgo is MariniImpiantone)
            {
                mgo.OnManage += impianto_on_manage_method;
            }
            else
            {
                mgo.OnChange += on_change_method;
            }

            if (mgo is MariniImpiantone)
            {
                mgo.OnChange += impianto_on_change_method;
            }
            else
            {
                mgo.OnChange += on_change_method;
            }

            /* Riempio la lista oggetti con i nodi figli */
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (children.Count > 0)
                {
                    //Console.WriteLine("Parent id: {0} node.childnodes = {1}", mgo.id, node.ChildNodes.Count);
                    mgo.ListaGenericObject.Add(CreateMariniObject(child));
                }
            }

            /* restituisco l'oggetto creato: GOF factory pattern */
            return mgo;
        }

        public static void CreateMariniObjectConsole(XmlNode node)
        {
            string id = "idgenerico";
            string name = "nomegenerico";
            string description = "descrizionegenerica";
            //MariniGenericObject mgo = new MariniOggettoBase();

            //Print the node type, node name and node value of the node
            if (node.NodeType == XmlNodeType.Text)
            {
                Console.WriteLine("Type = [" + node.NodeType + "] Value = " + node.Value);
            }
            else
            {
                Console.WriteLine("Type = [" + node.NodeType + "] Name = " + node.Name);
                //mgo = new MariniOggettoBase();
            }

            //Print attributes of the node
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        case "id":
                            //mgo.id = attr.Value;
                            id = attr.Value;
                            break;
                        case "name":
                            //mgo.name = attr.Value;
                            name = attr.Value;
                            break;
                        case "description":
                            //mgo.description = attr.Value;
                            description = attr.Value;
                            break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

            //Console.WriteLine("MariniObject id: {0}", mgo.id);


            //Print individual children of the node, gets only direct children of the node
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {

                //Console.WriteLine("Nodo id: {0} name {1} description: {2}",id, name, description);
                //mgo.ListaGenericObject.Add(CreateMariniObject(child));
                CreateMariniObjectConsole(child);

            }

            //return mgo;

        }


    }
}
