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

            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: manager
            mdsClient = new MDSClient("manager");

            LogMessage(String.Format("Manager..."));

            //Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += my_MessageReceived;

            //Connect to DotNetMQ server
            mdsClient.Connect();
            

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

