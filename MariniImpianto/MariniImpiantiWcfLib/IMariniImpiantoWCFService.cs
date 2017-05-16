using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.Text;

namespace MariniImpiantiWcfLib
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IMariniImpiantoWCFService
    {


        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "xml/{id}")]
        XMLDescription GetXMLSerializedObjectFromId(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertyvalue/{id}/{prop}")]
        string GetPropertyValue(string id, string prop);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertychange/{id}/{prop}/{value}")]
        void ChangePropertyValue(string id, string prop, string value);






    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "MIWcfServiceLibrary.ContractType".
    [DataContract]
    public class XMLDescription
    {
        private string _sXMLData;
        [DataMember]
        public string XMLData
        {
            get { return WebUtility.HtmlDecode(_sXMLData); }
            set { _sXMLData = value; }
        }


    }

}
