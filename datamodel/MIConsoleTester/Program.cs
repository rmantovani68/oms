using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria importata con Nuget (ricercare log4net e installarlo)
// poi, per farla funzionare correttamente, bisogna modificare il file App.config
// Subito sotto a <configuration> e prima di <startup> bisogna aggiungere
//
//  <configSections>
//    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
//  </configSections>
//  <log4net>
//    <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
//      <file value="Logs\\Tester.txt"/>
//      <appendToFile value="true"/>
//      <rollingStyle value="Size"/>
//      <maxSizeRollBackups value="3"/>
//      <maximumFileSize value="10240KB"/>
//      <staticLogFileName value="true"/>
//      <layout type="log4net.Layout.PatternLayout">
//        <conversionPattern value="%-5level %date [%-5.5thread] %-40.40logger - %message%newline"/>
//      </layout>
//    </appender>
//    <root>
//      <appender-ref ref="RollingFileAppender"/>
//    </root>
//  </log4net>
//
// Il file dove verra' riversato il log e' quello specificato in <file value="Logs\\MIConsoleTester.log"/>
//
// Un altro file da modificare e' AssemblyInfo.cs, nel quale va aggiunto
// [assembly: log4net.Config.XmlConfigurator(Watch = true)]
// subito sotto [assembly: AssemblyCulture("")]
//
// Per avere un tail in Powershell usare il comando: Get-Content .\MIConsoleTester.txt -Tail 30
using log4net;
// Libreria per usare l'oggetto MethodBase in Log4Net
using System.Reflection;
// Libreria Marini!!!
using DataModel;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;
// Sottolibreria per gli events handlers
using MIConsoleTester.EventsHandlers;

namespace MIConsoleTester
{
    class Program
    {

        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Logger.Info("<<<<<<<<<< TEST STARTED >>>>>>>>>>\n");

            string XMLfilename=@"impianto-test.xml";

            try
            {
                Logger.Info("MariniImpiantoDataManager - Inizio Creazione");

                MariniImpiantoDataManager mariniDataManager = new MariniImpiantoDataManager(
                    XMLfilename, 
                    new MariniStandardXmlSerializer(), new List<IMariniEventHandler>(
                        new IMariniEventHandler[]{                            
                            new ImpiantoEventHandler(),
                            new MotoreEventHandler(),
                            new Motore1AlarmHandler()
                        }));
                Logger.Info("MariniImpiantoDataManager - Fine Creazione");
                Console.ReadKey();

                // Proviamo una descizione ricorsiva con ToPlainTextRecursive()
                Console.WriteLine("\n----------> Ecco una descrizione ricorsiva fatta mediante ToPlainTextRecursive()");
                Logger.Info("MariniImpiantoDataManager - Inizio ToPlainTextRecursive()");
                mariniDataManager.DataTree.ToPlainTextRecursive();
                Logger.Info("MariniImpiantoDataManager - Fine ToPlainTextRecursive()\n");
                // Vediamo come e' composto il dizionario con key = path
                Console.WriteLine("\n----------> Questo e' il dizionario dei path");
                foreach (KeyValuePair<string, MariniGenericObject> pair in mariniDataManager.PathObjectsDictionary)
                {
                    Console.WriteLine("{0}, {1}", pair.Key, pair.Value.name);
                }
                Console.ReadKey();

                Console.WriteLine("\n----------> Provo a cambiare la descrizione di Impianto");
                Console.WriteLine("la vecchia descrizione di {0} e' {1}", mariniDataManager.GetObjectByPath("Impianto").id, mariniDataManager.GetObjectByPath("Impianto").description);
                mariniDataManager.GetObjectByPath("Impianto").description = "ImpiantoNewDescription";
                Console.WriteLine("la nuova descrizione di {0} e' {1}", mariniDataManager.GetObjectByPath("Impianto").id, mariniDataManager.GetObjectByPath("Impianto").description);
                Console.ReadKey();

                Console.WriteLine("\n----------> Ecco un xml di IMPIANTO");
                string sXML = mariniDataManager.SerializeObject("Impianto");
                Console.WriteLine("{0}", sXML);

                Console.WriteLine("\n----------> Scatta l'allarme di Motore1");
                ((MariniProperty)mariniDataManager.GetObjectByPath("Impianto.ZonaPredosaggio.Nastro1.Motore1.Allarme")).value = "true";
                Console.ReadKey();

                Console.WriteLine("\n----------> Recupero alcune property di MOTORE 3");
                MariniBaseObject mbo = ((MariniBaseObject)mariniDataManager.GetObjectByPath("Impianto.ZonaPredosaggio.Nastro1.Motore1"));
                Console.WriteLine("Il valore della proprieta' Ritorno di {0} e' {1}", mbo.path, mbo.GetMariniPropertyFromId("Ritorno").value);
                Console.WriteLine("Il valore della proprieta' ValoreAmperometro di {0} e' {1}", mbo.path, mbo.GetMariniPropertyFromId("ValoreAmperometro").value);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("MariniImpiantoFactory: Exception caught - {0}", e);
            }
            
                              

            Logger.Info("<<<<<<<<<< TEST FINISHED >>>>>>>>>>\n\n\n");
        }

    }
}
