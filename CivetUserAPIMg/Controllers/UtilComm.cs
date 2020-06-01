using CivetHR.UserAPI;
using CivetUserAPIMg.Models;
using PDSS.CivetPublic.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CivetUserAPIMg.Controllers
{
    public class UtilComm
    {
        const string sessionKey = "CivetUserAPIMg_UserInfo";
        /// <summary>
        /// 香信登录信息
        /// </summary>
        public static  ModUserInfo UserInfo
        {
            get
            {
                if (HttpContext.Current.Session[sessionKey] != null)
                {
                    return (ModUserInfo)HttpContext.Current.Session[sessionKey];
                }
                return new ModUserInfo { BaseInfo = new OAuth.UserInfo() };
            }
            set
            {
                HttpContext.Current.Session[sessionKey] = value;
            }
        }
        const string Keys_ = "CivetUserAPIMg_Manage";
        /// <summary>
        /// 管理员信息
        /// </summary>  
        public static ModUApiManager UApi_Manager
        {
            get
            {
                if (HttpContext.Current.Session[Keys_] != null)
                {
                    return (ModUApiManager)HttpContext.Current.Session[Keys_];
                }
                else
                {
                    return new ModUApiManager
                    {
                        _id = "",
                        is_disabled = true,
                        last_login_time = DateTime.Now,
                        user_name = "" 
                    };
                }
            }
            set
            {
                HttpContext.Current.Session[Keys_] = value;
            }
        }
    }
}