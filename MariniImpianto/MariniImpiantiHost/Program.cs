using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using MariniImpiantiWcfLib;
using log4net;
using System.Reflection;
using MariniImpianti;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;


namespace MariniImpiantiHost
{
    class Program
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static MDSClient mdsClient { get; private set; }

        public static string ApplicationName { get; private set; }

        public static string PLCServerApplicationName { get; private set; }

        static void Main(string[] args)
        {


            MariniImpiantoTree.InitializeFromXmlFile(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            MariniImpiantoTree mariniImpiantoTree = MariniImpiantoTree.Instance;


            Logger.Info("***********************************");
            Logger.Info("--- HOST STARTED");
            Logger.Info("***********************************");


            // Name of this application: HMIClient
            ApplicationName = "HMIClient";
            // Name of the plc server application: PLCServer
            PLCServerApplicationName = "PLCServer";

            // Create MDSClient object to connect to DotNetMQ
            mdsClient = new MDSClient(ApplicationName);

            // Connect to DotNetMQ server
            try
            {
                mdsClient.Connect();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += manager_MessageReceived;

            // sottoscrizione ai tags

            ServiceHost serviceHost = null;

            try
            {

                serviceHost = new ServiceHost(typeof(MariniImpiantiWcfLib.MariniImpiantoWCFService));
                serviceHost.Open();
                Console.WriteLine("Service is started");
                if (serviceHost != null)
                {

                    Logger.Info("***********************************");
                    Logger.Info("--- WCF SERVICE STARTED");
                    Logger.Info("***********************************");

                    bool bExit = false;
                    while (!bExit) 
                    {
                        Console.Write("\nMariniImpiantiHost >>> "); // Prompt
                        string[] tokens = Console.ReadLine().Split();
                        string command = tokens[0];
                        
                        switch (command)
                        {
                            case "getxml":
                                if (tokens.Length<2)
                                {
                                    Console.WriteLine("ERROR: getxml required argument: Object ID ");
                                }
                                else
                                {
                                    string id = tokens[1];
                                    Console.WriteLine("{0}", MariniImpiantoTree.Instance.SerializeObject(id));
                                }
                                break;

                            case "getproperty":
                                if (tokens.Length < 3)
                                {
                                    Console.WriteLine("ERROR: getproperty required argument: Object ID, property name ");
                                }
                                else
                                {
                                    string id = tokens[1];
                                    string prop = tokens[2];
                                    Console.WriteLine("{0}", MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop).GetValue(MariniImpiantoTree.Instance.GetObjectById(id), null).ToString());
                                }
                                break;
                                
                            case "changeproperty":
                                if (tokens.Length < 4)
                                {
                                    Console.WriteLine("ERROR: changeproperty required argument: Object ID, property name, new value");
                                }
                                else
                                {
                                    string id = tokens[1];
                                    string prop = tokens[2];
                                    string value = tokens[3];

                                    PropertyInfo propertyInfo = MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop);
                                    propertyInfo.SetValue(MariniImpiantoTree.Instance.GetObjectById(id), Convert.ChangeType(value, propertyInfo.PropertyType), null);

                                    Console.WriteLine("{0}.{1} = {2}",id,prop, MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop).GetValue(MariniImpiantoTree.Instance.GetObjectById(id), null).ToString());
                                }
                                break;

                            case "exit":
                                Console.WriteLine("Bye!");
                                bExit = true;
                                break;
                        }

                        
                    }
                                                            
                    serviceHost.Close();
                    serviceHost = null;
                }
            }
            catch (Exception ex)
            {
                
                Logger.Error("--- WCF SERVICE ERROR!!!");
                Logger.Error("Service can not be started, Error :" + ex.Message);

                serviceHost = null;
                Console.WriteLine("Service can not be started, Error :" + ex.Message);
                Console.ReadKey();
            }  


        }
        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        private static void manager_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                // Get message 
                var Message = e.Message;
                // Get message data
                var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

                switch (MsgData.MsgCode)
                {
                 /*
                    case MsgCodes.PLCTagsChanged:
                        // gestione da fare 
                        break;
                    case MsgCodes.PLCTagChanged:
                        PLCTagChanged(Message);
                        break;
                */
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }
    }
}
