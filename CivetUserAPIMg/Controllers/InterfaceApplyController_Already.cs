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
    /**
     * 已审核接口列表
     * */
    public partial class InterfaceApplyController : BaseController
    {
        public ActionResult Index_Already(SeeAboutModel model, int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            //queries.Add(Query.NE("status", new BsonString("待审核")));
            queries.Add(Query.Or(Query.EQ("status", new BsonString("审核通过")), Query.EQ("status", new BsonString("审核驳回"))));
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.civetno))
                {
                    queries.Add(Query.Or(Query.EQ("civetno", new BsonString(model.civetno)), Query.EQ("user_name", new BsonString(model.civetno))));
                }
                if (!string.IsNullOrEmpty(model.serviceno_uid))
                {
                    queries.Add(Query.EQ("serviceno_uid", new BsonString(model.serviceno_uid)));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.SearchCondtion = model;
            var serachResult = mongoh.GetCollection<ModUApiPowerAplictionFind>().FindByAnd(queries)
                .MOrderByDescending(m => m._id).MToPagedList(pageid, 20);
            return View(serachResult);
        }


        public ActionResult AuditOneMessage_Already(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "参数错误"));
            }
            MongoCollection<ModUApiPowerAplictionFind> modUApiPowerAplictionFind = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind mod = modUApiPowerAplictionFind.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            if (mod == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "数据读取异常"));
            }
            ViewBag.uapi_interface_list = mongoh.GetCollection<ModUApiInterfaceList>().FindAll().MOrderBy(m => m.index).ToList();
            ViewBag.uapi_field_list = mongoh.GetCollection<ModUApiFieldList>().FindAll().MOrderBy(m => m.index).ToList();
            return View(mod);
        }
        public ActionResult UpdateInfo(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "参数错误"));
            }
            MongoCollection<ModUApiPowerAplictionFind> modUApiPowerAplictionFind = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind mod = modUApiPowerAplictionFind.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            if (mod == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "数据读取异常"));
            }
            return View(mod);
        }
        public ActionResult UpdateInfoSave(ModUApiPowerAplictionFind mod, FormCollection form)
        {
            string id =CUtil.CStr(form["id"]);
            MongoCollection<ModUApiPowerAplictionFind> modUApiPowerAplictionFind = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind Model = modUApiPowerAplictionFind.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            if (Model == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "数据读取异常"));
            }
            var strLocation = Query.EQ("_id", new ObjectId(id));
            modUApiPowerAplictionFind.Update((strLocation), Update.Set("serviceno_uid", mod.serviceno_uid == null ? "" : mod.serviceno_uid).Set("civetno", mod.civetno == null ? "" : mod.civetno).Set("user_name", mod.user_name == null ? "" : mod.user_name).Set("mobile", mod.mobile == null ? "" : mod.mobile).Set("ext_phone", mod.ext_phone == null ? "" : mod.ext_phone).Set("bg_name", mod.bg_name == null ? "" : mod.bg_name).Set("bu_name", mod.bu_name == null ? "" : mod.bu_name).Set("dept_name", mod.dept_name == null ? "" : mod.dept_name).Set("cost_code", mod.cost_code == null ? "" : mod.cost_code).Set("apply_reason", mod.apply_reason == null ? "" : mod.apply_reason));
            return Content(Util.JsCustom("alert('修改成功');window.parent._close_dialog(); window.parent.refresh();"));
        }

        public ActionResult Delete(string id = "")
        {
            MongoCollection<ModUApiPowerAplictionFind> modUApiPowerAplictionFind = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind Model = modUApiPowerAplictionFind.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            if (Model == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index_Already"), "数据不存在"));
            }
            modUApiPowerAplictionFind.Remove(Query.EQ("_id", new ObjectId(id)));
            return Content(Util.JsUrlTo(Url.Action("Index_Already"), "删除成功"));
        }
    }
}