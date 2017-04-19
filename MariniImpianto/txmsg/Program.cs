using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using App.Msg;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;

namespace txmsg
{
    class Program
    {
        //Create MDSClient object to connect to DotNetMQ
        //Name of this application: Application1
        private static MDSClient mdsClient;
        static int msg_counter;

        // Specify what you want to happen when the Elapsed event is raised.
        static private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            String messageText = String.Format("Messaggio numero {0} [{1}]", msg_counter, DateTime.Now);
            
            //Create a DotNetMQ Message to send 
            IOutgoingMessage message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = "plcserver";

            
            //Create a message

            /*
            var appMsg = new PLCTagMsg { 
                
                MsgCode = App.Msg.MsgCodes.SetPLCTag, 
                MsgDateTime = DateTime.Now, 
                MsgDestination = message.DestinationApplicationName,
                MsgSender = "txmsg",
                MsgText   = messageText,

                PLCTagID = "PLCTAG_AbilitaBilanciaRiciclato",
                PLCTagValue = Convert.ToString((msg_counter % 2) == 0 ? 0 : 1)
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(appMsg);
            */
            
            message.MessageData = Encoding.UTF8.GetBytes(messageText);

            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try 
            {
                //Send message
                message.Send();
                Console.WriteLine("Inviato Messaggio {0}",messageText);
                msg_counter++;
                if(msg_counter % 100 == 0){
                    Console.WriteLine("Inviato Messaggio numero {0}",msg_counter);
                }
            } 
            catch 
            {
                // non sono riuscito a inviare il messaggio
                Console.WriteLine("Messaggio non inviato");
            }
        }

        static void Main()
        {
            mdsClient = new MDSClient("txmsg");
            //Connect to DotNetMQ server
            mdsClient.Connect();

            System.Timers.Timer tTimer = new System.Timers.Timer();
            tTimer.Elapsed+=new ElapsedEventHandler(OnTimedEvent);
            tTimer.Interval=1000;
            tTimer.Enabled=true;

            Console.WriteLine("Write a text and press enter to send to Application2. Write 'exit' to stop application.");

            //Get a message from user
            var messageText = Console.ReadLine();
            while(true){
                if (string.IsNullOrEmpty(messageText) || messageText == "exit")
                {
                    break;
                }
            }

            //Disconnect from DotNetMQ server
            mdsClient.Disconnect();
        }
    }
}

