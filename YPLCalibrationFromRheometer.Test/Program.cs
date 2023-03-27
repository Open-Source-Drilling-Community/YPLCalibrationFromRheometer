using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.ModelClientShared;

namespace YPLCalibrationFromRheometer.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test(args);
            Thread.Sleep(10);
        }

        static async void Test(string[] args)
        {
            Console.Write("YPLCalibrationFromRheometer Tests");
            //string host = "https://app.DigiWells.no/";
            string host = "http://localhost:5002/";
            if (args != null && args.Length >= 1)
            {
                host = args[0];
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host + "YPLCalibrationFromRheometer/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            #region test1: read rheogram IDs
            List<Guid> initialRheometerIDs;
            var a = httpClient.GetAsync("CouetteRheometers");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialRheometerIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (initialRheometerIDs != null)
                    {
                        Console.WriteLine("Test #1: read rheometer IDs: success. IDs: ");
                        for (int i = 0; i < initialRheometerIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {initialRheometerIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Test #1: read rheometer IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #1: read rheometer IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #1: read rheometer IDs: failure a");
            }
            #endregion

            #region test2: post a new rheometer

            CouetteRheometer addedRheometer = new CouetteRheometer
            {

                ID = Guid.NewGuid(),
                Name = "New rheometer",
                Description = "Rheometer test",
                RheometerType = RheometerTypeEnum.RotatingBob,
                BobRadius = 0.013329,
                Gap = 0.001131,
                NewtonianEndEffectCorrection = 1.1,
                BobLength = 0.040,
                ConicalAngle = Math.PI / 6.0,
                MeasurementPrecision = 0.02,
                UseISOConvention = true,
                FixedSpeedList = null
            };

            StringContent content = new StringContent(addedRheometer.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("CouetteRheometers", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #2: post of rheometer: success");
            }
            else
            {
                Console.WriteLine("Test #2: post of rheometer: failure a");
            }
            #endregion

            #region test3: check that the new ID is present
            List<Guid> newRheometerIDs;
            a = httpClient.GetAsync("CouetteRheometers");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    newRheometerIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (newRheometerIDs != null && newRheometerIDs.Contains(addedRheometer.ID))
                    {
                        Console.WriteLine("Test #3: check if new ID is present: success. IDs: ");
                        for (int i = 0; i < newRheometerIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {newRheometerIDs[i]}");
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

            #region test4: read rheogram IDs
            List<Guid> initialRheogramIDs;
            a = httpClient.GetAsync("Rheograms");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (initialRheogramIDs != null)
                    {
                        Console.WriteLine("Test #4: read rheograms IDs: success. IDs: ");
                        for (int i = 0; i < initialRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {initialRheogramIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Test #4: read rheograms IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #4: read rheograms IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #4: read rheograms IDs: failure a");
            }
            #endregion

            #region test5: post a new rheogram
            Rheogram addedRheogram = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = "New rheogram",
                Description = "Rheogram test",
                CouetteRheometerID = addedRheometer.ID,
                RateSource = RateSourceEnum.BobNewtonianShearRate,
                StressSource = StressSourceEnum.BobNewtonianShearStress,
                Measurements = new List<RheometerMeasurement>()
            };
            double tau0 = 2.0;
            double K = 0.5;
            double n = 0.75;
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement
                {
                    BobNewtonianShearRate = gammaDot,
                    BobNewtonianShearStress = tau0 + K * System.Math.Pow(gammaDot, n)
                };
                addedRheogram.Measurements.Add(measurement);
            }

            content = new StringContent(addedRheogram.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("Rheograms", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #5: post of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #5: post of rheogram: failure a");
            }
            #endregion

            #region test6: check that the new ID is present
            List<Guid> newRheogramIDs;
            Rheogram updatedRheogram = null;
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
                        Console.WriteLine("Test #6: check if new ID is present: success. IDs: ");
                        for (int i = 0; i < newRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {newRheogramIDs[i]}");
                        }
                        Console.WriteLine();

                        // Then load the updated rheogram (including all conversions ISO/Bob/Raw)
                        a = httpClient.GetAsync("Rheograms/" + addedRheogram.ID.ToString());
                        a.Wait();
                        if (a.Result.IsSuccessStatusCode)
                        {
                            str = await a.Result.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(str))
                            {
                                updatedRheogram = JsonConvert.DeserializeObject<Rheogram>(str);
                                if (updatedRheogram != null)
                                {
                                    Console.WriteLine("Test #6: retrieving updated rheogram : success");
                                }
                                else
                                {
                                    Console.WriteLine("Test #6: retrieving updated rheogram : failure d");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Test #6: check if new ID is present: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #6: check if new ID is present: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #6: check if new ID is present: failure a");
            }
            #endregion

            #region test7: post a new YPLCalibration based on the updated rheogram
            Guid addedYPLCalibrationID = Guid.NewGuid();
            YPLCalibration addedYplCalibration = new YPLCalibration
            {
                ID = addedYPLCalibrationID,
                Name = "New yplCalibration",
                Description = "Test yplCalibration",
                RheogramInput = updatedRheogram
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
            addedYplCalibration.YPLModelLevenbergMarquardt = new YPLModel
            {
                ID = Guid.NewGuid(),
                Name = addedYplCalibration.Name + "-calculated-LevenbergMarquardt"
            };
            content = new StringContent(addedYplCalibration.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("YPLCalibrations", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #7: post of YPLCalibration: success");
            }
            else
            {
                Console.WriteLine("Test #7: post of YPLCalibration: failure");
            }
            #endregion

            #region test8: read the YPLCalibration for the new ID 
            YPLCalibration yplCalibration = null;
            a = httpClient.GetAsync("YPLCalibrations/" + addedYplCalibration.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    yplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(str);
                    if (yplCalibration != null && yplCalibration.YPLModelMullineux != null && yplCalibration.YPLModelKelessidis != null)
                    {
                        Console.WriteLine("Test #8: read the input rheogram for the new YPLCalibration ID");
                        Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa)");
                        foreach (RheometerMeasurement measurement in yplCalibration.RheogramInput.Measurements)
                        {
                            Console.WriteLine("\t" + measurement.BobNewtonianShearRate.ToString() + "\t" + measurement.BobNewtonianShearStress.ToString());
                        }
                        Console.WriteLine();
                        Console.WriteLine("\tRead the Kellessidis calibration pararmeters for the new YPLCalibration ID.");
                        Console.WriteLine("\t\tTau0 = " + yplCalibration.YPLModelKelessidis.Tau0 + " Pa, K = " + yplCalibration.YPLModelKelessidis.K + " Pa.s, n = " + yplCalibration.YPLModelKelessidis.N + ", chi2 = " + yplCalibration.YPLModelKelessidis.Chi2);
                        Console.WriteLine("\tRead the Mullineux calibration parameters for the new YPLCalibration ID.");
                        Console.WriteLine("\t\tTau0 = " + yplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + yplCalibration.YPLModelMullineux.K + " Pa.s, n = " + yplCalibration.YPLModelMullineux.N + ", chi2 = " + yplCalibration.YPLModelMullineux.Chi2);
                        Console.WriteLine("\tRead the Levenberg-Marquardt calibration parameters for the new YPLCalibration ID.");
                        Console.WriteLine("\t\tTau0 = " + yplCalibration.YPLModelLevenbergMarquardt.Tau0 + " Pa, K = " + yplCalibration.YPLModelLevenbergMarquardt.K + " Pa.s, n = " + yplCalibration.YPLModelLevenbergMarquardt.N + ", chi2 = " + yplCalibration.YPLModelLevenbergMarquardt.Chi2);
                    }
                    else
                    {
                        Console.WriteLine("Test #8: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #8: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #8: failure a");
            }
            #endregion

            #region test9: post a new YPLCorrection based on the new rheogram
            Guid addedYPLCorrectionID = Guid.NewGuid();
            YPLCorrection addedYplCorrection = new YPLCorrection
            {
                ID = addedYPLCorrectionID,
                Name = "New yplCorrection",
                Description = "Test yplCorrection",
                RheogramInput = updatedRheogram,
                RheogramFullyCorrected = new List<ShearRateAndStress>(),
                RheogramShearRateCorrected = new List<ShearRateAndStress>(),
                RheogramShearStressCorrected = new List<ShearRateAndStress>()
            };
            content = new StringContent(addedYplCorrection.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PostAsync("YPLCorrections", content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #9: post of YPLCorrection: success");
            }
            else
            {
                Console.WriteLine("Test #9: post of YPLCorrection: failure");
            }
            #endregion

            #region test10: read the YPLCorrection for the new ID
            List<RheometerMeasurement> inputRheoMeasurements;
            List<ShearRateAndStress> correctedRheoMeasurements = new List<ShearRateAndStress>();
            a = httpClient.GetAsync("YPLCorrections/" + addedYPLCorrectionID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLCorrection yplCorrection = JsonConvert.DeserializeObject<YPLCorrection>(str);
                    if (yplCorrection != null && yplCorrection.RheogramFullyCorrected != null && yplCorrection.RheogramShearRateCorrected != null && yplCorrection.RheogramShearStressCorrected != null)
                    {
                        Console.WriteLine("\tRead the input and corrected rheogram for the new YPLCorrection ID");
                        Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa) \t\tcorrected shear rate (1/s)\tcorrected shear stress (Pa)");
                        inputRheoMeasurements = (List<RheometerMeasurement>)yplCorrection.RheogramInput.Measurements;
                        correctedRheoMeasurements = (List<ShearRateAndStress>)yplCorrection.RheogramFullyCorrected;
                        for (int i = 0; i < inputRheoMeasurements.Count; ++i)
                        {
                            Console.WriteLine("\t" + inputRheoMeasurements[i].BobNewtonianShearRate.ToString() + "\t" + inputRheoMeasurements[i].BobNewtonianShearStress.ToString() +
                                            "\t\t" + correctedRheoMeasurements[i].ShearRate.ToString() + "\t" + correctedRheoMeasurements[i].ShearStress.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Test #10: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #10: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #10: failure a");
            }
            #endregion

            #region test11: put (update) the rheogram
            tau0 = 3.0;
            K = 0.75;
            n = 0.50;
            updatedRheogram.Measurements.Clear();
            for (double gammaDot = 1.0; gammaDot <= 100.0; gammaDot += 1.0)
            {
                RheometerMeasurement measurement = new RheometerMeasurement
                {
                    BobNewtonianShearRate = gammaDot,
                    BobNewtonianShearStress = tau0 + K * System.Math.Pow(gammaDot, n)
                };
                updatedRheogram.Measurements.Add(measurement);
            }
            content = new StringContent(updatedRheogram.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PutAsync("Rheograms/" + updatedRheogram.ID.ToString(), content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #11: update of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #11: update of rheogram: failure");
            }
            #endregion

            #region test12: verify that the added YPLCalibration and added YPLCorrection referencing the updated rheogram have been updated
            // YPLCalibrations
            a = httpClient.GetAsync("YPLCalibrations/" + addedYPLCalibrationID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLCalibration updatedYplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(str);
                    if (updatedYplCalibration != null)
                    {
                        Console.WriteLine("\tReading the updated YPLCalibration: success");
                        Console.WriteLine();
                        Console.WriteLine("\t\tMullineux calibration parameters for the original YPLCalibration");
                        Console.WriteLine("\t\t\tTau0 = " + yplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + yplCalibration.YPLModelMullineux.K + " Pa.s, n = " + yplCalibration.YPLModelMullineux.N + ", chi2 = " + yplCalibration.YPLModelMullineux.Chi2);
                        Console.WriteLine("\t\tMullineux calibration parameters for the updated YPLCalibration");
                        Console.WriteLine("\t\t\tTau0 = " + updatedYplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + updatedYplCalibration.YPLModelMullineux.K + " Pa.s, n = " + updatedYplCalibration.YPLModelMullineux.N + ", chi2 = " + updatedYplCalibration.YPLModelMullineux.Chi2);
                    }
                    else
                    {
                        Console.Write("Test #12: check YPLCalibration updated after Rheogram update: failure no retrieved YPLCalibration");
                    }
                }
                else
                {
                    Console.WriteLine("Test #12: check YPLCalibration updated after Rheogram update: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #12: check YPLCalibration updated after Rheogram update: failure a");
            }

            // YPLCorrections
            List<ShearRateAndStress> updatedRheoMeasurements;
            a = httpClient.GetAsync("YPLCorrections/" + addedYPLCorrectionID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    YPLCorrection updatedYplCorrection = JsonConvert.DeserializeObject<YPLCorrection>(str);
                    if (updatedYplCorrection != null)
                    {
                        Console.WriteLine("\tRead the corrected rheogram before and after update of the input rheogram for the previously added YPLCorrection");
                        Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa) \t\tupdated shear rate (1/s)\tupdated shear stress (Pa)");
                        updatedRheoMeasurements = (List<ShearRateAndStress>)updatedYplCorrection.RheogramFullyCorrected;
                        for (int i = 0; i < updatedRheoMeasurements.Count; ++i)
                        {
                            Console.WriteLine("\t" + correctedRheoMeasurements[i].ShearRate.ToString() + "\t" + correctedRheoMeasurements[i].ShearStress.ToString() +
                                            "\t\t" + updatedRheoMeasurements[i].ShearRate.ToString() + "\t" + updatedRheoMeasurements[i].ShearStress.ToString());
                        }
                    }
                    else
                    {
                        Console.Write("Test #12: read YPLCorrection IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("Test #12: read YPLCorrection IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #1: read YPLCorrection IDs: failure a");
            }
            #endregion

            #region test13: put (update) the YPLCalibration
            addedYplCalibration.Name = "Updated name and rheogram";
            addedYplCalibration.RheogramInput = updatedRheogram;
            content = new StringContent(addedYplCalibration.GetJson(), Encoding.UTF8, "application/json");
            a = httpClient.PutAsync("YPLCalibrations/" + addedYplCalibration.ID.ToString(), content);
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #13: update of YPLCalibration: success");
            }
            else
            {
                Console.WriteLine("Test #13: update of YPLCalibration: failure");
            }
            #endregion

            #region test14: read the updated YPLCalibration
            a = httpClient.GetAsync("YPLCalibrations/" + addedYplCalibration.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    yplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(str);
                    if (yplCalibration != null && yplCalibration.Name.Equals("Updated name and rheogram"))
                    {
                        Console.WriteLine("Test #14: reading the updated YPLCalibration: success");
                        if (yplCalibration != null && yplCalibration.YPLModelMullineux != null && yplCalibration.YPLModelKelessidis != null)
                        {
                            Console.WriteLine("Test #14: read the input rheogram for the updated YPLCalibration ID");
                            Console.WriteLine("\tshear rate (1/s)\tshear stress (Pa)");
                            foreach (RheometerMeasurement measurement in yplCalibration.RheogramInput.Measurements)
                            {
                                Console.WriteLine("\t" + measurement.BobNewtonianShearRate.ToString() + "\t" + measurement.BobNewtonianShearStress.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("Test #14: read the Kellessidis calibration pararmeters for the updated YPLCalibration ID.");
                            Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelKelessidis.Tau0 + " Pa, K = " + yplCalibration.YPLModelKelessidis.K + " Pa.s, n = " + yplCalibration.YPLModelKelessidis.N + ", chi2 = " + yplCalibration.YPLModelKelessidis.Chi2);
                            Console.WriteLine("Test #14: read the Mullineux calibration parameters for the updated YPLCalibration ID.");
                            Console.WriteLine("\tTau0 = " + yplCalibration.YPLModelMullineux.Tau0 + " Pa, K = " + yplCalibration.YPLModelMullineux.K + " Pa.s, n = " + yplCalibration.YPLModelMullineux.N + ", chi2 = " + yplCalibration.YPLModelMullineux.Chi2);
                        }
                        else
                        {
                            Console.WriteLine("Test #14: failure c");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Test #14: reading the updated YPLCalibration: failure");
                    }
                }
                else
                {
                    Console.WriteLine("Test #14: read the default calibration for the new ID: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #14: read the default calibration for the new ID: failure a");
            }
            #endregion

            #region test15: delete the rheogram 
            a = httpClient.DeleteAsync("Rheograms/" + updatedRheogram.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #15: delete of rheogram: success");
            }
            else
            {
                Console.WriteLine("Test #15: delete of rheogram: failure");
            }
            #endregion

            #region test16: check that the rheogram has been deleted
            List<Guid> updatedRheogramIDs;
            a = httpClient.GetAsync("Rheograms");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (updatedRheogramIDs != null && !updatedRheogramIDs.Contains(updatedRheogram.ID))
                    {
                        Console.WriteLine("Test #16: that the rheogram has been deleted: success. IDs: ");
                        for (int i = 0; i < updatedRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {updatedRheogramIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #16: that the rheogram has been deleted: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #16: that the rheogram has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #16: that the rheogram has been deleted: failure a");
            }
            #endregion

            #region test17: check that the YPLCalibration has been deleted in the same time
            List<Guid> updatedYPLCalibrationIDs;
            a = httpClient.GetAsync("YPLCalibrations");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedYPLCalibrationIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (updatedYPLCalibrationIDs != null && !updatedYPLCalibrationIDs.Contains(addedYplCalibration.ID))
                    {
                        Console.WriteLine("Test #17: check that the YPLCalibration has been deleted in the same time: success. IDs: ");
                        for (int i = 0; i < updatedYPLCalibrationIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {updatedYPLCalibrationIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #17: check that the YPLCalibration has been deleted in the same time: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #17: check that the rheogram has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #17: check that the rheogram has been deleted: failure a");
            }
            #endregion

            #region test18: delete the rheometer 
            a = httpClient.DeleteAsync("CouetteRheometers/" + addedRheometer.ID.ToString());
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Test #18: delete of rheometer: success");
            }
            else
            {
                Console.WriteLine("Test #18: delete of rheometer: failure");
            }
            #endregion

            #region test19: check that the rheogram has been deleted
            List<Guid> updatedRheometerIDs;
            a = httpClient.GetAsync("CouetteRheometers");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    updatedRheometerIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (updatedRheometerIDs != null && !updatedRheometerIDs.Contains(addedRheometer.ID))
                    {
                        Console.WriteLine("Test #19: that the rheometer has been deleted: success. IDs: ");
                        for (int i = 0; i < updatedRheometerIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {updatedRheometerIDs[i]}");
                        }
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Test #19: that the rheometer has been deleted: failure c");
                    }
                }
                else
                {
                    Console.WriteLine("Test #19: that the rheometer has been deleted: failure b");
                }
            }
            else
            {
                Console.WriteLine("Test #19: that the rheometer has been deleted: failure a");
            }
            #endregion

            Console.ReadLine();
        }
    }
}
