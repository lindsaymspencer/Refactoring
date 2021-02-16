using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Refactoring
{
    public class Program
    {
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
            public Play Play { get; set; }
            public double Amount { get; set; }
        }

        public class Invoice
        {
            public string Customer { get; set; }
            public Performance[] Performances { get; set; }
        }

        public static string Statement(Invoice invoice, Plays plays)
        {
            var statementData = new Dictionary<string, object>();
            statementData.Add("Customer", invoice.Customer);
            statementData.Add("Performances", invoice.Performances.Select(EnrichPerformance).ToArray());
            return RenderPlainText(statementData, plays);

            Performance EnrichPerformance(Performance aPerformance)
            {
                    var result = new Performance() { Audience = aPerformance.Audience, PlayId = aPerformance.PlayId };
                    result.Play = PlayFor(result);
                    result.Amount = AmountFor(result);
                    return result;
            }

            Play PlayFor(Performance aPerformance) => (Play)plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);
            int AmountFor(Performance aPerformance)
            {
                int result;
                switch (aPerformance.Play.Type)
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
                        throw new Exception($"unknown type: {aPerformance.Play.Type}");
                }

                return result;
            }
        }

        private static string RenderPlainText(Dictionary<string, object> data, Plays plays)
        {
            string Usd(double aNumber) => (aNumber / 100).ToString("c", new CultureInfo("en-US"));


            double TotalAmount() => ((Performance[])data["Performances"]).Aggregate<Performance, double>(0, (current, perf) => current + perf.Amount);

            double VolumeCreditsFor(Performance aPerformance)
            {
                double result = 0;
                result += Math.Max(aPerformance.Audience - 30, 0);
                if ("comedy" == aPerformance.Play.Type) result += Math.Floor(aPerformance.Audience / (double)5);
                return result;
            }

            double TotalVolumeCredits() => ((Performance[])data["Performances"]).Sum(VolumeCreditsFor);

            {
                var result = $"Statement for {data["Customer"]}\n";
                foreach (var perf in ((Performance[])data["Performances"]))
                {
                    result += $"  {perf.Play.Name}: {Usd(perf.Amount)} ({perf.Audience} seats)\n";
                }

                result += $"Amount owed is {Usd(TotalAmount())}\n";
                result += $"You earned {TotalVolumeCredits()} credits";
                return result;
            }
        }
    }
}
