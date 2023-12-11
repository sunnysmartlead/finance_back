using Finance.Debugging;

namespace Finance
{
    public class FinanceConsts
    {
        public const string LocalizationSourceName = "Finance";

        public const string ConnectionStringName = "Default";

        public const int Mail_Password_Date = 80;     //距离上一次修改密码多少天进行提醒        

        public const int SmtpPort_Sunny = 25;          //SMTP服务器端口
        public const string SmtpServer_Sunny = "mail.sunnyoptical.com"; //SMTP服务器
        public const string MailFrom_Sunny = "zlcwhjbjxt@sunnyoptical.com"; //登陆用户名，邮箱
        public const string MailUserPassword_Sunny = "mb395$BQ";//登录密码

        public const string MailForMaintainer = "wslinzhang@sunnyoptical.com";

        public const string AliServer_In_IP = "172.26.144.105"; //阿里云内网IP地址
        public const string AliServer_Out_IP = "139.196.216.165"; //阿里云外网IP地址
        public const string ShangHaiServerIP = "10.1.1.131"; //上海服务器IP地址

        public const int SmtpPort_Tencent = 587;    //SMTP服务器端口
        public const string SmtpServer_Tencent = "smtp.qq.com"; //SMTP服务器
        public const string MailFrom_Tencent = "274439023@qq.com"; //登陆用户名，邮箱
        public const string MailUserPassword_Tencent = "ynsmqsxldqdmcaid";//授权码

        public const bool MultiTenancyEnabled = false;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 1000;

        public const int MinYear = 1000;
        public const int MaxYear = 3000;
        public const string Sorting = "Id Desc";

        public const string SystemLogFileType = ".csv";

        public const string ElectronicName = "电子料";
        public const string StructuralName = "结构料";
        public const string GlueMaterialName = "胶水等辅材";
        public const string SMTOutSourceName = "SMT外协";
        public const string PackingMaterialName = "包材";

        public const string ProductTypeNameOther = "其他";


        public const string EccnCode_Eccn = "ECCN";
        public const string EccnCode_Ear99 = "EAR99";
        public const string EccnCode_Uninvolved = "不涉及";
        public const string EccnCode_Pending = "待定";

        public const int ExcelColumnWidth = 20;


        /// <summary>
        /// 部门PathId的生成规则
        /// </summary>
        public const string DepartmentPathIdRegular = "{0}.{1}";

        /// <summary>
        /// 部门PathName的生成规则
        /// </summary>
        public const string DepartmentPathNameRegular = $"{{0}}{DepartmentNameSeparator}{{1}}";


        /// <summary>
        /// 部门名称的正则表达式，用来验证是否包含/
        /// </summary>
        public const string DepartmentNameRegular = $"^((?!{DepartmentNameSeparator}).)*$";

        /// <summary>
        /// 部门Name的分隔符
        /// </summary>
        public const string DepartmentNameSeparator = "/";

        /// <summary>
        /// 字典名称的正则表达式，用来验证是否包含-
        /// </summary>
        public const string FinanceDictionaryNameRegular = $"^((?!{FinanceDictionaryNameSeparator}).)*$";

        /// <summary>
        /// 字典名的分隔符
        /// </summary>
        public const string FinanceDictionaryNameSeparator = "-";

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "f58393d400854e16be45a08409982c15";


        #region 核价需求录入
        /// <summary>
        /// 客户性质（字典表的Name）
        /// </summary>
        public const string CustomerNature = "CustomerNature";

        /// <summary>
        /// 客户国家（字典表的Name）
        /// </summary>
        public const string Country = "Country";

        /// <summary>
        /// 客户国家之伊朗（字典明细表的Name）
        /// </summary>
        public const string Country_Iran = "Country_Iran";

        /// <summary>
        /// 客户国家之朝鲜（字典明细表的Name）
        /// </summary>
        public const string Country_NorthKorea = "Country_NorthKorea";

        /// <summary>
        /// 客户国家之叙利亚（字典明细表的Name）
        /// </summary>
        public const string Country_Syria = "Country_Syria";

        /// <summary>
        /// 客户国家之古巴（字典明细表的Name）
        /// </summary>
        public const string Country_Cuba = "Country_Cuba";

        /// <summary>
        /// 客户国家之其他国家（字典明细表的Name）
        /// </summary>
        public const string Country_Other = "Country_Other";

        /// <summary>
        /// 车厂（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_CarFactory = "CustomerNature_CarFactory";

        /// <summary>
        /// 传统OEM（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_OEM = "CustomerNature_OEM";

        /// <summary>
        /// 造车新势力（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_NewForce = "CustomerNature_NewForce";

        /// <summary>
        /// Tier1（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Tier1 = "CustomerNature_Tier1";

        /// <summary>
        /// 方案商（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_SolutionProvider = "CustomerNature_SolutionProvider";

        /// <summary>
        /// 平台（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Platform = "CustomerNature_Platform";

        /// <summary>
        /// 算法（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Algorithm = "CustomerNature_Algorithm";

        /// <summary>
        /// 代理（字典明细表的Name）
        /// </summary>
        public const string CustomerNature_Agent = "CustomerNature_Agent";


        /// <summary>
        /// 终端性质（字典表的Name）
        /// </summary>
        public const string TerminalNature = "TerminalNature";

        /// <summary>
        /// 终端性质之车厂（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_CarFactory = "TerminalNature_CarFactory";

        /// <summary>
        /// 终端性质之传统OEM（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_OEM = "TerminalNature_OEM";

        /// <summary>
        /// 终端性质之造车新势力（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_NewForce = "TerminalNature_NewForce";

        /// <summary>
        /// 终端性质之Tier1（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_Tier1 = "TerminalNature_Tier1";

        /// <summary>
        /// 终端性质之Other（字典明细表的Name）
        /// </summary>
        public const string TerminalNature_Other = "TerminalNature_Other";

        /// <summary>
        /// 报价形式（字典表的Name）
        /// </summary>
        public const string QuotationType = "QuotationType";

        /// <summary>
        /// 报价形式之量产品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_ProductForMassProduction = "QuotationType_ProductForMassProduction";

        /// <summary>
        /// 报价形式之样品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_Sample = "QuotationType_Sample";

        /// <summary>
        /// 报价形式之定制化样品报价（字典明细表的Name）
        /// </summary>
        public const string QuotationType_CustomizationSample = "QuotationType_CustomizationSample";

        /// <summary>
        /// 样品报价类型（字典表的Name）
        /// </summary>
        public const string SampleQuotationType = "SampleQuotationType";

        /// <summary>
        /// 样品报价类型之现有样品（字典明细表的Name）
        /// </summary>
        public const string SampleQuotationType_Existing = "SampleQuotationType_Existing";

        /// <summary>
        /// 样品报价类型之定制化样品（字典明细表的Name）
        /// </summary>
        public const string SampleQuotationType_Customization = "SampleQuotationType_Customization";

        /// <summary>
        /// 价格有效期
        /// </summary>
        public const string UpdateFrequency = "UpdateFrequency";

        /// <summary>
        /// 报价更新频率之年度
        /// </summary>
        public const string UpdateFrequency_Year = "UpdateFrequency_Year";

        /// <summary>
        /// 报价更新频率之半年度
        /// </summary>
        public const string UpdateFrequency_HalfYear = "UpdateFrequency_HalfYear";

        /// <summary>
        /// 核价类型
        /// </summary>
        public const string PriceEvalType = "PriceEvalType";

        /// <summary>
        /// 核价类型之量产品核价
        /// </summary>
        public const string PriceEvalType_Quantity = "PriceEvalType_Quantity";

        /// <summary>
        /// 核价类型之样品核价
        /// </summary>
        public const string PriceEvalType_Sample = "PriceEvalType_Sample";

        /// <summary>
        /// 样品阶段名称
        /// </summary>
        public const string SampleName = "SampleName";

        /// <summary>
        /// 样品阶段名称:A样
        /// </summary>
        public const string SampleName_A = "SampleName_A";

        /// <summary>
        /// 样品阶段名称:B样
        /// </summary>
        public const string SampleName_B = "SampleName_B";

        /// <summary>
        /// 样品阶段名称:C样
        /// </summary>
        public const string SampleName_C = "SampleName_C";

        /// <summary>
        /// 样品阶段名称:Other样
        /// </summary>
        public const string SampleName_Other = "SampleName_Other";

        /// <summary>
        /// 产品（字典表的Name）
        /// </summary>
        public const string Product = "Product";

        /// <summary>
        /// 产品大类（字典表的Name）
        /// </summary>
        public const string ProductType = "ProductType";

        /// <summary>
        /// 产品大类之外摄显像（字典明细表的Name）
        /// </summary>
        public const string ProductType_ExternalImaging = "ProductType_ExternalImaging";

        /// <summary>
        /// 产品大类之环境感知（字典明细表的Name）
        /// </summary>
        public const string ProductType_EnvironmentalPerception = "ProductType_EnvironmentalPerception";

        /// <summary>
        /// 产品大类之舱内监测（字典明细表的Name）
        /// </summary>
        public const string ProductType_CabinMonitoring = "ProductType_CabinMonitoring";

        /// <summary>
        /// 产品大类之显像感知（字典明细表的Name）
        /// </summary>
        public const string ProductType_Imaging = "ProductType_Imaging";

        /// <summary>
        /// 产品大类之软硬件业务（字典明细表的Name）
        /// </summary>
        public const string ProductType_SoftHard = "ProductType_SoftHard";

        /// <summary>
        /// 产品大类之其他（字典明细表的Name）
        /// </summary>
        public const string ProductType_Other = "ProductType_Other";

        /// <summary>
        /// 模具费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfMouldCost = "AllocationOfMouldCost";

        /// <summary>
        /// 治具费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfFixtureCost = "AllocationOfFixtureCost";

        /// <summary>
        /// 设备费分摊（字典表的Name）
        /// </summary>
        public const string AllocationOfEquipmentCost = "AllocationOfEquipmentCost";

        /// <summary>
        /// 信赖性费用分摊（字典表的Name）
        /// </summary>
        public const string ReliabilityCost = "ReliabilityCost";


        /// <summary>
        /// 开发费分摊（字典表的Name）
        /// </summary>
        public const string DevelopmentCost = "DevelopmentCost";

        /// <summary>
        /// 落地工厂（字典表的Name）
        /// </summary>
        public const string LandingFactory = "LandingFactory";


        /// <summary>
        /// 落地工厂之舜宇智领（字典明细表的Name）
        /// </summary>
        public const string LandingFactory_SunnySmartLead = "LandingFactory_SunnySmartLead";

        #region 类型选择
        /// <summary>
        /// 类型选择（字典表的Name）
        /// </summary>
        public const string TypeSelect = "TypeSelect";

        /// <summary>
        /// 类型选择之我司推荐（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Recommend = "TypeSelect_Recommend";

        /// <summary>
        /// 类型选择之客户指定（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Appoint = "TypeSelect_Appoint";

        /// <summary>
        /// 类型选择之客户供应（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Supply = "TypeSelect_Supply";

        /// <summary>
        /// 类型选择之客户要求（字典明细表的Name）
        /// </summary>
        public const string TypeSelect_Ask = "TypeSelect_Ask";
        #endregion

        /// <summary>
        /// 销售类型（字典表的Name）
        /// </summary>
        public const string SalesType = "SalesType";

        /// <summary>
        /// 销售类型之内销（字典明细表的Name）
        /// </summary>
        public const string SalesType_ForTheDomesticMarket = "SalesType_ForTheDomesticMarket";

        /// <summary>
        /// 销售类型之外销（字典明细表的Name）
        /// </summary>
        public const string SalesType_ForExport = "SalesType_ForExport";

        /// <summary>
        /// 报价币种（字典表的Name）
        /// </summary>
        public const string Currency = "Currency";

        /// <summary>
        /// 运输方式（字典表的Name）
        /// </summary>
        public const string ShippingType = "ShippingType";

        /// <summary>
        /// 运输方式之陆运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_LandTransportation = "ShippingType_LandTransportation";

        /// <summary>
        /// 运输方式之海运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_OceanShipping = "ShippingType_OceanShipping";

        /// <summary>
        /// 运输方式之空运（字典明细表的Name）
        /// </summary>
        public const string ShippingType_AirTransport = "ShippingType_AirTransport";

        /// <summary>
        /// 运输方式之手提取货（字典明细表的Name）
        /// </summary>
        public const string ShippingType_HandPick = "ShippingType_HandPick";


        /// <summary>
        /// 包装方式（字典表的Name）
        /// </summary>
        public const string PackagingType = "PackagingType";

        /// <summary>
        /// 包装方式之周转箱（字典明细表的Name）
        /// </summary>
        public const string PackagingType_TurnoverBox = "PackagingType_TurnoverBox";

        /// <summary>
        /// 包装方式之一次性纸箱（字典明细表的Name）
        /// </summary>
        public const string PackagingType_DisposableCarton = "PackagingType_DisposableCarton";

        /// <summary>
        /// 包装方式之客户无特殊要求（字典明细表的Name）
        /// </summary>
        public const string PackagingType_NoSpecialRequirements = "PackagingType_NoSpecialRequirements";

        #endregion
        //Nre 核价 项目管理部  下的 差旅费 的事由下拉
        #region
        /// <summary>
        /// NRE字典表  事由
        /// </summary>
        public const string NreReasons = "NreReasons";
        /// <summary>
        /// 字典明细表事由->车厂跟线
        /// </summary>
        public const string NreReasons_DepotWithLine = "NreReaso1ns_DepotWithLine";
        /// <summary>
        /// 字典明细表事由->项目交流
        /// </summary>
        public const string NreReasons_ProjectCommunication = "NreReaso1ns_ProjectCommunication";
        /// <summary>
        /// 字典明细表事由->客户端交流
        /// </summary>
        public const string NreReasons_ClientCommunication = "NreReasons_ClientCommunication";
        /// <summary>
        /// 字典明细表事由->其他
        /// </summary>
        public const string NreReasons_Else = "NreReaso1ns_Else";

        /// <summary>
        /// 贸易方式
        /// </summary>
        public const string TradeMethod = "TradeMethod";

        /// <summary>
        /// 贸易方式-EXW
        /// </summary>
        public const string TradeMethodEXW = "TradeMethodEXW";

        /// <summary>
        /// 贸易方式-FCA
        /// </summary>
        public const string TradeMethodFCA = "TradeMethodFCA";

        /// <summary>
        /// 贸易方式-FAS
        /// </summary>
        public const string TradeMethodFAS = "TradeMethodFAS";

        /// <summary>
        /// 贸易方式-FOB
        /// </summary>
        public const string TradeMethodFOB = "TradeMethodFOB";

        /// <summary>
        /// 贸易方式-CFR
        /// </summary>
        public const string TradeMethodCFR = "TradeMethodCFR";

        /// <summary>
        /// 贸易方式-CIF
        /// </summary>
        public const string TradeMethodCIF = "TradeMethodCIF";

        /// <summary>
        /// 贸易方式-CPT
        /// </summary>
        public const string TradeMethodCPT = "TradeMethodCPT";

        /// <summary>
        /// 贸易方式-CIP
        /// </summary>
        public const string TradeMethodCIP = "TradeMethodCIP";

        /// <summary>
        /// 贸易方式-DAF
        /// </summary>
        public const string TradeMethodDAF = "TradeMethodDAF";

        /// <summary>
        /// 贸易方式-DES
        /// </summary>
        public const string TradeMethodDES = "TradeMethodDES";

        /// <summary>
        /// 贸易方式-DEQ
        /// </summary>
        public const string TradeMethodDEQ = "TradeMethodDEQ";

        /// <summary>
        /// 贸易方式-DDU
        /// </summary>
        public const string TradeMethodDDU = "TradeMethodDDU";

        /// <summary>
        /// 贸易方式-DDP
        /// </summary>
        public const string TradeMethodDDP = "TradeMethodDDP";


        #endregion


        #region 工作流相关
        public const string YesOrNo = "YesOrNo";
        public const string YesOrNo_Yes = "YesOrNo_Yes";
        public const string YesOrNo_No = "YesOrNo_No";
        public const string YesOrNo_Save = "YesOrNo_Save";

        public const string Done = "Done";

        public const string Save = "Save";


        /// <summary>
        /// 核价原因
        /// </summary>
        public const string EvalReason = "EvalReason";

        /// <summary>
        /// 样品
        /// </summary>
        public const string EvalReason_Yp = "EvalReason_Yp";

        /// <summary>
        /// 非方案变更
        /// </summary>
        public const string EvalReason_Ffabg = "EvalReason_Ffabg";

        /// <summary>
        /// 年降
        /// </summary>
        public const string EvalReason_Nj = "EvalReason_Nj";

        /// <summary>
        /// 售后件
        /// </summary>
        public const string EvalReason_Shj = "EvalReason_Shj";

        /// <summary>
        /// 版本1
        /// </summary>
        public const string EvalReason_Bb1 = "EvalReason_Bb1";

        /// <summary>
        /// 方案变更
        /// </summary>
        public const string EvalReason_Fabg = "EvalReason_Fabg";

        /// <summary>
        /// 其他
        /// </summary>
        public const string EvalReason_Qt = "EvalReason_Qt";

        /// <summary>
        /// 节点成本预估
        /// </summary>
        public const string EvalReason_Jdcbpg = "EvalReason_Jdcbpg";

        /// <summary>
        /// 项目变更
        /// </summary>
        public const string EvalReason_Xmbg = "EvalReason_Xmbg";

        /// <summary>
        /// 其他引用流程
        /// </summary>
        public const string EvalReason_Qtyylc = "EvalReason_Qtyylc";

        /// <summary>
        /// 本年年降
        /// </summary>
        public const string EvalReason_Bnnj = "EvalReason_Bnnj";

        /// <summary>
        /// 推广样品
        /// </summary>
        public const string EvalReason_Tgyp = "EvalReason_Tgyp";

        /// <summary>
        /// 首次核价
        /// </summary>
        public const string EvalReason_Schj = "EvalReason_Schj";

        /// <summary>
        /// 量产样品
        /// </summary>
        public const string EvalReason_Lcyp = "EvalReason_Lcyp";

        /// <summary>
        /// 下年年降
        /// </summary>
        public const string EvalReason_Xnnj = "EvalReason_Xnnj";

        /// <summary>
        /// 其他上传流程
        /// </summary>
        public const string EvalReason_Qtsclc = "EvalReason_Qtsclc";

        /// <summary>
        /// 其他零星报价
        /// </summary>
        public const string EvalReason_Qtlxbj = "EvalReason_Qtlxbj";


        /// <summary>
        /// 报价反馈
        /// </summary>
        public const string EvalFeedback = "EvalFeedback";

        /// <summary>
        /// 报价反馈 接受报价
        /// </summary>
        public const string EvalFeedback_Js = "EvalFeedback_Js";

        /// <summary>
        /// 报价反馈 不接受此此价，不用再次报价/重新核价
        /// </summary>
        public const string EvalFeedback_Bjsbzc = "EvalFeedback_Bjsbzc";

        /// <summary>
        /// 报价反馈 不接受此价，但接受降价，不用重新核价
        /// </summary>
        public const string EvalFeedback_Bjsdjsjj = "EvalFeedback_Bjsdjsjj";

        /// <summary>
        /// 报价反馈 报价金额小于审批金额
        /// </summary>
        public const string EvalFeedback_Bjxysp = "EvalFeedback_Bjxysp";

        /// <summary>
        /// BOM成本审核名称
        /// </summary>
        public const string Bomcbsh = "BOM成本审核";

        /// <summary>
        /// 结构BOM单价审核选项
        /// </summary>
        public const string StructBomEvalSelect = "StructBomEvalSelect";

        /// <summary>
        /// 结构BOM单价审核选项 同意
        /// </summary>
        public const string StructBomEvalSelect_Yes = "StructBomEvalSelect_Yes";

        ///// <summary>
        ///// 结构BOM单价审核选项 退回到【上传结构BOM】
        ///// </summary>
        //public const string StructBomEvalSelect_Scjgbom = "StructBomEvalSelect_Scjgbom";

        /// <summary>
        /// 结构BOM单价审核选项 退回到【定制结构件】
        /// </summary>
        public const string StructBomEvalSelect_Dzjgj = "StructBomEvalSelect_Dzjgj";

        /// <summary>
        /// 结构BOM单价审核选项 退回到【结构BOM匹配修改】
        /// </summary>
        public const string StructBomEvalSelect_Jgbomppxg = "StructBomEvalSelect_Jgbomppxg";


        /// <summary>
        /// 电子BOM单价审核选项
        /// </summary>
        public const string ElectronicBomEvalSelect = "ElectronicBomEvalSelect";

        /// <summary>
        /// 电子BOM单价审核选项 同意
        /// </summary>
        public const string ElectronicBomEvalSelect_Yes = "ElectronicBomEvalSelect_Yes";

        ///// <summary>
        ///// 电子BOM单价审核选项 退回到【上传结构BOM】
        ///// </summary>
        //public const string ElectronicBomEvalSelect_Scjgbom = "ElectronicBomEvalSelect_Scjgbom";

        /// <summary>
        /// 电子BOM单价审核选项 退回到【电子BOM匹配修改】
        /// </summary>
        public const string ElectronicBomEvalSelect_Dzbomppxg = "ElectronicBomEvalSelect_Dzbomppxg";


        /// <summary>
        /// BOM成本审核选项
        /// </summary>
        public const string BomEvalSelect = "BomEvalSelect";

        /// <summary>
        /// BOM成本审核选项 同意
        /// </summary>
        public const string BomEvalSelect_Yes = "BomEvalSelect_Yes";

        ///// <summary>
        ///// BOM成本审核选项 退回到【上传结构BOM】
        ///// </summary>
        //public const string BomEvalSelect_Scjgbom = "BomEvalSelect_Scjgbom";

        /// <summary>
        /// BOM成本审核选项 退回到【定制结构件】
        /// </summary>
        public const string BomEvalSelect_Dzjgj = "BomEvalSelect_Dzjgj";

        /// <summary>
        /// BOM成本审核选项 退回到【结构BOM匹配修改】
        /// </summary>
        public const string BomEvalSelect_Jgbomppxg = "BomEvalSelect_Jgbomppxg";

        ///// <summary>
        ///// BOM成本审核选项 退回到【上传电子BOM】
        ///// </summary>
        //public const string BomEvalSelect_Scdzbom = "BomEvalSelect_Scdzbom";

        /// <summary>
        /// BOM成本审核选项 退回到【电子BOM匹配修改】
        /// </summary>
        public const string BomEvalSelect_Dzbomppxg = "BomEvalSelect_Dzbomppxg";


        /// <summary>
        /// 贸易不合规选项
        /// </summary>
        public const string MybhgSelect = "MybhgSelect";

        /// <summary>
        /// 贸易不合规选项 【不退回】
        /// </summary>
        public const string MybhgSelect_No = "MybhgSelect_No";

        /// <summary>
        /// 贸易不合规选项 退回到【上传结构BOM】
        /// </summary>
        public const string MybhgSelect_Scjgbom = "MybhgSelect_Scjgbom";

        /// <summary>
        /// 贸易不合规选项 退回到【定制结构件】
        /// </summary>
        public const string MybhgSelect_Dzjgj = "MybhgSelect_Dzjgj";

        /// <summary>
        /// 贸易不合规选项 退回到【结构BOM匹配修改】
        /// </summary>
        public const string MybhgSelect_Jgbomppxg = "MybhgSelect_Jgbomppxg";

        /// <summary>
        /// 贸易不合规选项 退回到【上传电子BOM】
        /// </summary>
        public const string MybhgSelect_Scdzbom = "MybhgSelect_Scdzbom";

        /// <summary>
        /// 贸易不合规选项 退回到【电子BOM匹配修改】
        /// </summary>
        public const string MybhgSelect_Dzbomppxg = "MybhgSelect_Dzbomppxg";

        /// <summary>
        /// 贸易不合规选项 退回到【物流成本录入】
        /// </summary>
        public const string MybhgSelect_Wlcblr = "MybhgSelect_Wlcblr";

        /// <summary>
        /// 贸易不合规选项 退回到【工序工时添加】
        /// </summary>
        public const string MybhgSelect_Gsgxtj = "MybhgSelect_Gsgxtj";

        /// <summary>
        /// 贸易不合规选项 退回到【COB制造成本录入】
        /// </summary>
        public const string MybhgSelect_Cobzzcblr = "MybhgSelect_Cobzzcblr";

        /// <summary>
        /// 贸易不合规选项 退回到【NRE模具费录入】
        /// </summary>
        public const string MybhgSelect_Nremjflr = "MybhgSelect_Nremjflr";

        /// <summary>
        /// 贸易不合规选项 退回到【NRE-可靠性实验费录入】
        /// </summary>
        public const string MybhgSelect_Nrekkxsyflr = "MybhgSelect_Nrekkxsyflr";

        /// <summary>
        /// 贸易不合规选项 退回到【NRE手板件】
        /// </summary>
        public const string MybhgSelect_Nresbj = "MybhgSelect_Nresbj";

        /// <summary>
        /// 贸易不合规选项 退回到【NRE-EMC实验费录入】
        /// </summary>
        public const string MybhgSelect_Nreemcsyflr = "MybhgSelect_Nreemcsyflr";


        /// <summary>
        /// 核价看板选项
        /// </summary>
        public const string HjkbSelect = "HjkbSelect";


        /// <summary>
        /// 核价看板选项 【同意】
        /// </summary>
        public const string HjkbSelect_Yes = "HjkbSelect_Yes";

        /// <summary>
        /// 核价看板选项 【不合规是否退回】
        /// </summary>
        public const string HjkbSelect_Bhg = "HjkbSelect_Bhg";

        /// <summary>
        /// 核价看板选项 【退回到核价需求录入】
        /// </summary>
        public const string HjkbSelect_Input = "HjkbSelect_Input";

        /// <summary>
        /// 核价看板选项 退回到【上传结构BOM】
        /// </summary>
        public const string HjkbSelect_Scjgbom = "HjkbSelect_Scjgbom";

        /// <summary>
        /// 核价看板选项 退回到【定制结构件】
        /// </summary>
        public const string HjkbSelect_Dzjgj = "HjkbSelect_Dzjgj";

        /// <summary>
        /// 核价看板选项 退回到【结构BOM匹配修改】
        /// </summary>
        public const string HjkbSelect_Jgbomppxg = "HjkbSelect_Jgbomppxg";

        /// <summary>
        /// 核价看板选项 退回到【上传电子BOM】
        /// </summary>
        public const string HjkbSelect_Scdzbom = "HjkbSelect_Scdzbom";

        /// <summary>
        /// 核价看板选项 退回到【电子BOM匹配修改】
        /// </summary>
        public const string HjkbSelect_Dzbomppxg = "HjkbSelect_Dzbomppxg";

        /// <summary>
        /// 核价看板选项 退回到【物流成本录入】
        /// </summary>
        public const string HjkbSelect_Wlcblr = "HjkbSelect_Wlcblr";

        /// <summary>
        /// 核价看板选项 退回到【工序工时添加】
        /// </summary>
        public const string HjkbSelect_Gsgxtj = "HjkbSelect_Gsgxtj";

        /// <summary>
        /// 核价看板选项 退回到【COB制造成本录入】
        /// </summary>
        public const string HjkbSelect_Cobzzcblr = "HjkbSelect_Cobzzcblr";

        /// <summary>
        /// 核价看板选项 退回到【NRE模具费录入】
        /// </summary>
        public const string HjkbSelect_Nremjflr = "HjkbSelect_Nremjflr";

        /// <summary>
        /// 核价看板选项 退回到【NRE-可靠性实验费录入】
        /// </summary>
        public const string HjkbSelect_Nrekkxsyflr = "HjkbSelect_Nrekkxsyflr";

        /// <summary>
        /// 核价看板选项 退回到【NRE手板件】
        /// </summary>
        public const string HjkbSelect_Nresbj = "HjkbSelect_Nresbj";

        /// <summary>
        /// 核价看板选项 退回到【NRE-EMC实验费录入】
        /// </summary>
        public const string HjkbSelect_Nreemcsyflr = "HjkbSelect_Nreemcsyflr";

        /// <summary>
        /// 核价看板选项 退回到【结构BOM单价审核】
        /// </summary>
        public const string HjkbSelect_Jgbomdjsh = "HjkbSelect_Jgbomdjsh";

        /// <summary>
        /// 核价看板选项 退回到【电子BOM单价审核】
        /// </summary>
        public const string HjkbSelect_Dzbomdjsh = "HjkbSelect_Dzbomdjsh";

        /// <summary>
        /// 核价看板选项 退回到【核价审批录入】
        /// </summary>
        public const string HjkbSelect_Hjsplr = "HjkbSelect_Hjsplr";

        #endregion


        #region 页面标识符

        /// <summary>
        /// NRE_可靠性实验费录入
        /// </summary>
        public const string NRE_ReliabilityExperimentFeeInput = "NRE_ReliabilityExperimentFeeInput";

        /// <summary>
        /// 上传电子BOM 
        /// </summary>
        public const string ElectronicsBOM = "ElectronicsBOM";

        /// <summary>
        /// 上传结构BOM
        /// </summary>
        public const string StructureBOM = "StructureBOM";

        /// <summary>
        /// 工序工时添加
        /// </summary>
        public const string FormulaOperationAddition = "FormulaOperationAddition";

        /// <summary>
        /// 贸易合规
        /// </summary>
        public const string TradeCompliance = "TradeCompliance";

        /// <summary>
        /// NRE_EMC实验费录入
        /// </summary>
        public const string NRE_EMCExperimentalFeeInput = "NRE_EMCExperimentalFeeInput";

        /// <summary>
        /// COB制造成本录入
        /// </summary>
        public const string COBManufacturingCostEntry = "COBManufacturingCostEntry";

        /// <summary>
        /// 物流成本录入
        /// </summary>
        public const string LogisticsCostEntry = "LogisticsCostEntry";


        /// <summary>
        /// 项目核价审核
        /// </summary>
        public const string ProjectChiefAudit = "ProjectChiefAudit";

        /// <summary>
        /// 核价审批录入
        /// </summary>
        public const string PriceDemandReview = "PriceDemandReview";

        /// <summary>
        /// NRE手板件
        /// </summary>
        public const string NRE_ManualComponentInput = "NRE_ManualComponentInput";

        /// <summary>
        /// 查看每个方案初版BOM成本
        /// </summary>
        public const string UnitPriceInputReviewToExamine = "UnitPriceInputReviewToExamine";

        /// <summary>
        /// 核价看板
        /// </summary>
        public const string PriceEvaluationBoard = "PriceEvaluationBoard";


        /// <summary>
        /// 生成报价分析界面选择报价方案
        /// </summary>
        public const string QuoteAnalysis = "QuoteAnalysis";

        /// <summary>
        /// 核价需求录入
        /// </summary>
        public const string PricingDemandInput = "PricingDemandInput";

        #endregion


        public const string Sbzt = "Sbzt";
        public const string Sbzt_Zy = "Sbzt_Zy";
        public const string Sbzt_Gy = "Sbzt_Gy";
        public const string Sbzt_Xy = "Sbzt_Xy";
        public const string Sbzt_Xg = "Sbzt_Xg";
        public const string Sbzt_Gz = "Sbzt_Gz";


        /// <summary>
        /// 质量成本比例
        /// </summary>
        public const string QualityCostType = "QualityCostType";

        /// <summary>
        /// 质量成本比例-其他
        /// </summary>
        public const string QualityCostType_Qt = "QualityCostType_Qt";

        /// <summary>
        /// 质量成本比例-软硬件业务
        /// </summary>
        public const string QualityCostType_Ryjyw = "QualityCostType_Ryjyw";

        /// <summary>
        /// 质量成本比例-显像感知
        /// </summary>
        public const string QualityCostType_Xxgz = "QualityCostType_Xxgz";

        /// <summary>
        /// 质量成本比例-舱内监测
        /// </summary>
        public const string QualityCostType_Cnjc = "QualityCostType_Cnjc";

        /// <summary>
        /// 质量成本比例-环境感知
        /// </summary>
        public const string QualityCostType_Hjgz = "QualityCostType_Hjgz";

        /// <summary>
        /// 质量成本比例-外摄显像
        /// </summary>
        public const string QualityCostType_Wsxx = "QualityCostType_Wsxx";


        /// <summary>
        /// 审批报价策略与核价表选项
        /// </summary>
        public const string Spbjclyhjb = "Spbjclyhjb";

        /// <summary>
        /// 审批报价策略与核价表选项-【同意】
        /// </summary>
        public const string Spbjclyhjb_Yes = "Spbjclyhjb_Yes";

        /// <summary>
        /// 审批报价策略与核价表选项-【退回到核价看板】
        /// </summary>
        public const string Spbjclyhjb_Hjkb = "Spbjclyhjb_Hjkb";

        /// <summary>
        /// 审批报价策略与核价表选项-【退回到报价分析看板界面】
        /// </summary>
        public const string Spbjclyhjb_Bjfxkb = "Spbjclyhjb_Bjfxkb";

    }
}
