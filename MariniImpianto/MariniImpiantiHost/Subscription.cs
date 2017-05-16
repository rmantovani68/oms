using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariniImpiantiHost
{

    class Subscription : IComparable,IComparable<Subscription>
    {
        public string ObjID { get; private set; }
        public string ObjName { get; private set; }
        public string PropertyName { get; private set; }

        public Subscription(string objID, string objName, string propertyName)
        {
            ObjID = objID;
            ObjName = objName;
            PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            // throws exception if type is wrong
            var sub = obj as Subscription;

            if(sub==null) return false;

            return (ObjID == sub.ObjID && PropertyName == sub.PropertyName);
        }

        public override int GetHashCode()
        {
            return (ObjID + PropertyName).GetHashCode();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            // throws exception if type is wrong
            Subscription sub = (Subscription)obj;

            return CompareTo(sub);
        }

        public int CompareTo(Subscription sub)
        {
            var cmpPLCName = ObjID.CompareTo(sub.ObjID);
            if (cmpPLCName == 0)
                return PropertyName.CompareTo(sub.PropertyName);
            return cmpPLCName;            
        }

        #endregion

    }

}
