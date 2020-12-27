using System;
using FileSwissKnife.Utils.UnitsManagement;
using Xunit;

namespace FileSwissKnife.Test.Utils.UnitsManagement
{
    public class TimeSpanExtensionTest
    {
        [Theory]  //ms ,exp  
        [InlineData(1, "1ms")]
        [InlineData(100, "100ms")]
        [InlineData(101, "101ms")]
        [InlineData(900, "900ms")]
        [InlineData(999, "999ms")]
        public void WhenTimeIsLessThan1SecThenNumberOfMsIsDisplayed(int milliseconds, string expected)
        {
            var elapsedTime = new TimeSpan(0, 0, 0, 0, milliseconds).ToElapsedTime();
            Assert.Equal(expected, elapsedTime);
        }

        [Theory]  //s ,ms ,exp  
        [InlineData(1, 0, "1.00s")]
        [InlineData(1, 1, "1.00s")]
        [InlineData(1, 44, "1.04s")]
        [InlineData(1, 45, "1.04s")]
        [InlineData(1, 46, "1.05s")]
        [InlineData(10, 45, "10.04s")]
        [InlineData(10, 46, "10.05s")]
        [InlineData(32, 346, "32.35s")]
        [InlineData(44, 995, "44.99s")]
        [InlineData(44, 996, "45.00s")]
        [InlineData(59, 994, "59.99s")]
        [InlineData(59, 995, "59.99s")]
        public void WhenTimeIsMoreThan1SecButLessThan1MinThenTheNumberOfSecIsDisplayed(int seconds, int milliseconds, string expected)
        {
            var elapsedTime = new TimeSpan(0, 0, 0, seconds, milliseconds).ToElapsedTime();
            Assert.Equal(expected, elapsedTime);
        }

        [Theory]  //m, s ,ms ,exp  
        [InlineData(0, 59, 996, "1m00s")]
        [InlineData(1, 0, 0, "1m00s")]
        [InlineData(33, 1, 500, "33m01s")]
        [InlineData(33, 1, 501, "33m02s")]
        [InlineData(59, 41, 501, "59m42s")]
        [InlineData(59, 59, 500, "59m59s")]
        public void WhenTimeIsMoreThan1MinButLessThan1HourThenTheNumberOfMinAndSecIsDisplayed(int minutes, int seconds, int milliseconds, string expected)
        {
            var elapsedTime = new TimeSpan(0, 0, minutes, seconds, milliseconds).ToElapsedTime();
            Assert.Equal(expected, elapsedTime);
        }

        [Theory]  //d, h, m, s ,ms ,exp  
        [InlineData(0, 0, 59, 59, 501, "1h00m00s")]
        [InlineData(0, 1, 0, 0, 0, "1h00m00s")]
        [InlineData(0, 1, 0, 0, 500, "1h00m00s")]
        [InlineData(0, 1, 0, 0, 501, "1h00m01s")]
        [InlineData(0, 1, 59, 59, 500, "1h59m59s")]
        [InlineData(0, 1, 59, 59, 501, "2h00m00s")]
        [InlineData(1, 3, 35, 10, 500, "27h35m10s")]
        [InlineData(1, 3, 35, 10, 501, "27h35m11s")]
        [InlineData(1, 0, 59, 59, 501, "25h00m00s")]
        public void WhenTimeIsMoreThan1HourThenTheNumberOfHoursMinAndSecIsDisplayed(int days, int hours, int minutes, int seconds, int milliseconds, string expected)
        {
            var elapsedTime = new TimeSpan(days, hours, minutes, seconds, milliseconds).ToElapsedTime();
            Assert.Equal(expected, elapsedTime);
        }
    }
}
