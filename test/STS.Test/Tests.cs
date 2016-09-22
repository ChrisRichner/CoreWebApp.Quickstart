using System;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Theory]
        [InlineData(1, "hello")]
        [InlineData(2, "wilmaa")]
        [InlineData(3, "zattoo")]
        public void Test1(int x, string s) 
        {
            Assert.True(true);
        }
    }
}
