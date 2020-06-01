using CivetHR.UserAPI;
using CivetUserAPIMg.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using PDSS.MVC.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using PDSS.MongoDB.Utility;

namespace CivetUserAPIMg.Controllers
{
    /// <summary>
    /// 账号管理
    /// </summary>
    public class AccountManageController : BaseController
    {
        //
        // GET: /AccountManage/

        public ActionResult Index(SeeAboutModel model, int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.NE("", new BsonString("")));
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.civetno))
                {
                    queries.Add(Query.Or(Query.EQ("_id", new BsonString(model.civetno)), Query.EQ("user_name", new BsonString(model.civetno))));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.SearchCondtion = model;
            var serachResult = mongoh.GetCollection<ModUApiManager>().FindByAnd(queries)
                .MOrderByDescending(m => m.last_login_time).MToPagedList(pageid, 20);
            return View(serachResult);
        }

        public ActionResult AddUpdate(string id = "")
        {
            ModUApiManager Model = new ModUApiManager();
            if (string.IsNullOrEmpty(id))
            {
                TempData["Mgs"] = "1";
                return View(Model);
            }
            TempData["Mgs"] = "2";
            MongoCollection<ModUApiManager> mg = mongoh.GetCollection<ModUApiManager>();
            Model = mg.Find(Query.EQ("_id", new BsonString(id))).FirstOrDefault();
            return View(Model);
        }
        public ActionResult AddUpdateSave(ModUApiManager mod, FormCollection form)
        {
            ModUApiManager model = new ModUApiManager();
            string status_name = CUtil.CStr(form["status_name"]);
            MongoCollection<ModUApiManager> mg = mongoh.GetCollection<ModUApiManager>();
            var eq = Query.EQ("_id", new BsonString(mod._id));
            if (status_name == "1")
            {
                model = mg.Find(eq).FirstOrDefault();
                if (model != null)
                {
                    return Content(Util.JsUrlTo(Url.Action("Index"), "新增失败，香信号已存在"));
                }
                model = new ModUApiManager();
                model._id = mod._id.ToUpper();
                model.is_disabled = mod.is_disabled;
                model.user_name = mod.user_name;
                model.last_login_time = DateTime.Now;
                mg.Insert(model);
                return Content(Util.JsUrlTo(Url.Action("Index"), "新增成功"));
            }
            model = mg.Find(eq).FirstOrDefault();
            if (model == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index"), "修改失败，数据不存在"));
            }
            model.is_disabled = mod.is_disabled;
            model.user_name = mod.user_name;
            BsonDocument bd = BsonExtensionMethods.ToBsonDocument(model);
            mg.Update(Query.EQ("_id", new BsonString(mod._id)), new UpdateDocument(bd));
            return Content(Util.JsUrlTo(Url.Action("Index"), "修改成功"));

        }


        public ActionResult Delete(string id = "")
        {
            MongoCollection<ModUApiManager> mg = mongoh.GetCollection<ModUApiManager>();
            ModUApiManager Model = mg.Find(Query.EQ("_id", new BsonString(id))).FirstOrDefault();
            if (Model == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index", "AccountManage"), "数据不存在"));
            }
            mg.Remove(Query.EQ("_id", new BsonString(id)));
            return Redirect(Url.Action("Index", "AccountManage", "删除成功"));
        }
        public ActionResult ManageLog(SeeAboutModel model, int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.NE("", new BsonString("")));
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.civetno))
                {
                    queries.Add(Query.Or(Query.EQ("civet_no", new BsonString(model.civetno))));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.SearchCondtion = model;
            var serachResult = mongoh.GetCollection<ModUApiManagerLogFind>().FindByAnd(queries)
                .MOrderByDescending(m => m.login_time).MToPagedList(pageid, 20);
            return View(serachResult);
        }
    }
}
