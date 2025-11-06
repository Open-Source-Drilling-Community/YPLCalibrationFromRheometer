using System.Net.Http.Headers;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.ModelClientShared;

namespace YPLCalibrationFromRheometer.RemoveDamagedRheograms
{
    class Measurement
    {
        public double ShearRate { get; set; }
        public double ShearStress { get; set; }
    }
    class Rheogram
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RheometerType { get; set; } = 0;
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
    class Program
    {
        static void Main(string[] args)
        {
            ConvertFile();
            ReadRheogramSet();
            UploadRheograms(args);
            Thread.Sleep(10);
        }

        private static void ConvertFile()
        {
            if (File.Exists("..\\..\\..\\..\\Rheograms.txt"))
            {
                using (StreamWriter writer = new StreamWriter("..\\..\\..\\..\\RheogramSet.txt"))
                {
                    using (StreamReader reader = new StreamReader("..\\..\\..\\..\\Rheograms.txt"))
                    {

                        while (!reader.EndOfStream)
                        {
                            string? line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] tokens = line.Split('\t');
                                if (tokens.Length == 2)
                                {
                                    // this is the header
                                    writer.WriteLine(tokens[0] + "\t" + tokens[1] + "\t" + 1);
                                }
                                else if (tokens.Length == 3)
                                {
                                    // we skip
                                }
                                else if (tokens.Length == 4)
                                {
                                    writer.WriteLine(tokens[1] + "\t" + tokens[3]);
                                }
                            }
                            else
                            {
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }

        private static List<Rheogram> ReadRheogramSet()
        {
            List<Rheogram> rheograms = new List<Rheogram>();
            if (File.Exists("..\\..\\..\\..\\RheogramSet.txt"))
            {
                using (StreamWriter writer = new StreamWriter("..\\..\\..\\..\\RheogramSet.txt"))
                {
                }
            }
            return rheograms;
        }
        static async void UploadRheograms(string[] args)
        {
            Console.Write("YPLCalibrationFromRheometer Upload a set of Rheograms");
            //string host = "https://app.DigiWells.no/";
            //string host = "https://dev.DigiWells.no/";
            string host = "http://localhost:5002/";
            if (args != null && args.Length >= 1)
            {
                host = args[0];
            }
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host + "YPLCalibrationFromRheometer/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            #region read rheogram IDs
            List<Guid>? initialRheogramIDs;
            var a = httpClient.GetAsync("Rheograms");
            a.Wait();
            if (a.Result.IsSuccessStatusCode)
            {
                string str = await a.Result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(str))
                {
                    initialRheogramIDs = JsonConvert.DeserializeObject<List<Guid>>(str);
                    if (initialRheogramIDs != null)
                    {
                        Console.WriteLine("read rheogram IDs: success. IDs: ");
                        for (int i = 0; i < initialRheogramIDs.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {initialRheogramIDs[i]}");
                        }
                        Console.WriteLine();
                        #region download each rheogram and detect those which cannot be retrieved
                        List<Guid> unableToDownload = new List<Guid>();
                        foreach (Guid id in initialRheogramIDs)
                        {
                            a = httpClient.GetAsync("Rheograms/" + id.ToString());
                            a.Wait();
                            if (a.Result.IsSuccessStatusCode)
                            {
                                str = await a.Result.Content.ReadAsStringAsync();
                                if (!string.IsNullOrEmpty(str))
                                {
                                    Rheogram? rheogram = JsonConvert.DeserializeObject<Rheogram>(str);
                                    if (rheogram != null)
                                    {
                                        Console.WriteLine("Could download rheogram " + id.ToString() + ". Its name is: " + rheogram.Name + ".");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Rheogram " + id.ToString() + " could not deserialized.");
                                        unableToDownload.Add(id);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Rheogram " + id.ToString() + " is empty.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unsuccessful retrieval of rheogram: " + id.ToString() + ".");
                                unableToDownload.Add(id);
                            }
                        }
                        #endregion
                        #region Delete Rheograms that could be downloaded
                        foreach (Guid id in unableToDownload)
                        {
                            a = httpClient.DeleteAsync("Rheograms/" + id.ToString());
                            a.Wait();
                            if (a.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Managed to delete rheogram: " + id.ToString() + ".");
                            }
                            else
                            {
                                Console.WriteLine("Did not managed to dete rheogram: " + id.ToString() + ".");
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        Console.Write("read rheogram IDs: success. but no IDs");
                    }
                }
                else
                {
                    Console.WriteLine("read rheogram IDs: failure b");
                }
            }
            else
            {
                Console.WriteLine("read rheogram IDs: failure a");
            }
            #endregion

        }
    }
}