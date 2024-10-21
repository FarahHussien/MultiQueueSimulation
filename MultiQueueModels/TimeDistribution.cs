using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class TimeDistribution
    {
        public int Time { get; set; } // Service time
        public decimal Probability { get; set; } // second col
        public decimal CummProbability { get; set; } // third col
        public int MinRange { get; set; }
        public int MaxRange { get; set; }

    }
}
