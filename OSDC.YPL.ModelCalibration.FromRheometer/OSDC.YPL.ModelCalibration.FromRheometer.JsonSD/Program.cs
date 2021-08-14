using System;
using System.IO;
using NJsonSchema;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.JsonSD
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
                if (info != null && "OSDC.YPL.ModelCalibration.FromRheometer".Equals(info.Name))
                {
                    found = true;
                }
                else
                {
                    rootDir += "..\\";
                }
            } while (!found);
            rootDir += "OSDC.YPL.ModelCalibration.FromRheometer.Service\\wwwroot\\json-schemas\\";
            var rheogramSchema = JsonSchema.FromType<Rheogram>();
            var rheogramSchemaJson = rheogramSchema.ToJson();
            using (StreamWriter writer = new StreamWriter(rootDir + "Rheogram.jsd"))
            {
                writer.WriteLine(rheogramSchemaJson);
            }
            var YPLModelSchema = JsonSchema.FromType<YPLModel>();
            var YPLModelSchemaJson = YPLModelSchema.ToJson();
            using (StreamWriter writer = new StreamWriter(rootDir + "YPLModel.jsd"))
            {
                writer.WriteLine(YPLModelSchemaJson);
            }
            
        }
    }
}
