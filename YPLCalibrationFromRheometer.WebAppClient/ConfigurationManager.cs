using System;
using System.IO;

namespace YPLCalibrationFromRheometer.WebAppClient
{
    public class ConfigurationManager
    {
        private static ConfigurationManager instance_ = null;

        public Configuration Configuration { get; set; } = new Configuration();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private ConfigurationManager()
        {

        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new ConfigurationManager();
                    string homeDirectory = ".." + Path.DirectorySeparatorChar + "home";
                    if (!Directory.Exists(homeDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(homeDirectory);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    if (Directory.Exists(homeDirectory))
                    {
                        string configurationFilename = homeDirectory + Path.DirectorySeparatorChar + "configuration.json";
                        if (File.Exists(configurationFilename))
                        {
                            using (StreamReader reader = new StreamReader(configurationFilename))
                            {
                                string json = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(json))
                                {
                                    try
                                    {
                                        Configuration config = Configuration.FromJson(json);
                                        if (config != null && !string.IsNullOrEmpty(config.HostURL))
                                        {
                                            instance_.Configuration = config;
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }
                        else
                        {
                            string json = instance_.Configuration.GetJson();
                            if (!string.IsNullOrEmpty(json))
                            {
                                try
                                {
                                    using (StreamWriter writer = new StreamWriter(configurationFilename))
                                    {
                                        writer.WriteLine(json);
                                    }
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                }
                return instance_;
            }
        }
    }
}
