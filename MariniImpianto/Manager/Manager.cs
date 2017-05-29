using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using OMS.Core.Communication;
using MariniImpianti;
using System.Data.OleDb;
using System.Collections;
using System.ComponentModel;

namespace Manager
{
    class Manager
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Private Fields

        private MDSClient mdsClient;
        
        // lista dei plctags per nome tag
        private Hashtable _hashTagsAddressByName = new Hashtable();
        
        // lista dei plctags per address tag
        private Hashtable _hashTagsNameByAddress = new Hashtable();
        
        // lista dei plc connessi
        private List<PLCItem> ListPLCItems = new List<PLCItem>();

        // lista dei tag sottoscritti
        private List<TagItem> ListTagItems = new List<TagItem>();

        // lista delle properties sottoscritte
        private Dictionary<string, HashSet<Property>> ListSubscriptions = new Dictionary<string, HashSet<Property>>();

        MariniImpiantoTree mariniImpiantoTree;

        #endregion Private Fields

        #region Public Fields

        public string ApplicationName { get; private set; }
        public string PLCServerApplicationName { get; private set; }


        #endregion Public Fields

        #region Constructor
        public Manager()
        {
            ApplicationName = "Manager";
            PLCServerApplicationName = "PLCServer";

            Logger.InfoFormat("{0} application ready", ApplicationName);

            // Create MDSClient object to connect to DotNetMQ
            // Name of this application: PLCServer
            mdsClient = new MDSClient(ApplicationName);

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

            // Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += Manager_MessageReceived;

            PLCServerConnect();

            // spostare su plcserver
            PLCAdd("plc4", "213.131.0.161");

            
            // configurazione

            // lettura del dominio degli oggetti
            MariniImpiantoTree.InitializeFromXmlFile(@"C:\Users\uts.MARINI\Documents\projects\new-project\oms\MariniImpianto\impianto-test.xml");
            
            mariniImpiantoTree = MariniImpiantoTree.Instance;


            SubscribePLCTags(mariniImpiantoTree);


        }
        #endregion Constructor

        #region Public Methods
        #endregion Public Methods

        #region Private Methods
        
        internal void Exit()
        {
            //Disconnect from DotNetMQ server
            Logger.InfoFormat("{0} Exit Application", ApplicationName);

            // disconnetto dal plcserver
            PLCServerDisconnect();
            // disconnetto dal sistema di messaggisstica
            mdsClient.Disconnect();
        }

        // sottoscrive la lista delle properties dell'impianto aventi bindtype = plctag e binddirection oneway o twoway
        private bool SubscribePLCTags(MariniImpiantoTree mariniImpiantoTree)
        {
            bool RetVal = true;
            // recuperare la lista delle properties dell'impianto
            List<MariniProperty> props = mariniImpiantoTree.MariniImpianto.GetObjectListByType(typeof(MariniProperty)).Cast<MariniProperty>().ToList();

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

        // creo un tag plc completo (plcname/address:type da un tag name)
        private TagItem GetPLCTagData(string PLCTagName)
        {
            TagItem tag = null;

            string PLCTagFullAddress = null;
            string PLCTagPLCName = null;
            string PLCTagAddress = null;
            string PLCTagType = null;

            bool bOK = true;

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
            if (bOK)
            {
                tag = new TagItem() { 
                    Name = PLCTagName,
                    PLCName = PLCTagPLCName,
                    Address = PLCTagAddress,
                    Type = PLCTagType
                };
            }

            return tag;
        }

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
#if eliminato
                var responseMessage = message.SendAndGetResponse();
                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;
                if (RetValue == false)
                {
                    Logger.InfoFormat("Errore in connessione");
                }

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
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
#if eliminato
                var responseMessage = message.SendAndGetResponse();
                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;
                if (RetValue == false)
                {
                    Logger.InfoFormat("Errore in disconnessione");
                }

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
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


        // LoadOPCTags("OPCTags.xls", "plc4")
        private void LoadOPCTags(string xlsPathName, string PLCName)
        {
            ReadTagFromXLS(xlsPathName, PLCName);
        }

        /// <summary>
        /// Legge gli indirizzi dei tag dal file XLS e crea un hashtable in cui la chiave è il nome del tag (PLCTAG_...)
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
                                  };

                    foreach (var tag in PLCTags)
                    {
                        _hashTagsAddressByName.Add(tag.TagName, tag.TagAddress);   //Dictionary Name-Address
                        _hashTagsNameByAddress.Add(tag.TagAddress, tag.TagName);   //Dictionary Address-Name
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.WarnFormat("{0}", exc.Message);
            }
        }


        private void PLCAdd(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            if (plc.Connection(ApplicationName, PLCServerApplicationName))
            {
                ListPLCItems.Add(plc);
            }
        }

        private void PLCRemove(string plcName, string ipAddress)
        {
            var plc = new PLCItem(plcName, ipAddress, mdsClient);

            plc.Disconnection(ApplicationName, PLCServerApplicationName);
            ListPLCItems.Remove(plc);
        }

        private void PLCConnect(PLCItem plc)
        {
            plc.Connection(ApplicationName, PLCServerApplicationName);
        }

        private void PLCDisconnect(PLCItem plc)
        {
            plc.Disconnection(ApplicationName, PLCServerApplicationName);
        }

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
#if eliminato
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
#endif
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
#if eliminato
                var responseMessage = message.SendAndGetResponse();

                Logger.InfoFormat("Inviato Messaggio a {0}", message.DestinationApplicationName);

                //Get connect result
                var ResponseData = GeneralHelper.DeserializeObject(responseMessage.MessageData) as ResponseData;

                RetValue = ResponseData.Response;

                //Acknowledge received message
                responseMessage.Acknowledge();
#endif
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



        // verifica correttezza formale nome tag ( <plcname>/<nometag>:<tipotag> )
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
                    var tag = new TagItem() { PLCName = var1[0], Name = var2[0], Type = var2[1] };

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

            try
            {
                // Get message 
                var Message = e.Message;

                // Get message data
                var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as MsgData;

                // Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
                e.Message.Acknowledge();

                switch (MsgData.MsgCode)
                {
                    case MsgCodes.ConnectSubscriber:
                        ConnectSubscriber(Message);
                        break;

                    case MsgCodes.DisconnectSubscriber:
                        DisconnectSubscriber(Message);
                        break;

                    case MsgCodes.SubscribeProperty:
                        SubscribeProperty(Message);
                        break;

                    case MsgCodes.SubscribeProperties:
                        SubscribeProperties(Message);
                        break;

                    case MsgCodes.RemoveProperty:
                        RemoveProperty(Message);
                        break;

                    case MsgCodes.RemoveProperties:
                        RemoveProperties(Message);
                        break;

                    case MsgCodes.PLCTagsChanged:
                        PLCTagsChanged(Message);
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
                    RemoveProperty(Message.SourceApplicationName, new Property() { ObjPath= sub.ObjPath});
                }
            }

            /* invio messaggio di risposta */
            RetValue = SendResponse(Message, RetValue);

            return RetValue;
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
                    RemoveProperty(Message.SourceApplicationName, new Property() { ObjPath= sub.ObjPath});
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
            RetValue = SendResponse(Message, RetValue);

            return RetValue;
        }


        private bool SubscribeProperty(string subscriber, Property prop)
        {
            bool RetValue = true;

            // verifica che la property esista e sia sottoscrivibile
            var property = mariniImpiantoTree.GetObjectByPath(prop.ObjPath) as MariniProperty;

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
                    Logger.WarnFormat("Error subscribing property {0} : {1}", prop.ObjPath, exc.Message);
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
            RetValue = SendResponse(Message,RetValue );

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

            foreach(var prop in props)
            {
                if (!SubscribeProperty(Message.SourceApplicationName, prop)) { RetValue = false; }
            }
            /* invio messaggio di risposta */
            RetValue = SendResponse(Message, RetValue);

            return RetValue;
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
            RetValue = SendResponse(Message, RetValue);

            return RetValue;
        }

        private bool RemoveProperties(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PropertiesData;
            var props = MsgData.Props;

            Logger.InfoFormat("{0}", Message.SourceApplicationName);

            foreach (var prop in props)
            {
                if (!RemoveProperty(Message.SourceApplicationName, prop)) { RetValue = false; }
            }
            /* invio messaggio di risposta */
            RetValue = SendResponse(Message, RetValue);

            return RetValue;
        }

        private bool PLCTagChanged(IIncomingMessage Message)
        {
            bool RetValue = true;

            // get msg application data
            var MsgData = GeneralHelper.DeserializeObject(Message.MessageData) as PLCTagData;
            var plctag = MsgData.Tag;

            Logger.InfoFormat("Ricevuto Messaggio {1}/{2}:{3} da {0}", Message.SourceApplicationName, plctag.PLCName, plctag.Address, plctag.Value);

            // trova il tag corrispondente (uno solo) all'indirizzo sottoscritto
            TagItem tag = ListTagItems.FirstOrDefault(item => item.PLCName == plctag.PLCName && item.Address == plctag.Address);

            if (tag != null)
            {
                // 
                try
                {
                    tag.Value = MsgData.Tag.Value;
                    Logger.InfoFormat("Cambiato Tag {0} : {1}/{2}:{3} -> {4}", tag.Name, tag.PLCName, tag.Address, tag.Type, tag.Value);
                }
                catch (Exception exc)
                {
                    Logger.WarnFormat("Errore in cambio Tag value {0} : {1}/{2}:{3} -> {4} {5}", tag.Name, tag.PLCName, tag.Address, tag.Type, tag.Value,exc.Message);
                }
                
                // recuperare la lista delle properties dell'impianto
                List<MariniProperty> props = mariniImpiantoTree.MariniImpianto.GetObjectListByType(typeof(MariniProperty)).Cast<MariniProperty>().ToList();
                // trova la property associata e cambia il valore
                MariniProperty property = props.FirstOrDefault(prp=> prp.bind == tag.Name);

                if (property != null)
                {
                    try
                    {
                        property.value = tag.Value;
                    }
                    catch (Exception exc)
                    {
                        Logger.WarnFormat("Errore in cambio valore property {0}:{1} {2}", property.name,tag.Value,exc.Message);
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
                    List<MariniProperty> props = mariniImpiantoTree.MariniImpianto.GetObjectListByType(typeof(MariniProperty)).Cast<MariniProperty>().ToList();
                    // trova la property associata e cambia il valore
                    MariniProperty property = props.FirstOrDefault(prp => prp.bind == tag.Name);

                    if (property != null)
                    {
                        try
                        {
                            property.value = tag.Value;
                        }
                        catch (Exception exc)
                        {
                            Logger.WarnFormat("Errore in cambio valore property {0}:{1} {2}", property.name, tag.Value, exc.Message);
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


        // metodo chiamato sul cambiamento del valore di una property non originata da PLCtag
        public void PropertyValueChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            MariniProperty mp = sender as MariniProperty;

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

            // trovo l'eventuale sottoscrizione 
            ObjectPropertyNotifyToSubscribers(mp);
        }

        // invia la notifica ai sottoscrittori 
        private void ObjectPropertyNotifyToSubscribers(MariniProperty mp)
        {
            foreach (var subscriber in ListSubscriptions.Keys)
            {
                Property property=null;
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
                    bool bOK = true;

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
                        bOK = false;
                    }
                }
            }
        }

        // invia la notifica ai sottoscrittori 
        private bool PropertyNotifyToSubscriber(string subscriber, Property prop)
        {
            bool bOK=true;

            var mp = MariniImpiantoTree.Instance.GetObjectByPath(prop.ObjPath) as MariniProperty;
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





        private bool SetPropertyPLCTag(MariniProperty mp)
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

        private bool SendResponse(IIncomingMessage Message,bool bOK)
        {

            //Create a DotNetMQ Message to respond
            var ResponseMessage = Message.CreateResponseMessage();
            var ResponseData = new ResponseData
            {
                Response = bOK
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

                bOK = false;
            }
            return bOK;
        }

        #endregion Private Methods
    }
}
