using Refactoring.Models;
using Refactoring.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Refactoring
{
    public class Statement
    {
        public static Dictionary<string, object> CreateStatementData(Invoice invoice, Plays plays)
        {
            Play PlayFor(Performance aPerformance) =>
                (Play)plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);

            Performance EnrichPerformance(Performance aPerformance)
            {
                var calculator = CreatePerformanceCalculator(aPerformance, PlayFor(aPerformance));
                var result = new Performance() { Audience = aPerformance.Audience, PlayId = aPerformance.PlayId };
                result.Play = calculator.Play;
                result.Amount = calculator.Amount();
                result.VolumeCredits = calculator.VolumeCredits();
                return result;
            }

            double TotalAmount(Dictionary<string, object> data) =>
                ((Performance[])data["Performances"]).Aggregate<Performance, double>(0,
                    (current, perf) => current + perf.Amount);

            double TotalVolumeCredits(Dictionary<string, object> data)
            {
                var result = 0;
                foreach (var perf in ((Performance[])data["Performances"]))
                {
                    result += perf.VolumeCredits;
                }

                return result;
            }

            {
                var result = new Dictionary<string, object>();
                result.Add("Customer", invoice.Customer);
                result.Add("Performances", invoice.Performances.Select(EnrichPerformance).ToArray());
                result.Add("TotalAmount", TotalAmount(result));
                result.Add("TotalVolumeCredits", TotalVolumeCredits(result));
                return result;
            }
        }

        private static string Usd(double aNumber) => (aNumber / 100).ToString("c", new CultureInfo("en-US"));

        public static string PlainTextStatement(Invoice invoice, Plays plays)
        {
            return RenderPlainText(Refactoring.Statement.CreateStatementData(invoice, plays));
        }

        public static string RenderPlainText(Dictionary<string, object> data)
        {
            var result = $"PlainTextStatement for {data["Customer"]}\n";
            foreach (var perf in ((Performance[])data["Performances"]))
            {
                result += $"  {perf.Play.Name}: {Usd(perf.Amount)} ({perf.Audience} seats)\n";
            }
            result += $"Amount owed is {Usd((double)data["TotalAmount"])}\n";
            result += $"You earned {(double)data["TotalVolumeCredits"]} credits";
            return result;
        }

        public static string HtmlStatement(Invoice invoice, Plays plays)
        {
            return RenderHtml(Refactoring.Statement.CreateStatementData(invoice, plays));
        }

        public static string RenderHtml(Dictionary<string, object> data)
        {
            var result = $"<h1>PlainTextStatement for {data["Customer"]}</h1>";
            result += "<table>";
            result += "<tr><th>play</th><th>seats</th><th>cost</th></tr>";
            foreach (Performance perf in (Performance[])data["Performances"])
            {
                result += $"<tr><td>{perf.Play.Name}</td><td>{perf.Audience}</td>";
                result += $"<td>{Usd(perf.Amount)}</td></tr>";
            }

            result += "</table>";
            result += $"<p>Amount owed is <em>{Usd((double)data["TotalAmount"])}</em></p>";
            result += $"<p>You earned <em>{(double) data["TotalVolumeCredits"]}</em> credits</p>";
            return result;
        }

        public static PerformanceCalculator CreatePerformanceCalculator(Performance aPerformance, Play aPlay)
        {
            return aPlay.Type switch
            {
                "tragedy" => new TragedyCalculator(aPerformance, aPlay),
                "comedy" => new ComedyCalculator(aPerformance, aPlay),
                _ => throw new Exception($"Unknown type: {aPlay.Type}")
            };
        }
    }
}
