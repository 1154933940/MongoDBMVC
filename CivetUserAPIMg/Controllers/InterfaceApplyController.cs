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
     * 接口申请审核页面
     * **/
    public partial class InterfaceApplyController : BaseController
    {
        //
        // GET: /InterfaceApply/

        public ActionResult Index(SeeAboutModel model, string Status="", int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            //queries.Add(Query.EQ("status", new BsonString("待审核")));
            queries.Add(Query.Or(Query.EQ("status", new BsonString("待审核")), Query.EQ("status", new BsonString("申请中"))));
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
                if (!string.IsNullOrEmpty(Status))
                {
                    queries.Add(Query.EQ("status", new BsonString(Status)));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.status = Status;
            ViewBag.SearchCondtion = model;
            var serachResult = mongoh.GetCollection<ModUApiPowerAplictionFind>().FindByAnd(queries)
                .MOrderByDescending(m => m._id).MToPagedList(pageid, 20);
            return View(serachResult);

        }
        public ActionResult AuditOneMessage(string id)
        {
            MongoCollection<ModUApiPowerAplictionFind> modUApiPowerAplictionFind = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind modUApiPowerAplictionFindList = modUApiPowerAplictionFind.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            ViewBag.uapi_interface_list = mongoh.GetCollection<ModUApiInterfaceList>().FindAll().OrderBy(m => m.index).ToList();
            ViewBag.uapi_field_list = mongoh.GetCollection<ModUApiFieldList>().FindAll().OrderBy(m => m.index).ToList();
            return View(modUApiPowerAplictionFindList);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AuditOneMessageSave(FormCollection form)
        {

            string id = form["_id"] ?? "";
            string Audit = form["Audit"] ?? "";
            string remark = form["remark"] ?? "";
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(Audit))
            {
                return Content(Util.JsUrlTo(Url.Action("Index"), "数据错误"));
            }
            MongoCollection<ModUApiPowerAplictionFind> uapiaplict = mongoh.GetCollection<ModUApiPowerAplictionFind>();
            ModUApiPowerAplictionFind resultaplict = uapiaplict.Find(Query.EQ("_id", new ObjectId(id))).FirstOrDefault();
            if (resultaplict == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index"), "数据已删除或不存在"));
            }
            if (!resultaplict.status.Contains("待审核"))
            {
                return Content(Util.JsUrlTo(Url.Action("Index"), "申请信息已审核，请勿再次提交"));
            }
            #region 查找出需要的接口
            string needinter = "";
            //首先查询这条记录是不是第一次申请，如果没有原有接口，那么就直接新增接口，如果有原有接口，那么就要在原有接口上移除需要的接口再加入新增的接口
            if (resultaplict.interfaces != null && resultaplict.interfaces.Length > 0 && !string.IsNullOrEmpty(resultaplict.interfaces[0]))
            {
                #region 先得出所有需要移除的接口
                string removie = "";
                if (resultaplict.remove_interfaces != null && resultaplict.remove_interfaces.Length > 0 && !string.IsNullOrEmpty(resultaplict.remove_interfaces[0]))
                {

                    foreach (var item1 in resultaplict.remove_interfaces)
                    {
                        removie += item1 + ",";
                    }
                    removie = removie.TrimEnd(',');
                }
                List<string> str = resultaplict.interfaces.ToList();
                List<string> str_ = resultaplict.interfaces.ToList();
                foreach (var item in str)
                {
                    if (removie.Contains(item))
                    {
                        str_.Remove(item);
                    }
                }
                foreach (var item in str_)
                {
                    needinter += item + ",";
                }
                #endregion
                if (resultaplict.add_interfaces != null && resultaplict.add_interfaces.Length > 0 && !string.IsNullOrEmpty(resultaplict.add_interfaces[0]))
                {
                    foreach (var item1 in resultaplict.add_interfaces)
                    {
                        needinter += item1 + ",";
                    }
                }
            }
            else
            {
                if (resultaplict.add_interfaces != null && resultaplict.add_interfaces.Length > 0 && !string.IsNullOrEmpty(resultaplict.add_interfaces[0]))
                {
                    foreach (var item1 in resultaplict.add_interfaces)
                    {
                        needinter += item1 + ",";
                    }
                }
            }
            needinter = needinter.TrimEnd(',');
            string[] Arrayneedinter = needinter.Split(',');
            #endregion
            #region  查找出需要的栏位
            string needfields = "";
            //首先查询这条记录是不是第一次申请，如何没有原有接口，那么就直接新增接口，如果有原有接口，那么就要在原有接口上移除需要的接口再加入新增的接口
            if (resultaplict.fields != null && resultaplict.fields.Length > 0 && !string.IsNullOrEmpty(resultaplict.fields[0]))
            {
                #region 先得出所有需要移除的栏位
                string removie = "";
                if (resultaplict.remove_fields != null && resultaplict.remove_fields.Length > 0 && !string.IsNullOrEmpty(resultaplict.remove_fields[0]))
                {
                    foreach (var item1 in resultaplict.remove_fields)
                    {
                        removie += item1 + ",";
                    }
                }
                List<string> str = resultaplict.fields.ToList();
                List<string> str_ = resultaplict.fields.ToList();
                foreach (var item in str)
                {
                    if (removie.Contains(item))
                    {
                        str_.Remove(item);
                    }
                }
                foreach (var item in str_)
                {
                    needfields += item + ",";
                }
                #endregion
                if (resultaplict.add_fields != null && resultaplict.add_fields.Length > 0 && !string.IsNullOrEmpty(resultaplict.add_fields[0]))
                {
                    foreach (var item1 in resultaplict.add_fields)
                    {
                        needfields += item1 + ",";
                    }
                }
            }
            else
            {
                if (resultaplict.add_fields != null && resultaplict.add_fields.Length > 0 && !string.IsNullOrEmpty(resultaplict.add_fields[0]))
                {
                    foreach (var item1 in resultaplict.add_fields)
                    {
                        needfields += item1 + ",";
                    }
                }
            }
            needfields = needfields.TrimEnd(',');
            string[] Arrayneedfields = needfields.Split(',');
            #endregion
            //通过
            if (Audit.Contains("1"))
            {
                //添加用户权限数据
                var collection = mongoh.GetCollection<ModUApiPowerList>();
                if (collection.Count(Query.EQ("_id", new BsonString(resultaplict.serviceno_uid))) == 0)
                {
                    ModUApiPowerList power = new ModUApiPowerList
                    {
                        _id = resultaplict.serviceno_uid.ToUpper(),
                        bg_name = resultaplict.bg_name,
                        bu_name = resultaplict.bu_name,
                        dept_name = resultaplict.dept_name,
                        civetno = resultaplict.civetno,
                        user_name = resultaplict.user_name,
                        cost_code = resultaplict.cost_code,
                        start_date = DateTime.Today,
                        end_date = new DateTime(2020, 12, 31),
                        fields = Arrayneedfields,
                        interfaces = Arrayneedinter,
                        ext_phone = resultaplict.ext_phone,
                        mobile = resultaplict.mobile,
                        limit_monthly_count = resultaplict.add_day,
                        open_power = true,
                        private_key_des = ""
                    };
                    collection.Insert(power);
                }
                else
                {
                    MongoCollection<ModUApiPowerList> uapipower = mongoh.GetCollection<ModUApiPowerList>();
                    ModUApiPowerList modpower = uapipower.Find(Query.EQ("_id", new BsonString(resultaplict.serviceno_uid))).FirstOrDefault();
                    if (modpower == null)
                    {
                        return Content(Util.JsUrlTo(Url.Action("Index"), "数据不存在"));
                    }
                    modpower.fields = Arrayneedfields;
                    modpower.interfaces = Arrayneedinter;
                    modpower.bg_name = resultaplict.bg_name;
                    modpower.bu_name = resultaplict.bu_name;
                    modpower.dept_name = resultaplict.dept_name;
                    modpower.civetno = resultaplict.civetno;
                    modpower.user_name = resultaplict.user_name;
                    modpower.cost_code = resultaplict.cost_code;
                    modpower.ext_phone = resultaplict.ext_phone;
                    modpower.mobile = resultaplict.mobile;
                    modpower.limit_monthly_count = resultaplict.add_day;
                    BsonDocument bd = BsonExtensionMethods.ToBsonDocument(modpower);
                    //修改将需要的接口和栏位保存到权限表中去
                    uapipower.Update(Query.EQ("_id", new BsonString(resultaplict.serviceno_uid)), new UpdateDocument(bd));
                }
                uapiaplict.Update(Query.EQ("_id", new ObjectId(id)), Update.Set("status", "审核通过").Set("audit_time", DateTime.Now).Set("audit_civet_no", UserInfo.BaseInfo.civetno).Set("apply_reason", remark));
            }
            else
            {
                if (Audit.Contains("2"))
                {
                    uapiaplict.Update(Query.EQ("_id", new ObjectId(id)), Update.Set("status", "审核驳回").Set("audit_time", DateTime.Now).Set("audit_civet_no", UserInfo.BaseInfo.civetno).Set("apply_reason", remark));
                }
            }
            return Content(Util.JsUrlTo(Url.Action("Index")));
        }
    }
}
