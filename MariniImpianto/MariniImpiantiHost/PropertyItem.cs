#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace MariniImpiantiHost
{
    public class PropertyItem : INotifyPropertyChanged, IEquatable<PropertyItem>
    {

        private string objID;
        public string ObjID
        {
            get { return this.objID; }
            set
            {
                if (this.objID!= value)
                {
                    this.objID= value;
                    this.NotifyPropertyChanged("ObjID");
                }
            }
        }

        private string objName;
        public string ObjName
        {
            get { return this.objName; }
            set
            {
                if (this.objName != value)
                {
                    this.objName = value;
                    this.NotifyPropertyChanged("ObjName");
                }
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        private string type;
        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        private string tagName;
        public string TagName
        {
            get { return this.tagName; }
            set
            {
                if (this.tagName != value)
                {
                    this.tagName = value;
                    this.NotifyPropertyChanged("TagName");
                }
            }
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.NotifyPropertyChanged("Value");
                }
            }
        }

        public PropertyItem() { }

        void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", objName, name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            return Equals(obj as PropertyItem);
        }

        public bool Equals(PropertyItem prop)
        {
            // If parameter is null return false:
            if (prop == null)
            {
                return false;
            }

            // Return true if either fields match:
            return ((ObjID == prop.ObjID && Name == prop.Name));
        }
        public override int GetHashCode()
        {
            return (this.ObjID+this.Name).GetHashCode();
        }

        public static bool operator == (PropertyItem prop1, PropertyItem prop2)
        {
            if (((object)prop1) == ((object)prop2)) return true;
            if (((object)prop1) == null || ((object)prop2) == null) return false;

            return prop1.Equals(prop2);
        }

        public static bool operator !=(PropertyItem prop1, PropertyItem prop2)
        {
            return !(prop1 == prop2);
        }

    }
}
