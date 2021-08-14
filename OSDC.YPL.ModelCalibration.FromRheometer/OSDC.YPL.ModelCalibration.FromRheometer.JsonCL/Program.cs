using System;
using System.IO;
using System.Threading;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace OSDC.YPL.ModelCalibration.FromRheometer.JsonCL
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
                if (info != null && "OSDC.YPL.ModelCalibration.FromRheometer".Equals(info.Name))
                {
                    found = true;
                }
                else
                {
                    solutionRootDir += "..\\";
                }
            } while (!found);
            string jsonSchemaRootDir = solutionRootDir + "OSDC.YPL.ModelCalibration.FromRheometer.Service\\wwwroot\\json-schemas\\";
            string sourceCodeDir = solutionRootDir + "OSDC.YPL.ModelCalibration.FromRheometer.Test\\";
            if (args != null && args.Length >= 1 && Directory.Exists(args[0]))
            {
                sourceCodeDir = args[0];
            }
            string codeNamespace = "OSDC.YPL.ModelCalibration.FromRheometer.Test";
    if (args != null && args.Length >= 2 && !string.IsNullOrEmpty(args[1]))
            {
                codeNamespace = args[1];
            }
            JsonSchema YPLModelSchema = await JsonSchema.FromFileAsync(jsonSchemaRootDir + "YPLModel.jsd");
            CSharpGeneratorSettings settings = new CSharpGeneratorSettings();
            settings.Namespace = codeNamespace;
            var YPLModelGenerator = new CSharpGenerator(YPLModelSchema, settings);
            var YPLModelFile = YPLModelGenerator.GenerateFile();
            using (StreamWriter writer = new StreamWriter(sourceCodeDir + "YPLModelFromJson.cs"))
            {
                writer.WriteLine(YPLModelFile);
            }
            lock (lock_)
            {
                finished_ = true;
            }
        }
    }
}
