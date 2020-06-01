using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CivetUserAPIMg.Models;
using CivetUserAPIMg.Controllers;
using PDSS.CivetPublic.User;
using PDSS.MVC.Utility;
using CivetHR.UserAPI;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace CivetUserAPIMg.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Logins the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="ToController">To controller.</param>
        /// <param name="ToAction">To action.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Login(string Signature, string ToController, string ToAction, string id)
        {
            try
            {
                OAuth oauth = new OAuth(CivetHelper.AppID);
                OAuth.UserInfo uInfo = oauth.FastGetUserInfo(Request);
                //判斷用戶權限並給UserInfo賦值(存入Session)
                if (uInfo == null)
                {
                    return Redirect(Url.Action("AlertInfo", "Home", new { ErrMsg = LUtil.Lang(HttpContext, "无账号信息") }));
                }
                #region  添加管理员信息
                if (uInfo != null)
                {
                    if (!string.IsNullOrEmpty(uInfo.civetno))
                    {
                        MongoCollection<ModUApiManager> uapimanager = mongoh.GetDb().GetCollection<ModUApiManager>("uapi_managers");
                        ModUApiManager modeuap = uapimanager.Find(Query.And(
                            Query.EQ("_id", new BsonString(uInfo.civetno.ToUpper())),
                            Query.EQ("is_disabled", new BsonBoolean(false))
                            )).FirstOrDefault();
                        if (modeuap != null)
                        {
                            if (modeuap.last_login_time < DateTime.Now.AddMinutes(-2))
                            {
                                uapimanager.Update((Query.EQ("_id", new BsonString(uInfo.civetno.ToUpper()))), Update.Set("last_login_time", DateTime.Now));
                            }
                            UtilComm.UApi_Manager = modeuap;
                            //if (!modeuap.is_disabled)
                            //{
                            //    return Redirect(Url.Action("AlertInfo", "Home", new { ErrMsg = LUtil.Lang(HttpContext, "无权限") }));
                            //}
                        }
                        else
                        {
                            return Redirect(Url.Action("AlertInfo", "Home", new { ErrMsg = LUtil.Lang(HttpContext, "无账号信息") }));
                        }
                        Managelog(uInfo.civetno.ToUpper());
                    }

                }
                #endregion
                base.UserInfo = new ModUserInfo { BaseInfo = uInfo };
                string url = string.IsNullOrEmpty(id) ? Url.Action(ToAction, ToController) : Url.Action(ToAction, ToController, new { id = id });
                string QueryString = HtmlUtil.DealQuery(Request.QueryString.ToString(), "code", "id");
                if (QueryString != "")
                {
                    url += "?" + QueryString;
                }
                //完善登錄操作
                return Redirect(url);
            }
            catch (Exception)
            {
                return Redirect(Url.Action("AlertInfo", "Home", new { ErrMsg = LUtil.Lang(HttpContext, "数据读取异常") }));
            }
        }

        /// <summary>
        /// 自检测服务器
        /// </summary>
        /// <returns></returns>
        public ActionResult info(string Exception)
        {
            if (!string.IsNullOrEmpty(Exception))
            {
                throw new Exception(Exception);
            }
            string[] ips = Request.ServerVariables["Local_Addr"].Replace("::1", "127.0.0.1").Split('.');
            string ip = "*.*." + ips[2] + "." + ips[3];
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            return Content("IP:" + ip + "<br>Time:" + now + "<br>UserAgent:" + Request.UserAgent);
        }


        /// <summary>
        /// 警告提示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult AlertInfo(int id = 0, string ErrMsg = "", bool HistoryBack = false, string UrlTo = null)
        {
            switch (id)
            {
                case 4041:
                    ErrMsg = "找不到您要查看的内容，可能内容已被删除。";
                    break;
                default:
                    if (string.IsNullOrEmpty(ErrMsg))
                        ErrMsg = "数据读取异常。";
                    break;
            }
            ViewBag.ErrMsg = ErrMsg;
            ViewBag.HistoryBack = HistoryBack;
            ViewBag.UrlTo = UrlTo;
            return View();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            string url = Util.JsUrlTo(Url.Action("Index", "Home"));
            Session.Abandon();
            return Content(url);
        }

        /// <summary>
        /// 管理员登录日志
        /// </summary>
        private void Managelog(string civetno)
        {
            try
            {
                var collection = mongoh.GetCollection<ModUApiManagerLog>();
                ModUApiManagerLog log = new ModUApiManagerLog
                {
                    civet_no = civetno,
                    login_ip = Request.UserHostAddress,
                    login_time = DateTime.Now
                };
                collection.Insert(log);
            }
            catch (Exception)
            {
            }
        }
    }
}
