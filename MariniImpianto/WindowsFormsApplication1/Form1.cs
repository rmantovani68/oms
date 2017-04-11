using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MDS;
using MDS.Client;
using AxSoftingAxC;
using App.Msg;
using App.PlcDataExchange;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        PlcDataExchange plcConnection;
        TextBox tbText;

        public Form1()
        {
            InitializeComponent();

            tbText = this.textBox1;
            tbText.Clear();
            
            tbText.AppendText("Prova di inserimento testo\n");
            tbText.AppendText(String.Format("Valore intero {0}, valore booleano {1}", 0, false));
            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application2
            var mdsClient = new MDSClient("plcserver");

            tbText.AppendText(String.Format("PLCServer - Reading OPCTags ..."));

            plcConnection = new PlcDataExchange("plc4", "OPCTags.xls", true);

            //Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += my_MessageReceived;

            //Connect to DotNetMQ server
            mdsClient.Connect();



        }


        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        void my_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try {
                //Get message 
                var appMsg = (Msg) GeneralHelper.DeserializeObject(e.Message.MessageData);

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
                        var appPlcTagMsg = (PLCTagMsg) GeneralHelper.DeserializeObject(e.Message.MessageData);

                        tbText.AppendText(String.Format(" -> Set [{0}] : {1}",appPlcTagMsg.PLCTagID, appPlcTagMsg.PLCTagValue));

                        try {
                            var plctagitem = plcConnection[appPlcTagMsg.PLCTagID];
                            tbText.AppendText(String.Format(" name {0} ",plctagitem.TagName));
                            plctagitem.Value=true;//appPlcTagMsg.PLCTagValue;
                            tbText.AppendText(String.Format(" OK"));
                        } catch (Exception exc){
                            tbText.AppendText(String.Format(" FALLITO ! ({0})",exc.Message));
                        }

                    }
                    break;

                    case MsgCodes.GetPLCTag:
                    {
                        var appPlcTagMsg = (PLCTagMsg) GeneralHelper.DeserializeObject(e.Message.MessageData);

                        var value = plcConnection[appPlcTagMsg.PLCTagID].Value;

                        /* forzare l'invio del changed value al chiamante */
                    }
                    break;
                    
                    case MsgCodes.SubscribePLCTags:
                    {
                        var appPlcTagMsg = (PLCTagMsg) GeneralHelper.DeserializeObject(e.Message.MessageData);
                        // lista di plc tags da sottoscrivere

                    }
                    break;
                    
                }


            } catch {
                tbText.AppendText(String.Format("Errore in formato messaggio ID:{0} From : {1}",e.Message.MessageId,e.Message.SourceApplicationName));
            }
            //Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

    }
}
