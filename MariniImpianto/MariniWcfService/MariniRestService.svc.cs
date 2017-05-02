using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MariniImpianti;

namespace MariniWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MariniRestService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MariniRestService.svc or MariniRestService.svc.cs at the Solution Explorer and start debugging.
    public class MariniRestService : IMariniRestService
    {
        //public String GetXMLSerializedObjectFromId(String id)
        //{
        //    return "Requested XML of id " + id;


        //}

        public XMLDescription GetXMLSerializedObjectFromId(string id)
        {
            XMLDescription myXMLMGO = new XMLDescription();
            ////myMGO.sXmlSerializedMGO="prova";
            //MariniImpiantoTree mariniImpiantoTree = MariniImpiantoTree.Instance;
            //MariniGenericObject mgo=null;
            ////impiantoMarini.GetObjectById(id,ref mgo);
            //mgo = mariniImpiantoTree.MariniImpianto.GetObjectById(id);
            //if (mgo==null)
            //{
            //    Console.WriteLine("\nNon ho trovato nulla con id {0}", id);
            //    myXMLMGO.XMLData = "NN";
            //} 
            //else
            //{
            //    myXMLMGO.XMLData=mariniImpiantoTree.SerializeObject(id);
            //}
            myXMLMGO.XMLData = MariniImpiantoTree.Instance.SerializeObject(id);
            return myXMLMGO;
        }

    }
}
