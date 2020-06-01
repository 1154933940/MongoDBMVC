using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CivetUserAPIMg.Models
{
    /// <summary>
    /// 条件查询类
    /// </summary>
    public class SeeAboutModel
    {
        /// <summary>
        /// 频道号
        /// </summary>
        public string serviceno_uid { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string visited_time { get; set; }
        /// <summary>
        /// 香信号
        /// </summary>
        public string civetno { get; set; }

    }
    /// <summary>
    /// 曲线图
    /// </summary>
    public class CurveChart
    {
        /// <summary>
        /// 数量
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime date { get; set; }
    }
}