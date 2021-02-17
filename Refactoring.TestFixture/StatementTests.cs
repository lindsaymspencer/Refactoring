using System;
using System.Reflection.Metadata;
using Refactoring.Models;
using Xunit;



namespace Refactoring.TestFixture
{
    public class StatementTests
    {
        private Plays plays = new Plays()
        {
            hamlet = new Play { Name = "Hamlet", Type = "tragedy" },
            aslike = new Play { Name = "As You Like It", Type = "comedy" },
            othello = new Play { Name = "Othello", Type = "tragedy" }
        };
        Invoice[] invoices = new Invoice[1];

        private void Setup()
        {
            var x = new Performance { PlayId = "hamlet", Audience = 55 };
            var y = new Performance { PlayId = "aslike", Audience = 35 };
            var z = new Performance { PlayId = "othello", Audience = 40 };

            invoices[0] = new Invoice
            {
                Customer = "BigCo",
                Performances = new Performance[3]
            };
            invoices[0].Performances[0] = x;
            invoices[0].Performances[1] = y;
            invoices[0].Performances[2] = z;
        }

        [Fact]
        public void ShouldReturnValidBillPlainText()
        {
            Setup();
            var expected = "PlainTextStatement for BigCo\n  Hamlet: $650.00 (55 seats)\n  As You Like It: $580.00 (35 seats)\n  Othello: $500.00 (40 seats)\nAmount owed is $1,730.00\nYou earned 47 credits";

            var actual = Program.PlainTextStatement(invoices[0], plays);

            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ShouldReturnValidBillHtml()
        {
            Setup();
            var expected =
                "<h1>PlainTextStatement for BigCo</h1><table><tr><th>play</th><th>seats</th><th>cost</th></tr><tr><td>Hamlet</td><td>55</td><td>$650.00</td></tr><tr><td>As You Like It</td><td>35</td><td>$580.00</td></tr><tr><td>Othello</td><td>40</td><td>$500.00</td></tr></table><p>Amount owed is <em>$1,730.00</em></p><p>You earned <em>47</em> credits</p>";

            var actual = Program.HtmlStatement(invoices[0], plays);

            Assert.Equal(expected, actual);
        }
    }
}
