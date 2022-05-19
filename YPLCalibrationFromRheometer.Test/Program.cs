using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.ModelClientShared;

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
            //string host = "https://localhost:5001/";
            if (args != null && args.Length >= 1)
            {
                host = args[0];
            }
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(host + "YPLCalibrationFromRheometer/api/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            #region test1: read rheogram IDs
            List<Guid> initialRheogramIDs;
            var a = httpClient.GetAsync("Rheograms/");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (initialRheogramIDs != null)
                    {
                        Console.WriteLine("Test #1: read rheograms IDs: success. IDs: ");
                        for (int i = 0; i < initialRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i+1}) {initialRheogramIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Test #1: read rheograms IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #1: read rheograms IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #1: read rheograms IDs: failure a");
            }
            #endregion
            
            #region test2: post a new rheogram
            Rheogram addedRheogram = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = "New rheogram",
                Description = "Rheogram test",
                ShearStressStandardDeviation = 0.01,
                RheometerMeasurementList = new List<RheometerMeasurement>()
            };
            double tau0 = 2.0;
            double K = 0.5;
            double n = 0.75;
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement
                {
                    ShearRate = gammaDot,
                    ShearStress = tau0 + K * System.Math.Pow(gammaDot, n)
                };
                addedRheogram.RheometerMeasurementList.Add(measurement);
            }

            StringContent content = new StringContent(addedRheogram.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("Rheograms", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #2: post of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #2: post of rheogram: failure a");
            }
            #endregion
            
            #region test3: check that the new ID is present
            List<Guid> newRheogramIDs;
            a = httpClient.GetAsync("Rheograms");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    newRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (newRheogramIDs != null && newRheogramIDs.Contains(addedRheogram.ID))
                    {
                        Console.WriteLine("Test #3: check if new ID is present: success. IDs: ");
                        for (int i = 0; i < newRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {newRheogramIDs[i]}");
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
            #endregion
            
            #region test4: post a new YPLCalibration based on the new rheogram
            YPLCalibration addedYplCalibration = new YPLCalibration
            {
                ID = Guid.NewGuid(),
                Name = "New yplCalibration",
                Description = "Test yplCalibration",
                RheogramInput = addedRheogram
            };
            addedYplCalibration.YPLModelKelessidis = new YPLModel
            {
                ID = Guid.NewGuid(),
                Name = addedYplCalibration.Name + "-calculated-Kelessidis"
            };
            addedYplCalibration.YPLModelMullineux = new YPLModel
            {
                ID = Guid.NewGuid(),
                Name = addedYplCalibration.Name + "-calculated-Mullineux"
            };
            content = new StringContent(addedYplCalibration.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("Values", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #4: post of YPLCalibration: success");
            }
            else
            {
                Console.WriteLine("Test #4: post of YPLCalibration: failure");
            }
            #endregion
            
            #region test5: read the YPLCalibration for the new ID 
            a = httpClient.GetAsync("Values/" + addedYplCalibration.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLCalibration yplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(str);
                    if (yplCalibration != null && yplCalibration.YPLModelMullineux != null && yplCalibration.YPLModelKelessidis != null)
                    {
                        Console.WriteLine("Test #5: read the input rheogram for the new YPLCalibration ID");
                        Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in yplCalibration.RheogramInput.RheometerMeasurementList)
                        {
                            Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                        }
                        Console.WriteLine();
                        Console.WriteLine("Test #5: read the Kellessidis calibration pararmeters for the new YPLCalibration ID.");
                        Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelKelessidis.Tau0 + " Pa, K = " + yplCalibration.YPLModelKelessidis.K + " Pa.s, n = " + yplCalibration.YPLModelKelessidis.N + ", chi2 = " + yplCalibration.YPLModelKelessidis.Chi2);
                        Console.WriteLine("Test #5: read the Mullineux calibration parameters for the new YPLCalibration ID.");
                        Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + yplCalibration.YPLModelMullineux.K + " Pa.s, n = " + yplCalibration.YPLModelMullineux.N + ", chi2 = " + yplCalibration.YPLModelMullineux.Chi2);
                    }
                    else
                    {
                        Console.WriteLine("Test #5: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #5: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #5: failure a");
            }
            #endregion
            
            #region test6: put (update) the rheogram
            tau0 = 3.0;
            K = 0.75;
            n = 0.50;
            addedRheogram.RheometerMeasurementList.Clear();
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement
                {
                    ShearRate = gammaDot,
                    ShearStress = tau0 + K * System.Math.Pow(gammaDot, n)
                };
                addedRheogram.RheometerMeasurementList.Add(measurement);
            }
            content = new StringContent(addedRheogram.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PutAsync("Rheograms/" + addedRheogram.ID.ToString(), content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #6: update of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #6: update of rheogram: failure");
            }
            #endregion

            #region test7: read YPLCalibration IDs
            List<Guid> initialYPLCalibrationIDs;
            a = httpClient.GetAsync("Values/");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialYPLCalibrationIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (initialYPLCalibrationIDs != null)
                    {
                        Console.WriteLine("Test #7: read YPLCalibration IDs: success. IDs: ");
                        for (int i = 0; i < initialYPLCalibrationIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {initialYPLCalibrationIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Test #7: read YPLCalibration IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #7: read YPLCalibration IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #1: read YPLCalibration IDs: failure a");
            }
            #endregion

            #region test8: put (update) the YPLCalibration
            addedYplCalibration.Name = "Updated name and rheogram";
            addedYplCalibration.RheogramInput = addedRheogram;
            content = new StringContent(addedYplCalibration.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PutAsync("Values/" + addedYplCalibration.ID.ToString(), content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #8: update of YPLCalibration: success");
            }
            else
            {
                Console.WriteLine("Test #8: update of YPLCalibration: failure");
            }
            #endregion
            
            #region test9: read the updated YPLCalibration
            a = httpClient.GetAsync("Values/" + addedYplCalibration.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLCalibration yplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(str);
                    if (yplCalibration != null && yplCalibration.Name.Equals("Updated name and rheogram"))
                    {
                        Console.WriteLine("Test #9: reading the updated YPLCalibration: success");
                        if (yplCalibration != null && yplCalibration.YPLModelMullineux != null && yplCalibration.YPLModelKelessidis != null)
                        {
                            Console.WriteLine("Test #9: read the input rheogram for the updated YPLCalibration ID");
                            Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa)");
                            foreach (RheometerMeasurement measurement in yplCalibration.RheogramInput.RheometerMeasurementList)
                            {
                                Console.WriteLine("\t" + measurement.ShearRate.ToString() + "\t" + measurement.ShearStress.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("Test #9: read the Kellessidis calibration pararmeters for the updated YPLCalibration ID.");
                            Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelKelessidis.Tau0 + " Pa, K = " + yplCalibration.YPLModelKelessidis.K + " Pa.s, n = " + yplCalibration.YPLModelKelessidis.N + ", chi2 = " + yplCalibration.YPLModelKelessidis.Chi2);
                            Console.WriteLine("Test #9: read the Mullineux calibration parameters for the updated YPLCalibration ID.");
                            Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + yplCalibration.YPLModelMullineux.K + " Pa.s, n = " + yplCalibration.YPLModelMullineux.N + ", chi2 = " + yplCalibration.YPLModelMullineux.Chi2);
                        }
                        else
                        {
                            Console.WriteLine("Test #9: failure c");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Test #9: reading the updated YPLCalibration: failure");
                    }
                }
                else
                {
                    Console.WriteLine("Test #9: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #9: read the default calibration for the new ID: failure a");
            }
            #endregion
            
            # region test10: delete the rheogram 
            a = httpClient.DeleteAsync("Rheograms/" + addedRheogram.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #10: delete of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #10: delete of rheogram: failure");
            }
            #endregion

            #region test11: check that the rheogram has been deleted
            List<Guid> updatedRheogramIDs;
            a = httpClient.GetAsync("Rheograms");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (updatedRheogramIDs != null && !updatedRheogramIDs.Contains(addedRheogram.ID))
                    {
                        Console.WriteLine("Test #10: that the rheogram has been deleted: success. IDs: ");
                        for (int i = 0; i < updatedRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {updatedRheogramIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #11: that the rheogram has been deleted: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #11: that the rheogram has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #11: that the rheogram has been deleted: failure a");
            }
            #endregion

            #region test12: check that the YPLCalibration has been deleted in the same time
            List<Guid> updatedYPLCalibrationIDs;
            a = httpClient.GetAsync("Values");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedYPLCalibrationIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (updatedYPLCalibrationIDs != null && !updatedYPLCalibrationIDs.Contains(addedYplCalibration.ID))
                    {
                        Console.WriteLine("Test #12: check that the YPLCalibration has been deleted in the same time: success. IDs: ");
                        for (int i = 0; i < updatedYPLCalibrationIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {updatedYPLCalibrationIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #12: check that the YPLCalibration has been deleted in the same time: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #13: check that the rheogram has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #13: check that the rheogram has been deleted: failure a");
            }
            #endregion

            Console.ReadLine();
        }
    }
}
