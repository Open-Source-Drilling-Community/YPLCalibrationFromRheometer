//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.5.2.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------

namespace YPLCalibrationFromRheometer.Test
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class Rheogram 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(128, MinimumLength = 3)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Description", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Description { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearStressStandardDeviation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.Range(0.0D, 100.0D)]
        public double ShearStressStandardDeviation { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Measurements", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<RheometerMeasurement> Measurements { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class RheometerMeasurement 
    {
        [Newtonsoft.Json.JsonProperty("ID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ParentID", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ParentID { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearRate", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ShearRate { get; set; }
    
        [Newtonsoft.Json.JsonProperty("ShearStress", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double ShearStress { get; set; }
    
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class YPLModel 
    {
        [Newtonsoft.Json.JsonProperty("Tau0", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Tau0 { get; set; }
    
        [Newtonsoft.Json.JsonProperty("K", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double K { get; set; }
    
        [Newtonsoft.Json.JsonProperty("n", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double N { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Chi2", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Chi2 { get; set; }
    
        [Newtonsoft.Json.JsonProperty("Rheogram", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Rheogram Rheogram { get; set; }
    
    
    }
}
