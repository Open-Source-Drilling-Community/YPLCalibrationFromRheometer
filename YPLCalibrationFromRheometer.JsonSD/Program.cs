using System;
using System.IO;
using NJsonSchema;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.JsonSD
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateJsonSchemas();
        }

        static void GenerateJsonSchemas()
        {
            string rootDir = ".\\";
            bool found = false;
            do
            {
                DirectoryInfo info = Directory.GetParent(rootDir);
                if (info != null && info.Name != null && info.Name.StartsWith("YPLCalibrationFromRheometer"))
                {
                    found = true;
                }
                else
                {
                    rootDir += "..\\";
                }
            } while (!found);
            rootDir += "..\\YPLCalibrationFromRheometer.Service\\wwwroot\\YPLCalibrationFromRheometer\\json-schemas\\";
            var baseData1Schema = JsonSchema.FromType<Tuple<YPLCalibration, YPLCorrection>>();
            var baseData1SchemaJson = baseData1Schema.ToJson();
            using (StreamWriter writer = new StreamWriter(rootDir + "YPLCalibration.txt"))
            {
                writer.WriteLine(baseData1SchemaJson);
            }
        }
    }
}
