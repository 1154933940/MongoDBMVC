using CivetHR.UserAPI;
using PDSS.MongoDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CivetUserAPIMg.Models
{

    [Serializable]
    [MongoCollectionAttribute("uapi_power_lists")]
    public class ModChanncelList : ModUApiPowerList
    {
        /// <summary>
        /// 本月访问量
        /// </summary>
        public int MonthlyNum { get; set; }

        /// <summary>
        /// 总访问量
        /// </summary>
        public int TotalNum { get; set; }
    }

    [Serializable]
    [MongoCollectionAttribute("uapi_use_logs")]
    public class ModChanncelUseLogs : ModUApiUseLogFind
    {
        /// <summary>
        /// 本月访问量
        /// </summary>
        public int MonthlyNum { get; set; }

        /// <summary>
        /// 总访问量
        /// </summary>
        public int TotalNum { get; set; }
        public string interface_code_name { get; set; }
    }


}