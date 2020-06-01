using PDSS.CivetPublic.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CivetUserAPIMg.Common
{
    public class UtilCivetMsg
    {
        public static MsgSender Sender;

        public static string SendTextMsg(string Msg, string[] ToCivetNos)
        {

            return Sender.SendByCivetNo(new CivetTextMsg { Content = Msg }, ToCivetNos);
        }
    }
}