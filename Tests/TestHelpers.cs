using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public static class TestHelpers
    {
        public static void TrimmedEqual(string lhs, string rhs)
        {
            var lhsTrimmed = Trim(lhs);
            var rhsTrimmed = Trim(rhs);

            Assert.Equal(lhsTrimmed, rhsTrimmed);
        }

        public static void TrimmedContains(string lhs, string rhs)
        {
            var lhsTrimmed = Trim(lhs);
            var rhsTrimmed = Trim(rhs);

            Assert.Contains(lhsTrimmed, rhsTrimmed, StringComparison.Ordinal);
        }

        private static string Trim(string value)
        {
            var lines = value.Split('\n');

            return string.Join(Environment.NewLine, lines.Select(x => x.Trim()).Where(x => x.Length > 0));
        }
    }
}
