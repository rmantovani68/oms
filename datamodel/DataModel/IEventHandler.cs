using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per mettere a disposizione le strutture per PropertyChanged event.
using System.ComponentModel;
// Libreria per usare PropertyInfo all'interno degli handler handler
// using System.Reflection;

namespace DataModel
{
    public interface IEventHandler
    {
        /// <summary>
        /// Gestisce un evento PropertyChangedEventArgs associato a un sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Handle(object sender, PropertyChangedEventArgs e);
    }
}
