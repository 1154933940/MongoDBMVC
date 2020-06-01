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
    public class ChanncelPowerController : BaseController
    {
        //
        // GET: /ChanncelPower/

        public ActionResult Index(SeeAboutModel model, int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.NE("", new BsonString("")));
            //queries.Add(Query.EQ("status", new BsonString("待审核")));
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.civetno))
                {
                    queries.Add(Query.Or(Query.EQ("civetno", new BsonString(model.civetno)), Query.EQ("user_name", new BsonString(model.civetno))));
                }
                if (!string.IsNullOrEmpty(model.serviceno_uid))
                {
                    queries.Add(Query.EQ("_id", new BsonString(model.serviceno_uid)));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.SearchCondition = model;
            PagedList<ModChanncelList> result = mongoh.GetCollection<ModChanncelList>().FindByAnd(queries)
                .MOrderByDescending(m => m.start_date).MToPagedList(pageid, 20);


            if (result.Count != 0)
            {
                //获得本次显示的所有频道号
                List<string> ids = new List<string>();
                foreach (ModChanncelList item in result)
                {
                    ids.Add(item._id);
                }
                #region 查询统计数据
                BsonDocument match = new BsonDocument { 
                {
                    "$match", new BsonDocument{
                        {
                            "serviceno_uid", new BsonDocument("$in", new BsonArray(ids.ToArray()))
                        }
                    } 
                }
            };
                DateTime MonthlyFirstDate = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-01"));
                List<object> objarray = new List<object>();
                objarray.Add("$visited_time");
                objarray.Add(new BsonDateTime(MonthlyFirstDate));
                BsonDocument group = new BsonDocument { 
                {
                    "$group", new BsonDocument{
                        {
                            "_id", "$serviceno_uid"
                        },
                        {
                            "MonthlyNum", new BsonDocument{
                                {
                                    "$sum",  new BsonDocument{
                                            {
                                                "$cond", new BsonDocument
                                                {
                                                    { 
                                                        "if", new BsonDocument("$gte", new BsonArray(objarray.ToArray()))
                                                    }
                                                    ,
                                                    { "then", 1 }
                                                    ,
                                                    { "else",  0 }
                                                }
                                            }
                                        }
                                }
                            }
                        },
                        {
                            "TotalNum", new BsonDocument{
                                {
                                    "$sum", 1
                                }
                            }
                        }
                    } 
                }
            };
                AggregateArgs args = new AggregateArgs();
                args.Pipeline = new BsonDocument[] { match, group };
                var statResult = mongoh.GetCollection<ModUApiUseLogFind>().Aggregate(args).ToList();
                #endregion
                foreach (BsonDocument item in statResult)
                {
                    string uid = item["_id"].AsString;
                    int monthlyNum = item["MonthlyNum"].AsInt32;
                    int totalNum = item["TotalNum"].AsInt32;
                    ModChanncelList oneDate = (from m in result
                                               where m._id == uid
                                               select m).FirstOrDefault();
                    if (oneDate != null)
                    {
                        oneDate.MonthlyNum = monthlyNum;
                        oneDate.TotalNum = totalNum;
                    }
                }
            }
            return View(result);
        }


        public ActionResult PowerListDetail(string id = "")
        {
            MongoCollection<ModUApiPowerList> mg = mongoh.GetCollection<ModUApiPowerList>();
            ModUApiPowerList mod = mg.Find(Query.EQ("_id", new BsonString(id))).FirstOrDefault();
            if (mod == null)
            {
                return Content(Util.JsUrlTo(Url.Action("Index"), "数据不存在"));
            }
            return View(mod);
        }
    }
}
