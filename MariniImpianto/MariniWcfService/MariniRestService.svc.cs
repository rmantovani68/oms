using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MariniImpianti;
using System.Reflection;
using log4net;

namespace MariniWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MariniRestService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MariniRestService.svc or MariniRestService.svc.cs at the Solution Explorer and start debugging.
    public class MariniRestService : IMariniRestService
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public XMLDescription GetXMLSerializedObjectFromId(string id)
        {
            XMLDescription myXMLMGO = new XMLDescription();
            
            myXMLMGO.XMLData = MariniImpiantoTree.Instance.SerializeObject(id);
            Logger.InfoFormat("GetXMLSerializedObjectFromId di {0}",id);
            return myXMLMGO;
        }

        public string GetPropertyValue(string id, string prop)
        {
            Logger.InfoFormat("GetPropertyValue di {0}:{1} ", id, prop);
            return MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop).GetValue(MariniImpiantoTree.Instance.GetObjectById(id), null).ToString();
        }

        public void ChangePropertyValue(string id, string prop, string value)
        {
            Logger.InfoFormat("ChangePropertyValue di {0}:{1} con valore {2}", id, prop, value);
            PropertyInfo propertyInfo = MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop);
            propertyInfo.SetValue(MariniImpiantoTree.Instance.GetObjectById(id), Convert.ChangeType(value, propertyInfo.PropertyType), null);
        }


    }
}
