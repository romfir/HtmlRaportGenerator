using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace HtmlRaportGenerator.Tests.TestTools
{
    public class BaseBrowserTests : IAsyncDisposable
    {
        protected const float TimeOut = 1000;

        protected const string BaseUrl = "https://localhost:5001";

        private readonly Uri _baseUri = new(BaseUrl);

        private readonly IPlaywright _playwright;
        protected readonly IBrowser Browser;
        protected readonly IPage Page;

        public BaseBrowserTests()
        {
            _playwright = Playwright.CreateAsync().Result;

            Browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Timeout = 5000 }).Result;

            Page = Browser.NewPageAsync().Result;

            Page.SetDefaultTimeout(TimeOut);
            Page.SetDefaultNavigationTimeout(3000);
        }

        protected string GetUrl(string relativeUrl)
            => new Uri(_baseUri, relativeUrl).ToString();

        public async ValueTask DisposeAsync()
        {
            _playwright?.Dispose();

            await Browser.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
