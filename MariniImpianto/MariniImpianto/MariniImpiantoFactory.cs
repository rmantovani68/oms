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
            Console.WriteLine("Pippo : in <{0}> qualcosa e' cambiato {1}", sender.ToString(),e.idImpianto);
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
            

            //Print the node type, node name and node value of the node
            //if (node.NodeType == XmlNodeType.Text)
            //{
            //    Console.WriteLine("Type = [" + node.NodeType + "] Value = " + node.Value);
            //}
            //else
            //{
            //    Console.WriteLine("Type = [" + node.NodeType + "] Name = " + node.Name);
            //    //mgo = new MariniOggettoBase();
            //}

            switch (node.Name)
            {
                case "impianto":
                    mgo = new MariniImpianto();

                    /*
                    Type testType = typeof(MariniHandlers);

                    MethodInfo methodInfo = testType.GetMethod("GestioneImpianto");

                    mgo.OnManage+=methodInfo.CreateDelegate;
                    */

                    break;
                case "zona":
                    mgo = new MariniZona();
                    break;
                case "predosatore":
                    mgo = new MariniPredosatore();
                    break;
                case "plctag":
                    if (node.Attributes != null)
                    {
                        /* LG: come faccio ad evitare questo new???
                         * se lo tolgo si incavola perche' dice che mgo non e' assegnato,
                         * ma cosi' e' ridondante
                         */
                        mgo = new MariniPlctag();
                        XmlAttributeCollection attrs = node.Attributes;
                        foreach (XmlAttribute attr in attrs)
                        {
                            //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                            switch (attr.Name)
                            {
                                case "tagid":
                                    //mgo.tagid = attr.Value;
                                    mgo = new MariniPlctag(attr.Value);
                                    break;
                                default:
                                    mgo = new MariniPlctag();
                                    break;
                                //throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                            }
                        }
                    }
                    else
                    {
                        mgo = new MariniPlctag();
                    }
                    break;
                case "bilancia":
                    mgo = new MariniBilancia();
                    break;
                case "motore":
                    mgo = new MariniMotore();
                    break;
                case "nastro":
                    mgo = new MariniNastro();
                    break;
                case "amperometro":
                    mgo = new MariniAmperometro();
                    break;
                default:
                    mgo = new MariniOggettoBase();
                    break;
                //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
            }

 
            //Print attributes of the node
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        case "id":
                            mgo.id = attr.Value;
                            break;
                        case "name":
                            mgo.name = attr.Value;
                            break;
                        case "description":
                            mgo.description = attr.Value;
                            break;
                        //case "tagid":
                        //    mgo.tagid = attr.Value;
                        //    break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

            // tutti gli oggetti creati vengono gestiti dalla stessa ...
            if (mgo is MariniImpianto)
            {
                mgo.OnManage+= impianto_on_manage_method;
            }
            else
            {
                mgo.OnChange += on_change_method;
            }

            if (mgo is MariniImpianto)
            {
                mgo.OnChange += impianto_on_change_method;
            }
            else
            {
                mgo.OnChange += on_change_method;
            }
                         
            //Print individual children of the node, gets only direct children of the node
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {

                if (children.Count>0)
                {

                    //Console.WriteLine("Parent id: {0} node.childnodes = {1}", mgo.id, node.ChildNodes.Count);
                
                    mgo.ListaGenericObject.Add(CreateMariniObject(child));
                
                }
            }

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
                            id=attr.Value;
                            break;
                        case "name":
                            //mgo.name = attr.Value;
                            name=attr.Value;
                            break;
                        case "description":
                            //mgo.description = attr.Value;
                            description=attr.Value;
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
