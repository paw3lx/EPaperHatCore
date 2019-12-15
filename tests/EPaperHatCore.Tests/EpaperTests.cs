using System;
using System.IO;
using BetaSoft.EPaperHatCore;
using Shouldly;
using Xunit;

namespace Betasoft.EPaperHatCore.Tests
{
    public class EpaperTests
    {
        [Fact]
        public void Width_Or_Height_Less_Or_Equal_Zero_Throws()
        {
            var exception = Record.Exception(() => {
                new Epaper27(0, 0);
            });

            exception.ShouldBeOfType(typeof(ArgumentException));
        }
    }
}
