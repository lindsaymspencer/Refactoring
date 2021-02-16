using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;

namespace Refactoring
{
    public class Program
    {
        private static Plays plays;

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

        public static string Statement(Invoice invoice, Plays tempPlays)
        {
            plays = tempPlays;
            double totalAmount = 0;
            double volumeCredits = 0;
            var result = $"Statement for {invoice.Customer}\n";
            CultureInfo culture = new CultureInfo("en-US");

            foreach (var perf in invoice.Performances)
            {
                volumeCredits += VolumeCreditsFor(perf);

                // print line for this order
                result += $"  {PlayFor(perf).Name}: {(AmountFor(perf) / 100).ToString("C", culture)} ({perf.Audience} seats)\n";
                totalAmount += AmountFor(perf);
            }

            result += $"Amount owed is {(totalAmount / 100).ToString("C", culture)}\n";
            result += $"You earned {volumeCredits} credits";
            
            return result;
        }

        private static double VolumeCreditsFor(Performance perf)
        {
            double volumeCredits = 0;
            volumeCredits += Math.Max(perf.Audience - 30, 0);
            if ("comedy" == PlayFor(perf).Type) volumeCredits += Math.Floor(perf.Audience / (double) 5);
            return volumeCredits;
        }

        private static Play PlayFor(Performance aPerformance)
        {
            return (Play) plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);
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
