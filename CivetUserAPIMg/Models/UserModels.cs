using PDSS.CivetPublic.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CivetUserAPIMg.Models
{
    [Serializable]
    public class ModUserInfo
    {
        public OAuth.UserInfo BaseInfo { get; set; }
    }
}