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

            /* tutti gli oggetti creati vengono gestiti dalla stessa ... */
            if (mgo is MariniImpianto)
            {
                mgo.OnManage += impianto_on_manage_method;
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