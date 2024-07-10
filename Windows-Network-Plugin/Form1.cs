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
            chart1.ChartAreas[0].AxisX.Title = "Time";
            chart1.ChartAreas[0].AxisY.Title = "Bytes per Second";

            var seriesSent = new Series
            {
                Name = "Bytes Sent",
                ChartType = SeriesChartType.Line
            };
            chart1.Series.Add(seriesSent);

            var seriesReceived = new Series
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

            chart1.Series["Bytes Sent"].Points.AddY(bytesSentPerSecond);
            chart1.Series["Bytes Received"].Points.AddY(bytesReceivedPerSecond);

            if (chart1.Series["Bytes Sent"].Points.Count > 60) // Keep only 60 points
            {
                chart1.Series["Bytes Sent"].Points.RemoveAt(0);
                chart1.Series["Bytes Received"].Points.RemoveAt(0);
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
