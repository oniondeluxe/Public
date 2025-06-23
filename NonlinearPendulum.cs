using System;
using MathNet.Numerics;
using MathNet.Numerics.SpecialFunctions;

class NonlinearPendulum
{
    static void Main()
    {
        // Pendulum parameters
        double L = 1.0;               // Length (m)
        double g = 9.81;              // Gravity (m/s^2)
        double theta0 = Math.PI / 2; // Initial angle (radians, large amplitude)

        double k = Math.Sin(theta0 / 2);  // Modulus for elliptic functions
        double omega = Math.Sqrt(g / L);

        // Complete elliptic integral of the first kind
        double K = SpecialFunctions.EllipticK(k * k);
        double T = 4 * K / omega;

        Console.WriteLine($"Large-amplitude period T = {T:F4} s");

        // Simulate for one period
        int steps = 200;
        double dt = T / steps;

        for (int i = 0; i <= steps; i++)
        {
            double t = i * dt;
            double u = omega * t;

            // Compute Jacobi elliptic function sn(u, k)
            var sn = MathNet.Numerics.SpecialFunctions.JacobiEllipticFunctions.sn(u, k);

            // Compute theta(t)
            double theta = 2 * Math.Asin(k * sn);

            Console.WriteLine($"t = {t:F3} s, theta = {theta:F5} rad");
        }
    }
}



using System;
using System.Windows.Forms;
using MathNet.Numerics;
using System.Windows.Forms.DataVisualization.Charting;

namespace PendulumPlot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PlotPendulum();
        }

        private void PlotPendulum()
        {
            // Parameters
            double L = 1.0;               // Length of pendulum (m)
            double g = 9.81;              // Acceleration due to gravity (m/s^2)
            double theta0 = Math.PI / 2;  // Initial angle (radians)

            double k = Math.Sin(theta0 / 2);
            double omega = Math.Sqrt(g / L);
            double K = SpecialFunctions.EllipticK(k * k); // Complete elliptic integral
            double T = 4 * K / omega;

            int steps = 500;
            double dt = T / steps;

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            ChartArea chartArea = new ChartArea("PendulumArea");
            chart.ChartAreas.Add(chartArea);

            Series series = new Series("θ(t)")
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "PendulumArea"
            };

            for (int i = 0; i <= steps; i++)
            {
                double t = i * dt;
                double u = omega * t;
                double sn = MathNet.Numerics.SpecialFunctions.JacobiEllipticFunctions.sn(u, k);
                double theta = 2 * Math.Asin(k * sn);

                series.Points.AddXY(t, theta);
            }

            chart.Series.Add(series);
            Controls.Add(chart);
        }
    }
}
