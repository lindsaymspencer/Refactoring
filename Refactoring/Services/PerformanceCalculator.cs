using System;
using Refactoring.Models;

namespace Refactoring.Services
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
            var result = 0;
            result += Math.Max(Performance.Audience - 30, 0);
            return result;
        }

        public Play Play { get; set; }
        public Performance Performance { get; set; }
    }
}