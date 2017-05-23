﻿#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;

using OMS.Core.Communication;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Collections.ObjectModel;
#endregion

namespace MariniImpiantiHost
{
    public class Controller
    {
        #region Singleton
        private static Controller _instance;
        public static Controller Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new Controller();
                }
                return _instance;
            }
        }
        #endregion Singleton

        #region Public Properties

        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MDSClient mdsClient {get; private set;}
        
        public string ApplicationName {get; private set;}
        
        public string PLCServerApplicationName {get; private set;}
        
        public Model model { get; private set; }

        #endregion Public Properties
        
        #region Private Properties

        private short LoopTime { get; set; }

        #endregion Private Properties

        #region Constructor
        private Controller()
        {
            // Name of this application: Manager
            ApplicationName = "Manager";
            // Name of the plc server application: PLCServer
            PLCServerApplicationName = "PLCServer";
            
            model = new Model();

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

        }
        #endregion

        
        #region Public Methods
        public void PLCAdd(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            if (plc.Connection(ApplicationName, PLCServerApplicationName))
            {
                model.ListPLCItems.Add(plc);
            }
        }

        public void PLCRemove(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            plc.Disconnection(ApplicationName, PLCServerApplicationName);

            model.ListPLCItems.Remove(plc);
        }

        public void PLCConnect(PLCItem plc)
        {
            plc.Connection(ApplicationName, PLCServerApplicationName);
        }

        public void PLCDisconnect(PLCItem plc)
        {
            plc.Disconnection(ApplicationName, PLCServerApplicationName);
        }


        public bool PLCAddTag(TagItem tag)
        {
            bool RetValue = true;

            // se già presente non lo agiungo
            if (model.ListTagItems.Contains(tag))
            {
                Logger.InfoFormat("tag {0}/{1} già presente", tag.PLCName, tag.Name);
                return false;
            }


            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = PLCServerApplicationName;

            //Create a message
            var MsgData = new PLCTagData
            {
                MsgCode = MsgCodes.SubscribePLCTag,
                Tag = new PLCTag() { PLCName = tag.PLCName, Name = tag.Name}
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;
                if (RetValue == false)
                {
                    Logger.InfoFormat("Errore in aggiunta tag {0}", tag.Name);
                }
                else
                {
                    Logger.InfoFormat("tag {0} aggiunto", tag.Name);
                }

                //Acknowledge received message
                responseMessage.Acknowledge();


            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }
            if(RetValue)
            {
                Logger.InfoFormat("Aggiunto {0}/{1}:{2}",tag.PLCName,tag.Name,tag.Type);

                /* verifica il nome del plc tag */
                model.ListTagItems.Add(tag);
            }

            return RetValue;
        }

        public bool PLCRemoveTag(TagItem tag)
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = PLCServerApplicationName;

            //Create a message
            var MsgData = new PLCTagData
            {
                MsgCode = MsgCodes.RemovePLCTag,
                Tag = new PLCTag() { PLCName = tag.PLCName, Name = tag.Name }
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();

            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            if (RetValue)
            {
                model.ListTagItems.Remove(tag);
            }
            return RetValue;
        }

        public void Close()
        {
            foreach (var plc in model.ListPLCItems)
            {
                if (plc.ConnectionStatus == PLCConnectionStatus.Connected)
                {
                    plc.Disconnection(ApplicationName, PLCServerApplicationName);
                }
            }

            mdsClient.Disconnect();
        }

        // verifica presenza di plcname nelle connessioni plc presenti 
        public bool PLCNameExists(string plcName)
        {
            bool RetVal = false;

            foreach (var plc in model.ListPLCItems)
            {
                if (plc.Name == plcName)
                {
                    RetVal = true;
                    break;
                }
            }
            return RetVal;
        }

        // verifica presenza di ipaddress nelle connessioni plc presenti 
        public bool IPAddressExists(string ipAddress)
        {
            bool RetVal = false;
            foreach (var plc in model.ListPLCItems)
            {
                if (plc.IPAddress == ipAddress)
                {
                    RetVal = true;
                    break;
                }
            }
            return RetVal;
        }

        // verifica correttezza formale nome tag ( <plcname>/<nometag>:<tipotag> )
        public bool PLCTagIsCorrect(string tagName)
        {
            bool RetVal = true;

            // split plcname e varname (es : plc4/db86.dbd58:Bool)
            string[] var1 = tagName.Split('/');
            if (var1.Count() == 2)
            {
                // split varname e var type (es : db86.dbd58:Bool)
                string[] var2 = var1[1].Split(':');
                if (var2.Count() == 2)
                {
                    var tag = new TagItem() { PLCName = var1[0], Name = var2[0], Type = var2[1] };
                    // controlla che plcname sia un plc connesso
                    if (!PLCNameExists(tag.PLCName))
                    {
                        RetVal = false;
                    }
                    else
                    {
                        // controllo tipo tag

                    }
                    // controllo esistenza tag ?
                }
                else
                {
                    RetVal = false;
                }
            }
            else
            {
                RetVal = false;
            }

            return RetVal;
        }

        //// reperisce il default ip address dai settings
        //public string GetDefaultIpAddress()
        //{
        //    return Settings.Default.IpAddress;
        //}
        
        //// reperisce il default plc name dai settings
        //public string GetDefaultPLCName()
        //{
        //    return Settings.Default.PLCName;
        //}

        #endregion Public Methods

        #region Private Methods

        private void timer_Tick(object sender, EventArgs e)
        {
            // statusbar

            // disaccoppiare e gestire multi plc
            /*
            lblReadTime.Text = Plc.Instance.CycleReadTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            LSX++;
            if (LSX >= LoopTimeDisplayRange)
            {
                while(LP.Count>LoopTimeDisplayRange)
                    LP.RemoveAt(0);
            }

            
            LP.Add(new Point(LSX, Plc.Instance.CycleReadTime.TotalMilliseconds));
            */
        }

        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        private void manager_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                // Get message 
                var Message = e.Message;
                // Get message data
                var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

                switch (MsgData.MsgCode)
                {
                    case MsgCodes.SubscribeProperty:
                        SubscribeProperty(Message);
                        break;

                    case MsgCodes.SubscribeProperties:
                        break;
                    
                    
                    
                    case MsgCodes.PLCTagsChanged:
                        /* gestione da fare */
                        break;
                    case MsgCodes.PLCTagChanged:
                        PLCTagChanged(Message);
                        break;

                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

        private bool SubscribeProperty(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertyData;
            var prop = MsgData.Prop;

            Logger.InfoFormat("{1}/{2} da {0}", Message.SourceApplicationName, prop.ObjName, prop.Name);

            // gestione subscriptions
            if (!model._Subs.ContainsKey(Message.SourceApplicationName))
            {
                model._Subs.Add(Message.SourceApplicationName, new HashSet<Subscription>());
            }

            try
            {
                model._Subs[Message.SourceApplicationName].Add(new Subscription(prop.ObjID, prop.ObjName, prop.Name));
            } 
            catch (Exception exc)
            {
                Logger.WarnFormat("Obj {0} error subscribing property {1} : {2}", prop.ObjID, prop.Name, exc.Message);
                RetValue = false;

            }

            /* invio messaggio di risposta */

            //Create a DotNetMQ Message to respond
            var ResponseMessage = Message.CreateResponseMessage();

            var ResponseData = new ResponseData
            {
                Response = RetValue
            };

            //Set message data
            ResponseMessage.MessageData = GeneralHelper.SerializeObject(ResponseData);
            ResponseMessage.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                ResponseMessage.Send();

                Logger.InfoFormat("Inviata Risposta a {0}", ResponseMessage.DestinationApplicationName);
            }
            catch (Exception exc)
            {
                // non sono riuscito a inviare il messaggio
                Logger.WarnFormat("Risposta non inviata - {0}", exc.Message);
                RetValue = false;
            }

            return RetValue;

        }

        private bool PLCTagChanged(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PLCTagData;
            var plctag = MsgData.Tag;

            Logger.InfoFormat("Ricevuto Messaggio {1}/{2}:{3} da {0}", Message.SourceApplicationName, MsgData.Tag.PLCName, MsgData.Tag.Name, MsgData.Tag.Value);
            
            // tag
            var tag = model.ListTagItems.FirstOrDefault(item => item.PLCName == plctag.PLCName && item.Address==plctag.Name);

            if (tag != null)
            {
                // 
                tag.Value = MsgData.Tag.Value.ToString();
            }
            else
            {
                Logger.InfoFormat("Tag associato a : {0}/{1} non trovata", tag.PLCName, tag.Address);
                RetValue = false;
            }

            return RetValue;
        }

        #endregion Private Methods

    }
}
