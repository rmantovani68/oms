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
    public class ExampleOfMariniEventHandler : IMariniEventHandler
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Handle(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Sono in ExampleOfMariniEventHandler.Handle(), il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();
        }

        //public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        //{

        //    Console.WriteLine("Sono in MariniImpiantoEventHandlers.PropertyChangedEventHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
        //    //methodToBeCalledWhenPropertyIsSet();

        //}

        //public void MyImpiantoHandler(object sender, PropertyChangedEventArgs e)
        //{

        //    Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyImpiantoHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
        //    //methodToBeCalledWhenPropertyIsSet();

        //}

        //public void PlctagPropertyHandler(object sender, PropertyChangedEventArgs e)
        //{
        //    MariniPlctag mp = (sender as MariniPlctag);
        //    string prop_name = e.PropertyName;
        //    string parent_property_bind = (sender as MariniPlctag).parent_property_bind;
        //    Type parent_type = mp.parent.GetType();
        //    PropertyInfo parent_prop = parent_type.GetProperty(parent_property_bind);

        //    Logger.DebugFormat("Sono in MariniImpiantoEventHandlers.PlctagPropertyHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", mp.path, prop_name);
        //    //Console.WriteLine("Sono in MariniImpiantoEventHandlers.PlctagPropertyHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", mp.path, prop_name);
        //    //Console.WriteLine("New Plctag {0}  --->  {1} = {2}", mp.path, prop_name, mp.GetType().GetProperty(prop_name).GetValue(mp, null));
        //    //Console.WriteLine("Old Parent {0}  --->  {1} = {2}", mp.parent.path, parent_property_bind, parent_prop.GetValue(mp.parent,null));

        //    //Console.WriteLine("Cambio la proprieta' del parent");

        //    parent_prop.SetValue(mp.parent, Convert.ChangeType(mp.GetType().GetProperty(prop_name).GetValue(mp, null), parent_prop.PropertyType), null);

        //    //Console.WriteLine("New Parent {0}  --->  {1} = {2}", mp.parent.path, parent_property_bind, parent_prop.GetValue(mp.parent, null));

        //}

        //public void PropertyBoundToPlctagHandler(object sender, PropertyChangedEventArgs e)
        //{
        //    MariniGenericObject mgo = (sender as MariniGenericObject);
        //    string prop_name = e.PropertyName;
        //    //string parent_property_bind = (sender as MariniPlctag).parent_property_bind;
        //    //Type parent_type = mp.parent.GetType();
        //    //PropertyInfo parent_prop = parent_type.GetProperty(parent_property_bind);

        //    Logger.DebugFormat("Sono in MariniImpiantoEventHandlers.PropertyBoundToPlctagHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", mgo.path, prop_name);
        //    //Console.WriteLine("Sono in MariniImpiantoEventHandlers.PropertyBoundToPlctagHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", mgo.path, prop_name);


        //}

        //public void MariniPropertyHandler(object sender, PropertyChangedEventArgs e)
        //{
        //    MariniProperty mp = sender as MariniProperty;
        //    string p_name = e.PropertyName;

        //    Logger.DebugFormat("MariniPropertyHandler --- sender: {0}, proprieta': {1} valore: {2}", mp.path, p_name, mp.value);
        //    //Console.WriteLine( "MariniPropertyHandler --- sender: {0}, proprieta': {1} valore: {2}", mp.path, p_name, mp.value);

        //    // Qui devo verificare il tipo di variabile e fare il lavoro richiesto sul plctag


        //}

    }
}
