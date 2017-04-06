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
            Console.WriteLine("Cicio : sto gestendo {0}",sender.ToString());
        }

        private static void pippo(object sender, MariniGenericObject.OnChangeEventArgs e)
        {
            Console.WriteLine("Pippo : in <{0}> qualcosa e' cambiato {1}", sender.ToString(),e.idImpianto);
        }

        static void Main(string[] args)
        {


            XmlDocument doc = new XmlDocument();
            //doc.Load(@"E:\AeL\Varie\Impianto.xml");
            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            XmlNode root = doc.SelectSingleNode("*");

            MariniImpianto impiantoMarini = (MariniImpianto)MariniObjectCreator.CreateMariniObject(root);
            //MariniObjectCreator.CreateMariniObjectConsole(root);
                       
            //Console.ReadKey();

            //impiantoMarini.AutoManage();

            //Console.ReadKey();

            //foreach (var mgo in impiantoMarini.ListaGenericObject)
            //{
            //    mgo.AutoManage();
            //}

            // impiantoMarini.AutoManageAll();


            while(true)
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
            using (TextWriter writer = new StreamWriter(@"c:\temp\classtoxml.xml"))
            {
                
                x.Serialize(Console.Out, impiantoMarini);
                Console.ReadKey();
                Console.WriteLine("Inizio Serializzazione su file temporaneo");
                x.Serialize(writer, impiantoMarini);
                Console.WriteLine("Fine Serializzazione su file temporaneo");
                Console.ReadKey();
            }



            
            Console.ReadKey();
            //using (var sr = new StreamReader(@"c:\temp\classtoxml.xml"))
            //{
            //    ClassToXml myTest2;
            //    Console.WriteLine("Inizio DeSerializzazione su file temporaneo");
            //    myTest2 = (ClassToXml)x.Deserialize(sr);
            //    Console.WriteLine("Fine DeSerializzazione su file temporaneo");
            //    Console.WriteLine("myTest2: {0} {1} {2}", myTest2.intero, myTest2.stringa, myTest2.SubTest.substringa);

            //    Console.ReadKey();
            //}


            
        }
    }
}
