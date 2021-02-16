using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Refactoring
{
    public class Program
    {
        private static Plays plays;
        private static Invoice invoice;

        public static void Main(string[] args)
        {
            var tempPlays = JsonConvert.DeserializeObject<Plays>(File.ReadAllText(@"external\plays.json"));
            var invoices = JsonConvert.DeserializeObject<Invoice[]>(File.ReadAllText(@"external\invoices.json"));
            Console.Write(Statement(invoices[0], tempPlays));
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

        public static string Statement(Invoice tempInvoice, Plays tempPlays)
        {
            plays = tempPlays;
            invoice = tempInvoice;

            var result = $"Statement for {invoice.Customer}\n";

            foreach (var perf in invoice.Performances)
            {
                // print line for this order
                result += $"  {PlayFor(perf).Name}: {Usd(AmountFor(perf))} ({perf.Audience} seats)\n";
            }

            result += $"Amount owed is {Usd(TotalAmount())}\n";
            result += $"You earned {TotalVolumeCredits()} credits";

            return result;
        }

        private static double TotalAmount() => invoice.Performances.Aggregate<Performance, double>(0, (current, perf) => current + AmountFor(perf));

        private static double TotalVolumeCredits() => invoice.Performances.Sum(VolumeCreditsFor);

        private static string Usd(double aNumber) => (aNumber / 100).ToString("c", new CultureInfo("en-US"));

        private static double VolumeCreditsFor(Performance aPerformance)
        {
            double result = 0;
            result += Math.Max(aPerformance.Audience - 30, 0);
            if ("comedy" == PlayFor(aPerformance).Type) result += Math.Floor(aPerformance.Audience / (double)5);
            return result;
        }

        private static Play PlayFor(Performance aPerformance)
        {
            return (Play)plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);
        }

        private static int AmountFor(Performance aPerformance)
        {
            int result;
            switch (PlayFor(aPerformance).Type)
            {
                case "tragedy":
                    result = 40000;
                    if (aPerformance.Audience > 30)
                    {
                        result += 1000 * (aPerformance.Audience - 30);
                    }

                    break;
                case "comedy":
                    result = 30000;
                    if (aPerformance.Audience > 20)
                    {
                        result += 10000 + 500 * (aPerformance.Audience - 20);
                    }

                    result += 300 * aPerformance.Audience;
                    break;
                default:
                    throw new Exception($"unknown type: {PlayFor(aPerformance).Type}");
            }

            return result;
        }
    }
}
