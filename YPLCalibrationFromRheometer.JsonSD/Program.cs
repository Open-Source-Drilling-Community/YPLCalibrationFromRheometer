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
                if (info != null && "YPLCalibrationFromRheometer".Equals(info.Name))
                {
                    found = true;
                }
                else
                {
                    rootDir += "..\\";
                }
            } while (!found);
            rootDir += "YPLCalibrationFromRheometer.Service\\wwwroot\\YPLCalibrationFromRheometer\\json-schemas\\";
            var baseData1Schema = JsonSchema.FromType<YPLCalibrationMaster>();
            var baseData1SchemaJson = baseData1Schema.ToJson();
            using (StreamWriter writer = new StreamWriter(rootDir + "YPLCalibrationMaster.txt"))
            {
                writer.WriteLine(baseData1SchemaJson);
            }
        }
    }
}
