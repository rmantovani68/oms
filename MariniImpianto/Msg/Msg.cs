using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Msg
{
    public enum MsgCodes {
        SetPLCTag,  /* set plctag */
                    /* devono essere definiti tags di tipo read e di tipo write */
        GetPLCTag,  /* getplctag */
        SubscribePLCTags,
        
        ObjectMsg
    };

    [Serializable]
    public class Msg
    {
        public MsgCodes MsgCode {get; set;}
        public DateTime MsgDateTime {get; set;}
        public String   MsgSender {get; set;}
        public String   MsgDestination {get; set;}
        public String   MsgText {get; set;}
    }

    [Serializable]
    public class PLCTagMsg : Msg
    {
        public String PLCTagID {get; set;}
        public String PLCTagValue {get; set;} /* posso definire un oggetto di classe ... per gestire valori di vario tipo ?? */
    }
}
