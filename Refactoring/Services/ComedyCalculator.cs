using System;
using Refactoring.Models;

namespace Refactoring.Services
{
    public class ComedyCalculator : PerformanceCalculator
    {
        public ComedyCalculator(Performance aPerformance, Play aPlay) : base(aPerformance, aPlay) { }

        public override int Amount()
        {
            var result = 30000;
            if (Performance.Audience > 20)
            {
                result += 10000 + 500 * (Performance.Audience - 20);
            }
            result += 300 * Performance.Audience;
            return result;
        }

        public override int VolumeCredits() =>
            base.VolumeCredits() + (int)Math.Floor(Performance.Audience / (double)5);
    }
}