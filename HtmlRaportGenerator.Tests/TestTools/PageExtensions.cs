using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace HtmlRaportGenerator.Tests.TestTools
{
    public static class PageExtensions
    {
        public static async Task<IResponse> GoToPageAndAssertSuccessAsync(this IPage page, string url)
        {
            IResponse? response = await page.GotoAsync(url)
                .ConfigureAwait(false);

            Assert.NotNull(response);

            Assert.True(response!.Ok);

            return response;
        }

        public static async Task<IResponse> ReloadAndAssertSuccessAsync(this IPage page)
        {
            Assert.NotNull(page);

            IResponse? response = await page!
                .ReloadAsync()
                .ConfigureAwait(false);

            Assert.NotNull(response);

            Assert.True(response!.Ok);

            return response;
        }
    }
}
