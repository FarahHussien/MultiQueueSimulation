using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueTesting;
using MultiQueueModels;

namespace MultiQueueSimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        //[STAThread]
        public static void Main()
        {
            SimulationSystem system = new SimulationSystem();
            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);

            system.RunSimulation();
            MessageBox.Show(result);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Console.WriteLine(1);
        }
    }
}
