//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Serialization;
//using System.IO;
//using System.Reflection;

//namespace MariniImpianto
//{
//    class Program
//    {

//        private static void ciccio(object sender, MariniGenericObject.OnManageEventArgs e)
//        {
//            Console.WriteLine("Cicio : sto gestendo {0}", sender.ToString());
//        }

//        private static void pippo(object sender, MariniGenericObject.OnChangeEventArgs e)
//        {
//            Console.WriteLine("Pippo : in <{0}> qualcosa e' cambiato {1}", sender.ToString(), e.idImpianto);
//        }

//        static void Main(string[] args)
//        {


//            XmlDocument doc = new XmlDocument();
//            //doc.Load(@"E:\AeL\Varie\Impianto.xml");
//            Console.WriteLine("Carico il file xml impianto.xml");
//            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
//            XmlNode root = doc.SelectSingleNode("*");
            
//            Console.WriteLine("Creo l'oggetto MariniImpianto impiantoMarini mediante il factory MariniObjectCreator.CreateMariniObject");
//            MariniImpiantone impiantoMarini = (MariniImpiantone)MariniObjectCreator.CreateMariniObject(root);
            
//            Console.WriteLine("Ecco una descrizione ricorsiva fatta mediante ToPlainTextRecursive()");
//            impiantoMarini.ToPlainTextRecursive();
           
//            Console.WriteLine("\n\n\n");
//            while (true)
//            {
//                Console.WriteLine("\n-----------------------------------------------------------");
//                Console.WriteLine("\nOra inserisci un id valido per un oggetto e te lo stampo.\nInserisci 'exit' per uscire:");
//                string id = Console.ReadLine(); // Get string from user
//                if (id == "exit") // Check string
//                {
//                    break;
//                }
//                else
//                {
//                    MariniGenericObject mgo = null;
//                    //impiantoMarini.GetObjectById(id,ref mgo);
//                    mgo = impiantoMarini.GetObjectById(id);
//                    if (mgo == null)
//                    {
//                        Console.WriteLine("\nNon ho trovato nulla con id {0}", id);
//                    }
//                    else
//                    {
//                        Console.WriteLine("\nEcco una descrizione di {0} ricorsiva fatta mediante ToPlainTextRecursive()", id);
//                        mgo.ToPlainTextRecursive();
//                        Console.WriteLine("\nEcco una descrizione del babbo di {0} fatta mediante ToPlainText()", id);
//                        if (mgo.parent == null)
//                        {
//                            Console.WriteLine("\nIl Babbo di id {0} e' nullo", id);
//                        }
//                        else
//                        {
//                            mgo.parent.ToPlainText();
//                        }

//                        Console.WriteLine("\nEcco una descrizione del nonno di {0} fatta mediante ToPlainText()", id);
//                        if (mgo.parent.parent == null)
//                        {
//                            Console.WriteLine("\nIl Nonno di id {0} e' nullo", id);

//                        }
//                        else
//                        {
//                            mgo.parent.parent.ToPlainText();

//                        }

//                    }
//                }
//            }








           


//            Console.WriteLine("Becco i segnali al cambiamento: imserisci s o t per cambiare stato o f per finire");
//            while (true)
//            {
//                var Key = Console.ReadKey();

//                if (Key.KeyChar == 's')
//                {
//                    impiantoMarini.Start = true;
//                }
//                if (Key.KeyChar == 't')
//                {
//                    impiantoMarini.Start = false;
//                }
//                if (Key.KeyChar == 'f')
//                {
//                    break;
//                }
//            }

//            XmlSerializer x;

//            x = new XmlSerializer(impiantoMarini.GetType());
//            using (TextWriter writer = new StreamWriter(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml"))
//            {
//                Console.WriteLine("Serializzo impiantoMarini");
//                x.Serialize(Console.Out, impiantoMarini);
//                Console.ReadKey();
//                Console.WriteLine("Inizio Serializzazione su file temporaneo");
//                x.Serialize(writer, impiantoMarini);
//                Console.WriteLine("Fine Serializzazione su file temporaneo");
//                Console.ReadKey();
//            }




//            Console.ReadKey();
//            using (var sr = new StreamReader(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml "))
//            {
//                Console.WriteLine("DeSerializzo impiantoMarini.xml in impiantoMarini2");
//                MariniImpiantone impiantoMarini2;
//                impiantoMarini2 = (MariniImpiantone)x.Deserialize(sr);
//                Console.WriteLine("Fine DeSerializzazione");
//                Console.WriteLine("Ecco una descrizione ricorsiva fatta mediante AutoManageAll()");
//                impiantoMarini2.ToPlainTextRecursive();
//                Console.ReadKey();
//            }



//        }
//    }
//}
