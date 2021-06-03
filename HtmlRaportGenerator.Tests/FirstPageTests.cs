using System.Threading.Tasks;
using HtmlRaportGenerator.Tests.TestTools;
using Microsoft.Playwright;
using Xunit;

namespace HtmlRaportGenerator.Tests
{
    public class FirstPageTests : BaseBrowserTests
    {

        [Fact]
        public async Task Start_Shift_Works()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(3000);

            await StartWorkAsync(Page, "18", "3");

            string shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Started\"))");

            Assert.Equal("18:45", shiftStartedText);
        }

        [Theory]
        [InlineData("18", "3", "18:45")]
        [InlineData("0", "0", "00:00")]
        [InlineData("23", "3", "23:45")]
        public async Task Start_Shift_Is_Saved_To_LocalStorage(string hour, string quarter, string displayTime)
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(3000);

            await StartWorkAsync(Page, hour, quarter);

            await Page.ReloadAsync();

            await Task.Delay(1000);

            string shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Started\"))"); ;

            Assert.Equal(displayTime, shiftStartedText);

            //await File.WriteAllBytesAsync(@"C:\Users\romfi\OneDrive\Pulpit\temp\photo_clicked.png", await Page.ScreenshotAsync());
        }

        [Fact]
        public async Task End_Shift_Works()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(3000);

            await StartWorkAsync(Page, "18", "3");

            await EndWorkAsync(Page, "20", "2");

            string shiftEndedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))");

            Assert.Equal("20:30", shiftEndedText);
        }

        [Theory]
        [InlineData("18", "1", "20", "2", "02:15")]
        [InlineData("15", "1", "15", "0", "23:45")]
        public async Task Time_Worked_Works(string hourFrom, string quarterFrom, string hourTo, string quartetTo, string timeWorked)
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(3000);

            await StartWorkAsync(Page, hourFrom, quarterFrom);

            await EndWorkAsync(Page, hourTo, quartetTo);

            string timeWorkedText = await Page.TextContentAsync("span:near(span:text(\"Time Worked\"))");

            Assert.Equal(timeWorked, timeWorkedText);
        }

        [Theory]
        [InlineData("18", "3", "18:45")]
        [InlineData("0", "0", "00:00")]
        [InlineData("23", "3", "23:45")]
        public async Task End_Shift_Is_Saved_To_LocalStorage(string hour, string quarter, string displayTime)
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(3000);

            await StartWorkAsync(Page, "15", "1");

            await EndWorkAsync(Page, hour, quarter);

            await Page.ReloadAsync();

            await Task.Delay(1000);

            string shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))"); ;

            Assert.Equal(displayTime, shiftStartedText);
        }

        private static Task StartWorkAsync(IPage page, string hour, string quarter)
            => InputWorkAsync(page, hour, quarter, "text=Start Shift");

        private static Task EndWorkAsync(IPage page, string hour, string quarter)
            => InputWorkAsync(page, hour, quarter, "text=End Shift");

        private static async Task InputWorkAsync(IPage page, string hour, string quarter, string inputButtonText)
        {
            await page.ClickAsync(inputButtonText);

            await page.SelectOptionAsync("select:below(label:text(\"Hour\"))", new[] { hour });
            await page.SelectOptionAsync("select:below(label:text(\"Minutes\"))", new[] { quarter });

            await page.ClickAsync("text=Submit");
        }
    }
}
