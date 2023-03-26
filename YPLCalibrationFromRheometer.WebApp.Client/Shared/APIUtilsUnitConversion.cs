using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OSDC.UnitConversion.DrillingUnitConversion.ModelClientShared;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using OSDC.DotnetLibraries.General.DataManagement;

namespace YPLCalibrationFromRheometer.WebApp.Client
{
    public class APIUtilsUnitConversion
    {
        public static HttpClient SetHttpClient(string host, string microServiceUri)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(host + microServiceUri)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static async Task<List<MetaID>> LoadDrillingUnitChoiceSets(HttpClient httpClient, ILogger logger)
        {
            bool success = false;
            List<MetaID> unitChoiceSets = new();
            try
            {
                //ids of the existing UnitChoiceSets are retrieved first to keep controllers API standard
                var a = await httpClient.GetAsync("DrillingUnitChoiceSets");
                if (a.IsSuccessStatusCode)
                {
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        unitChoiceSets = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MetaID>>(str);
                        success = true;
                    }
                }
                else
                {
                    logger.LogWarning("Impossible to get UnitConversionSets from controller");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to load DrillingUnitChoiceSets");
            }
            if (success)
            {
                unitChoiceSets.Sort((m1, m2) => String.Compare(m1.Name, m2.Name, false, new CultureInfo("nb-NO")));
                logger.LogInformation("Loaded UnitConversionSets successfully");
                return unitChoiceSets;
            }
            else
            {
                logger.LogWarning("Impossible to load DrillingUnitChoiceSets");
                return null;
            }
        }

        public static async Task<DrillingUnitChoiceSet> LoadDrillingUnitChoiceSets(HttpClient httpClient, ILogger logger, Guid unitChoiceSetID)
        {
            bool success = false;
            DrillingUnitChoiceSet unitChoiceSet = null;
            try
            {
                var a = await httpClient.GetAsync("DrillingUnitChoiceSets/" + unitChoiceSetID);
                if (a.IsSuccessStatusCode)
                {
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        unitChoiceSet = Newtonsoft.Json.JsonConvert.DeserializeObject<DrillingUnitChoiceSet>(str);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to load DrillingUnitChoiceSet");
            }
            if (success)
            {
                logger.LogInformation("Loaded DrillingUnitChoiceSet successfully");
                return unitChoiceSet;
            }
            else
            {
                logger.LogWarning("Impossible to load DrillingUnitChoiceSet");
                return null;
            }
        }

        public static async Task<bool> PostDrillingUnitConversionSet(HttpClient httpClient, ILogger logger, DrillingUnitChoiceSet request)
        {
            bool success = false;
            try
            {
                string json = request.GetJson();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var a = await httpClient.PostAsync("DrillingUnitChoiceSets", content);
                if (a.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to post DrillingUnitChoiceSet");
            }
            if (success)
            {
                logger.LogInformation("Posted DrillingUnitChoiceSet successfully");
                return true;
            }
            else
            {
                logger.LogWarning("Impossible to post DrillingUnitChoiceSet");
                return false;
            }
        }

        public static async Task<bool> PutDrillingUnitConversionSet(HttpClient httpClient, ILogger logger, DrillingUnitChoiceSet request)
        {
            bool success = false;
            try
            {
                string json = request.GetJson();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var a = await httpClient.PutAsync("DrillingUnitChoiceSets/" + request.ID.ToString(), content);
                if (a.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to put DrillingUnitChoiceSet");
            }
            if (success)
            {
                logger.LogInformation("Put DrillingUnitChoiceSet successfully");
                return true;
            }
            else
            {
                logger.LogWarning("Impossible to put DrillingUnitChoiceSet");
                return false;
            }
        }

        public static async Task<bool> DeleteDrillingUnitConversionSet(HttpClient httpClient, ILogger logger, Guid ID)
        {
            bool success = false;
            try
            {
                var a = await httpClient.DeleteAsync("DrillingUnitChoiceSets/" + ID.ToString());
                success = a.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to delete DrillingUnitChoiceSet");
            }
            if (success)
            {
                logger.LogInformation("Deletion of DrillingUnitChoiceSet is successfull");
                return true;
            }
            else
            {
                logger.LogWarning("Impossible to delete DrillingUnitChoiceSet");
                return false;
            }
        }

        public static async Task<List<PhysicalQuantity>> LoadDrillingPhysicalQuantities(HttpClient httpClient, ILogger logger)
        {
            bool success = false;
            List<PhysicalQuantity> drillingPhysicalQuantities = new();
            try
            {
                var a = await httpClient.GetAsync("DrillingPhysicalQuantities");
                if (a.IsSuccessStatusCode)
                {
                    List<MetaID> MetaIDs = null;
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        MetaIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MetaID>>(str);
                    }
                    for (int i = 0; i < MetaIDs.Count; i++)
                    {
                        a = await httpClient.GetAsync("DrillingPhysicalQuantities/" + MetaIDs[i].ID.ToString());
                        if (a.IsSuccessStatusCode && a.Content != null)
                        {
                            str = await a.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(str))
                            {
                                PhysicalQuantity drillingPhysicalQuantity = JsonConvert.DeserializeObject<PhysicalQuantity>(str);
                                if (drillingPhysicalQuantity == null)
                                    throw new NullReferenceException("Impossible to deserialize DrillingPhysicalQuantities string:" + str);
                                drillingPhysicalQuantities.Add(drillingPhysicalQuantity);
                            }
                        }
                    }
                    if (drillingPhysicalQuantities.Count != MetaIDs.Count)
                        throw new Exception("Inconsistent count of DataUnitConversionSet-loaded IDs and loaded DrillingPhysicalQuantities. Verify that the database garbage collector is not set with a too small time update.");
                    success = true;
                }
                else
                {
                    logger.LogWarning("Impossible to get DrillingPhysicalQuantities from controller");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to load DrillingPhysicalQuantities");
            }
            if (success)
            {
                drillingPhysicalQuantities.Sort((dpq1, dpq2) => String.Compare(dpq1.Name, dpq2.Name, false, new CultureInfo("nb-NO")));
                logger.LogInformation("Loaded DrillingPhysicalQuantities successfully");
                return drillingPhysicalQuantities;
            }
            else
            {
                logger.LogWarning("Impossible to load DrillingPhysicalQuantities");
                return null;
            }
        }
    }
}
