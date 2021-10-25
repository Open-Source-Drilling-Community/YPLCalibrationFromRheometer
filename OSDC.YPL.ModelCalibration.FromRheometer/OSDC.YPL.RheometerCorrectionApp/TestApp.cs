using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;

namespace OSDC.YPL.RheometerCorrectionApp
{
    public partial class TestApp : Form
    {
        public const double RPM_TO_RADIAN_PER_SEC = Math.PI / 30.0;
        public enum Parameter { n, tau_y, k };

        public TestApp()
        {
            InitializeComponent();

            PlotFigure2();
            PlotFigure3();
            PlotFigure4();
            PlotFigure5();
            PlotFigure6();
            PlotIntegral8();
            PlotIntegral9();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PlotIntegral8(double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5)
        {
            var myModel = new PlotModel() { Title = "Graphical solution of Eq. (8) in C" };

            double[] flowIndex = { 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1 };
            int nbOfIntervals = 200;
            for (int j = 0; j < flowIndex.Length; j++)
            {
                var s1 = new LineSeries() { Title = $"n = {flowIndex[j]}" };
                for (int i = 0; i < nbOfIntervals; i++)
                {
                    double c = .0005 + (double)i * 0.002 / (nbOfIntervals - 1);
                    double integral = OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationEquation8(c, r1 / r2, flowIndex[j], tau_y, r2);
                    s1.Points.Add(new DataPoint(c, integral));
                }
                myModel.Series.Add(s1);
            }

            double[] velocities = { 1, 3, 6, 10, 20, 30, 60, 100, 200, 300 };
            velocities = velocities.Select(d => d * RPM_TO_RADIAN_PER_SEC).ToArray();
            for (int i = 0; i < velocities.Length; i++)
            {
                double y = velocities[i] * System.Math.Pow(k / tau_y, 1.0 / n);

                var s = new LineSeries() { Title = "RPM = " + (velocities[i] / RPM_TO_RADIAN_PER_SEC).ToString("F0") };
                double cMin = 2.0 * tau_y * r1 * r1 * Math.Log(r2 / r1) / (1 - r1 * r1 / (r2 * r2)); // min value of c for the case n=0.5
                double cDef = r2 * r2 * tau_y; // lower bound of the interval where the integral is defined for non-integer values of 1/n
                s.Points.Add(new DataPoint(cMin, y));
                s.Points.Add(new DataPoint(cDef, y));

                double kappa = r1 / r2;
                double yn = 2 * k * velocities[i] * kappa * kappa * r2 * r2 / (1 - kappa * kappa);

                OxyPlot.Series.ScatterSeries ss = new ();
                ss.Points.Add(new ScatterPoint(yn, velocities[i] * System.Math.Pow(k / tau_y, 1.0 / n)));

                myModel.Series.Add(s);
                myModel.Series.Add(ss);
            }
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Integration constant C (Pa/m^2)" });
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "(kappa/tau_y)^(1/n) * Omega (SI)" });

            myModel.Legends.Add(new OxyPlot.Legends.Legend());
            integral8PlotView.Model = myModel;
        }

        private void PlotIntegral9(double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5)
        {
            var myModel = new PlotModel() { Title = "Graphical solution of Eq. (9) in Rplug" };

            double[] velocities = { 1, 3, 6, 10, 20, 30, 60, 100, 200, 300 };
            velocities = velocities.Select(d => d * RPM_TO_RADIAN_PER_SEC).ToArray();

            int nbOfIntervals = 100;
            var s1 = new LineSeries() { Title = $"n = {n}" };
            for (int i = 0; i < nbOfIntervals; i++)
            {
                double rp = r1 + (double)i / (nbOfIntervals - 1) * (r2 - r1);
                double integral = OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationEquation9(rp, r1 / r2, n, r2);
                s1.Points.Add(new DataPoint(rp, integral));
            }
            myModel.Series.Add(s1);

            for (int l = 0; l < velocities.Length; l++)
            {
                double y = velocities[l] * System.Math.Pow(k / tau_y, 1.0 / n);
                var s = new LineSeries() { Title = "RPM = " + (velocities[l] / RPM_TO_RADIAN_PER_SEC).ToString("F0") };
                s.Points.Add(new DataPoint(r1, y));
                s.Points.Add(new DataPoint(r2, y));
                s.LineStyle = LineStyle.Dash;
                myModel.Series.Add(s);
            }

            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Radius (m)" });
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "(kappa/tau_y)^(1/n) * Omega (SI)" });
            myModel.Legends.Add(new OxyPlot.Legends.Legend());
            integral9plotView.Model = myModel;
        }

        private void PlotFigure2()
        {
            var myModel = new PlotModel() { Title = "Evaluation of the integral in Eq. (7)" };

            double[] flowIndex = { 0.5, 0.75, 1.0 };
            for (int j = 0; j < flowIndex.Length; j++)
            {
                var s1 = new LineSeries() { Title = $"n = {flowIndex[j]}" };

                GenerateIntegralCurve(flowIndex[j], out double[] res, out double[] x);
                for (int i = 0; i < res.Length; i++)
                {
                    s1.Points.Add(new OxyPlot.DataPoint(x[i], res[i]));
                }

                myModel.Series.Add(s1);
            }

            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "f(kappa, n)" });
            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Kappa" });

            myModel.Legends.Add(new OxyPlot.Legends.Legend());

            figure2PlotView.Model = myModel;
        }


        private void PlotFigure3()
        {
            double r1 = .017245;
            double r2 = 0.018415;
            double[] velocities = { 3, 6, 60, 100, 200, 300 };
            velocities = velocities.Select(d => d * RPM_TO_RADIAN_PER_SEC).ToArray();

            var myModel = new PlotModel() { Title = "Figure 3: evaluation of the normalized rotational velocity in the gap from Eq. (6)\n(markers indicate transition to yielded outer cylinder when relevant)" };

            var s2 = new LineSeries() { Title = "Newtonian result" };

            for (int j = 0; j < velocities.Length; ++j)
            {
                var s1 = new LineSeries() { Title = $"RPM = {velocities[j] / RPM_TO_RADIAN_PER_SEC}" };


                GenerateFigure3(velocities[j], out double[] res, out double[] xs, out int fullyShearedIdx, r1, r2);

                for (int i = 0; i < res.Length; i++)
                {
                    s1.Points.Add(new OxyPlot.DataPoint((xs[i] - r1) / (r2 - r1), res[i] / velocities[j]));
                    if (j == 0)
                    {
                        s2.Points.Add(new OxyPlot.DataPoint((xs[i] - r1) / (r2 - r1), (1 - r1 * r1 / (xs[i] * xs[i])) / (1 - r1 * r1 / (r2 * r2))));
                    }

                }
                myModel.Series.Add(s1);

                if (fullyShearedIdx < xs.Length - 1)
                {
                    var s1b = new ScatterSeries();
                    s1b.Points.Add(new ScatterPoint((xs[fullyShearedIdx] - r1) / (r2 - r1), res[fullyShearedIdx] / velocities[j]));
                    myModel.Series.Add(s1b);
                }
            }

            myModel.Series.Add(s2);

            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Normalized angular velocity, omega(r) / OMEGA" });
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Position in gap, (r - R1)/(R2 - R1)" });

            myModel.Legends.Add(new OxyPlot.Legends.Legend());

            plotView2.Model = myModel;
        }

        private static void GenerateFigure3(double omega, out double[] result, out double[] xs, out int fullyShearedIdx, double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5, int nbOfPoints = 100)
        {
            result = new double[nbOfPoints];
            xs = new double[nbOfPoints];

            double minV = OSDC.YPL.RheometerCorrection.ShearRateCorrection.FindMinimumRotationalVelocity(tau_y, k, r1 / r2, n);
            double rPlug = -999.25;
            double c = -999.25;
            fullyShearedIdx = 0;

            if (omega > minV)
                c = OSDC.YPL.RheometerCorrection.ShearRateCorrection.CalculateC(k, omega, tau_y, n, r1, r2);
            if (omega <= minV)
            {
                rPlug = OSDC.YPL.RheometerCorrection.ShearRateCorrection.CalculateRPlug(k, omega, tau_y, n, r1, r2);
                c = tau_y * rPlug * rPlug;
            }

            double step = (r2 - r1) / (nbOfPoints - 1);
            for (int i = 0; i < nbOfPoints; i++)
            {
                double r = r1 + i * step;
                xs[i] = r;

                if ((omega <= minV & r < rPlug) | omega > minV)
                {
                    result[i] = OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationEquation6(c, k, tau_y, n, r1, r);
                    fullyShearedIdx = i;
                }
                else
                {
                    result[i] = omega;
                }
            }
        }

        private static void GenerateIntegralCurve(double n, out double[] result, out double[] xs, double kappaStart = .2, double kappaStop = 1.0, int nbOfPoints = 100)
        {
            result = new double[nbOfPoints];
            xs = new double[nbOfPoints];
            double step = (kappaStop - kappaStart) / (nbOfPoints - 1);
            for (int i = 0; i < nbOfPoints; i++)
            {
                double kappa = kappaStart + i * step;
                xs[i] = kappa;
                result[i] = OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationFKappaN(kappa, n);
            }
        }

        private void PlotFigure4()
        {
            String title = "Figure 4: ratio of wall shear rates (HB/N) for different flow behavior indices\n(markers indicate transition to fully sheared regime when relevant)";
            double[] paramValues = { 0.5, 0.75, 1.0 };
            PlotFigure456(title, Parameter.n, paramValues);
        }

        private void PlotFigure5()
        {
            String title = "Figure 5: ratio of wall shear rates (HB/N) for different yield stresses\n(markers indicate transition to fully sheared regime when relevant)";
            double[] paramValues = { 10.0, 5.0, 2.5 };
            PlotFigure456(title, Parameter.tau_y, paramValues);
        }

        private void PlotFigure6()
        {
            String title = "Figure 6: ratio of wall shear rates (HB/N) for different consistency indices\n(markers indicate transition to fully sheared regime when relevant)";
            double[] paramValues = { 0.1, 0.25, 0.5 };
            PlotFigure456(title, Parameter.k, paramValues);
        }

        private void PlotFigure456(String title, Parameter p, double[] pValues)
        {
            double minRPM = 1;
            double maxRPM = 300;

            var myModel = new PlotModel() { Title = $"{title}" };

            for (int j = 0; j < pValues.Length; j++)
            {
                var s1 = new LineSeries() { Title = $"{p} = {pValues[j]}" };
                var s1b = new ScatterSeries();

                double[] xs = null;
                double[] ratios = null;
                int nbOfPoints = 500;
                int fullyShearedIdx = nbOfPoints - 1;

                switch (p)
                {
                    case Parameter.n:
                        GenerateFigure456(minRPM, maxRPM, nbOfPoints, out xs, out ratios, out fullyShearedIdx, n: pValues[j]);
                        break;
                    case Parameter.tau_y:
                        GenerateFigure456(minRPM, maxRPM, nbOfPoints, out xs, out ratios, out fullyShearedIdx, tau_y: pValues[j]);
                        break;
                    case Parameter.k:
                        GenerateFigure456(minRPM, maxRPM, nbOfPoints, out xs, out ratios, out fullyShearedIdx, k: pValues[j]);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < ratios.Length; i++)
                {
                    s1.Points.Add(new DataPoint(xs[i], ratios[i]));
                }
                myModel.Series.Add(s1);

                if (fullyShearedIdx > 0)
                {
                    s1b.Points.Add(new ScatterPoint(xs[fullyShearedIdx], ratios[fullyShearedIdx]));
                    myModel.Series.Add(s1b);
                }
            }
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Wall shear rate ratio" });
            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Revolutions per minute (1/min)" });

            myModel.Legends.Add(new OxyPlot.Legends.Legend());

            switch (p)
            {
                case Parameter.n:
                    plotView1.Model = myModel;
                    break;
                case Parameter.tau_y:
                    plotView3.Model = myModel;
                    break;
                case Parameter.k:
                    plotView4.Model = myModel;
                    break;
                default:
                    break;
            }
        }

        private static void GenerateFigure456(double minRPM, double maxRPM, int nbOfPoints, out double[] xs, out double[] ratios, out int fullyShearedIdx, double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5)
        {
            xs = new double[nbOfPoints];
            ratios = new double[nbOfPoints];
            fullyShearedIdx = nbOfPoints - 1;
            double minOmega = RPM_TO_RADIAN_PER_SEC * minRPM;
            double maxOmega = RPM_TO_RADIAN_PER_SEC * maxRPM;

            for (int i = 0; i < nbOfPoints; i++)
            {
                double omega = minOmega + i * (maxOmega - minOmega) / (nbOfPoints - 1);
                double sr = OSDC.YPL.RheometerCorrection.ShearRateCorrection.GetShearRate(r1, r2, k, n, tau_y, omega, out bool isFullySheared);
                if (isFullySheared & fullyShearedIdx == nbOfPoints - 1)
                    fullyShearedIdx = i;
                double nsr = OSDC.YPL.RheometerCorrection.ShearRateCorrection.GetNewtonianShearRate(omega, r1 / r2);
                xs[i] = omega / RPM_TO_RADIAN_PER_SEC;
                ratios[i] = sr / nsr;
            }
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
