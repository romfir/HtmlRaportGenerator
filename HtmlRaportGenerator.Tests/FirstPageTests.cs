using System.Threading.Tasks;
using HtmlRaportGenerator.Tests.TestTools;
using Xunit;

namespace HtmlRaportGenerator.Tests
{
    public class FirstPageTests : BaseBrowserTests
    {
        [Fact]
        public async Task Start_Shift_Works()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            await StartWorkAsync("18", "3");

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

            await Task.Delay(FirstLoadTimeOut);

            await StartWorkAsync(hour, quarter);

            await Page.ReloadAsync();

            await Task.Delay(SiteChangeTimeOut);

            string shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Started\"))");

            Assert.Equal(displayTime, shiftStartedText);
        }

        [Fact]
        public async Task End_Shift_Works()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            await StartWorkAsync("18", "3");

            await EndWorkAsync("20", "2");

            string shiftEndedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))");

            Assert.Equal("20:30", shiftEndedText);
        }

        [Theory]
        [InlineData("18", "1", "20", "2", "02:15")]
        [InlineData("15", "1", "15", "0", "23:45")]
        public async Task Time_Worked_Works(string hourFrom, string quarterFrom, string hourTo, string quartetTo, string timeWorked)
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            await StartWorkAsync(hourFrom, quarterFrom);

            await EndWorkAsync(hourTo, quartetTo);

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

            await Task.Delay(FirstLoadTimeOut);

            await StartWorkAsync("15", "1");

            await EndWorkAsync(hour, quarter);

            await Page.ReloadAsync();

            await Task.Delay(SiteChangeTimeOut);

            string shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))");

            Assert.Equal(displayTime, shiftStartedText);
        }

        //todo edit tests
    }
}
