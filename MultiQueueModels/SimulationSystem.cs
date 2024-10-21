using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            this.Servers = new List<Server>();
            this.InterarrivalDistribution = new List<TimeDistribution>();
            this.PerformanceMeasures = new PerformanceMeasures();
            this.SimulationTable = new List<SimulationCase>();
        }

        ///////////// INPUTS ///////////// 
        public int NumberOfServers { get; set; }
        public int StoppingNumber { get; set; }
        public List<Server> Servers { get; set; }
        public List<TimeDistribution> InterarrivalDistribution { get; set; }
        public Enums.StoppingCriteria StoppingCriteria { get; set; }
        public Enums.SelectionMethod SelectionMethod { get; set; }


        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public void SetCase(
             int customerNumber,
             int randomInterArrival,
             int interArrival,
             int arrivalTime,
             int randomService,
             int serviceTime,
             Server assignedServer,
             int startTime,
             int endTime,
             int timeInQueue)
        {
            SimulationTable.Add(new SimulationCase
            {
                CustomerNumber = customerNumber,
                RandomInterArrival = randomInterArrival,
                InterArrival = interArrival,
                ArrivalTime = arrivalTime,
                RandomService = randomService,
                ServiceTime = serviceTime,
                AssignedServer = assignedServer,  // Pass an instance of a server
                StartTime = startTime,
                EndTime = endTime,
                TimeInQueue = timeInQueue
            });
        }

        // Method to run the full simulation sys
        public void RunSimulation()
        {
            int currentTime = 0;
            int previousArrivalTime = 0;

            for (int customerNumber = 1; customerNumber <= StoppingNumber; customerNumber++)
            {
                int randomInterArrival = GenerateRandomValue(); 
                int interArrival = GetInterArrivalTime(randomInterArrival);
                int arrivalTime = previousArrivalTime + interArrival;

                Server assignedServer = SelectServer(arrivalTime); 
                int randomService = GenerateRandomValue(); 
                int serviceTime = GetServiceTime(assignedServer, randomService);

                int startTime = Math.Max(arrivalTime, assignedServer.FinishTime); // Customer starts when the server is free
                int endTime = startTime + serviceTime;
                //int timeInQueue = 0; // task

                // Update the server's finish time
                assignedServer.FinishTime = endTime;

                // Set the case for this customer
                SetCase(
                    customerNumber: customerNumber,
                    randomInterArrival: randomInterArrival,
                    interArrival: interArrival,
                    arrivalTime: arrivalTime,
                    randomService: randomService,
                    serviceTime: serviceTime,
                    assignedServer: assignedServer,
                    startTime: startTime,
                    endTime: endTime,
                    timeInQueue: timeInQueue
                );

                // update prev arrival time
                previousArrivalTime = arrivalTime;
            }
        }

        
        private int GenerateRandomValue()
        {
            return new Random().Next(1, StoppingNumber + 1);
        }

        private int GetInterArrivalTime(int randomInterArrival)
        {
            foreach (var dist in InterarrivalDistribution)
            {
                if (randomInterArrival <= dist.MaxRange && randomInterArrival >= dist.MinRange)
                {
                    return dist.Time;
                }
            }
            return 0; 
        }

        private int GetServiceTime(Server server, int randomService)
        {
            foreach (var dist in server.TimeDistribution)
            {
                if (randomService <= dist.MaxRange && randomService >= dist.MinRange)
                {
                    return dist.Time;
                }
            }
            return 0;
        }
        // task 
        private Server SelectServer(int arrivalTime)
        {
            // must choose server based on there is only one idle then choose it
            // or more than one idle server then choose based on selection method
        }

    }
}
