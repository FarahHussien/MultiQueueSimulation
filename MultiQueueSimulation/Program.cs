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
            string testCase1 = "D:\\Materials FCIS\\Year 4\\Semester 7\\Modeling\\Labs\\Lab 2_Task1\\Template_Students\\MultiQueueSimulation\\MultiQueueSimulation\\TestCases\\TestCase1.docx";
            string testCase2 = "D:\\Materials FCIS\\Year 4\\Semester 7\\Modeling\\Labs\\Lab 2_Task1\\Template_Students\\MultiQueueSimulation\\MultiQueueSimulation\\TestCases\\TestCase2.docx";
            string testCase3 = "D:\\Materials FCIS\\Year 4\\Semester 7\\Modeling\\Labs\\Lab 2_Task1\\Template_Students\\MultiQueueSimulation\\MultiQueueSimulation\\TestCases\\TestCase3.docx";

            SimulationSystem system = new SimulationSystem();

            ReadFiles readFiles = new ReadFiles();
            readFiles.ParseFile(testCase1);

            system.InitializeSystem(readFiles);
            system.RunSimulation();

            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            MessageBox.Show(result);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(system));

            Console.WriteLine(1);
        }
    }
}
