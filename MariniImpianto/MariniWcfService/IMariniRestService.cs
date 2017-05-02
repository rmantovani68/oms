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
    //// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMariniRestService" in both code and config file together.
    //[ServiceContract]
    //public interface IMariniRestService
    //{
    //    [OperationContract]
    //    [WebInvoke(Method = "GET",
    //        ResponseFormat = WebMessageFormat.Xml,
    //        BodyStyle = WebMessageBodyStyle.Wrapped,
    //        UriTemplate = "xml/{id}")]
    //    String GetXMLSerializedObjectFromId(String id);
    //}

    [ServiceContract]
    public interface IMariniRestService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            //BodyStyle= WebMessageBodyStyle.Bare,
            UriTemplate = "xml/{id}")]
        //string GetXMLSerializedObjectFromId(string id);
        XMLDescription GetXMLSerializedObjectFromId(string id);
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
