using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using AxSoftingAxC;
//using SoftingAxC;

public enum TagQuality
{
    BAD = 0,
    BAD_ConfigurationErrorInServer = 4,
    BAD_NotConnected = 8,
    BAD_DeviceFailure = 12,
    BAD_SensorFailure = 16,
    BAD_LastKnowValuePassed = 20,
    BAD_CommFailure = 24,
    BAD_ItemSetInactive = 28,
    UNCERTAIN = 64,
    UNCERTAIN_LastUsableValue = 68,
    UNCERTAIN_SensorNotAccurate = 80,
    UNCERTAIN_EngineeringUnitExeeded = 84,
    UNCERTAIN_ValueFromMultipleSource = 88,
    GOOD = 192,
    GOOD_LocalOverride = 216
}

namespace App.PlcDataExchange
{
    public class PlcTagItem
    {
        private PlcTagItem()
        {
        }

        public PlcTagItem(AxSoftingAxC.AxOPCDataControl dataControl, SoftingAxC.OPCDataItem item, string plcName, string tagName)
        {
            _dataControl = dataControl;
            _item = item;
            _plcName = plcName;
            TagName = tagName;
        }

        private AxSoftingAxC.AxOPCDataControl _dataControl = null;

        private SoftingAxC.OPCDataItem _item = null;

        private string _plcName = null;
        private string _tagName = null;

        public void ChangedValue(string newValue)
        {
            if (m_onChange != null)
                m_onChange(this, new ChangeEventArgs(_plcName, this.Value));
        }

        public object Value
        {
            get { return (_item.Value); }
            set
            {
                _item.Value = value;
                _dataControl.SOUpdate();
            }
        }

        public TagQuality Quality
        {
            get { return ((TagQuality)_item.Quality); }
        }

        public string PlcName
        {
            get { return _plcName; }
        }

        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        #region Events

        public delegate void OnChangeEventHandler(object sender, ChangeEventArgs e);
        private event OnChangeEventHandler m_onChange;
        public event OnChangeEventHandler OnChange
        {
            add
            {
                m_onChange += value;
            }
            remove { m_onChange -= value; }
        }

        public class ChangeEventArgs : EventArgs
        {
            public ChangeEventArgs(string plcName, object newValue)
            {
                _plcName = plcName;
                _value = newValue;
            }

            private string _plcName = null;
            public string PlcName
            {
                get { return (_plcName); }
            }

            private object _value = null;
            public object Value
            {
                get { return (_value); }
            }
        }
        #endregion
    }

    public class PlcDataExchange
    {
        #region Fields

        //Gestione lettura tags da file excel
        private Hashtable _hashTagsAddressByName = new Hashtable();
        private string xlsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Cybertronic 500\\OPCTags.xls";

        // Connessione OPCServer Softing; server locale
        private const String OPC_REMOTE_HOST = "127.0.0.1";
        private const Int16 OPC_UPDATE_RATE = 0;
        private const String OPC_SERVER = "Softing.OPC.S7.DA";

        private String _plcName = "...undefined...";

        private List<PlcTagItem> _tagItems = new List<PlcTagItem>();
        private Hashtable _hashTagsItemsByName = new Hashtable();
        private Hashtable _hashTagsItemsByAddress = new Hashtable();
        private AxSoftingAxC.AxOPCDataControl axOPCDataControl2;                //Channel Opc to Main used For Diagnostic

        #endregion

        #region Properties

        public string PlcName
        {
            get { return (_plcName); }
        }

        public bool IsConnected
        {
            get { return (axOPCDataControl2.IsConnected); }
        }

        #endregion

        #region Constructors

        private PlcDataExchange()
        {
        }

        public PlcDataExchange(string plcName, string xlsFileName, bool connect)
        {
            _plcName = plcName;
             
            axOPCDataControl2 = new AxOPCDataControl();
            axOPCDataControl2.CreateControl();
            axOPCDataControl2.OnDataChanged += new _IOPCDataControlEvents_OnDataChangedEventHandler(axOPCDataControl2_OnDataChanged);

            //Inizializza la connessione con l'opc server
            InitializeConnection();

            //xlsFilePath = @"C:\Program Files (x86)\Cybertronic 500\" + xlsFileName;
            xlsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Cybertronic 500\\" + xlsFileName;

            //riempio l'hashtable contenente i nomi dei tags con i relativi indirizzi letti dal file CybertronicTags.xls
            ReadTagFromXLS(_plcName);

            if (connect)
                CreateTags();
        }

        #endregion

        #region Methods

        //20161118
        public void SendEventTagsCreated()
        {
            if (_hashTagsAddressByName.Count > 0)
            {
                if (m_onTagCreation != null)
                    m_onTagCreation(this, new TagCreationEventArgs());   //event tag creation
            }
        }
        //210161118

        /// <summary>
        /// Metodo con cui inizializzo la connessione con l'opc server
        /// </summary>
        private void InitializeConnection()
        {
            axOPCDataControl2.RemoteHost = OPC_REMOTE_HOST;
            axOPCDataControl2.UpdateRate = OPC_UPDATE_RATE;
            axOPCDataControl2.ServerName = OPC_SERVER;
        }

        //Create a Tag based on Name and Address and update Dictionaries
        public PlcTagItem AddTag(string tagName, string tagAddress)
        {
            try
            {
                axOPCDataControl2.Items.AddItem(tagAddress);   //creo il tag sull'indirizzo
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, String.Format("PlcDataExchange.AddTag - {0} - Address: {1}", exc.Message, tagAddress));
                return (null);
            }
            SoftingAxC.OPCDataItem item = axOPCDataControl2.Items[axOPCDataControl2.Items.Count - 1];
            PlcTagItem newItem = new PlcTagItem(axOPCDataControl2, item, _plcName, tagName);    //creo un oggetto TagItem passando il DataItem ed il Nome

            try
            {
                _tagItems.Add(newItem);
                _hashTagsItemsByName.Add(tagName, newItem);         //create Dictionary (Name-TagItem)
                _hashTagsItemsByAddress.Add(tagAddress, newItem);   //create Dictionary (Address-TagItem)
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, String.Format("PlcDataExchange.AddTag - {0} - Address: {1}", exc.Message, tagAddress));
            }

            return (newItem);
        }

        /// <summary>
        /// Remove all the tags
        /// </summary>
        public void RemoveAll()
        {
            axOPCDataControl2.Items.RemoveAll();
            _hashTagsItemsByName.Clear();
            _hashTagsItemsByAddress.Clear();
        }

        //Remove the Configurated Items
        public void RemoveConfiguratedItems()
        {
            try
            {
                RemoveAll();
                CreateCommandTags(false);
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, String.Format("PlcDataExchange.RemoveConfiguratedItems - {0} - Address: {1}", exc.Message, ""));
            }
        }

        /// <summary>
        /// Obtain an item tag by tag address
        /// </summary>
        /// <param name="tagAddress"></param>
        /// <returns></returns>
        public PlcTagItem GetTagItemByAddress(string tagAddress)
        {
            return (_hashTagsItemsByAddress[tagAddress] as PlcTagItem);
        }

        /// <summary>
        /// Obtain the tag item list
        /// </summary>
        public List<PlcTagItem> TagItems
        {
            get { return (_tagItems); }
        }

        //Get Address from Tag name
       public string GetAddressFromName(PlcTagItem tag)
        {
           string ret = "";
           try
           {
                ret=_hashTagsItemsByName[tag.TagName].ToString();
           }
           catch (Exception exc) 
           { 
               //Logger.Instance.Log(Logger.Level.EXCEPTION, "PlcConfig.GetAddressFromName" + exc.Message); 
           }
           return ret;
        }
        /// <summary>
        /// Obtain an item tag by the name
        /// Set tag tagName to value tagvalue
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public PlcTagItem this[string tagName]
        {
            get { return (_hashTagsItemsByName[tagName] as PlcTagItem); }
            set
            {
                try
                {
                    (_hashTagsItemsByName[tagName] as PlcTagItem).Value = value;
                }
                catch (Exception exc)
                {
                    //Logger.Instance.Log(Logger.Level.EXCEPTION, "PlcDataExchange.SetTags - " + exc.Message);
                }
            }
        }

        /// <summary>
        /// Create Diagnostic Tags
        /// </summary>
        private void CreateTags()
        {
            foreach (DictionaryEntry tag in _hashTagsAddressByName)
            {
                AddTag(tag.Key.ToString(), tag.Value.ToString());
            }

            //Dopo la creazione dei tags eseguo la connect()
            Connect();
        }

        //Create Diagnostic Command Tags
        public void CreateCommandTags(Boolean connect)
        {
            try
            {
                if (axOPCDataControl2.IsConnected)
                {
                    Disconnect();
                }
                foreach (DictionaryEntry tag in _hashTagsAddressByName)
                {
                    if (!(tag.Key.ToString().StartsWith("PLCTAG_EB") || tag.Key.ToString().StartsWith("PLCTAG_AB") || tag.Key.ToString().StartsWith("PLCTAG_PEW") || tag.Key.ToString().StartsWith("PLCTAG_PAW")))
                    {
                        AddTag(tag.Key.ToString(), tag.Value.ToString());
                    }
                }

                if (connect)
                    Connect();
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, "PlcConfig.CreateCommandTags" + exc.Message);
            }
        }

        //Create Diagnostic IO Tags
        public void CreateIOTags(Boolean connect)
        {
            try
            {
                if (axOPCDataControl2.IsConnected)
                {
                    Disconnect();
                }
                foreach (DictionaryEntry tag in _hashTagsAddressByName)
                {
                    if ((tag.Key.ToString().StartsWith("PLCTAG_EB") || tag.Key.ToString().StartsWith("PLCTAG_AB") || tag.Key.ToString().StartsWith("PLCTAG_PEW") || tag.Key.ToString().StartsWith("PLCTAG_PAW")))
                    {
                        AddTag(tag.Key.ToString(), tag.Value.ToString());
                    }
                }

                if (connect)
                    Connect();
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, "PlcConfig.CreateIOTags" + exc.Message);
            }
        }

        //Disconnect
        public void Disconnect()
        {
            axOPCDataControl2.Disconnect();
        }

        //Connect
        public void Connect()
        {
            axOPCDataControl2.Connect();
        }

        //Refresh
        public void Refresh()
        {
            axOPCDataControl2.Refresh(SoftingAxC.enumUpdateMode.ccSynchronous);
        }

        /// <summary>
        /// Legge gli indirizzi dei tag dal file XLS e crea un hashtable in cui la chiave è il nome del tag (PLCTAG_...)
        /// </summary>
        /// <param name="PlcName"></param>
        public void ReadTagFromXLS(String PlcName)
        {
            try
            {
                string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", xlsFilePath);
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [" + PlcName + "$]", connectionString))
                {
                    DataTable dataTable = new DataTable("CybertronicTags");
                    dataAdapter.Fill(dataTable);

                    var CybertronicTags = from row in dataTable.AsEnumerable()
                                          select new
                                          {
                                              TagName = row.Field<String>(0),
                                              TagAddress = row.Field<String>(1),
                                          };

                    foreach (var tag in CybertronicTags)
                    {
                        _hashTagsAddressByName.Add(tag.TagName, tag.TagAddress);   //Dictionary Name-Address
                    }
                }
            }
            catch (Exception exc)
            {
                //Logger.Instance.Log(Logger.Level.EXCEPTION, String.Format("PlcDataExchange.ReadTagFromXLS - {0}", exc.Message));
               
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Listen Changed Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axOPCDataControl2_OnDataChanged(object sender, _IOPCDataControlEvents_OnDataChangedEvent e)
        {
            foreach (SoftingAxC.OPCDataItem item in e.changedItems)
            {
                PlcTagItem pti = (_hashTagsItemsByAddress[item.ItemID] as PlcTagItem);
                if (pti != null)
                {
                    pti.TagName = GetTagItemByAddress(item.ItemID).TagName;
                    if (item.Value!=null)
                    {
                        pti.ChangedValue((item.Value as object).ToString());
                        if (m_onDataChanged!= null)
                            m_onDataChanged(this, new DataChangedEventArgs(_plcName, pti));   //event data changed
                    }
                }
            }
        }

        //Launch Event Data Changed
        public delegate void OnDataChangedEventHandler(object sender, DataChangedEventArgs e);
        private event OnDataChangedEventHandler m_onDataChanged = null;

        public event OnDataChangedEventHandler OnDataChanged
        {
            add { m_onDataChanged += value; }
            remove { m_onDataChanged -= value; }
        }

        public class DataChangedEventArgs : EventArgs
        {
            public DataChangedEventArgs(string plcName, PlcTagItem tag)
            {
                _plcName = plcName;
                _tagChanged = tag;
            }

            private string _plcName;
            public string PlcName
            {
                get { return (_plcName); }
            }

            private PlcTagItem _tagChanged;
            public PlcTagItem TagChanged
            {
                get { return _tagChanged; }
            }
        }
        //Launch Event Connected
        public delegate void OnConnectEventHandler(object sender, ConnectEventArgs e);
        private event OnConnectEventHandler m_onConnect = null;

        public event OnConnectEventHandler OnConnect
        {
            add { m_onConnect += value; }
            remove { m_onConnect -= value; }
        }

        public class ConnectEventArgs : EventArgs
        {
            public ConnectEventArgs(string plcName, bool isConnected)
            {
                _plcName = plcName;
                _isConnected = isConnected;
            }

            private string _plcName;
            public string PlcName
            {
                get { return (_plcName); }
            }

            private bool _isConnected;
            public bool IsConnected
            {
                get { return _isConnected; }
            }
        }

        //20161118
        //Launch Tag Creation
        public delegate void OnTagCreationEventHandler(object sender, TagCreationEventArgs e);
        private event OnTagCreationEventHandler m_onTagCreation = null;

        public event OnTagCreationEventHandler OnTagCreation
        {
            add { m_onTagCreation += value; }
            remove { m_onTagCreation -= value; }
        }

        public class TagCreationEventArgs : EventArgs
        {
            public TagCreationEventArgs()
            {
            }
        }

        //20161118
        //Launch Event Quality Change
        //public delegate void OnQualityEventHandler(object sender, QualityEventArgs e);
        //private event OnQualityEventHandler m_onQuality = null;

        //public event OnQualityEventHandler OnQuality
        //{
        //    add { m_onQuality += value; }
        //    remove { m_onQuality -= value; }
        //}

        //public class QualityEventArgs : EventArgs
        //{
        //    public QualityEventArgs()
        //    {
        //    }

        //    private object _Quality;
        //    public object GetQuality
        //    {
        //        get { (_hashTagsItemsByName[""] as PlcTagItem).Quality) ; }
        //    }
        //}

        #endregion
    }
}
