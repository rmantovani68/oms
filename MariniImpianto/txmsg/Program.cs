using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDS.Client;


namespace txmsg
{
    class Program
    {
        static void Main()
        {
            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application1
            var mdsClient = new MDSClient("txmsg");

            //Connect to DotNetMQ server
            mdsClient.Connect();

            Console.WriteLine("Write a text and press enter to send to Application2. Write 'exit' to stop application.");

            while (true)
            {
                //Get a message from user
                var messageText = Console.ReadLine();
                if (string.IsNullOrEmpty(messageText) || messageText == "exit")
                {
                    break;
                }

                //Create a DotNetMQ Message to send to Application2
                var message = mdsClient.CreateMessage();
                //Set destination application name
                message.DestinationApplicationName = "rxmsg";
                //message.DestinationServerName = "this_server2";
                //Set message data
                message.MessageData = Encoding.UTF8.GetBytes(messageText);

                //Send message
                message.Send();
            }

            //Disconnect from DotNetMQ server
            mdsClient.Disconnect();
        }
    }
}

