using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactoring.Models;

namespace Refactoring.Services
{
}

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
