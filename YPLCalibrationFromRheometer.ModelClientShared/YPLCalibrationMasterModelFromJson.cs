//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.5.2.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class YPLCalibration 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("RheogramInput", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Rheogram RheogramInput { get; set; }
    
        [Newtonsoft.Json.JsonProperty("YPLModelKelessidis", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public YPLModel YPLModelKelessidis { get; set; }
    
        [Newtonsoft.Json.JsonProperty("YPLModelMullineux", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public YPLModel YPLModelMullineux { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Rheogram 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearStressStandardDeviation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ShearStressStandardDeviation { get; set; }
    
        [Newtonsoft.Json.JsonProperty("RheometerMeasurementList", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<RheometerMeasurement> RheometerMeasurementList { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Count", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Count { get; set; }
    
        [Newtonsoft.Json.JsonProperty("IsReadOnly", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool IsReadOnly { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class RheometerMeasurement 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearRate", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ShearRate { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearStress", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ShearStress { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class YPLModel 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Tau0", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Tau0 { get; set; }
    
        [Newtonsoft.Json.JsonProperty("K", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double K { get; set; }
    
        [Newtonsoft.Json.JsonProperty("N", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double N { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Chi2", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Chi2 { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class YPLCorrection 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Guid ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("R1", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double R1 { get; set; }
    
        [Newtonsoft.Json.JsonProperty("R2", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double R2 { get; set; }
    
        [Newtonsoft.Json.JsonProperty("RheogramInput", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Rheogram RheogramInput { get; set; }
    
        [Newtonsoft.Json.JsonProperty("RheogramShearRateCorrected", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Rheogram RheogramShearRateCorrected { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class YPLCalibrationMaster 
    {
        [Newtonsoft.Json.JsonProperty("YPLCalibration", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public YPLCalibration YPLCalibration { get; set; }
    
        [Newtonsoft.Json.JsonProperty("YPLCorrection", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public YPLCorrection YPLCorrection { get; set; }
    
    
    }
}
