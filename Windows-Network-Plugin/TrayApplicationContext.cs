using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Windows_Network_Plugin
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Timer timer;
        private long previousBytesSent;
        private long previousBytesReceived;

        public TrayApplicationContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };

            // Add menu items to context menu
            trayIcon.ContextMenuStrip.Items.Add("Show Network Traffic", null, ShowNetworkTraffic);
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

            // Initialize Timer
            timer = new Timer
            {
                Interval = 1000 // 1 second
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTrayIcon();
        }

        private void UpdateTrayIcon()
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0]; // Get the first network interface
            var statistics = networkInterface.GetIPv4Statistics();

            long bytesSentPerSecond = statistics.BytesSent - previousBytesSent;
            long bytesReceivedPerSecond = statistics.BytesReceived - previousBytesReceived;

            previousBytesSent = statistics.BytesSent;
            previousBytesReceived = statistics.BytesReceived;

            // Create the graph as a Bitmap
            Bitmap bitmap = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);

                // Draw the graph
                Pen sentPen = new Pen(Color.Blue, 2);
                Pen receivedPen = new Pen(Color.Red, 2);

                // Simulate the graph drawing (this should be replaced with actual drawing code)
                g.DrawLine(sentPen, new Point(0, 32), new Point(32, 32 - (int)(bytesSentPerSecond % 32)));
                g.DrawLine(receivedPen, new Point(0, 32), new Point(32, 32 - (int)(bytesReceivedPerSecond % 32)));

                sentPen.Dispose();
                receivedPen.Dispose();
            }

            // Update the tray icon
            IntPtr hIcon = bitmap.GetHicon();
            try
            {
                trayIcon.Icon = Icon.FromHandle(hIcon);
            }
            finally
            {
                DestroyIcon(hIcon);
                bitmap.Dispose();
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        void ShowNetworkTraffic(object sender, EventArgs e)
        {
            // Show the network traffic window
            Form1 networkForm = new Form1();
            networkForm.Show();
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
