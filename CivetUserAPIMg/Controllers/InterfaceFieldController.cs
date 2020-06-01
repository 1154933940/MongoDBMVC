using CivetHR.UserAPI;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CivetUserAPIMg.Controllers
{
    public class InterfaceFieldController : BaseController
    {
        //
        // GET: /InterfaceField/

        //接口、栏位管理
        [OutputCache(Duration = 600)]
        public ActionResult Index()
        {
            MongoCollection<ModUApiInterfaceList> mg = mongoh.GetCollection<ModUApiInterfaceList>();
            List<ModUApiInterfaceList> mglist = mg.FindAll().OrderBy(m => m.index).ToList();
            MongoCollection<ModUApiFieldList> mfd = mongoh.GetCollection<ModUApiFieldList>();
            List<ModUApiFieldList> mfdlist = mfd.FindAll().OrderBy(m => m.index).ToList();
            ViewBag.ModUApiFieldList = mfdlist;
            ViewBag.ModUApiInterfaceList = mglist;
            return View();
        }
    }
}
