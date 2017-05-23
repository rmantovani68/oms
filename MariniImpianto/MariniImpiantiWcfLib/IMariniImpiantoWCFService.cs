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
            UriTemplate = "xmlfromid/{id}")]
        XMLDescription GetXMLSerializedObjectFromId(string id);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertyvaluefromid/{id}/{prop}")]
        string GetPropertyValueFromId(string id, string prop);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertychangefromid/{id}/{prop}/{value}")]
        void ChangePropertyValueFromId(string id, string prop, string value);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "xmlfrompath/{path}")]
        XMLDescription GetXMLSerializedObjectFromPath(string path);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertyvaluefrompath/{path}/{prop}")]
        string GetPropertyValueFromPath(string path, string prop);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "propertychangefrompath/{path}/{prop}/{value}")]
        void ChangePropertyValueFromPath(string path, string prop, string value);





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
