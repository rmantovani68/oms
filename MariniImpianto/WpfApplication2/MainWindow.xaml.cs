using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

using MariniImpianto;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using App.Msg;

namespace Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MDSClient mdsClient;

        public MainWindow()
        {
            InitializeComponent();
            
            tbText.Clear();

            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: manager
            mdsClient = new MDSClient("manager");

            LogMessage(String.Format("Manager...\n"));

            // Connect to DotNetMQ server
            try {
                mdsClient.Connect();
            } catch (Exception) {
                LogMessage(String.Format("Connessione a MDS non riuscita\n"));
            }

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += my_MessageReceived;

            XmlDocument doc = new XmlDocument();

            LogMessage(String.Format("Carico il file xml impianto.xml\n"));
            
            doc.Load(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            
            XmlNode root = doc.SelectSingleNode("*");
            
            MariniImpiantone impiantoMarini = (MariniImpiantone)MariniObjectCreator.CreateMariniObject(root);

            /* 
            * scorrimento dell'oggetto 
            * e creazione lista plctags 
            */
            List<MariniGenericObject> listaPlcTags = new List<MariniGenericObject>();
            
            impiantoMarini.GetObjectsByType(typeof(MariniPlcTag), listaPlcTags);

            foreach(var item in listaPlcTags){
                
                string plcTagName = ((MariniPlcTag)item).tagid;

                LogMessage(String.Format("PLCTag Name : {0}\n",plcTagName));

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
                message.TransmitRule = MessageTransmitRules.NonPersistent;

                try 
                {
                    //Send message
                    message.Send();
                    LogMessage(String.Format("Inviato Messaggio a {0}\n",message.DestinationApplicationName ));
                } 
                catch (Exception exc)
                {
                    // non sono riuscito a inviare il messaggio
                    LogMessage(String.Format("Messaggio non inviato\n"));
                }

                
            }

        }

        void LogMessage(String Message)
        {
            tbText.AppendText(Message);
            tbText.ScrollToEnd();
        }
        
        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        void my_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                try {
                    // Get message 
                    var appMsg = GeneralHelper.DeserializeObject(e.Message.MessageData) as Msg;


                    switch(appMsg.MsgCode){
                    
                    }


                } catch (Exception exc) {
                    LogMessage(String.Format("Errore {0} in formato messaggio ID:{1} From : {2}",exc.InnerException.Message, e.Message.MessageId,e.Message.SourceApplicationName));
                }
            });
            // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

    }
}

