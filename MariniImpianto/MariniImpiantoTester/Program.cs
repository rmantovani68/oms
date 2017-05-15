using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using MariniImpianti;
using log4net;


namespace MariniImpiantiTester
{
    class Program
    {

        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {


            if (false)
            {
                Logger.Info("***********************************");
                Logger.Info("          TEST STARTED");
                Logger.Info("***********************************");

                MariniImpiantoTree.InitializeFromXmlFile(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
                MariniImpiantoTree mariniImpiantoTree = MariniImpiantoTree.Instance;

                //Console.WriteLine("Ecco una descrizione ricorsiva 2 fatta mediante ToPlainTextRecursive()");
                //mariniImpiantoTree.MariniImpianto.ToPlainTextRecursive();
                //Console.WriteLine("\n\n\n");
                //Console.WriteLine("Ecco un xml di MOTORE_02");
                //string sMotore02XML=mariniImpiantoTree.SerializeObject("MOTORE_02");
                //Console.WriteLine("{0}", sMotore02XML);

                Console.WriteLine("\n\n\n");
                Console.WriteLine("Provo a cambiare il nome di IMPIANTO");
                mariniImpiantoTree.GetObjectById("IMPIANTO").name = "nuovonome";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("IMPIANTO").id, mariniImpiantoTree.GetObjectById("IMPIANTO").name);
                mariniImpiantoTree.GetObjectById("IMPIANTO").name = "nuovonome2";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("IMPIANTO").id, mariniImpiantoTree.GetObjectById("IMPIANTO").name);
                mariniImpiantoTree.GetObjectById("IMPIANTO").name = "nuovonome2";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("IMPIANTO").id, mariniImpiantoTree.GetObjectById("IMPIANTO").name);
                mariniImpiantoTree.GetObjectById("MOTORE_02").name = "nuovonome3";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").name);
                mariniImpiantoTree.GetObjectById("MOTORE_02").name = "nuovonome3";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").name);
                mariniImpiantoTree.GetObjectById("MOTORE_02").description = "nuovadescr1";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").description);
                mariniImpiantoTree.GetObjectById("MOTORE_02").description = "nuovadescr2";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").description);
                mariniImpiantoTree.GetObjectById("MOTORE_02").description = "nuovadescr2";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").description);
                mariniImpiantoTree.GetObjectById("MOTORE_01").description = "NUOVA_descrizione_motore_01";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_01").id, mariniImpiantoTree.GetObjectById("MOTORE_01").description);
                mariniImpiantoTree.GetObjectById("MOTORE_01").description = "NUOVISSIMA_descrizione_motore_01";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_01").id, mariniImpiantoTree.GetObjectById("MOTORE_01").description);
                Console.WriteLine("Prova a cambiare il nome di MOTORE_02 con le web API");
                Console.WriteLine("Quando hai finito premi invio");
                Console.ReadLine();
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("MOTORE_02").id, mariniImpiantoTree.GetObjectById("MOTORE_02").name);
                Console.ReadLine();

                //Console.WriteLine("\n\n\n");
                //Console.WriteLine("Ecco una descrizione ricorsiva 2 fatta mediante ToPlainTextRecursive()");
                //mariniImpiantoTree.MariniImpianto.ToPlainTextRecursive();
                //Console.ReadLine();

                Console.WriteLine("Provo a cambiare il nome di IMPIANTO con mariniImpiantoTree");
                mariniImpiantoTree.GetObjectById("IMPIANTO").name = "AHAhahaha";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.GetObjectById("IMPIANTO").id, mariniImpiantoTree.GetObjectById("IMPIANTO").name);

                Console.ReadLine();

                Console.WriteLine("Provo a cambiare il nome di IMPIANTO con Dict indexer");
                mariniImpiantoTree.MariniImpiantoObjectsDictionary["IMPIANTO"].name = "Buhuhuhuhhu";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.MariniImpiantoObjectsDictionary["IMPIANTO"].id, mariniImpiantoTree.MariniImpiantoObjectsDictionary["IMPIANTO"].name);

                Console.ReadLine();

                Console.WriteLine("Provo a cambiare il nome di MOTORE_02 con Dict indexer");
                mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].name = "MOTORE_02namechanged";
                Console.WriteLine("il nuovo nome di {0} e' {1}", mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].id, mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].name);

                Console.ReadLine();

                Console.WriteLine("Provo a cambiare la descrizione di MOTORE_02 con Dict indexer");
                mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].description = "MOTORE_02descrchanged";
                Console.WriteLine("La nuova descrizione di {0} e' {1}", mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].id, mariniImpiantoTree.MariniImpiantoObjectsDictionary["MOTORE_02"].description);

                Console.ReadLine();


                Console.WriteLine("\n\n\n");
                Console.WriteLine("Creo una lista di oggetti PlcTags di MOTORE_02");
                List<MariniGenericObject> mgoList = null;
                mgoList = mariniImpiantoTree.GetObjectById("MOTORE_02").GetObjectListByType(typeof(MariniPlctag));
                foreach (MariniGenericObject mgo in mgoList)
                {
                    mgo.ToPlainText();
                }

                Console.WriteLine("\n\n\n");
                Console.WriteLine("Ecco una descrizione fatta mediante ToPlainText()");
                mariniImpiantoTree.MariniImpianto.ToPlainText();
                Console.ReadLine();

                XmlSerializer x;
                x = new XmlSerializer(mariniImpiantoTree.MariniImpianto.GetType());
                using (TextWriter writer = new StreamWriter(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml"))
                {
                    Console.WriteLine("Serializzo impiantoMarini");
                    x.Serialize(Console.Out, mariniImpiantoTree.MariniImpianto);
                    Console.ReadKey();
                    Console.WriteLine("Inizio Serializzazione su file temporaneo");
                    x.Serialize(writer, mariniImpiantoTree.MariniImpianto);
                    Console.WriteLine("Fine Serializzazione su file temporaneo");
                    Console.ReadKey();
                }

                Console.ReadKey();
                using (var sr = new StreamReader(@"Q:\VARIE\ael\new-project\doc\analisi\impiantoMariniSerializzato.xml "))
                {
                    Console.WriteLine("DeSerializzo impiantoMarini.xml in impiantoMarini2");
                    MariniImpianto impiantoMarini2;
                    impiantoMarini2 = (MariniImpianto)x.Deserialize(sr);
                    Console.WriteLine("Fine DeSerializzazione");
                    Console.WriteLine("Ecco una descrizione ricorsiva fatta mediante AutoManageAll()");
                    impiantoMarini2.ToPlainTextRecursive();
                    Console.ReadKey();
                }

                Logger.Info("***********************************");
                Logger.Info("          TEST FINISHED");
                Logger.Info("***********************************");

            }
            Console.ReadKey();
        }
    }
}
