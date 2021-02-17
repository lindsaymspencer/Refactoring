using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Refactoring.Models;

namespace Refactoring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tempPlays = JsonConvert.DeserializeObject<Plays>(File.ReadAllText(@"external\plays.json"));
            var invoices = JsonConvert.DeserializeObject<Invoice[]>(File.ReadAllText(@"external\invoices.json"));
            Console.Write(PlainTextStatement(invoices[0], tempPlays));
        }

        static string Usd(double aNumber) => (aNumber / 100).ToString("c", new CultureInfo("en-US"));

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

        public static Statement.PerformanceCalculator CreatePerformanceCalculator(Performance aPerformance, Play aPlay)
        {
            return aPlay.Type switch
            {
                "tragedy" => new Statement.TragedyCalculator(aPerformance, aPlay),
                "comedy" => new Statement.ComedyCalculator(aPerformance, aPlay),
                _ => throw new Exception($"Unknown type: {aPlay.Type}")
            };
        } 
    }
}
