using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;


namespace MariniImpianto
{
    class Program
    {

        private static void ciccio(object sender, MariniGenericObject.OnManageEventArgs e)
        {
            Console.WriteLine("Cicio : sto gestendo {0}", sender.ToString());
        }

        private static void pippo(object sender, MariniGenericObject.OnChangeEventArgs e)
        {
            Console.WriteLine("Pippo : in <{0}> qualcosa e' cambiato {1}", sender.ToString(), e.idImpianto);
        }

        static void Main(string[] args)
        {


            XmlDocument doc = new XmlDocument();
            //doc.Load(@"E:\AeL\Varie\Impianto.xml");
            Console.WriteLine("Carico il file xml impianto.xml");
            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            XmlNode root = doc.SelectSingleNode("*");
            Console.WriteLine("Creo l'oggetto MariniImpianto impiantoMarini mediante il factory MariniObjectCreator.CreateMariniObject");
            MariniImpiantone impiantoMarini = (MariniImpiantone)MariniObjectCreator.CreateMariniObject(root);
            Console.WriteLine("Ecco una descrizione ricorsiva fatta mediante ToPlainTextRecursive()");
            impiantoMarini.ToPlainTextRecursive();
            Console.ReadKey();


            Console.WriteLine("Becco i segnali al cambiamento: imserisci s o t per cambiare stato o f per finire");
            while (true)
            {
                var Key = Console.ReadKey();

                if (Key.KeyChar == 's')
                {
                    impiantoMarini.Start = true;
                }
                if (Key.KeyChar == 't')
                {
                    impiantoMarini.Start = false;
                }
                if (Key.KeyChar == 'f')
                {
                    break;
                }
            }

            XmlSerializer x;

            x = new XmlSerializer(impiantoMarini.GetType());
            using (TextWriter writer = new StreamWriter(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml"))
            {
                Console.WriteLine("Serializzo impiantoMarini");
                x.Serialize(Console.Out, impiantoMarini);
                Console.ReadKey();
                Console.WriteLine("Inizio Serializzazione su file temporaneo");
                x.Serialize(writer, impiantoMarini);
                Console.WriteLine("Fine Serializzazione su file temporaneo");
                Console.ReadKey();
            }




            Console.ReadKey();
            using (var sr = new StreamReader(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml "))
            {
                Console.WriteLine("DeSerializzo impiantoMarini.xml in impiantoMarini2");
                MariniImpiantone impiantoMarini2;
                impiantoMarini2 = (MariniImpiantone)x.Deserialize(sr);
                Console.WriteLine("Fine DeSerializzazione");
                Console.WriteLine("Ecco una descrizione ricorsiva fatta mediante AutoManageAll()");
                impiantoMarini2.ToPlainTextRecursive();
                Console.ReadKey();
            }



        }
    }
}
