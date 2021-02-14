using System.Reflection.Metadata;
using Xunit;

namespace Refactoring.TestFixture
{
    public class StatementTests
    {
        [Fact]
        public void ShouldReturnValidBill()
        {
            var expected =
                "Statement for BigCo\n  Hamlet: $650.00 (55 seats)\n  As You Like It: $580.00 (35 seats)\n  Othello: $500.00 (40 seats)\nAmount owed is $1,730.00\nYou earned 47 credits";
            var plays = new Program.Plays()
            {
                hamlet = new Program.Play {Name = "Hamlet", Type = "tragedy"},
                aslike = new Program.Play {Name = "As You Like It", Type = "comedy"},
                othello = new Program.Play {Name = "Othello", Type = "tragedy"}
            };

            var x = new Program.Performance {PlayId = "hamlet", Audience = 55};
            var y = new Program.Performance { PlayId = "aslike", Audience = 35 };
            var z = new Program.Performance { PlayId = "othello", Audience = 40 };

            Program.Invoice[] invoices = new Program.Invoice[1];
            invoices[0] = new Program.Invoice
            {
                Customer = "BigCo",
                Performances = new Program.Performance[3]
            };
            invoices[0].Performances[0] = x;
            invoices[0].Performances[1] = y;
            invoices[0].Performances[2] = z;

            var actual = Program.Statement(invoices[0], plays);

            Assert.Equal(expected, actual);
        }
    }
}
