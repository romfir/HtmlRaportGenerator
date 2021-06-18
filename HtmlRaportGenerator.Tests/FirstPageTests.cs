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
            _ = await Page.GoToPageAndAssertSuccessAsync(BaseUrl)
                .ConfigureAwait(false);

            await Task.Delay(FirstLoadTimeOut)
                .ConfigureAwait(false);

            await StartWorkAsync("18", "3")
                .ConfigureAwait(false);

            string? shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Started\"))")
                .ConfigureAwait(false);

            Assert.NotNull(shiftStartedText);

            Assert.Equal("18:45", shiftStartedText);
        }

        [Theory]
        [InlineData("18", "3", "18:45")]
        [InlineData("0", "0", "00:00")]
        [InlineData("23", "3", "23:45")]
        public async Task Start_Shift_Is_Saved_To_LocalStorage(string hour, string quarter, string displayTime)
        {
            _ = await Page.GoToPageAndAssertSuccessAsync(BaseUrl)
                .ConfigureAwait(false);

            await Task.Delay(FirstLoadTimeOut)
                .ConfigureAwait(false);

            await StartWorkAsync(hour, quarter)
                .ConfigureAwait(false);

            _ = await Page.ReloadAndAssertSuccessAsync()
                .ConfigureAwait(false);

            await Task.Delay(SiteChangeTimeOut)
                .ConfigureAwait(false);

            string? shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Started\"))")
                .ConfigureAwait(false);

            Assert.NotNull(shiftStartedText);

            Assert.Equal(displayTime, shiftStartedText);
        }

        [Fact]
        public async Task End_Shift_Works()
        {
            _ = await Page.GoToPageAndAssertSuccessAsync(BaseUrl)
                .ConfigureAwait(false);

            await Task.Delay(FirstLoadTimeOut)
                .ConfigureAwait(false);

            await StartWorkAsync("18", "3")
                .ConfigureAwait(false);

            await EndWorkAsync("20", "2")
                .ConfigureAwait(false);

            string? shiftEndedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))")
                .ConfigureAwait(false);
            
            Assert.NotNull(shiftEndedText);

            Assert.Equal("20:30", shiftEndedText);
        }

        [Theory]
        [InlineData("18", "1", "20", "2", "02:15")]
        [InlineData("15", "1", "15", "0", "23:45")]
        public async Task Time_Worked_Works(string hourFrom, string quarterFrom, string hourTo, string quartetTo, string timeWorked)
        {
            _ = await Page.GoToPageAndAssertSuccessAsync(BaseUrl)
                .ConfigureAwait(false);

            await Task.Delay(FirstLoadTimeOut)
                .ConfigureAwait(false);

            await StartWorkAsync(hourFrom, quarterFrom)
                .ConfigureAwait(false);

            await EndWorkAsync(hourTo, quartetTo)
                .ConfigureAwait(false);

            string? timeWorkedText = await Page.TextContentAsync("span:near(span:text(\"Time Worked\"))")
                .ConfigureAwait(false);

            Assert.NotNull(timeWorkedText);

            Assert.Equal(timeWorked, timeWorkedText);
        }

        [Theory]
        [InlineData("18", "3", "18:45")]
        [InlineData("0", "0", "00:00")]
        [InlineData("23", "3", "23:45")]
        public async Task End_Shift_Is_Saved_To_LocalStorage(string hour, string quarter, string displayTime)
        {
            _ = await Page.GoToPageAndAssertSuccessAsync(BaseUrl)
                .ConfigureAwait(false);

            await Task.Delay(FirstLoadTimeOut)
                .ConfigureAwait(false);

            await StartWorkAsync("15", "1")
                .ConfigureAwait(false);

            await EndWorkAsync(hour, quarter)
                .ConfigureAwait(false);

            _ = await Page.ReloadAndAssertSuccessAsync()
                .ConfigureAwait(false);

            await Task.Delay(SiteChangeTimeOut)
                .ConfigureAwait(false);

            string? shiftStartedText = await Page.TextContentAsync("button:near(span:text(\"Shift Ended\"))")
                .ConfigureAwait(false);

            Assert.NotNull(shiftStartedText);

            Assert.Equal(displayTime, shiftStartedText);
        }

        //todo edit tests
    }
}
