using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Statement
    {
        public class PerformanceCalculator
        {
            public PerformanceCalculator(Program.Performance aPerformance, Program.Play aPlay)
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

            public Program.Play Play { get; set; }
            public Program.Performance Performance { get; set; }
        }

        public class TragedyCalculator : PerformanceCalculator
        {
            public TragedyCalculator(Program.Performance aPerformance, Program.Play aPlay) :
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
            public ComedyCalculator(Program.Performance aPerformance, Program.Play aPlay) :
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

        public static Dictionary<string, object> CreateStatementData(Program.Invoice invoice, Program.Plays plays)
        {
            Program.Play PlayFor(Program.Performance aPerformance) =>
                (Program.Play)plays.GetType().GetProperty(aPerformance.PlayId)?.GetValue(plays, null);

            Program.Performance EnrichPerformance(Program.Performance aPerformance)
            {
                var calculator = Program.CreatePerformanceCalculator(aPerformance, PlayFor(aPerformance));
                var result = new Program.Performance() { Audience = aPerformance.Audience, PlayId = aPerformance.PlayId };
                result.Play = calculator.Play;
                result.Amount = calculator.Amount();
                result.VolumeCredits = calculator.VolumeCredits();
                return result;
            }

            double TotalAmount(Dictionary<string, object> data) =>
                ((Program.Performance[])data["Performances"]).Aggregate<Program.Performance, double>(0,
                    (current, perf) => current + perf.Amount);

            double TotalVolumeCredits(Dictionary<string, object> data)
            {
                var result = 0;
                foreach (var perf in ((Program.Performance[])data["Performances"]))
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
