using AutoMapper;
using Finance.MakeOffers.AnalyseBoard.DTo;
using Finance.MakeOffers.AnalyseBoard.Model;
using Finance.Nre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.MakeOffers.AnalyseBoard.Method
{
    /// <summary>
    /// 
    /// </summary>
    public static class AnalysisBoardMapper
    {
        /// <summary>
        /// 对象映射
        /// </summary>
        /// <param name="configuration"></param>
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ProductBoardDtoOrEvery, DynamicUnitPrice>();
            configuration.CreateMap<InitialResourcesManagement, ExpensesStatementModel>();
            configuration.CreateMap<ProductBoardModel, DynamicUnitPriceOffers>()
                .ForMember(u => u.Id, options => options.Ignore());
            configuration.CreateMap<BiddingStrategy, BiddingStrategyModel>();
        }
    }
}
