using HtmlRaportGenerator.Tests.TestTools;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HtmlRaportGenerator.Tests
{
    public class MonthEditTests : BaseBrowserTests
    {
        [Fact]
        public async Task Start_Shift_Added_In_First_Page_Display_In_Table()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            int currentDayNumber = DateTime.Now.Day;

            await StartWorkAsync("18", "3");

            await Page.GotoAsync(MonthEditUrl);

            await Task.Delay(1000);

            string shiftStartedTime = await Page.EvalOnSelectorAsync<string>(
                $"tbody > tr:nth-child({currentDayNumber}) > td:nth-child(3) > input", "s => s.value");

            Assert.Equal("18:45", shiftStartedTime);
        }

        [Fact]
        public async Task End_Shift_Added_In_First_Page_Display_In_Table()
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            int currentDayNumber = DateTime.Now.Day;

            await StartWorkAsync("18", "3");
            await EndWorkAsync("20", "3");

            await Page.GotoAsync(MonthEditUrl);

            await Task.Delay(1000);

            string shiftStartedTime = await Page.EvalOnSelectorAsync<string>(
                $"tbody > tr:nth-child({currentDayNumber}) > td:nth-child(4) > input", "s => s.value");

            Assert.Equal("20:45", shiftStartedTime);
        }

        [Theory]
        [InlineData("18", "1", "20", "2", "02:15")]
        [InlineData("15", "1", "15", "0", "23:45")]
        public async Task Hour_Sum_Added_In_First_Page_Display_In_Table(string hourFrom, string quarterFrom, string hourTo, string quartetTo, string timeWorked)
        {
            await Page.GotoAsync(BaseUrl);

            await Task.Delay(FirstLoadTimeOut);

            int currentDayNumber = DateTime.Now.Day;

            await StartWorkAsync(hourFrom, quarterFrom);
            await EndWorkAsync(hourTo, quartetTo);

            await Page.GotoAsync(MonthEditUrl);

            await Task.Delay(1000);

            string shiftStartedTime =
                await Page.TextContentAsync($"tbody > tr:nth-child({currentDayNumber}) > td:nth-child(5) > div");

            Assert.Equal(timeWorked, shiftStartedTime);
        }

        //todo edit tests and with month changing
        //todo clear tests
        //todo save button tests
        //todo clear changes tests
    }
}
