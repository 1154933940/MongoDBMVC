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
    public class CounterController : BaseController
    {
        //
        // GET: /Counter/
        /*用户使用接口次数统计
         *查询接口日志记录表，已记录每个接口的访问信息
         */
        public ActionResult Index(FormCollection form)
        {
            string interfacecode = form["interface_code"] ?? "";
            MongoCollection<ModUApiInterfaceList> uapinter = mongoh.GetCollection<ModUApiInterfaceList>();
            #region 统计当月，今日，合计调用次数
            DateTime MonthlyFirstDate = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-01"));
            List<object> objarray = new List<object>();
            objarray.Add("$visited_time");
            objarray.Add(new BsonDateTime(MonthlyFirstDate));
            DateTime TodayFirstDate = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd"));
            List<object> objarrayToday = new List<object>();
            objarrayToday.Add("$visited_time");
            objarrayToday.Add(new BsonDateTime(TodayFirstDate));
            BsonDocument group = new BsonDocument 
            { 
                {
                    "$group", new BsonDocument{
                        {
                            "_id", "$interface_code"
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
                            "TodayNum", new BsonDocument{
                                {
                                    "$sum",  new BsonDocument{
                                            {
                                                "$cond", new BsonDocument
                                                {
                                                    { 
                                                        "if", new BsonDocument("$gte", new BsonArray(objarrayToday.ToArray()))
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
            #endregion
            AggregateArgs args = new AggregateArgs();
            args.Pipeline = new List<BsonDocument> { group };
            var bsonDocumentlist = mongoh.GetCollection<ModUApiUseLogFind>().Aggregate(args).ToList();
            if (string.IsNullOrEmpty(interfacecode))
            {
                foreach (var item in bsonDocumentlist)
                {
                    interfacecode = item["_id"].ToString();
                    break;
                }
            }
            #region 统计本周流程图
            DateTime dt = DateTime.Now;
            DateTime starweek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));
            string endtime = dt.ToString("yyyy-MM-dd 23:59:59");
            string begintime = starweek.ToString("yyyy-MM-dd 00:00:00");
            List<IMongoQuery> queries = new List<IMongoQuery>();
            if (!string.IsNullOrEmpty(interfacecode))
            {
                queries.Add(Query.EQ("interface_code", new BsonString(interfacecode)));
            }
            queries.Add(Query.GTE("visited_time", CUtil.CDate(begintime)));
            queries.Add(Query.LTE("visited_time", CUtil.CDate(endtime)));
            var serachResult = mongoh.GetCollection<ModUApiUseLogFind>().FindByAnd(queries).ToList();
            var curveChart = (from b in serachResult
                              orderby b.visited_time
                              group b by b.visited_time.Date into b
                              select new CurveChart
                            {
                                amount = b.Count().ToString(),
                                date = b.Key
                            }).ToList();
            string amount = "";
            string date = "";
            if (curveChart.Count > 0)
            {
                foreach (var item in curveChart)
                {
                    amount += item.amount + ",";
                    date += item.date.ToString("yyyyMMdd") + ",";
                }
            }
            amount = amount.TrimEnd(',');
            #endregion
            ViewBag.BsonDocumentlist = bsonDocumentlist;
            ViewBag.interfacecode = interfacecode;
            ViewBag.ModUApiInterfaceList = uapinter.FindAll().ToList();
            ViewBag.Amount = amount;
            ViewBag.Date = date;
            return View();

        }
        public ActionResult Detail(SeeAboutModel model, string code = "", int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.EQ("interface_code", new BsonString(code)));
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.serviceno_uid))
                {
                    queries.Add(Query.EQ("serviceno_uid", new BsonString(model.serviceno_uid)));
                }
                if (!string.IsNullOrEmpty(model.visited_time))
                {
                    queries.Add(Query.GTE("visited_time", CUtil.CDate(model.visited_time)));
                }
            }
            else
            {
                model = new SeeAboutModel();
            }
            ViewBag.seeAboutModel = model;
            ViewBag.Code = code;
            var serachResult = mongoh.GetCollection<ModUApiUseLogFind>().FindByAnd(queries)
                .MOrderByDescending(m => m.visited_time).MToPagedList(pageid, 10);
            return View(serachResult);
        }
    }
}
