using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Windows_Network_Plugin
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private long previousBytesSent;
        private long previousBytesReceived;

        public Form1()
        {
            InitializeComponent();
            InitializeChart();
            InitializeTimer();
        }

        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // Add Chart Area
            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Time";
            chartArea.AxisY.Title = "Bytes per Second";
            chart1.ChartAreas.Add(chartArea);

            // Add Series for Bytes Sent
            Series seriesSent = new Series
            {
                Name = "Bytes Sent",
                ChartType = SeriesChartType.Line
            };
            chart1.Series.Add(seriesSent);

            // Add Series for Bytes Received
            Series seriesReceived = new Series
            {
                Name = "Bytes Received",
                ChartType = SeriesChartType.Line
            };
            chart1.Series.Add(seriesReceived);
        }

        private void InitializeTimer()
        {
            timer = new Timer
            {
                Interval = 1000 // 1 second
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0]; // Get the first network interface
            var statistics = networkInterface.GetIPv4Statistics();

            long bytesSentPerSecond = statistics.BytesSent - previousBytesSent;
            long bytesReceivedPerSecond = statistics.BytesReceived - previousBytesReceived;

            previousBytesSent = statistics.BytesSent;
            previousBytesReceived = statistics.BytesReceived;

            // Update chart
            UpdateChart("Bytes Sent", bytesSentPerSecond);
            UpdateChart("Bytes Received", bytesReceivedPerSecond);
        }

        private void UpdateChart(string seriesName, long value)
        {
            var series = chart1.Series[seriesName];
            series.Points.AddY(value);

            if (series.Points.Count > 60) // Keep only 60 points
            {
                series.Points.RemoveAt(0);
            }

            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true; // Keep the window on top
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10, Screen.PrimaryScreen.WorkingArea.Height - this.Height - 10); // Position near the taskbar
        }
    }
}
