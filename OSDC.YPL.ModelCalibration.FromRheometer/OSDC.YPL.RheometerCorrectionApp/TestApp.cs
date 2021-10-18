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
        public TestApp()
        {
            InitializeComponent();

            PlotFigure2();
            PlotFigure4();
            PlotIntegral8();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void PlotIntegral8( double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5)
        {
            var myModel = new PlotModel() { Title = "Integral 8" };

            var s1 = new LineSeries() { Title = "n = 0.5" };
            for (int i = 0; i < 100; i++)
            {
                double c = .001 + (double)i * 0.001 / 99;
                s1.Points.Add(new DataPoint(c, OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationEquation8(c, r1 / r2, n, tau_y, r2)));
            }
            myModel.Series.Add(s1);

            double[] velocities = { 3.0, 6.0, 30, 60, 100, 200, 300 };
            velocities = velocities.Select(d => d / 60.0).ToArray();



            for (int i = 0; i < velocities.Length; i++)
            {
                double y = velocities[i] * System.Math.Pow(k / tau_y, 1.0 / n);

                var s = new LineSeries() { Title = "RPM = " + (velocities[i] * 60.0).ToString("F0") };
                s.Points.Add(new DataPoint(.001, y));
                s.Points.Add(new DataPoint(.002, y));

                double kappa = r1 / r2;
                double yn = 2 * k * velocities[i] * kappa * kappa*r2*r2 / (1 - kappa * kappa);

                OxyPlot.Series.ScatterSeries ss = new ScatterSeries();
                ss.Points.Add(new ScatterPoint(yn, velocities[i] * System.Math.Pow(k / tau_y, 1.0 / n)));

                myModel.Series.Add(s);
                myModel.Series.Add(ss);
            }

            myModel.Legends.Add(new OxyPlot.Legends.Legend());
            integral8PlotView.Model = myModel;
        }

        private void PlotFigure2()
        {
            var myModel = new PlotModel() { Title = "Integration curves" };


            var s1 = new LineSeries() { Title = "n = 0.5" };

            GenerateIntegralCurve(.5, out double[] res, out double[] x);
            for (int i = 0; i < res.Length; i++)
            {
                s1.Points.Add(new OxyPlot.DataPoint(x[i], res[i]));
                Console.WriteLine(res[i]);
            }

            myModel.Series.Add(s1);
            s1 = new LineSeries() { Title = "n = 0.75" };
            Console.WriteLine();

            GenerateIntegralCurve(.75, out res, out x);
            for (int i = 0; i < res.Length; i++)
            {
                s1.Points.Add(new OxyPlot.DataPoint(x[i], res[i]));
                Console.WriteLine(res[i]);
            }
            myModel.Series.Add(s1);
            s1 = new LineSeries() { Title = "n = 1.0" };

            GenerateIntegralCurve(1.0, out res, out x);
            for (int i = 0; i < res.Length; i++)
            {
                s1.Points.Add(new OxyPlot.DataPoint(x[i], res[i]));
                Console.WriteLine(res[i]);
            }
            myModel.Series.Add(s1);

            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "f(kappa, n)" });
            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Kappa" });

            myModel.Legends.Add(new OxyPlot.Legends.Legend());

            figure2PlotView.Model = myModel;
        }


        private void PlotFigure3()
        {
            double[] velocities = { 3.0, 6.0, 30, 60, 100, 200, 300 };
            velocities = velocities.Select(d => d  / 60.0).ToArray();


            var myModel = new PlotModel() { Title = "Integration curves" };


            var s1 = new LineSeries() { Title = "n = 0.5" };

            GenerateIntegralCurve(.5, out double[] res, out double[] x);
            for (int i = 0; i < res.Length; i++)
            {
                s1.Points.Add(new OxyPlot.DataPoint(x[i], res[i]));
                Console.WriteLine(res[i]);
            }
        }

        private void GenerateIntegralCurve(double n, out double[] result,out double[] xs,  double kappaStart = .2,double kappaStop = 1.0,  int nbOfPoints = 100)
        {
            result = new double[nbOfPoints];
            xs = new double[nbOfPoints];
            double step = (kappaStop - kappaStart) / (nbOfPoints-1);
            for (int i = 0; i < nbOfPoints; i++)
            {
                double kappa = kappaStart + i * step;
                xs[i] = kappa;
                result[i] = OSDC.YPL.RheometerCorrection.ShearRateCorrection.IntegrationFKappaN(kappa, n);
            }
        }


        private void PlotFigure4()
        {
            double[] velocities = { 3.0, 6.0, 30, 60, 100, 200, 300 , 600};
            velocities = velocities.Select(d => d / 60.0).ToArray();


            var myModel = new PlotModel() { Title = "Integration curves" };


            var s1 = new LineSeries() { Title = "Ratio: n = 0.5" };
            var s2 = new LineSeries() { Title = "Newtonian rates: n = 0.5" };
            var s3 = new LineSeries() { Title = "Non-Newtonian rates: n = 0.5" };

            GenerateFigure4(1.0 / 60.0, 600.0 / 60.0, out double[] xs, out double[] ratios, out double[] newtonianShearRates, out double[] nonNewtonianShearRates);
         
            for (int i = 0; i < ratios.Length; i++)
            {
                s1.Points.Add(new OxyPlot.DataPoint(xs[i], ratios[i]));
                s2.Points.Add(new OxyPlot.DataPoint(xs[i], newtonianShearRates[i]));
                s3.Points.Add(new OxyPlot.DataPoint(xs[i], nonNewtonianShearRates[i]));
            }

            

            myModel.Series.Add(s1);
            myModel.Axes.Add(new OxyPlot.Axes.LogarithmicAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom });
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis(){Position = OxyPlot.Axes.AxisPosition.Left, Key = "YAXIS1" });
            myModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Right, Key = "YAXIS2" });

            s1.YAxisKey = "YAXIS1";
            s2.YAxisKey = "YAXIS2";
            s3.YAxisKey = "YAXIS2";

            myModel.Series.Add(s2);
            myModel.Series.Add(s3);
            myModel.Legends.Add(new OxyPlot.Legends.Legend());

            plotView1.Model = myModel;
        }

        private void GenerateFigure4(double minRPM , double maxrpm, out double[] xs, out double[] ratios, out double[] newtonianRates, out double[] nonNewtonianRates, double r1 = .017245, double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5, int nbOfPoints = 100)
        {
            xs = new double[nbOfPoints];
            ratios = new double[nbOfPoints];
            newtonianRates = new double[nbOfPoints];
            nonNewtonianRates = new double[nbOfPoints];
            for (int i = 0; i < 100; i++)
            {
                double omega = minRPM + i * (maxrpm - minRPM) / 99;

                double sr = OSDC.YPL.RheometerCorrection.ShearRateCorrection.GetShearRate(r1, r2, k, n, tau_y, omega);
                double nsr = OSDC.YPL.RheometerCorrection.ShearRateCorrection.GetNewtonianShearRate(omega,  r1 / r2);
                xs[i] = 60* omega;
                ratios[i] = sr / nsr;
                newtonianRates[i] = nsr;
                nonNewtonianRates[i] = sr;
            }
        }
        private void GenerateAngularVelocityCurve(double rpm,out double[] xs, out double[] normalizedVelocities, double r1 = .017245,  double r2 = 0.018415, double tau_y = 5.0, double k = .1, double n = .5, int nbOfPoints = 100)
        {
            normalizedVelocities = new double[nbOfPoints];
            xs = new double[nbOfPoints];
            double step = 1.0 / (nbOfPoints - 1);
            for (int i = 0; i < nbOfPoints; i++)
            {
                double kappa =( r1 + i* (r2 - r1) / 99) / r2;
                xs[i] = step;
                normalizedVelocities[i] = RheometerCorrection.ShearRateCorrection.FindMinimumRotationalVelocity(tau_y, k, kappa, n);
            }
        }


        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
