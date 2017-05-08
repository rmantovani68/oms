using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MariniImpianti
{
    public class MariniImpiantoEventHandlers
    {

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            
            Console.WriteLine("Sono in MariniImpiantoEventHandlers.PropertyChangedEventHandler, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            //methodToBeCalledWhenPropertyIsSet();
            
        }
    }
}
