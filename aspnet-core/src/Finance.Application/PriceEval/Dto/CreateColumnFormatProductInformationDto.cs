using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.PriceEval.Dto
{
    /// <summary>
    /// 创建产品信息表（输入格式：列格式）
    /// </summary>
    public class CreateColumnFormatProductInformationDto
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// 产品（产品名称从这里取）（字典明细表主键）
        /// </summary>
        [Required]
        public virtual string Product { get; set; }

        /// <summary>
        /// 客户目标价（根据需求变化，新增的字段）
        /// </summary>
        [Required]
        public virtual decimal CustomerTargetPrice { get; set; }

        /// <summary>
        /// Sensor（品牌/型号）
        /// </summary>
        public virtual string Sensor { get; set; }

        /// <summary>
        /// Sensor类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string SensorTypeSelect { get; set; }

        /// <summary>
        /// Sensor单价
        /// </summary>
        public virtual decimal? SensorPrice { get; set; }

        /// <summary>
        /// Sensor币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long SensorCurrency { get; set; }

        /// <summary>
        /// Sensor汇率
        /// </summary>
        [Required]
        public virtual decimal SensorExchangeRate { get; set; }

        /// <summary>
        /// Lens
        /// </summary>
        public virtual string Lens { get; set; }

        /// <summary>
        /// Lens类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string LensTypeSelect { get; set; }

        /// <summary>
        /// Lens单价
        /// </summary>
        public virtual decimal? LensPrice { get; set; }

        /// <summary>
        /// Lens币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long LensCurrency { get; set; }

        /// <summary>
        /// Lens汇率
        /// </summary>
        [Required]
        public virtual decimal LensExchangeRate { get; set; }

        /// <summary>
        /// Isp
        /// </summary>
        public virtual string Isp { get; set; }

        /// <summary>
        /// Isp类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string IspTypeSelect { get; set; }

        /// <summary>
        /// Isp单价
        /// </summary>
        public virtual decimal? IspPrice { get; set; }

        /// <summary>
        /// Isp币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long IspCurrency { get; set; }

        /// <summary>
        /// Isp汇率
        /// </summary>
        [Required]
        public virtual decimal IspExchangeRate { get; set; }

        /// <summary>
        /// 串行芯片
        /// </summary>
        public virtual string SerialChip { get; set; }

        /// <summary>
        /// 串行芯片 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string SerialChipTypeSelect { get; set; }

        /// <summary>
        /// 串行芯片 单价
        /// </summary>
        public virtual decimal? SerialChipPrice { get; set; }

        /// <summary>
        /// 串行芯片币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long SerialChipCurrency { get; set; }

        /// <summary>
        /// 串行芯片汇率
        /// </summary>
        [Required]
        public virtual decimal SerialChipExchangeRate { get; set; }

        /// <summary>
        /// 线束
        /// </summary>
        public virtual string Cable { get; set; }

        /// <summary>
        /// 线束 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string CableTypeSelect { get; set; }

        /// <summary>
        /// 线束 单价
        /// </summary>
        public virtual decimal? CablePrice { get; set; }

        /// <summary>
        /// 线束币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long CableCurrency { get; set; }

        /// <summary>
        /// 线束汇率
        /// </summary>
        [Required]
        public virtual decimal CableExchangeRate { get; set; }

        ///// <summary>
        ///// LED
        ///// </summary>
        //public virtual string Led { get; set; }

        ///// <summary>
        ///// LED 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        ///// </summary>
        //public virtual string LedTypeSelect { get; set; }

        ///// <summary>
        ///// LED 单价
        ///// </summary>
        //public virtual decimal? LedPrice { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public virtual string Other { get; set; }

        /// <summary>
        /// 其他 类型选择（字典明细表主键，根据字典名，调用【FinanceDictionary/GetFinanceDictionaryAndDetailByName】取字典，字典名Name是【TypeSelect】）
        /// </summary>
        public virtual string OtherTypeSelect { get; set; }

        /// <summary>
        /// 其他 单价
        /// </summary>
        public virtual decimal? OtherPrice { get; set; }

        /// <summary>
        /// 其他币别（汇率录入表（ExchangeRate）主键）
        /// </summary>
        [Required]
        public virtual long OtherCurrency { get; set; }

        /// <summary>
        /// 其他汇率
        /// </summary>
        [Required]
        public virtual decimal OtherExchangeRate { get; set; }

        /// <summary>
        /// 制程
        /// </summary>
        public virtual string ManufactureProcess { get; set; }

        /// <summary>
        /// 安装位置
        /// </summary>
        public virtual string InstallationPosition { get; set; }
    }
}
