using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
// Libreria Marini!!!
using DataModel;
// Libreria per mettere a disposizione le strutture per PropertyChanged event.
using System.ComponentModel;
// Libreria per il Log. Magari provare a toglierla dall'handler e spostare tutto su main con throw try catch???
using log4net;
// Libreria per usare l'oggetto MethodBase in Log4Net
using System.Reflection;

namespace MIConsoleTester.EventsHandlers
{
    class Motore1AlarmHandler : IEventHandler
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Handle(object sender, PropertyChangedEventArgs e)
        {
            PropertyObject mp = sender as PropertyObject;
            string p_name = e.PropertyName;
            Console.WriteLine("Motore1AlarmHandler->Handler --- sender: {0} proprieta: {1} valore: {2}", mp.path, p_name, mp.value);
            //methodToBeCalledWhenPropertyIsSet();
        }
    }
}
