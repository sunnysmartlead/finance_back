using Abp.Domain.Uow;
using Finance.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.EntityFrameworkCore.Seed.Host
{
    public class DefaultFinanceDictionaryCreator
    {
        private readonly FinanceDbContext _context;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DefaultFinanceDictionaryCreator(FinanceDbContext context, IUnitOfWorkManager unitOfWorkManager)
        {
            _context = context;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void Create()
        {
            CreateEditions();
        }
        private void CreateEditions()
        {
            if (_unitOfWorkManager is not null)
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);
            }

            //仅把ChartPointCountName提取成配置，其他的字符串仅在程序中使用一次，不提取
            var financeDictionaryList = new List<FinanceDictionary>
            {
                new FinanceDictionary { Id=FinanceConsts.CustomerNature, DisplayName="客户类别",  },
                new FinanceDictionary { Id=FinanceConsts.Country, DisplayName="出口国家",  },

                new FinanceDictionary { Id=FinanceConsts.TerminalNature,DisplayName="终端性质",},
                new FinanceDictionary { Id=FinanceConsts.QuotationType,DisplayName="报价形式",},
                new FinanceDictionary { Id=FinanceConsts.SampleQuotationType,DisplayName="样品报价类型",},
                new FinanceDictionary { Id=FinanceConsts.Product,DisplayName="产品",},
                new FinanceDictionary { Id=FinanceConsts.ProductType,DisplayName="产品大类",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfMouldCost,DisplayName="模具费分摊",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfFixtureCost,DisplayName="治具费分摊",},
                new FinanceDictionary { Id=FinanceConsts.AllocationOfEquipmentCost,DisplayName="设备费分摊",},
                new FinanceDictionary { Id=FinanceConsts.ReliabilityCost,DisplayName="信赖性费用分摊",},
                new FinanceDictionary { Id=FinanceConsts.DevelopmentCost,DisplayName="开发费分摊",},
                new FinanceDictionary { Id=FinanceConsts.LandingFactory,DisplayName="落地工厂",},
                new FinanceDictionary { Id=FinanceConsts.TypeSelect,DisplayName="类型选择",},
                new FinanceDictionary { Id=FinanceConsts.SalesType,DisplayName="销售类型",},
                new FinanceDictionary { Id=FinanceConsts.Currency,DisplayName="报价币种",},
                new FinanceDictionary { Id=FinanceConsts.ShippingType,DisplayName="运输方式",},
                new FinanceDictionary { Id=FinanceConsts.PackagingType,DisplayName="包装方式",},
                 new FinanceDictionary { Id=FinanceConsts.NreReasons,DisplayName="NRE事由",},
                 new FinanceDictionary { Id=FinanceConsts.TradeMethod,DisplayName="贸易方式",},

                 new FinanceDictionary { Id=FinanceConsts.UpdateFrequency,DisplayName="价格有效期",},
                 new FinanceDictionary { Id=FinanceConsts.PriceEvalType,DisplayName="核价类型",},
                 new FinanceDictionary { Id=FinanceConsts.SampleName,DisplayName="样品阶段",},

                 new FinanceDictionary { Id=FinanceConsts.YesOrNo,DisplayName="同意|不同意",},
                 new FinanceDictionary { Id=FinanceConsts.EvalReason,DisplayName="核价原因",},
                 new FinanceDictionary { Id=FinanceConsts.Done,DisplayName="完成",},
                 new FinanceDictionary { Id=FinanceConsts.EvalFeedback,DisplayName="报价反馈",},

                 new FinanceDictionary { Id=FinanceConsts.StructBomEvalSelect,DisplayName="结构BOM单价审核选项",},
                 new FinanceDictionary { Id=FinanceConsts.BomEvalSelect,DisplayName="BOM成本审核选项",},
                 new FinanceDictionary { Id=FinanceConsts.MybhgSelect,DisplayName="贸易不合规选项",},
                 new FinanceDictionary { Id=FinanceConsts.HjkbSelect,DisplayName="核价看板选项",},
                 new FinanceDictionary { Id=FinanceConsts.ElectronicBomEvalSelect,DisplayName="电子BOM单价审核选项",},

                 new FinanceDictionary { Id=FinanceConsts.Sbzt,DisplayName="设备状态",},

                 new FinanceDictionary { Id=FinanceConsts.QualityCostType,DisplayName="质量成本比例",},

                 new FinanceDictionary { Id=FinanceConsts.Spbjclyhjb,DisplayName="审批报价策略与核价表选项",},


            };

            var noDb = financeDictionaryList.Where(p => !_context.FinanceDictionary.Contains(p));
            if (noDb.Any())
            {
                _context.FinanceDictionary.AddRange(noDb);
                _context.SaveChanges();
            }
            var isDeleted = _context.FinanceDictionary.Where(p => financeDictionaryList.Contains(p) && p.IsDeleted).ToList();
            if (isDeleted.Any())
            {
                isDeleted.ForEach(p => p.IsDeleted = false);
                _context.FinanceDictionary.UpdateRange(isDeleted);
                _context.SaveChanges();
            }

            //////////////////

            //仅把ChartPointCountName提取成配置，其他的字符串仅在程序中使用一次，不提取
            var financeDictionaryDetailList = new List<FinanceDictionaryDetail>
            {
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Recommend,DisplayName="我司推荐",  },
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Appoint,DisplayName="客户指定采购",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Supply,DisplayName="客户供应",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TypeSelect, Id=FinanceConsts.TypeSelect_Ask,DisplayName="客户要求",},

                //客户类别
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_CarFactory,DisplayName="车厂",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_OEM,DisplayName="传统OEM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_NewForce,DisplayName="造车新势力",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Tier1,DisplayName="Tier1",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_SolutionProvider,DisplayName="方案商",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Platform,DisplayName="平台",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Algorithm,DisplayName="算法",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.CustomerNature, Id=FinanceConsts.CustomerNature_Agent,DisplayName="代理",},

                //终端性质
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_OEM,DisplayName="传统OEM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_NewForce,DisplayName="造车新势力",},
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_CarFactory,DisplayName="车厂",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_Tier1,DisplayName="Tier1",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TerminalNature, Id=FinanceConsts.TerminalNature_Other,DisplayName="其他",},

                //报价形式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_ProductForMassProduction ,DisplayName="量产品报价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_Sample,DisplayName="现有样品报价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QuotationType, Id=FinanceConsts.QuotationType_CustomizationSample,DisplayName="定制化样品报价",},

                 //样品报价类型
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleQuotationType, Id=FinanceConsts.SampleQuotationType_Existing ,DisplayName="现有样品",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleQuotationType, Id=FinanceConsts.SampleQuotationType_Customization,DisplayName="定制化样品",},

                //价格有效期
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.UpdateFrequency, Id=FinanceConsts.UpdateFrequency_Year ,DisplayName="年度",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.UpdateFrequency, Id=FinanceConsts.UpdateFrequency_HalfYear ,DisplayName="半年度",},


                //落地工厂
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.LandingFactory, Id=FinanceConsts.LandingFactory_SunnySmartLead,DisplayName="舜宇智领",},

                //销售类型
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SalesType, Id=FinanceConsts.SalesType_ForTheDomesticMarket, DisplayName="内销",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SalesType, Id=FinanceConsts.SalesType_ForExport, DisplayName="外销",},

                //出口国家
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Iran, DisplayName="伊朗",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_NorthKorea, DisplayName="朝鲜",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Syria, DisplayName="叙利亚",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Cuba, DisplayName="古巴",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Country, Id=FinanceConsts.Country_Other, DisplayName="其他国家",},

                //运输方式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_LandTransportation, DisplayName="陆运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_OceanShipping, DisplayName="海运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_AirTransport, DisplayName="空运",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ShippingType, Id=FinanceConsts.ShippingType_HandPick, DisplayName="手提取货",},


                //包装方式 
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_TurnoverBox, DisplayName="周转箱",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_DisposableCarton, DisplayName="一次性纸箱",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PackagingType, Id=FinanceConsts.PackagingType_NoSpecialRequirements, DisplayName="客户无特殊要求",},

                //产品大类
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_ExternalImaging, DisplayName="外摄显像",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_EnvironmentalPerception, DisplayName="环境感知",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_CabinMonitoring, DisplayName="舱内监测",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_Imaging, DisplayName="显像感知",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_SoftHard, DisplayName="软硬件业务",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ProductType, Id=FinanceConsts.ProductType_Other, DisplayName="其他",},

                //NRE事由
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_DepotWithLine, DisplayName="车厂跟线",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_ProjectCommunication, DisplayName="项目交流",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_ClientCommunication, DisplayName="客户端交流",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.NreReasons, Id=FinanceConsts.NreReasons_Else, DisplayName="其他",},

                //贸易方式
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodEXW, DisplayName="EXW",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFCA, DisplayName="FCA",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFAS, DisplayName="FAS",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodFOB, DisplayName="FOB",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCFR, DisplayName="CFR",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCIF, DisplayName="CIF",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCPT, DisplayName="CPT",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodCIP, DisplayName="CIP",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDAF, DisplayName="DAF",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDES, DisplayName="DES",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDEQ, DisplayName="DEQ",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDDU, DisplayName="DDU",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.TradeMethod, Id=FinanceConsts.TradeMethodDDP, DisplayName="DDP",},

                //核价类型
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PriceEvalType, Id=FinanceConsts.PriceEvalType_Quantity, DisplayName="量产品核价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.PriceEvalType, Id=FinanceConsts.PriceEvalType_Sample, DisplayName="样品核价",},

                //样品阶段
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleName, Id=FinanceConsts.SampleName_A, DisplayName="A样", Order=400,},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleName, Id=FinanceConsts.SampleName_B, DisplayName="B样", Order=300, },
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleName, Id=FinanceConsts.SampleName_C, DisplayName="C样", Order=200,},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.SampleName, Id=FinanceConsts.SampleName_Other, DisplayName="其他", Order=100},

                //是否同意
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.YesOrNo, Id=FinanceConsts.YesOrNo_Yes, DisplayName="同意",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.YesOrNo, Id=FinanceConsts.YesOrNo_No, DisplayName="不同意",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.YesOrNo, Id=FinanceConsts.YesOrNo_Save, DisplayName="保存",},

                //核价原因
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Yp, DisplayName="样品",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Ffabg, DisplayName="非方案变更",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Nj, DisplayName="年降",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Shj, DisplayName="售后件",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Bb1, DisplayName="版本1",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Fabg, DisplayName="方案变更",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Qt, DisplayName="其他",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Jdcbpg, DisplayName="节点成本预估",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalReason, Id=FinanceConsts.EvalReason_Xmbg, DisplayName="项目变更",},

                //完成与否
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Done, Id = FinanceConsts.Done, DisplayName="完成",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Done, Id = FinanceConsts.Save, DisplayName="保存",},

                //报价反馈
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalFeedback, Id = FinanceConsts.EvalFeedback_Js, DisplayName="接受报价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalFeedback, Id = FinanceConsts.EvalFeedback_Bjsbzc, DisplayName="不接受此此价，不用再次报价/重新核价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalFeedback, Id = FinanceConsts.EvalFeedback_Bjsdjsjj, DisplayName="不接受此价，但接受降价，不用重新核价",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.EvalFeedback, Id = FinanceConsts.EvalFeedback_Bjxysp, DisplayName="报价金额小于审批金额",},

                //结构BOM单价审核选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.StructBomEvalSelect, Id = FinanceConsts.StructBomEvalSelect_Yes, DisplayName="同意",},
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.StructBomEvalSelect, Id = FinanceConsts.StructBomEvalSelect_Scjgbom, DisplayName="退回到【上传结构BOM】",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.StructBomEvalSelect, Id = FinanceConsts.StructBomEvalSelect_Dzjgj, DisplayName="退回到【定制结构件】",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.StructBomEvalSelect, Id = FinanceConsts.StructBomEvalSelect_Jgbomppxg, DisplayName="退回到【结构BOM匹配修改】",},

                //BOM成本审核选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Yes, DisplayName="同意",},
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Scjgbom, DisplayName="上传结构BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Dzjgj, DisplayName="定制结构件",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Jgbomppxg, DisplayName="结构BOM匹配修改",},
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Scdzbom, DisplayName="上传电子BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.BomEvalSelect, Id = FinanceConsts.BomEvalSelect_Dzbomppxg, DisplayName="电子BOM匹配修改",},

                //贸易不合规选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_No, DisplayName="不退回",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Scjgbom, DisplayName="上传结构BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Dzjgj, DisplayName="定制结构件",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Jgbomppxg, DisplayName="结构BOM匹配修改",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Scdzbom, DisplayName="上传电子BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Dzbomppxg, DisplayName="电子BOM匹配修改",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Wlcblr, DisplayName="物流成本录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Gsgxtj, DisplayName="工序工时添加",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.MybhgSelect, Id = FinanceConsts.MybhgSelect_Cobzzcblr, DisplayName="COB制造成本录入",},

                //核价看板选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Yes, DisplayName="同意",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Scjgbom, DisplayName="上传结构BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Dzjgj, DisplayName="定制结构件",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Jgbomppxg, DisplayName="结构BOM匹配修改",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Scdzbom, DisplayName="上传电子BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Dzbomppxg, DisplayName="电子BOM匹配修改",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Wlcblr, DisplayName="物流成本录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Gsgxtj, DisplayName="工序工时添加",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Cobzzcblr, DisplayName="COB制造成本录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Nremjflr, DisplayName="NRE模具费录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Nrekkxsyflr, DisplayName="NRE-可靠性实验费录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Nresbj, DisplayName="NRE手板件",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Nreemcsyflr, DisplayName="NRE-EMC实验费录入",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.HjkbSelect, Id = FinanceConsts.HjkbSelect_Input, DisplayName="核价需求录入",},


                //电子BOM单价审核选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ElectronicBomEvalSelect, Id = FinanceConsts.ElectronicBomEvalSelect_Yes, DisplayName="同意",},
                //new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ElectronicBomEvalSelect, Id = FinanceConsts.ElectronicBomEvalSelect_Scjgbom, DisplayName="上传结构BOM",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.ElectronicBomEvalSelect, Id = FinanceConsts.ElectronicBomEvalSelect_Dzbomppxg, DisplayName="电子BOM匹配修改",},
           
                //设备状态
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Sbzt, Id = FinanceConsts.Sbzt_Zy, DisplayName="专用",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Sbzt, Id = FinanceConsts.Sbzt_Gy, DisplayName="共用",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Sbzt, Id = FinanceConsts.Sbzt_Xy, DisplayName="现有",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Sbzt, Id = FinanceConsts.Sbzt_Xg, DisplayName="新购",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Sbzt, Id = FinanceConsts.Sbzt_Gz, DisplayName="改造",},

                //质量成本比例
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Qt, DisplayName="其他",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Ryjyw, DisplayName="软硬件业务",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Xxgz, DisplayName="显像感知",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Cnjc, DisplayName="舱内监测",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Hjgz, DisplayName="环境感知",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.QualityCostType, Id = FinanceConsts.QualityCostType_Wsxx, DisplayName="外摄显像",},
            
                //审批报价策略与核价表选项
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Spbjclyhjb, Id = FinanceConsts.Spbjclyhjb_Yes, DisplayName="同意",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Spbjclyhjb, Id = FinanceConsts.Spbjclyhjb_Hjkb, DisplayName="退回到核价看板",},
                new FinanceDictionaryDetail {FinanceDictionaryId = FinanceConsts.Spbjclyhjb, Id = FinanceConsts.Spbjclyhjb_Bjfxkb, DisplayName="退回到报价分析看板界面",},


            };

            var noDbDetail = financeDictionaryDetailList.Where(p => !_context.FinanceDictionaryDetail.Contains(p));
            if (noDbDetail.Any())
            {
                _context.FinanceDictionaryDetail.AddRange(noDbDetail);
                _context.SaveChanges();
            }
            var isDeletedDetail = _context.FinanceDictionaryDetail.Where(p => financeDictionaryDetailList.Contains(p) && p.IsDeleted).ToList();
            if (isDeletedDetail.Any())
            {
                isDeletedDetail.ForEach(p => p.IsDeleted = false);
                _context.FinanceDictionaryDetail.UpdateRange(isDeletedDetail);
                _context.SaveChanges();
            }
        }
    }
}
