using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HtmlRaportGenerator.Tests.TestTools
{
    public abstract class BaseBrowserTests : IAsyncLifetime
    {
        private readonly bool _headless;

        protected const float ActionTimeOut = 4000;

        protected const int FirstLoadTimeOut = 12000;

        protected const int SiteChangeTimeOut = 4000;

        protected const string BaseUrl = "https://localhost:5001";

        protected const string MonthEditUrl = BaseUrl + "/MonthEdit";

        private IPlaywright _playwright = null!;

        protected IBrowser Browser { get; private set; } = null!;

        protected IPage Page { get; private set; } = null!;

        protected BaseBrowserTests(bool headless = true)
            => _headless = headless;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync().ConfigureAwait(false);

            Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Timeout = 5000,
                Headless = _headless
            })
                .ConfigureAwait(false);

            Page = await Browser.NewPageAsync(new BrowserNewPageOptions { IgnoreHTTPSErrors = true })
                .ConfigureAwait(false);

            Page.SetDefaultTimeout(ActionTimeOut);
            Page.SetDefaultNavigationTimeout(FirstLoadTimeOut);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await Browser.DisposeAsync();

            _playwright.Dispose();

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected Task StartWorkAsync(string hour, string quarter)
            => InputWorkAsync(hour, quarter, "Start Shift");

        protected Task EndWorkAsync(string hour, string quarter)
            => InputWorkAsync(hour, quarter, "End Shift");

        private async Task InputWorkAsync(string hour, string quarter, string inputButtonText)
        {
            await Page.ClickAsync($"text={inputButtonText}")
                .ConfigureAwait(false);

            _ = await Page.SelectOptionAsync("select:below(label:text(\"Hour\"))", hour)
                .ConfigureAwait(false);
            _ = await Page.SelectOptionAsync("select:below(label:text(\"Minutes\"))", quarter)
                .ConfigureAwait(false);

            await Page.ClickAsync("text=Submit")
                .ConfigureAwait(false);
        }
    }
}
