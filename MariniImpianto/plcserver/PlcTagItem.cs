using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxSoftingAxC;
using SoftingAxC;

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

namespace plcserver
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
}
