using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Method
{
    public static class StaticName
    {
        #region  走量信息
        /// <summary>
        /// 走量
        /// </summary>
        public const string ZL = "走量(PCS)";
        /// <summary>
        /// 年将率
        /// </summary>
        public const string NJL = "年降率(%)";
        /// <summary>
        /// 年度返利要求
        /// </summary>
        public const string NDFLYQ = "年度返利要求(%)";
        /// <summary>
        /// 一次性这让率
        /// </summary>
        public const string YCXZRL = "一次性折让率（%）";
        #endregion
        #region  核心部件
        /// <summary>
        /// Sensor芯片
        /// </summary>
        public const string Sensor = "Sensor芯片";
        /// <summary>
        /// Sensor芯片 前端統一显示名称
        /// </summary>
        public const string Sensor_CoreComponent = "Sensor";
        /// <summary>
        /// 芯片IC-ISP
        /// </summary>
        public const string ICISP = "芯片IC-ISP";
        /// <summary>
        /// 芯片IC-ISP 前端統一显示名称
        /// </summary>
        public const string ICISP_CoreComponent = "ISP";
        /// <summary>
        /// 串行芯片
        /// </summary>
        public const string SerialChip = "串行芯片";
        /// <summary>
        /// 串行芯片 前端統一显示名称
        /// </summary>
        public const string SerialChip_CoreComponent = "串行板芯片";
        /// <summary>
        /// LED/VCSEL
        /// </summary>
        public const string LEDVCSEL = "LED/VCSEL";
        /// <summary>
        /// LED/VCSEL 前端統一显示名称
        /// </summary>
        public const string LEDVCSEL_CoreComponent = "LED";
        /// <summary>
        /// 镜头
        /// </summary>
        public const string Shot = "镜头";
        /// <summary>
        /// 镜头 前端統一显示名称
        /// </summary>
        public const string Shot_CoreComponent = "Lens";
        /// <summary>
        /// 线束
        /// </summary>
        public const string Harness = "线束";
        /// <summary>
        /// 线束 前端統一显示名称
        /// </summary>
        public const string Harness_CoreComponent = "线缆";
        #endregion
        #region NRE
        /// <summary>
        /// 手板件费
        /// </summary>
        public const string SBJF = "手板件费";
        /// <summary>
        /// 模具费
        /// </summary>
        public const string MJF = "模具费";
        /// <summary>
        /// 检具费
        /// </summary>
        public const string JJF = "检具费";
        /// <summary>
        /// 生产设备费
        /// </summary>
        public const string SCSBF = "生产设备费";
        /// <summary>
        /// 工装治具费
        /// </summary>
        public const string GZZJF = "工装治具费";
        /// <summary>
        /// 实验费
        /// </summary>
        public const string SYF = "实验费";
        /// <summary>
        /// 测试软件费
        /// </summary>
        public const string CSRJF = "测试软件费";
        /// <summary>
        /// 差旅费
        /// </summary>
        public const string CLF = "差旅费";
        /// <summary>
        /// 其他费用
        /// </summary>
        public const string QTFY = "其他费用";
        #endregion
        /// <summary>
        /// 总成本
        /// </summary>
        public const string ZCB = "总成本";
        /// <summary>
        /// 备注
        /// </summary>
        public const string BZ = "备注";
        #region 内部核价信息
        /// <summary>
        /// BOM成本
        /// </summary>
        public const string BOMCB = "BOM成本";
        /// <summary>
        /// 制造成本
        /// </summary>
        public const string ZZCB = "制造成本";
        /// <summary>
        /// 损耗成本
        /// </summary>
        public const string SHCB = "损耗成本";
        /// <summary>
        /// 物流成本
        /// </summary>
        public const string WLCB = "物流成本";
        /// <summary>
        /// MOQ分摊成本
        /// </summary>
        public const string MOQFTCB = "MOQ分摊成本";
        /// <summary>
        /// 其他（质量+财务成本）
        /// </summary>
        public const string QT = "其他（质量+财务成本）";
        #endregion  
    }
}
