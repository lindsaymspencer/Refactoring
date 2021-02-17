using Refactoring.Models;

namespace Refactoring.Services
{
    public class TragedyCalculator : PerformanceCalculator
    {
        public TragedyCalculator(Performance aPerformance, Play aPlay) : base(aPerformance, aPlay) { }
       
        public override int Amount()
        {
            var result = 40000;
            if (Performance.Audience > 30)
            {
                result += 1000 * (Performance.Audience - 30);
            }

            return result;
        }
    }
}