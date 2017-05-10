using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using log4net;
using System.Reflection;

namespace MariniImpianti
{
    //public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    public class MariniImpiantoEventHandlers
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            
            Console.WriteLine("Sono in MariniImpiantoEventHandlers.PropertyChangedEventHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();
            
        }

        public void MyDefaultHandler(object sender, PropertyChangedEventArgs e)
        {

            Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyDefaultHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();

        }

        public void MyImpiantoHandler(object sender, PropertyChangedEventArgs e)
        {

            Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyImpiantoHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();

        }

        public void MyMotoreHandler(object sender, PropertyChangedEventArgs e)
        {
            Logger.DebugFormat("Sono in MariniImpiantoEventHandlers.MyMotoreHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyMotoreHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();

        }

        public void MyMotoreHandlerCustom(object sender, PropertyChangedEventArgs e)
        {
            Logger.DebugFormat("Sono in MariniImpiantoEventHandlers.MyMotoreHandlerCustom, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            Console.WriteLine("YEAH!!!!");
            Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyMotoreHandlerCustom, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            Console.WriteLine("YEYEYEYE!!!! E' custom!");
        }



        public void MyNastroHandler(object sender, PropertyChangedEventArgs e)
        {

            Console.WriteLine("Sono in MariniImpiantoEventHandlers.MyNastroHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();

        }
    }
}
