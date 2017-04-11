﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDS;
using MDS.Client;
using AxSoftingAxC;
using App.Msg;
using App.PlcDataExchange;

namespace plcserver
{
    class Program
    {
        static int msg_counter;
        static PlcDataExchange plcConnection;

        [STAThread]
        static void Main(string[] args)
        {
            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application2
            var mdsClient = new MDSClient("plcserver");

            msg_counter = 0;

            Console.WriteLine("PLCServer - Reading OPCTags ...");

            plcConnection = new PlcDataExchange("plc4", "OPCTags.xls", true);

            //Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += plcserver_MessageReceived;

            //Connect to DotNetMQ server
            mdsClient.Connect();

            //Wait user to press enter to terminate application
            Console.WriteLine("PLCServer - Ready to receive ... Press enter to exit...");
            Console.ReadLine();

            //Disconnect from DotNetMQ server
            mdsClient.Disconnect();
        }

        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        static void plcserver_MessageReceived(object sender, MessageReceivedEventArgs e)
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
                msg_counter ++;
                //if(msg_counter % 100 == 0){
                //    Console.WriteLine("Ricevuto messaggio {0}",msg_counter);
                //}
                switch(appMsg.MsgCode){
                    case MsgCodes.SetPLCTag:
                    {
                        var appPlcTagMsg = (PLCTagMsg) GeneralHelper.DeserializeObject(e.Message.MessageData);

                        Console.Write(" -> Set [{0}] : {1}",appPlcTagMsg.PLCTagID, appPlcTagMsg.PLCTagValue);

                        try {
                            var plctagitem = plcConnection[appPlcTagMsg.PLCTagID];
                            Console.Write(" name {0} ",plctagitem.TagName);
                            plctagitem.Value=true;//appPlcTagMsg.PLCTagValue;
                            Console.WriteLine(" OK");
                        } catch (Exception exc){
                            Console.WriteLine(" FALLITO ! ({0})",exc.Message);
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
                Console.WriteLine("Errore in formato messaggio ID:{0} From : {1}",e.Message.MessageId,e.Message.SourceApplicationName);
            }
            //Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }
    }
}