using CivetHR.UserAPI;
using CivetUserAPIMg.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using PDSS.MVC.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using PDSS.MongoDB.Utility;

namespace CivetUserAPIMg.Controllers
{
    public class DownCountController : BaseController
    {
        //
        // GET: /DownCount/
        public ActionResult Index(SeeAboutModel model, int pageid = 1)
        {
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.NE("", new BsonString("")));
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
            ViewBag.SearchCondtion = model;
            var serachResult = mongoh.GetCollection<ModUApiUseLogFind>().FindByAnd(queries)
                .MOrderByDescending(m => m.visited_time).MToPagedList(pageid, 20);
            int totalCount = 0;
            int theMonth = 0;
            int thetoday = 0;
            string serviceno_uid_name = "";
            if (!string.IsNullOrEmpty(model.serviceno_uid))//仅限于某个频道号的统计
            {
                List<IMongoQuery> newqueries = new List<IMongoQuery>();
                newqueries.Add(Query.EQ("serviceno_uid", new BsonString(model.serviceno_uid)));
                var newserachResult = mongoh.GetCollection<ModUApiUseLogFind>().FindByAnd(newqueries).ToList();
                TimeCount(ref totalCount, ref theMonth, ref thetoday, newserachResult);
                serviceno_uid_name = model.serviceno_uid;
            }
            ViewBag.TotalCount = totalCount;
            ViewBag.TheMonth = theMonth;
            ViewBag.Thetoday = thetoday;
            ViewBag.ServicenoUidName = serviceno_uid_name;
            return View(serachResult);
        }
        public ActionResult ToExcel(string servicenouid = "", string visitedtime = "")
        {
            if (string.IsNullOrEmpty(servicenouid))
            {
                return Content(Util.JsHistoryBack("请输入频道账号"));
            }
            List<IMongoQuery> queries = new List<IMongoQuery>();
            queries.Add(Query.NE("", new BsonString("")));
            if (!string.IsNullOrEmpty(servicenouid))
            {
                queries.Add(Query.EQ("serviceno_uid", new BsonString(servicenouid)));
            }
            if (!string.IsNullOrEmpty(visitedtime))
            {
                queries.Add(Query.GTE("visited_time", CUtil.CDate(visitedtime)));
            }
            var modUApiUseLogFindList = mongoh.GetCollection<ModUApiUseLogFind>().FindByAnd(queries)
                .MOrderByDescending(m => m.visited_time).ToList();
            #region 赋值datatable
            DataTable dt = new DataTable();
            dt.Columns.Add("频道账号", typeof(string));
            dt.Columns.Add("接口名称", typeof(string));
            dt.Columns.Add("查询参数", typeof(string));
            dt.Columns.Add("访问时间", typeof(string));
            dt.Columns.Add("访问IP", typeof(string));
            foreach (var item in modUApiUseLogFindList)
            {
                DataRow dr = dt.NewRow();
                dr[0] = item.serviceno_uid;
                dr[1] = item.interface_code;
                dr[2] = item.request_data;
                dr[3] = item.visited_time;
                dr[4] = item.visited_ip;
                dt.Rows.Add(dr);
            }
            int totalCount = 0;
            int theMonth = 0;
            int thetoday = 0;
            TimeCount(ref totalCount, ref theMonth, ref thetoday, modUApiUseLogFindList);
            DataRow drs = dt.NewRow();
            drs[0] = " 合计  " + totalCount;
            drs[1] = "  本月调用次数 " + theMonth;
            drs[2] = "  今日调用次数 " + thetoday;
            dt.Rows.Add(drs);
            #endregion
            string pathfile = Request.PhysicalApplicationPath + "UploadFiles";
            if (!System.IO.Directory.Exists(pathfile))
            {
                System.IO.Directory.CreateDirectory(pathfile);
            }
            string ExcelNpoiUtil_Name = "频道账号调用统计_" + DateTime.Now.ToString("hhmmss");

            ExcelNpoiUtil.ExportExcel(dt, Server.MapPath("~/UploadFiles/") + "" + ExcelNpoiUtil_Name + "" + ".xls", false);
            return Content(Util.JsHistoryBack("已下载"));
        }

        private void TimeCount(ref int TotalCount, ref int TheMonth, ref int Thetoday, List<ModUApiUseLogFind> list)
        {
            DateTime dateTime = DateTime.Now;
            string toDaytime = dateTime.ToString("yyyy/MM/dd 00:00:00");
            string beginMonth = dateTime.AddDays(1 - dateTime.Day).ToString("yyyy/MM/dd 00:00:00");
            TheMonth = list.Where(m => m.visited_time >= DateTime.Parse(beginMonth)).Count();
            Thetoday = list.Where(m => m.visited_time >= DateTime.Parse(toDaytime)).Count();
            TotalCount = list.Count();
        }
    }
}
