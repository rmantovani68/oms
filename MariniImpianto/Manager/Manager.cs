using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;

using log4net;
using MariniImpianto;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using App.Msg;


namespace Manager
{
    public class Manager
    {
        MDSClient mdsClient;
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Manager(String xmlFilename)
        {
            Logger.Info("Manager application ready");

            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: manager
            mdsClient = new MDSClient("manager");

            // Connect to DotNetMQ server
            try {
                mdsClient.Connect();
            } catch (Exception ex) {
                Logger.Warn(ex.Message, ex);
            }

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += manager_MessageReceived;

            XmlDocument doc = new XmlDocument();

            // doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            doc.Load(xmlFilename);
            
            XmlNode root = doc.SelectSingleNode("*");
            
            MariniImpiantone impiantoMarini = (MariniImpiantone)MariniObjectCreator.CreateMariniObject(root);

            /* 
            * scorrimento dell'oggetto 
            * e creazione lista plctags 
            */
            List<MariniGenericObject> listaPlcTags = new List<MariniGenericObject>();
            
            // impiantoMarini.GetObjectsByType(typeof(MariniPlcTag), listaPlcTags);

            foreach(var item in listaPlcTags){
                
                string plcTagName = ((MariniPlctag)item).tagid;

                Logger.Info(String.Format("PLCTag Name : {0}\n", plcTagName));

                //Create a DotNetMQ Message to send 
                IOutgoingMessage message = mdsClient.CreateMessage();

                //Set destination application name
                message.DestinationApplicationName = "plcserver";

                //Create a message
                var Msg = new PLCTagMsg { 
                    MsgCode = MsgCodes.SubscribePLCTag, 
                    MsgDateTime = DateTime.Now, 
                    MsgDestination = message.DestinationApplicationName,
                    MsgSender = "manager",
                    PLCTagID = plcTagName,
                };

                //Set message data
                message.MessageData = GeneralHelper.SerializeObject(Msg);
            
                // message.MessageData = Encoding.UTF8.GetBytes(messageText);
                //message.TransmitRule = MessageTransmitRules.NonPersistent;

                try 
                {
                    //Send message
                    message.Send();
                    Logger.Info(String.Format("Inviato Messaggio a {0}\n",message.DestinationApplicationName ));
                } 
                catch (Exception ex)
                {
                    // non sono riuscito a inviare il messaggio
                    Logger.Warn(ex.Message, ex);
                }
            }

        }

    }
}

