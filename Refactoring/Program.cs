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
            public int VolumeCredits { get; set; }
        }

        public class Invoice
        {
            public string Customer { get; set; }
            public Performance[] Performances { get; set; }
        }

        public static string Statement(Invoice invoice, Plays plays)
        {
            return RenderPlainText(Refactoring.Statement.CreateStatementData(invoice, plays));
        }

        private static string RenderPlainText(Dictionary<string, object> data)
        {
            string Usd(double aNumber) => (aNumber / 100).ToString("c", new CultureInfo("en-US"));

            {
                var result = $"Statement for {data["Customer"]}\n";
                foreach (var perf in ((Performance[])data["Performances"]))
                {
                    result += $"  {perf.Play.Name}: {Usd(perf.Amount)} ({perf.Audience} seats)\n";
                }

                result += $"Amount owed is {Usd((double)data["TotalAmount"])}\n";
                result += $"You earned {(double)data["TotalVolumeCredits"]} credits";
                return result;
            }
        }
    }
}
