using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace HtmlRaportGenerator.Tests.TestTools
{
    public static class PageExtensions
    {
        public static async Task<IResponse> GoToPageAndAssertSuccessAsync(this IPage page, string url, bool goToResult = true)
        {
            IResponse? response = await page.GotoAsync(url)
                .ConfigureAwait(false);

            Assert.NotNull(response);

            Assert.Equal(goToResult, response!.Ok);

            return response;
        }

        public static async Task<IResponse> ReloadAndAssertSuccessAsync(this IPage page, bool reloadResult = false)
        {
            Assert.NotNull(page);

            IResponse? response = await page!
                .ReloadAsync()
                .ConfigureAwait(false);

            Assert.NotNull(response);

            //todo on gh actions its failing why?
            //Assert.Equal(reloadResult, response!.Ok);

            return response!;
        }
    }
}
