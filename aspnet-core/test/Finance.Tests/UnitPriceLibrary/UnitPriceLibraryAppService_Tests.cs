using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using Finance.PriceEval;
using Finance.PropertyDepartment.UnitPriceLibrary;
using Finance.PropertyDepartment.UnitPriceLibrary.Dto;
using Finance.PropertyDepartment.UnitPriceLibrary.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests.UnitPriceLibrary
{
    public class UnitPriceLibraryAppService_Tests : FinanceTestBase
    {
        private readonly UnitPriceLibraryAppService _unitPriceLibraryAppService;
        public UnitPriceLibraryAppService_Tests()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>().Begin();
            _unitPriceLibraryAppService = Resolve<UnitPriceLibraryAppService>();
        }

        [Fact]
        public async Task GetGainUInitPriceForm()
        {          

            PagedResultDto<UInitPriceFormModel> pagedResultDto = await _unitPriceLibraryAppService.GetGainUInitPriceForm(new QueryInteractionClass() { Filter = "", MaxResultCount = 20, SkipCount = 0 });
        

            await _unitPriceLibraryAppService.AddPublicMaterialWarehouse(new List<SharedMaterialWarehouseMode>() { new SharedMaterialWarehouseMode() { AssemblyQuantity=10, EntryName="测试", MaterialCode="测试", MaterialName="测试", ModuleThroughputs=null, ProjectSubcode="测试"} });

            await _unitPriceLibraryAppService.DeletePublicMaterials(1);

            await _unitPriceLibraryAppService.DeleteMultiplePublicMaterials(new List<long>() {1});

            await _unitPriceLibraryAppService.GetQueryPublicMaterialWarehouse(new QueryInteractionClass() { Filter = "", MaxResultCount = 20, SkipCount = 0 });

            await _unitPriceLibraryAppService.GetGrossMargin(new GrossMarginInputDto() {  GrossMarginName = "", MaxResultCount = 20, SkipCount = 0 });

            await _unitPriceLibraryAppService.PostGrossMargin(new BacktrackGrossMarginDto() { GrossMarginName="测试", GrossMarginPrice=null, IsDefaultn=true});

            await _unitPriceLibraryAppService.DeleteGrossMargin(1);

            await _unitPriceLibraryAppService.PostExchangeRate(new ExchangeRateDto() { ExchangeRateKind="测试", ExchangeRateValue=new List<PropertyDepartment.Entering.Model.YearOrValueMode>() { new PropertyDepartment.Entering.Model.YearOrValueMode() {  Year=2023, UpDown= YearType.Year, Value=10} } });

            await _unitPriceLibraryAppService.GetExchangeRate(new ExchangeRateInputDto() {  ExchangeRateKind = "", MaxResultCount = 20, SkipCount = 0 });

            await _unitPriceLibraryAppService.DeleteExchangeRate(1);
        }
    }
}
