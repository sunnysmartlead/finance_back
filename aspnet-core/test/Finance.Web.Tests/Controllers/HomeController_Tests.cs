using System.Threading.Tasks;
using Finance.Models.TokenAuth;
using Finance.Web.Controllers;
using Shouldly;
using Xunit;

namespace Finance.Web.Tests.Controllers
{
    public class HomeController_Tests : FinanceWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}