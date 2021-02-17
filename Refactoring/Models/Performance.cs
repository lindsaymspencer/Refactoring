﻿namespace Refactoring.Models
{
    public class Performance
    {
        public string PlayId { get; set; }
        public int Audience { get; set; }
        public Play Play { get; set; }
        public double Amount { get; set; }
        public int VolumeCredits { get; set; }
    }
}