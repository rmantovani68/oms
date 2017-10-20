#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Manager
{
    public class TagItem : INotifyPropertyChanged, IEquatable<TagItem>
    {

        #region properties
        private string name;
        private string address;
        private string plcName;
        private string type;
        private object value;

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
        public string Address
        {
            get { return this.address; }
            set
            {
                if (this.address != value)
                {
                    this.address = value; this.NotifyPropertyChanged("Address");
                }
            }
        }

        public string PLCName
        {
            get { return this.plcName; }
            set
            {
                if (this.plcName != value)
                {
                    this.plcName = value; this.NotifyPropertyChanged("PLCName");
                }
            }
        }

        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value; this.NotifyPropertyChanged("Type");
                }
            }
        }

        public object Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value; this.NotifyPropertyChanged("Value");
                }
            }
        }
        #endregion

        #region constructor
        public TagItem(string name, string address, string plcName, string type) 
        {
            Name = name;
            Address = address;
            PLCName = plcName;
            Type = type;
        }
        #endregion

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region private methods
        private void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region public methods
        public override string ToString()
        {
            return string.Format("{0}", name);
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as TagItem);
        }

        public bool Equals(TagItem tag)
        {
            // If parameter is null return false:
            if (tag == null)
            {
                return false;
            }

            // Return true if either fields match:
            return (Name == tag.Name);
        }

        public override int GetHashCode()
        {
            return (this.Name).GetHashCode();
        }
        #endregion

        #region operators
        public static bool operator ==(TagItem tag1, TagItem tag2)
        {
            if (((object)tag1) == ((object)tag2)) return true;
            if (((object)tag1) == null || ((object)tag2) == null) return false;

            return tag1.Equals(tag2);
        }

        public static bool operator !=(TagItem tag1, TagItem tag2)
        {
            return !(tag1 == tag2);
        }
        #endregion
    }
}
