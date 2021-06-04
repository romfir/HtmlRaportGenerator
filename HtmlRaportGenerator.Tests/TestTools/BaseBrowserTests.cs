using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace HtmlRaportGenerator.Tests.TestTools
{
    public abstract class BaseBrowserTests : IAsyncDisposable
    {
        protected const float ActionTimeOut = 1000;

        protected const int FirstLoadTimeOut = 12000;

        protected const string BaseUrl = "https://localhost:5001";

        protected const string MonthEditUrl = BaseUrl + "/MonthEdit";

        private readonly IPlaywright _playwright;
        protected readonly IBrowser Browser;
        protected readonly IPage Page;

        protected BaseBrowserTests(bool headless = true)
        {
            _playwright = Playwright.CreateAsync().Result;

            Browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Timeout = 5000, Headless = headless }).Result;

            Page = Browser.NewPageAsync(new BrowserNewPageOptions { IgnoreHTTPSErrors = true }).Result;

            Page.SetDefaultTimeout(ActionTimeOut);
            Page.SetDefaultNavigationTimeout(FirstLoadTimeOut);
        }

        public async ValueTask DisposeAsync()
        {
            _playwright?.Dispose();

            await Browser.DisposeAsync();

            GC.SuppressFinalize(this);
        }

        protected Task StartWorkAsync(string hour, string quarter)
            => InputWorkAsync(hour, quarter, "Start Shift");

        protected Task EndWorkAsync(string hour, string quarter)
            => InputWorkAsync(hour, quarter, "End Shift");

        private async Task InputWorkAsync(string hour, string quarter, string inputButtonText)
        {
            await Page.ClickAsync($"text={inputButtonText}");

            await Page.SelectOptionAsync("select:below(label:text(\"Hour\"))", hour);
            await Page.SelectOptionAsync("select:below(label:text(\"Minutes\"))", quarter);

            await Page.ClickAsync("text=Submit");
        }
    }
}
