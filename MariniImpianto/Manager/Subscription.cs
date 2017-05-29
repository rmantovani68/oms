using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{

    public class Subscription : IComparable,IComparable<Subscription>
    {
        public string PropertyPath { get; private set; }
        public string PropertyID{ get; private set; }

        public Subscription(string propertyPath, string propertyID)
        {
            PropertyPath = propertyPath;
            PropertyID = propertyID;
        }

        public override bool Equals(object obj)
        {
            // throws exception if type is wrong
            var sub = obj as Subscription;

            if(sub==null) return false;

            return (PropertyPath == sub.PropertyPath);
        }

        public override int GetHashCode()
        {
            return PropertyPath.GetHashCode();
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
            return PropertyPath.CompareTo(sub.PropertyPath);
        }

        #endregion

    }

}
