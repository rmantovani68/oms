#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.ComponentModel;
using System.Timers;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using OMS.Core.Communication;
using DataModel;
#endregion

namespace Manager
{
    class Manager
    {
        
        #region Public Fields
        public string XMLfilename = @"impianto-test.xml";
        #endregion

        #region Private Fields
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MDSClient mdsClient;

        private System.Timers.Timer timer = new System.Timers.Timer();

        // default loop time = 100
        static private int _defaultLoopTime = 100;
        // default application Name
        static private string _defaultApplicationName = "Manager";
        // default plcserver application Name
        static private string _defaultPLCServerApplicationName = "PLCServer";

        private int _loopTime;
        private DateTime lastReadTime;

        
        // lista dei plctags totali per nome tag
        private Hashtable _hashTagsAddressByName = new Hashtable();

        // lista dei plctags totali per address tag
        private Hashtable _hashTagsNameByAddress = new Hashtable();
        
        // lista dei plc connessi
        private List<PLCItem> ListPLCItems = new List<PLCItem>();

        // lista dei tag sottoscritti su plcserver
        private List<TagItem> ListTagItems = new List<TagItem>();

        // lista delle properties sottoscritte dai clients
        private Dictionary<string, HashSet<Property>> ListSubscriptions = new Dictionary<string, HashSet<Property>>();

        // MariniImpiantoTree mariniImpiantoTree;

        DataManager dataManager;

        #endregion Private Fields

        #region Properties
        
        public string ApplicationName { get; private set; }
        public string PLCServerApplicationName { get; private set; }

        public TimeSpan CycleReadTime { get; private set; }

        public int LoopTime
        {
            get { return _loopTime; }

            set
            {
                timer.Enabled = false;
                timer.Interval = value; // ms
                _loopTime = value;
                timer.Enabled = true;
            }
        }


        #endregion Properties

        #region Constructor
        
        public Manager(int loopTime, string applicationName, string plcserverApplicationName)
        {
            ApplicationName = applicationName;
            PLCServerApplicationName = plcserverApplicationName;

            Logger.InfoFormat("{0} application ready", ApplicationName);

            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: Manager
            mdsClient = new MDSClient(ApplicationName);
            mdsClient.AutoAcknowledgeMessages = true;

            // Connect to DotNetMQ server
            try
            {
                mdsClient.Connect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                // esco
                this.Exit();
            }

            // configurazione
            
            // data path 
            // xml file name
            // opctags filename
            // lista plc connessi  copn plcname,ipaddress

            // lettura del dominio dei tags 
            LoadOPCTags(@"C:\Users\uts.MARINI\Documents\projects\cyb500n\Versione 9.6.x\Exe\OPCTags.xls", "plc4");
            LoadOPCTags(@"C:\Users\uts.MARINI\Documents\projects\cyb500n\Versione 9.6.x\Exe\OPCTags.xls", "plc4cist");
            LoadOPCTags(@"C:\Users\uts.MARINI\Documents\projects\cyb500n\Versione 9.6.x\Exe\OPCTags.xls", "plc5");
            LoadOPCTags(@"C:\Users\uts.MARINI\Documents\projects\cyb500n\Versione 9.6.x\Exe\OPCTags.xls", "plc2");
            LoadOPCTags(@"C:\Users\uts.MARINI\Documents\projects\cyb500n\Versione 9.6.x\Exe\OPCTags.xls", "WamFoam");

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += Manager_MessageReceived;

            PLCServerConnect();

            // spostare su plcserver
            PLCAdd("plc4",     "213.131.0.161");
            PLCAdd("plc4cist", "213.131.0.161");
            PLCAdd("plc5",     "213.131.0.161");
            PLCAdd("plc2",     "213.131.0.161");
            PLCAdd("WamFoam",  "213.131.0.161");

            
            // configurazione

            // lettura del dominio degli oggetti
            // MariniImpiantoTree.InitializeFromXmlFile(@"C:\Users\uts.MARINI\Documents\projects\new-project\oms\MariniImpianto\impianto-test.xml");

                     
            // mariniImpiantoTree = MariniImpiantoTree.Instance;

            
            dataManager = new DataManager(
            XMLfilename, 
            new StandardXmlSerializer(), new List<IEventHandler>(
                new IEventHandler[]{                            
                    // new ImpiantoEventHandler(),
                    // new MotoreEventHandler(),
                    // new Motore1AlarmHandler()
                }));



            SubscribePLCTags(dataManager);

            LoopTime = loopTime;
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;

            Logger.InfoFormat("{0} application ready", ApplicationName);
        }

        public Manager()
            : this(_defaultLoopTime, _defaultApplicationName, _defaultPLCServerApplicationName)
        {
        }

        #endregion Constructor

        #region Event Handlers
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            CycleReadTime = DateTime.Now - lastReadTime;

            timer.Enabled = true;
            lastReadTime = DateTime.Now;
        }
        #endregion Event Handlers

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        
        internal void Exit()
        {
            //Disconnect from DotNetMQ server
            Logger.InfoFormat("{0} Exit Application", ApplicationName);

            // spostare su plcserver
            PLCRemove("plc4",     "213.131.0.161");
            PLCRemove("plc4cist", "213.131.0.161");
            PLCRemove("plc5",     "213.131.0.161");
            PLCRemove("plc2",     "213.131.0.161");
            PLCRemove("WamFoam",  "213.131.0.161");

            // disconnetto dal plcserver
            PLCServerDisconnect();

            // disconnetto dal sistema di messaggistica
            mdsClient.Disconnect();
        }

        /// <summary>
        /// sottoscrive la lista delle properties dell'impianto aventi bindtype = plctag e binddirection oneway o twoway
        /// </summary>
        /// <param name="DataManager"></param>
        /// <returns></returns>
        private bool SubscribePLCTags(DataManager dm)
        {
            bool RetVal = true;


            // recuperare la lista delle properties dell'impianto
            List<PropertyObject> props = dataManager.PathObjectsDictionary.Values.Where(item => item.GetType() == typeof(PropertyObject)).Cast<PropertyObject>().ToList();
            

            foreach (var prop in props)
            {
                // chiamo l'handler sul cambiamento del valore della property 
                prop.PropertyChanged += PropertyValueChangedHandler;

                // se la property è connessa ad un tag plc sottoscrivo il tag (plcserver)
                if (prop.bindtype == BindType.PLCTag)
                {
                    bool bOK = true;
                    string PLCTagName = prop.bind;

                    var plctag = GetPLCTagData(PLCTagName);

                    if (plctag == null) { bOK = false; }

                    if (bOK)
                    {

                        switch (prop.binddirection)
                        {
                            case BindDirection.OneWayToSource:
                                break;
                            case BindDirection.OneTime:
                                break;
                            case BindDirection.OneWay:
                            // sottoscrivo il tag in quanto è in lettura da PLC
                            case BindDirection.TwoWay:
                                // sottoscrivo il tag in quanto è in lettura/scrittura da PLC
                                // gestione errore
                                Logger.InfoFormat("sottoscrivo property : {0}:{1}-{2}/{3}:{4} {5}",
                                    prop.name,
                                    plctag.Name,
                                    plctag.PLCName,
                                    plctag.Address,
                                    plctag.Type,
                                    prop.binddirection);

                                PLCAddTag(plctag);

                                break;
                        }
                    }
                    else
                    {
                        // gestione errore
                        Logger.WarnFormat("Errore in lettura property : {0}:{1}", prop.name, prop.bind != null ? prop.bind : "no bind");
                        RetVal = false;
                    }
                }
            }
            return RetVal;
        }

        /// <summary>
        /// creo un tag plc completo (plcname/address:type da un tag name)
        /// </summary>
        /// <param name="PLCTagName"></param>
        /// <returns></returns>
        private TagItem GetPLCTagData(string PLCTagName)
        {
            bool bOK = true;

            string PLCTagFullAddress = null;
            string PLCTagPLCName = null;
            string PLCTagAddress = null;
            string PLCTagType = null;

            if (PLCTagName == null || PLCTagName.Length == 0)
            {
                bOK = false;
            }

            try
            {
                PLCTagFullAddress = _hashTagsAddressByName[PLCTagName].ToString();
                bOK = true;
            }
            catch (Exception exc)
            {
                Logger.WarnFormat("Tag : {0} non presente {1}", PLCTagName, exc.Message);
                bOK = false;
            }

            if (bOK)
            {
                bOK = false;

                // split plcname e varname (es : plc4/db86.dbd58:Bool)
                string[] var1 = PLCTagFullAddress.Split('/');
                if (var1.Count() == 2)
                {
                    // controllo plc name
                    PLCTagPLCName = var1[0];
                    // split varname e var type (es : db86.dbd58:Bool)
                    string[] var2 = var1[1].Split(':');
                    if (var2.Count() == 2)
                    {
                        // controlla address
                        PLCTagAddress = var2[0];
                        // controlla tipo
                        PLCTagType = var2[1];
                        bOK = true;
                    }
                }
            }
            
            TagItem tag = null;
            if (bOK)
            {
                tag = new TagItem(PLCTagName, PLCTagAddress, PLCTagPLCName, PLCTagType);
            }
            return tag;
        }

        /// <summary>
        /// Connette a plcserver
        /// </summary>
        /// <returns></returns>
        private bool PLCServerConnect()
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = PLCServerApplicationName;

            //Create a message
            var MsgData = new MsgData
            {
                MsgCode = MsgCodes.ConnectSubscriber,
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            if (RetValue)
            {
                Logger.InfoFormat("Connesso");
            }

            return RetValue;
        }

        /// <summary>
        /// Disconnette da plcserver
        /// </summary>
        /// <returns></returns>
        private bool PLCServerDisconnect()
        {
            bool RetValue = true;

            //Create a DotNetMQ Message to send 
            var message = mdsClient.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = PLCServerApplicationName;

            //Create a message
            var MsgData = new MsgData
            {
                MsgCode = MsgCodes.DisconnectSubscriber,
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            if (RetValue)
            {
                Logger.InfoFormat("Disconnesso");
            }

            return RetValue;
        }


        
        private void LoadOPCTags(string xlsPathName, string PLCName)
        {
            ReadTagFromXLS(xlsPathName, PLCName);
        }

        /// <summary>
        /// Legge gli indirizzi dei tag dal file XLS e aggiunge gli items a due hashtables : name/address e address/nome
        /// </summary>
        /// <param name="PlcName"></param>
        private void ReadTagFromXLS(string xlsFilePath, string PLCName)
        {
            try
            {
                string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", xlsFilePath);
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [" + PLCName + "$]", connectionString))
                {
                    DataTable dataTable = new DataTable("CybertronicTags");
                    dataAdapter.Fill(dataTable);

                    var PLCTags = from row in dataTable.AsEnumerable()
                                  select new
                                  {
                                      TagName = row.Field<String>(0).Trim(),
                                      TagAddress = row.Field<String>(1).Trim(),
                                      TagDescription = row.Field<String>(2)!=null ? row.Field<String>(2).Trim():"",
                                  };

                    foreach (var tag in PLCTags)
                    {
                        _hashTagsAddressByName.Add(tag.TagName, tag.TagAddress);   // Dictionary Name-Address
                        _hashTagsNameByAddress.Add(tag.TagAddress, tag.TagName);   // Dictionary Address-Name
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.WarnFormat("{0}", exc.Message);
            }
        }

        /// <summary>
        /// Aggiunta PLC a lista PLC
        /// </summary>
        /// <param name="plcName">Nome del plc</param>
        /// <param name="ipAddress">indirizzo ip del plc</param>
        private void PLCAdd(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            if (plc.Connection(ApplicationName, PLCServerApplicationName))
            {
                ListPLCItems.Add(plc);
            }
        }

        /// <summary>
        /// Rimozione PLC da lista PLC
        /// </summary>
        /// <param name="plcName">Nome del plc</param>
        /// <param name="ipAddress">indirizzo ip del plc</param>
        private void PLCRemove(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            plc.Disconnection(ApplicationName, PLCServerApplicationName);
            ListPLCItems.Remove(plc);
        }

        /// <summary>
        /// Connessione a PLC
        /// </summary>
        /// <param name="plc">PLC da connettere</param>
        private void PLCConnect(PLCItem plc)
        {
            plc.Connection(ApplicationName, PLCServerApplicationName);
        }

        /// <summary>
        /// Disconnessione da PLC
        /// </summary>
        /// <param name="plc">PLC da disconnettere</param>
        private void PLCDisconnect(PLCItem plc)
        {
            plc.Disconnection(ApplicationName, PLCServerApplicationName);
        }

        /// <summary>
        /// Sottoscrizione tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private bool PLCAddTag(TagItem tag)
        {
            bool RetValue = true;

            // se già presente non lo agiungo
            if (ListTagItems.Contains(tag))
            {
                Logger.InfoFormat("tag {0}/{1} già presente", tag.PLCName, tag.Address);
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
                Tag = new PLCTag() { PLCName = tag.PLCName, Address = tag.Address }
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            if (RetValue)
            {
                Logger.InfoFormat("Aggiunto {0}/{1}:{2}", tag.PLCName, tag.Address, tag.Type);

                /* verifica il nome del plc tag */
                ListTagItems.Add(tag);
            }

            return RetValue;
        }

        /// <summary>
        /// Rimozione sottoscrizione tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private bool PLCRemoveTag(TagItem tag)
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
                Tag = new PLCTag() { PLCName = tag.PLCName, Address = tag.Address }
            };

            //Set message data
            message.MessageData = GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch
            {
                // non sono riuscito a inviare il messaggio
                Logger.InfoFormat("Messaggio non inviato");
                RetValue = false;
            }

            if (RetValue)
            {
                ListTagItems.Remove(tag);
            }

            return RetValue;
        }



        /// <summary>
        /// verifica correttezza formale nome tag ( <plcname>/<nometag>:<tipotag> )
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private bool PLCTagIsCorrect(string tagName)
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


        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        private void Manager_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Get message 
            var Message = e.Message;

            try
            {
                // Get message data
                var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

                switch (MsgData.MsgCode)
                {
                    case MsgCodes.ConnectSubscriber:      ConnectSubscriber(Message);     break;
                    case MsgCodes.DisconnectSubscriber:   DisconnectSubscriber(Message);  break;
                    case MsgCodes.SubscribeProperty:      SubscribeProperty(Message);     break;
                    case MsgCodes.SubscribeProperties:    SubscribeProperties(Message);   break;
                    case MsgCodes.RemoveProperty:         RemoveProperty(Message);        break;
                    case MsgCodes.RemoveProperties:       RemoveProperties(Message);      break;
                    case MsgCodes.PLCTagsChanged:         PLCTagsChanged(Message);        break;
                    case MsgCodes.PLCTagChanged:          PLCTagChanged(Message);         break;

                    case MsgCodes.ResultSubscribePLCTag:     /* Da Implementare */ break;
                    case MsgCodes.ResultSubscribePLCTags:    /* Da Implementare */ break;
                    case MsgCodes.ResultRemovePLCTag:        /* Da Implementare */ break;
                    case MsgCodes.ResultRemovePLCTags:       /* Da Implementare */ break;
                    case MsgCodes.ResultStartCheckPLCTags:   /* Da Implementare */ break;
                    case MsgCodes.ResultStopCheckPLCTags:    /* Da Implementare */ break;
                    case MsgCodes.ResultSetPLCTag:           /* Da Implementare */ break;
                    case MsgCodes.ResultSetPLCTags:          /* Da Implementare */ break;
                    case MsgCodes.ResultGetPLCTag:           /* Da Implementare */ break;
                    case MsgCodes.ResultGetPLCTags:          /* Da Implementare */ break;
                    case MsgCodes.ResultConnectPLC:          /* Da Implementare */ break;
                    case MsgCodes.ResultDisconnectPLC:       /* Da Implementare */ break;
                    case MsgCodes.PLCStatusChanged:          /* Da Implementare */ break;
                    case MsgCodes.SubscribedPLCTags:         /* Da Implementare */ break;
                    case MsgCodes.PLCStatus:                 /* Da Implementare */ break;

                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.Message, ex);
            }

            // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }

        private bool ConnectSubscriber(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data (not useful for now)
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

            Logger.InfoFormat("{1} da {0}", Message.SourceApplicationName, MsgData.MsgCode);

            // gestione subscriptions
            if (!ListSubscriptions.ContainsKey(Message.SourceApplicationName))
            {
                ListSubscriptions.Add(Message.SourceApplicationName, new HashSet<Property>());
            }
            else
            {
                foreach (var sub in ListSubscriptions[Message.SourceApplicationName].ToList())
                {
                    RemoveProperty(Message.SourceApplicationName, new Property() { ObjPath = sub.ObjPath });
                }
            }

            /* invio messaggio di risposta */
            return SendResponse(Message, MsgCodes.ResultConnectSubscriber, MsgData, RetValue);
        }

        private bool DisconnectSubscriber(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data (not useful for now)
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

            Logger.InfoFormat("{1} da {0}", Message.SourceApplicationName, MsgData.MsgCode);

            // gestione subscriptions
            if (ListSubscriptions.ContainsKey(Message.SourceApplicationName))
            {
                foreach (var sub in ListSubscriptions[Message.SourceApplicationName].ToList())
                {
                    RemoveProperty(Message.SourceApplicationName, new Property() { ObjPath = sub.ObjPath });
                }
                ListSubscriptions.Remove(Message.SourceApplicationName);
            }
            else
            {
                // non esiste !
                Logger.WarnFormat("{0} non sottoscritto!", Message.SourceApplicationName);
                RetValue = false;
            }

            /* invio messaggio di risposta */
            return SendResponse(Message, MsgCodes.ResultDisconnectSubscriber, MsgData, RetValue);
        }


        private bool SubscribeProperty(string subscriber, Property prop)
        {
            bool RetValue = true;

            // verifica che la property esista e sia sottoscrivibile
            var property = dataManager.GetObjectByPath(prop.ObjPath) as PropertyObject;

            if (property == null)
            {
                RetValue = false;
            }

            if (RetValue)
            {
                // gestione subscriptions
                if (!ListSubscriptions.ContainsKey(subscriber))
                {
                    // prima volta .... registro il richiedente
                    ListSubscriptions.Add(subscriber, new HashSet<Property>());
                }

                try
                {
                    ListSubscriptions[subscriber].Add(prop);
                }
                catch (Exception exc)
                {
                    Logger.WarnFormat("Error subscribing property {0} : {1}", property.path, exc.Message);
                    RetValue = false;
                }
            }

            return RetValue;
        }

        private bool SubscribeProperty(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertyData;
            var prop = MsgData.Prop;

            Logger.InfoFormat("{1} da {0}", Message.SourceApplicationName, prop.ObjPath);

            
            RetValue = SubscribeProperty(Message.SourceApplicationName, prop);

            /* invio messaggio di risposta */
            RetValue = SendResponse(Message, MsgCodes.ResultSubscribeProperty, MsgData, RetValue);

            // all'atto della sottoscrizione invio valore attuale della property
            if (RetValue) 
            {
                PropertyNotifyToSubscriber(Message.SourceApplicationName, prop);
            }


            return RetValue;
        }

        private bool SubscribeProperties(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertiesData;
            var props = MsgData.Props;

            Logger.InfoFormat("{0}", Message.SourceApplicationName);

            for (int i = 0; i < props.Count(); i++)
            {
                var prop = props[i];
                if (!SubscribeProperty(Message.SourceApplicationName, prop)) 
                { 
                    RetValue = false;
                    prop.Validation = false;
                }
                else
                {
                    prop.Validation = true;
                }
                
            }

            /* invio messaggio di risposta */
            return SendResponse(Message, MsgCodes.ResultSubscribeProperties, MsgData, RetValue);
        }

        private bool RemoveProperty(string subscriber, Property prop)
        {
            bool RetValue = true;

            // verifica che la property sia sottoscritta
            if (ListSubscriptions.ContainsKey(subscriber))
            {
                if(ListSubscriptions[subscriber].Contains(prop)){
                    // trovata, la cancello
                    try
                    {
                        ListSubscriptions[subscriber].Remove(prop);
                    }
                    catch (Exception exc)
                    {
                        Logger.WarnFormat("Error removing property {0} : {1}", prop.ObjPath, exc.Message);
                        RetValue = false;
                    }
                }
                else 
                {
                    // non c'è
                    Logger.WarnFormat("Property {0} : doesn't exists", prop.ObjPath);
                    RetValue = false;
                }
            }

            return RetValue;
        }


        private bool RemoveProperty(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertyData;
            var prop = MsgData.Prop;

            Logger.InfoFormat("{1} da {0}", Message.SourceApplicationName, prop.ObjPath);

            RetValue = RemoveProperty(Message.SourceApplicationName, prop);

            /* invio messaggio di risposta */
            return SendResponse(Message, MsgCodes.ResultRemoveProperty, MsgData, RetValue);
        }

        private bool RemoveProperties(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertiesData;
            var props = MsgData.Props;

            Logger.InfoFormat("{0}", Message.SourceApplicationName);

            for (int i = 0; i < props.Count(); i++)
            {
                var prop = props[i];
                if (!RemoveProperty(Message.SourceApplicationName, prop)) 
                { 
                    RetValue = false;
                    prop.Validation = false;
                }
                else
                {
                    prop.Validation = true;
                }
                
            }

            /* invio messaggio di risposta */
            return SendResponse(Message, MsgCodes.ResultRemoveProperties, MsgData, RetValue);
        }

        /// <summary>
        /// Ricezione messaggio di valore di plctag sottoscritto cambiato
        /// se la property associata è sottoscritta invio messaggio di notifica al sottoscrittore
        /// </summary>
        /// <param name="Message"></param>
        /// <returns><c>true</c> if the tag is effectively subscribed; otherwise, <c>false</c>.</returns>
        private bool PLCTagChanged(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PLCTagData;
            var plctag = MsgData.Tag;

            Logger.InfoFormat("{0} -> {1}/{2}:{3}", Message.SourceApplicationName, plctag.PLCName, plctag.Address, plctag.Value);

            // trova il tag corrispondente all'indirizzo sottoscritto
            TagItem tag;
            if((tag = ListTagItems.FirstOrDefault(item => item.PLCName == plctag.PLCName && item.Address == plctag.Address))!=null)
            {
                // 
                try
                {
                    tag.Value = MsgData.Tag.Value;
                }
                catch (Exception exc)
                {
                    Logger.WarnFormat("Errore in cambio Tag value {0} : {1}/{2}:{3} -> {4} {5}", tag.Name, tag.PLCName, tag.Address, tag.Type, tag.Value,exc.Message);
                    RetValue = false;
                }
                
                // recupero la lista delle properties dell'impianto
                // LG: Ma la uso anche in SubscribePLCTags!!! Devo fare tutte le volte sta roba? 
                List<PropertyObject> props = dataManager.PathObjectsDictionary.Values.Where(item => item.GetType() == typeof(PropertyObject)).Cast<PropertyObject>().ToList();

                // trova la property associata e cambia il valore
                PropertyObject property = props.FirstOrDefault(prp => prp.bind == tag.Name);

                if (property != null)
                {
                    try
                    {
                        property.value = tag.Value;
                    }
                    catch (Exception exc)
                    {
                        Logger.WarnFormat("Errore in cambio valore property {0}:{1} {2}", property.path,tag.Value,exc.Message);
                        RetValue = false;
                    }
                }
            }
            else
            {
                Logger.InfoFormat("Tag associato a : {0}/{1} non trovato", plctag.PLCName, plctag.Address);
                RetValue = false;
            }
            return RetValue;
        }

        private bool PLCTagsChanged(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PLCTagsData;
            var plctags = MsgData.Tags;

            Logger.InfoFormat("Ricevuto Messaggio da {0}", Message.SourceApplicationName);

            foreach (var plctag in plctags){
                bool bOK = true;

                // trova il tag corrispondente (uno solo) all'indirizzo sottoscritto
                TagItem tag = ListTagItems.FirstOrDefault(item => item.PLCName == plctag.PLCName && item.Address == plctag.Address);

                if (tag != null)
                {
                    // 
                    try
                    {
                        tag.Value = plctag.Value;
                        Logger.InfoFormat("Cambiato Tag {0} : {1}/{2}:{3} -> {4}", tag.Name, tag.PLCName, tag.Address, tag.Type, tag.Value);
                    }
                    catch (Exception exc)
                    {
                        Logger.WarnFormat("Errore in cambio Tag value {0} : {1}/{2}:{3} -> {4} {5}", tag.Name, tag.PLCName, tag.Address, tag.Type, tag.Value, exc.Message);
                    }

                    // recuperare la lista delle properties dell'impianto
                    // LG: Ma la uso anche in SubscribePLCTags e PLCTagChanged!!! Devo fare tutte le volte sta roba? 
                    List<PropertyObject> props = dataManager.PathObjectsDictionary.Values.Where(item => item.GetType() == typeof(PropertyObject)).Cast<PropertyObject>().ToList();
                    // trova la property associata e cambia il valore
                    PropertyObject property = props.FirstOrDefault(prp => prp.bind == tag.Name);

                    if (property != null)
                    {
                        try
                        {
                            property.value = tag.Value;
                        }
                        catch (Exception exc)
                        {
                            Logger.WarnFormat("Errore in cambio valore property {0}:{1} {2}", property.path, tag.Value, exc.Message);
                            bOK = false;
                        }
                    }
                }
                else
                {
                    Logger.InfoFormat("Tag associato a : {0}/{1} non trovato", plctag.PLCName, plctag.Address);
                    bOK = false;
                }
                if (bOK == false) { RetValue = bOK; }
            }
            return RetValue;
        }


        /// <summary>
        /// metodo chiamato sul cambiamento del valore di una property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PropertyValueChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            PropertyObject mp = sender as PropertyObject;

            /* se la property ha un plctag associato ... */
            switch (mp.binddirection)
            {
                case BindDirection.TwoWay:
                case BindDirection.OneWayToSource:
                    // setto il plc tag corrispondente
                    SetPropertyPLCTag(mp);
                    break;
                case BindDirection.OneTime:
                    break;
                case BindDirection.OneWay:
                    break;
            }

            // notifico l'eventuale sottoscrizione 
            ObjectPropertyNotifyToSubscribers(mp);
        }

        /// <summary>
        /// invia la notifica ai sottoscrittori 
        /// </summary>
        /// <param name="mp">Property to notify</param>
        private void ObjectPropertyNotifyToSubscribers(PropertyObject mp)
        {
            foreach (var subscriber in ListSubscriptions.Keys)
            {
                Property property = null;
                // cerco la property con path corrispondente
                foreach(var prop in ListSubscriptions[subscriber].ToList()){
                    if (prop.ObjPath == mp.path)
                    {
                        property = prop;
                        break;
                    }
                }

                if (property != null)
                {
                    // assegno il valore alla property
                    property.Value = mp.value;

                    // Mando messaggio di property changed al sottoscrittore
                    //Create a DotNetMQ Message to send 
                    var message = mdsClient.CreateMessage();

                    //Set destination application name
                    message.DestinationApplicationName = subscriber;

                    //Create a message
                    var MsgData = new PropertyData
                    {
                        MsgCode = MsgCodes.PropertyChanged,
                        Prop = property
                    };

                    //Set message data
                    message.MessageData = GeneralHelper.SerializeObject(MsgData);
                    message.TransmitRule = MessageTransmitRules.NonPersistent;

                    try
                    {
                        //Send message
                        message.Send();
                        Logger.InfoFormat("{1}:{2} : Inviato valore a {0}", message.DestinationApplicationName, property.ObjPath, property.Value.ToString());
                    }
                    catch (Exception exc)
                    {
                        // non sono riuscito a inviare il messaggio
                        Logger.WarnFormat("valore non inviato - {0}", exc.Message);
                    }
                }
            }
        }

        // invia la notifica ai sottoscrittori 
        private bool PropertyNotifyToSubscriber(string subscriber, Property prop)
        {
            bool bOK=true;

            var mp = dataManager.GetObjectByPath(prop.ObjPath) as PropertyObject;
            if(mp !=null)
            {
                prop.Value = mp.value;
            } 
            else
            {
                bOK = false;
            }

            if(bOK)
            {
                // Mando messaggio di property changed al sottoscrittore
                var message = mdsClient.CreateMessage();

                //Set destination application name
                message.DestinationApplicationName = subscriber;

                //Create a message
                var MsgData = new PropertyData
                {
                    MsgCode = MsgCodes.PropertyChanged,
                    Prop = prop
                };

                //Set message data
                message.MessageData = GeneralHelper.SerializeObject(MsgData);
                message.TransmitRule = MessageTransmitRules.NonPersistent;

                try
                {
                    //Send message
                    message.Send();
                    Logger.InfoFormat("{1}:{2} : Inviato valore a {0}", message.DestinationApplicationName, prop.ObjPath, prop.Value.ToString());
                }
                catch (Exception exc)
                {
                    // non sono riuscito a inviare il messaggio
                    Logger.WarnFormat("valore non inviato - {0}", exc.Message);
                    bOK = false;
                }
            }
            return bOK;
        }





        private bool SetPropertyPLCTag(PropertyObject mp)
        {
            bool bOK = true;

            string PLCTagName = mp.bind;

            var plctag = GetPLCTagData(PLCTagName);

            if (plctag == null) { bOK = false; }

            if (bOK)
            {
                // Mando messaggio di set tag a plcserver
                //Create a DotNetMQ Message to send 
                var message = mdsClient.CreateMessage();

                //Set destination application name
                message.DestinationApplicationName = PLCServerApplicationName;

                //Create a message
                var MsgData = new PLCTagData
                {
                    MsgCode = MsgCodes.SetPLCTag,
                    Tag = new PLCTag()
                    {
                        PLCName = plctag.PLCName,
                        Address = plctag.Address,
                        Value = mp.value
                    }
                };

                //Set message data
                message.MessageData = GeneralHelper.SerializeObject(MsgData);
                message.TransmitRule = MessageTransmitRules.NonPersistent;

                try
                {
                    // send message
                    message.Send();
                    Logger.InfoFormat("Set Value {0}:{1}", plctag.Name,  mp.value);
                }
                catch (Exception exc)
                {
                    // non sono riuscito a inviare il messaggio
                    Logger.WarnFormat("Exception : {0}", exc.Message);
                }
            }
            return bOK;
        }


        private bool SendResponse(IIncomingMessage receivedMsg, MsgCodes MsgCode, MsgData MessageData, bool Result)
        {
            bool bOK = true;
            var message = mdsClient.CreateMessage();

            // Set message data
            MessageData.MsgCode = MsgCode;
            MessageData.validation = Result;

            // Set message params
            message.MessageData = GeneralHelper.SerializeObject(MessageData);
            message.DestinationApplicationName = receivedMsg.SourceApplicationName;
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
                Logger.InfoFormat("Inviato msg a {0}", message.DestinationApplicationName);
            }
            catch (Exception exc)
            {
                // non sono riuscito a inviare il messaggio
                Logger.WarnFormat("Messaggio non inviato - {0}", exc.Message);
                bOK = false;
            }

            return bOK;
        }

        #endregion Private Methods
    }
}
