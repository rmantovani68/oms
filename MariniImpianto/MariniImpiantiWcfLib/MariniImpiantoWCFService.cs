using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MariniImpianti;
using System.Reflection;
using log4net;

namespace MariniImpiantiWcfLib
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class MariniImpiantoWCFService : IMariniImpiantoWCFService
    {
       



        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initialize the service.
        /// </summary>
        /// <remarks>
        /// It's launched by the class in the folder App_Code with the method AppInitialize().
        /// </remarks>
        public static void Initialize()
        {
            // This will get called on startup

            Logger.Info("***********************************");
            Logger.Info("       WCF SERVICE STARTED");
            Logger.Info("***********************************");

            MariniImpiantoTree.InitializeFromXmlFile(@"Q:\VARIE\ael\new-project\doc\analisi\impianto.xml");
            MariniImpiantoTree mariniImpiantoTree = MariniImpiantoTree.Instance;
        }

        /// <summary>
        /// Gets the xml description of an object
        /// </summary>
        /// <param name="id">The ID of the object</param>
        /// <returns>An xml description of the object with the given ID</returns>
        public XMLDescription GetXMLSerializedObjectFromId(string id)
        {
            XMLDescription myXMLMGO = new XMLDescription();

            myXMLMGO.XMLData = MariniImpiantoTree.Instance.SerializeObjectById(id);
            Logger.InfoFormat("GetXMLSerializedObjectFromId di {0}", id);
            return myXMLMGO;
        }

        /// <summary>
        /// Get the value of a property of an object
        /// </summary>
        /// <param name="id">ID of an object</param>
        /// <param name="prop">Name of the property of the object</param>
        /// <returns>A string with the value of the property</returns>
        public string GetPropertyValueFromId(string id, string prop)
        {
            Logger.InfoFormat("GetPropertyValue di {0}:{1} ", id, prop);
            return MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop).GetValue(MariniImpiantoTree.Instance.GetObjectById(id), null).ToString();
        }

        /// <summary>
        /// Change the value of a property of an object
        /// </summary>
        /// <param name="id">ID of an object</param>
        /// <param name="prop">Name of the property of the object</param>
        /// <param name="value">Value of the property</param>
        public void ChangePropertyValueFromId(string id, string prop, string value)
        {
            Logger.InfoFormat("ChangePropertyValue di {0}:{1} con valore {2}", id, prop, value);
            PropertyInfo propertyInfo = MariniImpiantoTree.Instance.GetObjectById(id).GetType().GetProperty(prop);
            propertyInfo.SetValue(MariniImpiantoTree.Instance.GetObjectById(id), Convert.ChangeType(value, propertyInfo.PropertyType), null);
        }


        /// <summary>
        /// Gets the xml description of an object
        /// </summary>
        /// <param name="path">The path of the object</param>
        /// <returns>An xml description of the object with the given path</returns>
        public XMLDescription GetXMLSerializedObjectFromPath(string path)
        {
            XMLDescription myXMLMGO = new XMLDescription();

            myXMLMGO.XMLData = MariniImpiantoTree.Instance.SerializeObjectByPath(path);
            Logger.InfoFormat("GetXMLSerializedObjectFromPath di {0}", path);
            return myXMLMGO;
        }

        /// <summary>
        /// Get the value of a property of an object
        /// </summary>
        /// <param name="path">path of an object</param>
        /// <param name="prop">Name of the property of the object</param>
        /// <returns>A string with the value of the property</returns>
        public string GetPropertyValueFromPath(string path, string prop)
        {
            Logger.InfoFormat("GetPropertyValue di {0}:{1} ", path, prop);
            return MariniImpiantoTree.Instance.GetObjectByPath(path).GetType().GetProperty(prop).GetValue(MariniImpiantoTree.Instance.GetObjectByPath(path), null).ToString();
        }

        /// <summary>
        /// Change the value of a property of an object
        /// </summary>
        /// <param name="path">path of an object</param>
        /// <param name="prop">Name of the property of the object</param>
        /// <param name="value">Value of the property</param>
        public void ChangePropertyValueFromPath(string path, string prop, string value)
        {
            Logger.InfoFormat("ChangePropertyValue di {0}:{1} con valore {2}", path, prop, value);
            PropertyInfo propertyInfo = MariniImpiantoTree.Instance.GetObjectByPath(path).GetType().GetProperty(prop);
            propertyInfo.SetValue(MariniImpiantoTree.Instance.GetObjectByPath(path), Convert.ChangeType(value, propertyInfo.PropertyType), null);
        }








    }
}
