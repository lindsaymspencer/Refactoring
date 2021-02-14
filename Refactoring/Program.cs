using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace Refactoring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var plays = JsonConvert.DeserializeObject<Plays>(File.ReadAllText(@"external\plays.json"));
            var invoices = JsonConvert.DeserializeObject<Invoice[]>(File.ReadAllText(@"external\invoices.json"));
            Console.Write(Statement(invoices[0], plays));
        }

        public class Plays
        {
            public Play hamlet { get; set; }
            public Play aslike { get; set; }
            public Play othello { get; set; }
        }

        public class Play
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class Performance
        {
            public string PlayId { get; set; }
            public int Audience { get; set; }
        }

        public class Invoice
        {
            public string Customer { get; set; }
            public Performance[] Performances { get; set; }
        }

        public static string Statement(Invoice invoice, Plays plays)
        {
            double totalAmount = 0;
            double volumeCredits = 0;
            var result = $"Statement for {invoice.Customer}\n";
            CultureInfo culture = new CultureInfo("en-US");

            foreach (var perf in invoice.Performances)
            {
                var play = (Play) plays.GetType().GetProperty(perf.PlayId)?.GetValue(plays, null);

                var thisAmount = AmountFor(play, perf);

                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += Math.Floor(perf.Audience / (double) 5);

                // print line for this order
                result += $"  {play.Name}: {(thisAmount / 100).ToString("C", culture)} ({perf.Audience} seats)\n";
                totalAmount += thisAmount;
            }

            result += $"Amount owed is {(totalAmount / 100).ToString("C", culture)}\n";
            result += $"You earned {volumeCredits} credits";
            
            return result;
        }

        private static int AmountFor(Play play, Performance perf)
        {
            var thisAmount = 0;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = 40000;
                    if (perf.Audience > 30)
                    {
                        thisAmount += 1000 * (perf.Audience - 30);
                    }

                    break;
                case "comedy":
                    thisAmount = 30000;
                    if (perf.Audience > 20)
                    {
                        thisAmount += 10000 + 500 * (perf.Audience - 20);
                    }

                    thisAmount += 300 * perf.Audience;
                    break;
                default:
                    throw new Exception($"unknown type: {play.Type}");
            }

            return thisAmount;
        }
    }
}
