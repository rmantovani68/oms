#region Using
using System;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;

using OMS.Core.Communication;
using System.Collections.Specialized;
using System.ComponentModel;
#endregion

namespace MariniImpiantiHost
{
    public class Model
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // lista dei tags sottoscritti
        // da creare dopo la lettura del file impianto
        public ObservableUniqueCollection<TagItem> ListTagItems { get; set; }

        // lista dei plc connessi
        public ObservableUniqueCollection<PLCItem> ListPLCItems { get; set; }

        /* lista properties dell'impianto */
        public HashSet<PropertyItem> _Properties { get; set; }

        /* associazione sender / subscriptions */
        public Dictionary<string, HashSet<Subscription>> _Subs { get; set; }


        #region Constructor
        public Model()
        {


            ListTagItems = new ObservableUniqueCollection<TagItem>();

            // plc connessi (deve stare in plc server)
            ListPLCItems = new ObservableUniqueCollection<PLCItem>();

            // da gestire con le richieste di sottoscrizione dei sottoscrittori
            _Subs = new Dictionary<string, HashSet<Subscription>>();

            // da riempire con i dati del xml
            _Properties = new HashSet<PropertyItem>();


        }
        #endregion Constructor

    }


    
    public class ObservableUniqueCollection<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            var exists = false;

            foreach (var myItem in Items.Where(myItem => myItem.Equals(item)))
                exists = true;

            if (!exists)
                base.InsertItem(index, item);
        }
    }
}
