using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class Server
    {
        public Server()
        {
            this.TimeDistribution = new List<TimeDistribution>();
        }

        public int ID { get; set; }
        public decimal IdleProbability { get; set; } // output
        public decimal AverageServiceTime { get; set; } // output
        public decimal Utilization { get; set; } // output

        public List<TimeDistribution> TimeDistribution;

        //optional if needed use them
        public int FinishTime { get; set; }
        public int TotalWorkingTime { get; set; }

        // Function that set the time distributions for each server
        public void SetTimeDistribution(List<TimeDistribution> distributionData)
        {
            foreach (var data in distributionData)
            {
                TimeDistribution.Add(new TimeDistribution
                {
                    Time = data.Time,
                    Probability = data.Probability,
                    CummProbability = data.CummProbability,
                    MinRange = data.MinRange,
                    MaxRange = data.MaxRange
                });
            }
        }

    }
}
