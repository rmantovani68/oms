using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;

namespace MariniWcfService
{
       
    [ServiceContract]
    public interface IMariniRestService
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
