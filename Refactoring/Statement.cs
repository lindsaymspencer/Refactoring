﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactoring.Models;

namespace Refactoring
{
    public class Statement
    {
        public class PerformanceCalculator
        {
            public PerformanceCalculator(Performance aPerformance, Play aPlay)
            {
                Performance = aPerformance;
                Play = aPlay;
            }
            public virtual int Amount() => throw new Exception("Subclass responsibility.");

            public virtual int VolumeCredits()
            {
                int result = 0;
                result += Math.Max(Performance.Audience - 30, 0);
                return result;
            }

            public Play Play { get; set; }
            public Performance Performance { get; set; }
        }

        public class TragedyCalculator : PerformanceCalculator
        {
            public TragedyCalculator(Performance aPerformance, Play aPlay) :
                base(aPerformance, aPlay)
            { }
            public override int Amount()
            {
                int result = 40000;
                if (Performance.Audience > 30)
                {
                    result += 1000 * (Performance.Audience - 30);
                }

                return result;
            }

        }
        public class ComedyCalculator : PerformanceCalculator
        {
            public ComedyCalculator(Performance aPerformance, Play aPlay) :
                base(aPerformance, aPlay)
            { }

            public override int Amount()
            {
                int result = 30000;
                if (Performance.Audience > 20)
                {
                    result += 10000 + 500 * (Performance.Audience - 20);
                }
                result += 300 * Performance.Audience;
                return result;
            }

            public override int VolumeCredits() => base.VolumeCredits() + (int)Math.Floor(Performance.Audience / (double)5);
        }

        public static Dictionary<string, object> CreateStatementData(Invoice invoice, Plays plays)
        {
            Play PlayFor(Performance aPerformance) =>
                (Play)plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);

            Performance EnrichPerformance(Performance aPerformance)
            {
                var calculator = Program.CreatePerformanceCalculator(aPerformance, PlayFor(aPerformance));
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
    }
}
