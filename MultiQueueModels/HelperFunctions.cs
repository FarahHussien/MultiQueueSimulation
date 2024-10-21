using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiQueueModels
{
    class HelperFunctions
    {
        // return the cumulative probability
        public static List<double> GetCumulativeSums(List<double> numbers)
        {
            List<double> cumulativeSums = new List<double>();
            double sum = 0;

            foreach (double number in numbers)
            {
                sum += number;
                cumulativeSums.Add(sum);
            }
            return cumulativeSums;
        }

        // return a list of random integers in range(1, maxNumberOfCustomers)
        public static List<int> GenerateRandomIntegers(int maxNumberOfCustomers)
        {
            List<int> randomIntegers = new List<int>();
            Random random = new Random();

            for (int i = 0; i < maxNumberOfCustomers; i++)
            {
                int randomValue = random.Next(1, maxNumberOfCustomers + 1);
                randomIntegers.Add(randomValue);
            }
            return randomIntegers;
        }

        // return a list of ranges
        public static List<double> GenerateRanges(double cumProbability)
        {
            return GenerateRanges(cumProbability);
        }
    }
}