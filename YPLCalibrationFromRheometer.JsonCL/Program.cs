using System;
using System.IO;
using System.Threading;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace YPLCalibrationFromRheometer.JsonCL
{
    class Program
    {
        private static bool finished_ = false;
        private static object lock_ = new object();
        static void Main(string[] args)
        {
            Generate(args);
            bool finished = false;
            do
            {
                Thread.Sleep(100);
                lock (lock_)
                {
                    finished = finished_;
                }
            } while (!finished);
        }

        static async void Generate(string[] args)
        {
            string solutionRootDir = ".\\";
            bool found = false;
            do
            {
                DirectoryInfo info = Directory.GetParent(solutionRootDir);
                if (info != null && info.Name != null && info.Name.StartsWith("YPLCalibrationFromRheometer"))
                {
                    found = true;
                }
                else
                {
                    solutionRootDir += "..\\";
                }
            } while (!found);
            string jsonSchemaRootDir = solutionRootDir + "..\\YPLCalibrationFromRheometer.Service\\wwwroot\\YPLCalibrationFromRheometer\\json-schemas\\";
            string sourceCodeDir = solutionRootDir + "..\\YPLCalibrationFromRheometer.ModelClientShared\\";
            if (args != null && args.Length >= 1 && Directory.Exists(args[0]))
            {
                sourceCodeDir = args[0];
            }
            string codeNamespace = "YPLCalibrationFromRheometer.ModelClientShared";
            if (args != null && args.Length >= 2 && !string.IsNullOrEmpty(args[1]))
            {
                codeNamespace = args[1];
            }
            JsonSchema modelSchema = await JsonSchema.FromFileAsync(jsonSchemaRootDir + "YPLCalibration.txt");
            CSharpGeneratorSettings settings = new CSharpGeneratorSettings();
            settings.Namespace = codeNamespace;
            var modelGenerator = new CSharpGenerator(modelSchema, settings);
            var modelFile = modelGenerator.GenerateFile();
            using (StreamWriter writer = new StreamWriter(sourceCodeDir + "YPLCalibrationModelFromJson.cs"))
            {
                writer.WriteLine(modelFile);
            }
            lock (lock_)
            {
                finished_ = true;
            }
        }
    }
}
