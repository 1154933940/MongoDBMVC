using PDSS.CivetPublic.User;
using PDSS.MVC.Utility;
using CivetUserAPIMg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CivetHR.UserAPI;
using PDSS.MongoDB.Utility;

namespace CivetUserAPIMg.Controllers
{
    public class BaseController : PDSS.MVC.Utility.WebBase.Controller
    {

	//定義數據庫DB
        //protected ReleaseSystemEntities db = new ReleaseSystemEntities();

        protected MongoHelper mongoh = new MongoHelper("CivetHrMongoDb", "");

        const string sessionKey = "CivetUserAPIMg_UserInfo";

        /// <summary>
        /// 设定或获取用户信息
        /// </summary>
        /// <value>用户信息</value>
        protected ModUserInfo UserInfo
        {
            get
            {
                if (Session[sessionKey ] != null)
                {
                    return (ModUserInfo)Session[sessionKey ];
                }
                return new ModUserInfo { BaseInfo = new OAuth.UserInfo() };
            }
            set
            {
                Session[sessionKey ] = value;
            }
        }

        /// <summary>
        /// 获得是否为local服务器
        /// </summary>
        /// <value>是否为local服务器</value>
        protected static bool IsLocal(HttpRequestBase Request)
        {
            return Request.ServerVariables["SERVER_NAME"].ToLower() == "localhost";
        }


        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            ViewBag.MyCivetNo = CUtil.CStr(UserInfo.BaseInfo.civetno).ToLower();　//当前用户的香信号
            string controller = this.RouteData.Values["controller"].ToString().ToLower();
            string action = this.RouteData.Values["action"].ToString().ToLower();

            if (controller != "Home".ToLower())  //需要验证的页面
            {
                if (ViewBag.MyCivetNo == "")
                {
                    OAuth oauth = new OAuth(CivetHelper.AppID);

                    string id = CUtil.CStr(this.RouteData.Values["ID"]);

                    string QueryString = HtmlUtil.DealQuery(Request.QueryString.ToString(), "code");
                    string redirect_url = "http://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/')
                        + "/Login-" + controller + "/" + action + (id != "" ? "/" + id : "")
                        + (QueryString != "" ? "?" + QueryString : "");

                    string msg = "";
                    if (ViewBag.MyCivetNo != "")
                    {
                        msg = "无权操作";
                    }
                    filterContext.Result = Content(Util.JsUrlTo(oauth.GenerateOAuthUrl(redirect_url), msg));
                    return;
                }
            }

            base.OnAuthorization(filterContext);
        }
    }
}