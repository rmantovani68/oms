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

using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using AxSoftingAxC;
using App.Msg;
using App.PlcDataExchange;

namespace PlcServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlcDataExchange plcConnection;
        // <app>,<plctags_list>
        Dictionary<String, List<String>> clientTags = new Dictionary<String, List<String>>();
        
        MDSClient mdsClient;

        public MainWindow()
        {
            InitializeComponent();
            
            tbText.Clear();

            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application2
            mdsClient = new MDSClient("plcserver");

            LogMessage(String.Format("PLCServer - Reading OPCTags ..."));

            // read tags from xls ad connect to opc server
            plcConnection = new PlcDataExchange("plc4", "OPCTagsnew.xls", true);

            //Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += my_MessageReceived;

            //Connect to DotNetMQ server
            mdsClient.Connect();
            
            LogMessage(String.Format("...{0}\n", plcConnection.TagItems.Count));

            foreach(var item in plcConnection.TagItems)
            {
                LogMessage(String.Format("{0}\n", item.TagName, item.Value));
            }

            //Register to OnDataChanged event to get changes
            plcConnection.OnDataChanged += my_OnDataChanged;

        }

        /// <summary>
        /// This method handles DataChanged events from the opc server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event parameters</param>
        void my_OnDataChanged(object sender, PlcDataExchange.DataChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                /* lista dei messaggi da inviare <app><tag>*/
                Dictionary<string,PlcTagItem> msgsToSend = new Dictionary<string,PlcTagItem>();

                LogMessage(String.Format("OnDataChanged - Tag:{0}->{1}\n", e.TagChanged.TagName, e.TagChanged.Value));

                /* controllo presenza del plc tag nella lista delle subscriptions */
                foreach (var item in clientTags){
                    String appname = item.Key;
                    List<String> list = item.Value;
                    foreach (var tagName in list){
                        if(e.TagChanged.TagName == tagName){
                            /* trovato */
                            msgsToSend.Add(appname,e.TagChanged);
                        }
                    }
                }

                /* a questo punto ho una lista di messaggi da inviare */
                foreach (var item in msgsToSend){
                    //Create a DotNetMQ Message to send 
                    IOutgoingMessage message = mdsClient.CreateMessage();

                    //Set destination application name
                    message.DestinationApplicationName = item.Key;

                    //Create a message
                    var Msg = new PLCTagMsg { 
                
                        MsgCode = MsgCodes.PLCTagChanged, 
                        MsgDateTime = DateTime.Now, 
                        MsgDestination = message.DestinationApplicationName,
                        MsgSender = "plcserver",
                        PLCTagID = item.Value.TagName,
                        PLCTagValue = Convert.ToString(item.Value.Value)
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
                    catch 
                    {
                        // non sono riuscito a inviare il messaggio
                        LogMessage(String.Format("Messaggio non inviato\n"));
                    }
                }
            });
        }



        /*
        public interface IOnDataChanged
        {
            void SendOnDataChanged(MDSClient cli, PlcDataExchange.DataChangedEventArgs e);
        }

        public class OnDataChangedMng: IOnDataChanged
        {
            public void SendOnDataChanged(MDSClient cli, PlcDataExchange.DataChangedEventArgs e)
            {
                cli.
            }
        }

        public class OnDataChangedFilteredMng: IOnDataChanged
        {
            public void SendOnDataChanged(MDSClient cli, PlcDataExchange.DataChangedEventArgs e)
            {
                foreach (var cli in registeredClients)
                {
                    if (IsInterested(cli, e))
                        cli.Send(e);
                }
            }
        }

        IOnDataChanged onDataChangedManager = new OnDataChangedMng();
        //public delegate void OnDataChangedEventHandler(object sender, DataChangedEventArgs e);
        void my_OnDataChanged(object sender, PlcDataExchange.DataChangedEventArgs e)
        {
            // questo serve per potere 'utilizzare' i controlli creati nella MainWindow
            this.Dispatcher.Invoke(() =>
            {
                //SendOnDataChanged(e);
                onDataChangedManager.SendOnDataChanged(cli, e);
            });
        }

        private void SendOnDataChanged(PlcDataExchange.DataChangedEventArgs e)
        {
            foreach (var cli in registeredClients)
            {
                if (IsInterested(cli, e))
                    cli.SendTagChanged(e);
            }
        }

        Dictionary<string, List<PlcTagItem>> clientTags;
        bool IsInterested(MDSClient cli, PlcDataExchange.DataChangedEventArgs e)
        {
            if (clientTags[cli.ID].)
                
        }
        */

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
            // questo serve per potere 'utilizzare' i controlli creati nella MainWindow
            //this.Dispatcher.Invoke((Action)(() =>
            this.Dispatcher.Invoke(() =>
            {
                //this refer to form in WPF application 
                try {
                    //Get message 
                    var appMsg = GeneralHelper.DeserializeObject(e.Message.MessageData) as Msg;

                    /*
                    //Process message
                    Console.WriteLine("Received Message ID:{0} From : {1}",e.Message.MessageId,e.Message.SourceApplicationName);
                    Console.WriteLine("Message Code           : " + appMsg.MsgCode);
                    Console.WriteLine("Message MsgDateTime    : " + appMsg.MsgDateTime);
                    Console.WriteLine("Message MsgSender      : " + appMsg.MsgSender);
                    Console.WriteLine("Message MsgDestination : " + appMsg.MsgDestination);
                    Console.WriteLine("Message MsgText        : " + appMsg.MsgText);
                    Console.WriteLine("Source application     : " + e.Message.SourceApplicationName);
                    */

                    switch(appMsg.MsgCode){
                        case MsgCodes.SetPLCTag:
                        {
                            var appPlcTagMsg = GeneralHelper.DeserializeObject(e.Message.MessageData) as PLCTagMsg;

                            LogMessage(String.Format(" -> Set [{0}] : {1}",appPlcTagMsg.PLCTagID, appPlcTagMsg.PLCTagValue));

                            try {
                                plcConnection[appPlcTagMsg.PLCTagID].Value=appPlcTagMsg.PLCTagValue;
                                LogMessage(String.Format(" OK\n"));
                            } catch (Exception exc){
                                // tbText.AppendText(String.Format(" FALLITO ! ({0})\n",exc.Message));
                                LogMessage(String.Format(" FALLITO ! ({0})\n",exc.Message));
                            }

                        }
                        break;

                        case MsgCodes.GetPLCTag:
                        {
                            var appPlcTagMsg = GeneralHelper.DeserializeObject(e.Message.MessageData) as PLCTagMsg;

                            var value = plcConnection[appPlcTagMsg.PLCTagID].Value;

                            /* forzare l'invio del changed value al chiamante */
                        }
                        break;
                    
                        case MsgCodes.SubscribePLCTag:
                        {
                            var appPlcTagMsg = GeneralHelper.DeserializeObject(e.Message.MessageData) as PLCTagMsg;
                            
                            LogMessage(String.Format("SubscribePLCTag {0} - {1}\n",appPlcTagMsg.MsgSender,appPlcTagMsg.PLCTagID));
                            
                            /* verifico esistenza subscriber in lista */
                            if(clientTags.ContainsKey(appPlcTagMsg.MsgSender)){
                                /* altro susbscribe di una app già registrata */
                                /* ricavo la lista dal disctionary */
                                List<String> list = clientTags[appPlcTagMsg.MsgSender];
                                /* aggiungo il tag name in lista  */
                                list.Add(appPlcTagMsg.PLCTagID);
                            } else {
                                /* primo susbscribe di una nuova app */
                                /* creo la lista */
                                List<String> list = new List<String>();
                                /* aggiungo il plc tag alla lista */
                                list.Add(appPlcTagMsg.PLCTagID);
                                /* aggiungo la chiave (app) e il valore (lista) al disctionary */
                                clientTags.Add(appPlcTagMsg.MsgSender,list);
                            }
                            
                        }
                        break;
                    
                    }


                } catch (Exception exc) {
                    LogMessage(String.Format("Errore {0} in formato messaggio ID:{1} From : {2}",exc.InnerException.Message, e.Message.MessageId,e.Message.SourceApplicationName));
                }
            });
            //Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

    }
}
