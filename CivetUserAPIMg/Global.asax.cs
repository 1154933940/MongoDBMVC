using PDSS.CivetPublic.User;
using PDSS.MVC.Utility.WebBase;
using CivetUserAPIMg.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CivetUserAPIMg
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        //是否服務器第一次請求
        static bool FristRequest = true;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleTable.EnableOptimizations = (ConfigurationManager.AppSettings["BundleTableZip"] == "On"); //区分测试环境与真实环境
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            CivetHelper.APIUrl = ConfigurationManager.AppSettings["APIUrl"];
            CivetHelper.AppID = ConfigurationManager.AppSettings["AppID"];
            CivetHelper.Password = ConfigurationManager.AppSettings["Password"];
            CivetHelper.UID = ConfigurationManager.AppSettings["UID"];
            LogManager.CallCivetInterfaceLog = ConfigurationManager.AppSettings["CallCivetInterfaceLog"];
            //如果不存在CivetLog目录则创建
            string LogPath = HttpContext.Current.Server.MapPath("~").TrimEnd('/') + "/CivetLog";
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            LogManager.PathLog = Server.MapPath("~/CivetLog");
            //发放消息对象
            UtilCivetMsg.Sender = new MsgSender(CivetHelper.UID, CivetHelper.Password);
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (FristRequest) //服務器第一次請求
            {
                FristRequest = false;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            GlobalHelper global = new GlobalHelper();
            global.RecException(this);
        }
    }
}