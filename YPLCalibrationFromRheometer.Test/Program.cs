using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace YPLCalibrationFromRheometer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test(args);
        }

        static async void Test(string[] args)
        {
            string host = "https://app.DigiWells.no/";
            if (args != null && args.Length >= 1)
            {
                host = args[0];
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(host + "YPLCalibrationFromRheometer/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // test #1: read the IDs
            List<int> initialRheogramIDs;
            var a = client.GetAsync("Values");
            a.Wait();
            HttpResponseMessage responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialRheogramIDs = JsonConvert.DeserializeObject<List<int>>(str);
                    if (initialRheogramIDs != null)
                    {
                        Console.Write("Test #1: read IDs: success. IDs: ");
                        for (int i = 0; i < initialRheogramIDs.Count; i++)
                        {
                            Console.Write(initialRheogramIDs[i].ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Test #1: read IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #1: read IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #1: read IDs: failure a");
            }
            // test #2: post a new rheogram
            Rheogram rheogram = new Rheogram();
            double tau0 = 2.0;
            double K = 0.5;
            double n = 0.75;
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement();
                measurement.ShearRate = gammaDot;
                measurement.ShearStress = tau0 + K * System.Math.Pow(gammaDot, n);
                rheogram.Measurements.Add(measurement);
            }
            StringContent content = new StringContent(rheogram.GetJson(), Encoding.UTF8, "application/json");
            a = client.PostAsync("Values", content);
            a.Wait();
            HttpResponseMessage responseTaskPostRheogram = a.Result;
            if (responseTaskPostRheogram.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #2: post of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #2: post of rheogram: failure a");
            }
            // test #3: check that the new ID is present
            List<int> newRheogramIDs;
            a = client.GetAsync("Values");
            a.Wait();
            responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    newRheogramIDs = JsonConvert.DeserializeObject<List<int>>(str);
                    if (newRheogramIDs != null && newRheogramIDs.Contains(rheogram.ID))
                    {
                        Console.Write("Test #3: check if new ID is present: success. IDs: ");
                        for (int i = 0; i < newRheogramIDs.Count; i++)
                        {
                            Console.Write(newRheogramIDs[i].ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #3: check if new ID is present: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #3: check if new ID is present: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #3: check if new ID is present: failure a");
            }
            // test #4: read the default calibration for the new ID 
            a = client.GetAsync("Values/" + rheogram.ID);
            a.Wait();
            responseGetRheogramIDs = a.Result; 
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLModel YPLModel = JsonConvert.DeserializeObject<YPLModel>(str);
                    if (YPLModel != null && YPLModel.Rheogram != null && YPLModel.Rheogram.Measurements != null)
                    {
                        Console.WriteLine("Test #4: read the default calibration for the new ID. Tau0 = " + YPLModel.Tau0 + " Pa, K = " + YPLModel.K + " Pa.s, n = " + YPLModel.N + ", chi2 = " + YPLModel.Chi2);
                        Console.WriteLine("rheogram:\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in YPLModel.Rheogram.Measurements)
                        {
                            Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #4: read the default calibration for the new ID: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #4: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #4: read the default calibration for the new ID: failure a");
            }
            // test #5: read the Zamora calibration for the new ID 
            a = client.GetAsync("Values/" + rheogram.ID + "/Zamora");
            a.Wait();
            responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLModel YPLModel = JsonConvert.DeserializeObject<YPLModel>(str);
                    if (YPLModel != null && YPLModel.Rheogram != null && YPLModel.Rheogram.Measurements != null)
                    {
                        Console.WriteLine("Test #5: read the Zamora calibration for the new ID. Tau0 = " + YPLModel.Tau0 + " Pa, K = " + YPLModel.K + " Pa.s, n = " + YPLModel.N + ", chi2 = " + YPLModel.Chi2);
                        Console.WriteLine("rheogram:\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in YPLModel.Rheogram.Measurements)
                        {
                            Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #5: read the default calibration for the new ID: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #5: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #5: read the default calibration for the new ID: failure a");
            }
            // test #6: read the Mullineux calibration for the new ID 
            a = client.GetAsync("Values/" + rheogram.ID + "/Mullineux");
            a.Wait();
            responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLModel YPLModel = JsonConvert.DeserializeObject<YPLModel>(str);
                    if (YPLModel != null && YPLModel.Rheogram != null && YPLModel.Rheogram.Measurements != null)
                    {
                        Console.WriteLine("Test #6: read the Zamora calibration for the new ID. Tau0 = " + YPLModel.Tau0 + " Pa, K = " + YPLModel.K + " Pa.s, n = " + YPLModel.N + ", chi2 = " + YPLModel.Chi2);
                        Console.WriteLine("rheogram:\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in YPLModel.Rheogram.Measurements)
                        {
                            Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #6: read the default calibration for the new ID: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #6: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #6: read the default calibration for the new ID: failure a");
            }
            // test #7: put (update) the previous rheogram
            tau0 = 3.0;
            K = 0.75;
            n = 0.50;
            rheogram.Measurements.Clear();
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement();
                measurement.ShearRate = gammaDot;
                measurement.ShearStress = tau0 + K * System.Math.Pow(gammaDot, n);
                rheogram.Measurements.Add(measurement);
            }
            content = new StringContent(rheogram.GetJson(), Encoding.UTF8, "application/json");
            a = client.PutAsync("Values", content);
            a.Wait();
            responseTaskPostRheogram = a.Result;
            if (responseTaskPostRheogram.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #7: put of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #7: put of rheogram: failure a");
            }
            // test #8: read the default calibration for the updated ID 
            a = client.GetAsync("Values/" + rheogram.ID);
            a.Wait();
            responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLModel YPLModel = JsonConvert.DeserializeObject<YPLModel>(str);
                    if (YPLModel != null && YPLModel.Rheogram != null && YPLModel.Rheogram.Measurements != null)
                    {
                        Console.WriteLine("Test #8: read the default calibration for the new ID. Tau0 = " + YPLModel.Tau0 + " Pa, K = " + YPLModel.K + " Pa.s, n = " + YPLModel.N + ", chi2 = " + YPLModel.Chi2);
                        Console.WriteLine("rheogram:\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in YPLModel.Rheogram.Measurements)
                        {
                            Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #8: read the default calibration for the new ID: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #8: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #8: read the default calibration for the new ID: failure a");
            }
            // test #9: delete the rheogram 
            a = client.DeleteAsync("Values/" + rheogram.ID);
            a.Wait();
            responseTaskPostRheogram = a.Result;
            if (responseTaskPostRheogram.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #9: delete of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #9: delete of rheogram: failure a");
            }
            // test #10: check that the rheogram has been deleted
            List<int> updatedRheogramIDs;
            a = client.GetAsync("Values");
            a.Wait();
            responseGetRheogramIDs = a.Result;
            if (responseGetRheogramIDs.IsSuccessStatusCode)
            {
                string str = await responseGetRheogramIDs.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedRheogramIDs = JsonConvert.DeserializeObject<List<int>>(str);
                    if (updatedRheogramIDs != null && !updatedRheogramIDs.Contains(rheogram.ID))
                    {
                        Console.Write("Test #10: that the rheogram has been deleted: success. IDs: ");
                        for (int i = 0; i < updatedRheogramIDs.Count; i++)
                        {
                            Console.Write(updatedRheogramIDs[i].ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #10: that the rheogram has been deleted: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #10: that the rheogram has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #10: that the rheogram has been deleted: failure a");
            }
        }
    }
}
