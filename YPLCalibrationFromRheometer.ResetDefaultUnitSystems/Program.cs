using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.UnitConversion.Conversion.DrillingEngineering;

namespace YPLCalibrationFromRheometer.ResetDefaultUnitSystems
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
            Console.Write("YPLCalibrationFromRheometer Reset Default Unit Systems");
            string host = "https://app.DigiWells.no/";
            //string host = "https://dev.DigiWells.no/";
            //string host = "http://localhost:5002/";
            if (args != null && args.Length >= 1)
            {
                host = args[0];
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host + "YPLCalibrationFromRheometer/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            #region read unit system IDs
            List<MetaInfo>? initialUnitSystemSetIDs;
            var a = httpClient.GetAsync("DrillingUnitChoiceSets");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialUnitSystemSetIDs = JsonConvert.DeserializeObject<List<MetaInfo>>(str);
                    if (initialUnitSystemSetIDs != null)
                    {
                        Console.WriteLine("read unit system set IDs: success. IDs: ");
                        for (int i = 0; i < initialUnitSystemSetIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {initialUnitSystemSetIDs[i].ID}");
                        }
                        Console.WriteLine();
                        #region find missing default unit system set
                        DrillingUnitChoiceSet SI = DrillingUnitChoiceSet.DrillingSIUnitChoiceSet;
                        MetaInfo? SIInfo = null;
                        foreach (var item in initialUnitSystemSetIDs)
                        {
                            if (item != null && item.ID == SI.ID)
                            {
                                SIInfo = item;
                                break;
                            }
                        }
                        if (SIInfo == null)
                        {
                            Console.WriteLine("Missing SI Unit System.");
                            Add(httpClient, SI);
                        }
                        DrillingUnitChoiceSet metric = DrillingUnitChoiceSet.DrillingMetricUnitChoiceSet;
                        MetaInfo? metricInfo = null;
                        foreach (var item in initialUnitSystemSetIDs)
                        {
                            if (item != null && item.ID == metric.ID)
                            {
                                metricInfo = item;
                                break;
                            }
                        }
                        if (metricInfo == null)
                        {
                            Console.WriteLine("Missing metric Unit System.");
                            Add(httpClient, metric);
                        }
                        DrillingUnitChoiceSet US = DrillingUnitChoiceSet.DrillingUSUnitChoiceSet;
                        MetaInfo? USInfo = null;
                        foreach (var item in initialUnitSystemSetIDs)
                        {
                            if (item != null && item.ID == US.ID)
                            {
                                USInfo = item;
                                break;
                            }
                        }
                        if (USInfo == null)
                        {
                            Console.WriteLine("Missing US Unit System.");
                            Add(httpClient, US);
                        }
                        DrillingUnitChoiceSet imperial = DrillingUnitChoiceSet.DrillingImperialUnitChoiceSet;
                        MetaInfo? imperialInfo = null;
                        foreach (var item in initialUnitSystemSetIDs)
                        {
                            if (item != null && item.ID == imperial.ID)
                            {
                                imperialInfo = item;
                                break;
                            }
                        }
                        if (imperialInfo == null)
                        {
                            Console.WriteLine("Missing imperial Unit System.");
                            Add(httpClient, imperial);
                        }
                        #endregion                       
                    }
                    else
                    {
                        Console.Write("read unit system set IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("read unit system set IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("read unit system set IDs: failure a");
            }
            #endregion
        }

        private static void Add(HttpClient httpClient, DrillingUnitChoiceSet unitSystemSet)
        {
            if (unitSystemSet != null)
            {
                string json = JsonConvert.SerializeObject(unitSystemSet);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var a = httpClient.PostAsync("DrillingUnitChoiceSets", content);
                a.Wait();
                if (a.Result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successed to post the unit system set: " + unitSystemSet.Name + ".");
                }
                else
                {
                    Console.WriteLine("Failed to post the unit system set: " + unitSystemSet.Name + ".");
                }
            }
            else
            {
                Console.WriteLine("The unit system set was null.");
            }
        }
    }
}