using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDC.YPL.RheometerCorrectionApp
{
    public partial class TestApp : Form
    {
        public TestApp()
        {
            InitializeComponent();

            var myModel = new OxyPlot.PlotModel() { Title = "Integration curves" };


            var s1 = new OxyPlot.Series.LineSeries();
            

            //myModel.Series.Add()


            GenerateIntegralCurve(.5, out double[] res, out double[] x);
            for (int i = 0; i < res.Length; i++)
            {
                Console.WriteLine(res[i]);
            }

            Console.WriteLine();

            GenerateIntegralCurve(.75,  out res, out x);
            for (int i = 0; i < res.Length; i++)
            {
                Console.WriteLine(res[i]);
            }

            GenerateIntegralCurve(1.0, out res, out x);
            for (int i = 0; i < res.Length; i++)
            {
                Console.WriteLine(res[i]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
    }
}
