using DocumentFormat.OpenXml.Wordprocessing;
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

        public Queue<SimulationCase> queueCust = new Queue<SimulationCase>();

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; } // output

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
                TimeInQueue = timeInQueue // each customer delay
            });
        }

        public void InitializeSystem(ReadFiles fileData)
        {
            this.NumberOfServers = fileData.ConfigValues[0];
            this.StoppingNumber = fileData.ConfigValues[1];
            this.SelectionMethod = (Enums.SelectionMethod)fileData.ConfigValues[2];
            this.StoppingCriteria = (Enums.StoppingCriteria)fileData.ConfigValues[3];

            this.InterarrivalDistribution = fileData.InterarrivalDistribution
                .Select(dist_index => new TimeDistribution
                {
                    Time = dist_index.Item1,
                    Probability = (decimal)dist_index.Item2
                })
                .ToList();

            // Assign Service Distributions to each Server
            int servicePerServer = fileData.ServiceDistribution.Count / this.NumberOfServers; // Calc  #distributions each server will get
            int currentIndex = 0;

            for (int i = 0; i < this.NumberOfServers; i++)
            {
                Server curServer = new Server
                {
                    ID = i // Assign an ID or any other initialization logic you need
                };
                for (int j = 0; j < servicePerServer; j++) 
                {
                    var serviceDist = fileData.ServiceDistribution[currentIndex];

                    curServer.TimeDistribution.Add(new TimeDistribution
                    {
                        Time = serviceDist.Item1,
                        Probability = (decimal)serviceDist.Item2

                    });
                    currentIndex++;
                }
                // Now add the new server to the Servers list
                this.Servers.Add(curServer);
            }
        }

        // Method to run full simulation sys
        public void RunSimulation()
        {
            int previousArrivalTime = 0;
            int TCWIQ = 0;  //nada edit
            int TTCWIQ = 0; //nada edit
            int interArrival = 0;
            Console.WriteLine("before for that loop on customers");
            for (int customerNumber = 1; customerNumber <= StoppingNumber; customerNumber++)
            {
                int randomInterArrival = GenerateRandomValue();
                int interArrivalRandom = GetInterArrivalTime(randomInterArrival);
                /* fun that get the interarrival based on interArrivalRandom in which range (third col)*/
                int arrivalTime = previousArrivalTime + interArrival;

                int randomService = GenerateRandomValue();
                Console.WriteLine("before select server");
                Server assignedServer = SelectServer(arrivalTime, randomService, SelectionMethod);
                Console.WriteLine("after select server");

                int serviceTime = GetServiceTime(assignedServer, randomService); // return service time based on which server and its random service
                assignedServer.TotalWorkingTime += serviceTime;

                int startTime = Math.Max(arrivalTime, assignedServer.FinishTime); // Customer starts when the server is free
                int endTime = startTime + serviceTime;
                int timeInQueue = startTime - arrivalTime; // task -->nada edit

                // Update the server's finish time
                assignedServer.FinishTime = endTime;
                Console.WriteLine("before set each case");
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
                    timeInQueue: timeInQueue // task must be calculated for each customer delay in queue
                );

                // end of each case

                // set total customers waiting in queue (TCWIQ) //nada edit
                if (timeInQueue > 0)
                {
                    TCWIQ = TCWIQ + 1;
                    // set total time customers waited in queue (TTCWIQ) //nada edit
                    TTCWIQ = TTCWIQ + timeInQueue;
                }
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

        /*task*/
        private Server SelectServer(int arrivalTime, int RandomService, Enums.SelectionMethod selectionMethod)
        {
            // must choose server based on there is only one idle then choose it
            // or more than one idle server then choose based on selection method'

            /*
             * count_idel => to count the idle server 
             * sever_index => to store the indces of idel servers
             * index the index => of choosen sever
             */
            Console.WriteLine("inside function gehad");
            int count_idle = 0, index = -1;
            int[] server_index = new int[Servers.Count];

            for (int i = 0; i < Servers.Count; i++)
            {
                if (Servers[i].FinishTime <= arrivalTime)
                {
                    server_index[count_idle] = i;
                    count_idle++;
                }
            }
            Console.WriteLine("after first forloop");
            do
            {

                if (count_idle >= 2)
                {
                    //Highest Priority
                    /*
                        the sever which take minimum time to finish this task
                    *min_time => minimum time to finish the task
                    *length => length of the range of each server
                    *time => temp variable
                    */
                    int min_time = int.MaxValue;
                    if (selectionMethod.Equals(1))
                    {
                        int length, time = 0;
                        for (int i = 0; i < server_index.Length; i++)
                        {
                            length = Servers[server_index[i]].TimeDistribution.Count;
                            for (int j = 0; j < length; j++)
                            {
                                if (RandomService >= Servers[server_index[i]].TimeDistribution[j].MinRange && RandomService <= Servers[server_index[i]].TimeDistribution[j].MaxRange)
                                {
                                    time = Servers[server_index[i]].TimeDistribution[j].Time;
                                    break;
                                }
                            }
                            if (time < min_time)
                            {
                                min_time = time;
                                index = server_index[i];
                            }
                        }
                    }
                    //random
                    else if (selectionMethod.Equals(2))
                    {
                        Random rnd = new Random();
                        int number = rnd.Next(0, count_idle);
                        index = server_index[number];
                    }
                    /* //Least utilization
                        After calculating the time each server worked
                        the task will assigned to the least time 
                    */
                    else if (selectionMethod.Equals(3))
                    {
                        for (int i = 0; i < server_index.Length; i++)
                        {
                            if (Servers[server_index[i]].TotalWorkingTime < min_time)
                            {
                                min_time = Servers[server_index[i]].TotalWorkingTime;
                                index = server_index[i];
                            }
                        }
                    }
                }
                else if (count_idle == 1)
                {
                    index = server_index[0];
                }
                else
                {
                    // min_finish => time when the first server will finish
                    int min_finish = int.MaxValue;
                    for (int i = 0; i < Servers.Count; i++)
                    {
                        if (Servers[i].FinishTime < min_finish)
                        {
                            min_finish = Servers[i].FinishTime;
                            server_index[count_idle] = i;
                            count_idle++;
                        }
                    }
                }

            } while (index == -1);
            Console.WriteLine("after do while loop");

            return Servers[index];
        }
        public PerformanceMeasures SetPerformance_Measures(PerformanceMeasures PerformanceMeasures, int TCWIQ, int TTCWIQ)
        {

            PerformanceMeasures.AverageWaitingTime = TTCWIQ / StoppingNumber;
            PerformanceMeasures.WaitingProbability = TCWIQ / StoppingNumber;
            return PerformanceMeasures;
        }
        public int MaxQueueLength(SimulationSystem SysDone)
        {
            SimulationSystem newsyscopy = new SimulationSystem();
            int length = 0;
            int max = 0;

            for (int i = 0; i < SysDone.SimulationTable.Count(); i++)
            {
                if (SysDone.SimulationTable[i].TimeInQueue > 0)
                {
                    queueCust.Enqueue(SysDone.SimulationTable[i]);
                }
            }
            if (queueCust.Count() > 0)
            {
                max = 1;
            }
            for (int x = 0; x < queueCust.Count(); x++)
            {
                newsyscopy.SimulationTable.Add(new SimulationCase());

            }
            for (int z = 0; z < newsyscopy.SimulationTable.Count(); z++)
            {

                newsyscopy.SimulationTable[z] = queueCust.Dequeue();

            }
            for (int q = 0; q < newsyscopy.SimulationTable.Count(); q++)
            {
                int counter = 0;
                if (q == 0)
                {
                    length++;
                }
                else
                {
                    for (int j = q - 1; j >= 0; j--)
                    {

                        if (newsyscopy.SimulationTable[q].ArrivalTime < newsyscopy.SimulationTable[j].StartTime)
                        {
                            counter++;
                        }
                    }
                    if (counter >= length)
                    {
                        length++;
                        if (length > max)
                        {
                            max = length;
                        }
                    }
                }

            }

            // SysDone.PerformanceMeasures.MaxQueueLength = max;
            PerformanceMeasures.MaxQueueLength = max;
            return max;
        }
        public void CalculateServerPerformanceMetrics()
        {

            int maxSimulatedTime = 0;
            int totalServiceTime = 0;
            int customerCountForServer = 0;
            int idleTime;


            for (int i = 0; i < SimulationTable.Count; i++)
            {
                if (SimulationTable[i].EndTime > maxSimulatedTime)
                    maxSimulatedTime = SimulationTable[i].EndTime;
            }

            for (int i = 0; i < NumberOfServers; i++)
            {
                for (int r = 0; r < SimulationTable.Count; r++)
                {
                    if (SimulationTable[r].AssignedServer.ID == i + 1)
                    {
                        totalServiceTime += SimulationTable[r].ServiceTime;
                        customerCountForServer++;
                    }
                }

                idleTime = maxSimulatedTime - totalServiceTime;

                if (customerCountForServer < 1)
                {
                    customerCountForServer = 1;

                }

                Servers[i].AverageServiceTime = totalServiceTime / customerCountForServer;
                Servers[i].Utilization = totalServiceTime / maxSimulatedTime;
                Servers[i].IdleProbability = idleTime / maxSimulatedTime;
            }
        }
    }
}
