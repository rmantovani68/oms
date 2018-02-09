#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using log4net;
using MDS.Client;
using OMS.Core.Communication;
using MDS.Communication.Messages;
#endregion

namespace Manager
{
    public class PLCItem : INotifyPropertyChanged
    {
        #region private members
        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MDSClient MDSClientInstance;
        private string name;
        private string ipAddress;
        private PLCConnectionStatus connectionStatus;
        private short rack;
        private short slot;
        private int delay;

        #endregion private members

        #region Properties
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value; this.NotifyPropertyChanged("Name");
                }
            }
        }

        public string IPAddress
        {
            get { return this.ipAddress; }
            set
            {
                if (this.ipAddress != value)
                {
                    this.ipAddress = value; this.NotifyPropertyChanged("IPAddress");
                }
            }
        }

        public PLCConnectionStatus ConnectionStatus
        {
            get { return this.connectionStatus; }
            set
            {
                if (this.connectionStatus != value)
                {
                    this.connectionStatus = value; this.NotifyPropertyChanged("ConnectionStatus");
                }
            }
        }


        public short Rack
        {
            get { return this.rack; }
            set
            {
                if (this.rack != value)
                {
                    this.rack = value; this.NotifyPropertyChanged("Rack");
                }
            }
        }

        public short Slot
        {
            get { return this.slot; }
            set
            {
                if (this.slot != value)
                {
                    this.slot = value; this.NotifyPropertyChanged("Slot");
                }
            }
        }

        public int Delay
        {
            get { return this.delay; }
            set
            {
                if (this.delay != value)
                {
                    this.delay = value;
                    this.NotifyPropertyChanged("Delay");
                }
            }
        }
        #endregion Properties

        #region Constructor
        public PLCItem(string plcName, string ipAddress, MDSClient mdsClient)
            : this(plcName, ipAddress, 0, 2, 100, mdsClient)
        {
        }

        public PLCItem(string plcName, string ipAddress, short rack, short slot, int delay, MDSClient mdsClient)
        {
            Name = plcName;
            IPAddress = ipAddress;
            Rack = rack;
            Slot = slot;
            Delay = delay;
            ConnectionStatus = PLCConnectionStatus.NotConnected;
            MDSClientInstance = mdsClient;
        }
        #endregion Constructor

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Private Methods
        private void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion Private Methods

        #region Public Methods
        public bool Connection(string sender, string destination)
        {
            bool RetValue = true;
            //Create a DotNetMQ Message to send 
            var message = MDSClientInstance.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = destination;

            //Create a message
            var MsgData = new PLCConnectionData
            {
                MsgCode = MsgCodes.ConnectPLC,
                PLCName = this.Name,
                IpAddress = this.IPAddress,
                Rack = this.Rack,
                Slot = this.Slot,
                Delay = this.Delay
            };

            //Set message data
            message.MessageData = MDS.GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch (Exception exc)
            {
                // non sono riuscito a inviare il messaggio
                Logger.WarnFormat("Messaggio non inviato : {0}", exc.Message);
            }
            return RetValue;
        }

        public bool Disconnection(string sender, string destination)
        {
            bool RetVal = true;

            //Create a DotNetMQ Message to send 
            var message = MDSClientInstance.CreateMessage();

            //Set destination application name
            message.DestinationApplicationName = destination;

            //Create a message
            var MsgData = new PLCConnectionData
            {
                MsgCode = MsgCodes.DisconnectPLC,
                PLCName = this.Name,
            };

            //Set message data
            message.MessageData = MDS.GeneralHelper.SerializeObject(MsgData);

            // message.MessageData = Encoding.UTF8.GetBytes(messageText);
            message.TransmitRule = MessageTransmitRules.NonPersistent;

            try
            {
                //Send message
                message.Send();
            }
            catch (Exception exc)
            {
                // non sono riuscito a inviare il messaggio
                Logger.WarnFormat("Disconnection() : Messaggio non inviato : {0}", exc.Message);
            }
            return RetVal;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Name, IPAddress);
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as PLCItem);
        }

        public bool Equals(PLCItem plc)
        {
            // If parameter is null return false:
            if (plc == null)
            {
                return false;
            }

            // Return true if either fields match:
            return ((Name == plc.Name && IPAddress == plc.IPAddress));
        }

        public override int GetHashCode()
        {
            return (this.Name + this.IPAddress).GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator ==(PLCItem plc1, PLCItem plc2)
        {
            if (((object)plc1) == ((object)plc2)) return true;
            if (((object)plc1) == null || ((object)plc2) == null) return false;

            return plc1.Equals(plc2);
        }

        public static bool operator !=(PLCItem plc1, PLCItem plc2)
        {
            return !(plc1 == plc2);
        }
        #endregion
    }
}
