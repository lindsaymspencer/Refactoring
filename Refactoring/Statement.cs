﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Statement
    {
        public static Dictionary<string, object> CreateStatementData(Program.Invoice invoice, Program.Plays plays)
        {
            Program.Play PlayFor(Program.Performance aPerformance) =>
                (Program.Play) plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);

            int AmountFor(Program.Performance aPerformance)
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

            int VolumeCreditsFor(Program.Performance aPerformance)
            {
                int result = 0;
                result += Math.Max(aPerformance.Audience - 30, 0);
                if ("comedy" == aPerformance.Play.Type) result += (int) Math.Floor(aPerformance.Audience / (double) 5);
                return result;
            }

            Program.Performance EnrichPerformance(Program.Performance aPerformance)
            {
                var result = new Program.Performance() {Audience = aPerformance.Audience, PlayId = aPerformance.PlayId};
                result.Play = PlayFor(result);
                result.Amount = AmountFor(result);
                result.VolumeCredits = VolumeCreditsFor(result);
                return result;
            }

            double TotalAmount(Dictionary<string, object> data) =>
                ((Program.Performance[]) data["Performances"]).Aggregate<Program.Performance, double>(0,
                    (current, perf) => current + perf.Amount);

            double TotalVolumeCredits(Dictionary<string, object> data)
            {
                var result = 0;
                foreach (var perf in ((Program.Performance[]) data["Performances"]))
                {
                    result += perf.VolumeCredits;
                }

                return result;
            }

            {
                var statementData = new Dictionary<string, object>();
                statementData.Add("Customer", invoice.Customer);
                statementData.Add("Performances", invoice.Performances.Select(EnrichPerformance).ToArray());
                statementData.Add("TotalAmount", TotalAmount(statementData));
                statementData.Add("TotalVolumeCredits", TotalVolumeCredits(statementData));
                return statementData;
            }
        }
    }
}