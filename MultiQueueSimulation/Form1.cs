using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiQueueModels;
using MultiQueueTesting;

namespace MultiQueueSimulation
{
    public partial class Form1 : Form
    {
        public List<SimulationCase> SimulationTable { get; set; }
        public Form1(SimulationSystem system)
        {
            InitializeComponent();
            this.SimulationTable = system.SimulationTable;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            table.Columns.Add("CustomerNumber", typeof(int));
            table.Columns.Add("RandomInterArrival", typeof(int));
            table.Columns.Add("InterArrival", typeof(int));
            table.Columns.Add("ArrivalTime", typeof(int));
            table.Columns.Add("RandomService", typeof(int));
            table.Columns.Add("AssignedServer", typeof(int));
            table.Columns.Add("StartTime", typeof(int));
            table.Columns.Add("EndTime", typeof(int));
            table.Columns.Add("TimeInQueue", typeof(int));

            // Fill the DataTable with data from SimulationTable
            foreach (var simCase in SimulationTable)
            {
                table.Rows.Add(
                    simCase.CustomerNumber,
                    simCase.RandomInterArrival,
                    simCase.InterArrival,
                    simCase.ArrivalTime,
                    simCase.RandomService,
                    simCase.AssignedServer.ID,  // assuming AssignedServer has an ID property
                    simCase.StartTime,
                    simCase.EndTime,
                    simCase.TimeInQueue
                );
            }

            /*table.Rows.Add(1, 10, 10, 10, 10, 10, 10, 10, 10);
            table.Rows.Add(2, 20, 20, 20, 10, 10, 10, 10, 10);
            table.Rows.Add(3, 30, 30, 30, 10, 10, 10, 10, 10);
            table.Rows.Add(4, 40, 40, 40, 10, 10, 10, 10, 10);
            table.Rows.Add(5, 50, 50, 50, 10, 10, 10, 10, 10);
            table.Rows.Add(6, 60, 60, 60, 10, 10, 10, 10, 10);
            table.Rows.Add(7, 70, 70, 70, 10, 10, 10, 10, 10);
            table.Rows.Add(8, 80, 80, 80, 10, 10, 10, 10, 10);*/

            dataGridView1.DataSource = table;
        }
    }
}