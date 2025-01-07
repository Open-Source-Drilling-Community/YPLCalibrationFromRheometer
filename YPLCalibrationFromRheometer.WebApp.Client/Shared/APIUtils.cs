using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using OSDC.DotnetLibraries.General.DataManagement;
using System.Text;
using System.Data;
using System.Linq;
using ExcelDataReader;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using OSDC.UnitConversion.DrillingUnitConversion.ModelClientShared;

namespace YPLCalibrationFromRheometer.WebApp.Client.Shared
{
    public class APIUtils
    {
        public static readonly string YPLCalibrationFromRheometerHostName = YPLCalibrationFromRheometer.WebApp.Client.Configuration.YPLCalibrationHostURL;
        public static readonly string YPLCalibrationFromRheometerHostBasePath = "YPLCalibrationFromRheometer/api/";
        public static readonly HttpClient HttpClientYPLCalibrationFromRheometer = APIUtils.SetHttpClient(YPLCalibrationFromRheometerHostName, YPLCalibrationFromRheometerHostBasePath);

        public static readonly string DrillingUnitConversionHostName = YPLCalibrationFromRheometer.WebApp.Client.Configuration.DrillingUnitConversionHostURL;
        public static readonly string DrillingUnitConversionHostBasePath = "DrillingUnitConversion/api/";
        public static readonly HttpClient HttpClientDrillingUnitConversion = APIUtils.SetHttpClient(DrillingUnitConversionHostName, DrillingUnitConversionHostBasePath);

        public static HttpClient SetHttpClient(string host, string microServiceUri)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(host + microServiceUri)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        /// <summary>
        /// loads the list of Guid corresponding to a data identified by its datasUri from the microservice identified by its httpClient
        /// </summary>
        public static async Task<Guid[]> LoadIDs(ILogger logger, HttpClient httpClient, string datasUri)
        {
            bool success = false;
            Guid[] ids = null;
            try
            {
                var a = await httpClient.GetAsync(datasUri);
                if (a.IsSuccessStatusCode)
                {
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        ids = Newtonsoft.Json.JsonConvert.DeserializeObject<Guid[]>(str);
                        if (ids == null)
                            throw new NullReferenceException($"Impossible to deserialize Guid from string:" + str);
                    }
                    success = true;
                }
                else
                {
                    logger.LogWarning($"Impossible to get IDs from client {httpClient} at Uri {datasUri}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to load IDs from client {httpClient} at Uri {datasUri}");
            }
            if (success)
            {
                logger.LogInformation($"Loaded IDs from client {httpClient} at Uri {datasUri} successfully");
                return ids;
            }
            else
            {
                logger.LogWarning($"Impossible to load IDs from client {httpClient} at Uri {datasUri}");
                return null;
            }
        }

        /// <summary>
        /// loads the list of MetaInfos corresponding to a data identified by its datasUri from the microservice identified by its httpClient
        /// </summary>
        public static async Task<List<MetaInfo>> LoadMetaInfos(ILogger logger, HttpClient httpClient, string datasUri)
        {
            bool success = false;
            List<MetaInfo> metaInfos = null;
            try
            {
                var a = await httpClient.GetAsync(datasUri);
                if (a.IsSuccessStatusCode)
                {
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        metaInfos = (List<MetaInfo>)Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<MetaInfo>>(str);
                        if (metaInfos == null)
                            throw new NullReferenceException($"Impossible to deserialize MetaInfo from string:" + str);
                    }
                    success = true;
                }
                else
                {
                    logger.LogWarning($"Impossible to get MetaInfos from client {httpClient} at Uri {datasUri}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to load MetaInfos from client {httpClient} at Uri {datasUri}");
            }
            if (success)
            {
                logger.LogInformation($"Loaded IDs from client {httpClient} at Uri {datasUri} successfully");
                return metaInfos;
            }
            else
            {
                logger.LogWarning($"Impossible to load MetaInfos from client {httpClient} at Uri {datasUri}");
                return null;
            }
        }

        /// <summary>
        /// loads the data of type T and of given ID identified by its datasUri from the microservice identified by its httpClient
        /// </summary>
        public static async Task<T> LoadData<T>(ILogger logger, HttpClient httpClient, string datasUri, Guid guid) where T : class
        {
            bool success = false;
            T data = null;
            try
            {
                var a = await httpClient.GetAsync(datasUri + guid.ToString());
                if (a.IsSuccessStatusCode && a.Content != null)
                {
                    string str = await a.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(str))
                    {
                        data = JsonConvert.DeserializeObject<T>(str);
                        if (data == null)
                            throw new NullReferenceException($"Impossible to deserialize data of type {typeof(T).Name} string:" + str);
                        success = true;
                    }
                }
                else
                {
                    logger.LogWarning($"Impossible to get data of type {typeof(T).Name} from controller");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to load data of type {typeof(T).Name}");
            }
            if (success)
            {
                logger.LogInformation($"Loaded data of type {typeof(T).Name} successfully");
                return data;
            }
            else
            {
                logger.LogWarning($"Impossible to load {typeof(T).Name}");
                return null;
            }
        }

        public static async Task<List<T>> LoadDatas<T>(ILogger logger, HttpClient httpClient, string datasUri, bool fromGuid = true) where T : class
        {
            bool success = false;
            List<T> dataList = new List<T>();
            try
            {
                List<Guid> ids = new List<Guid>();
                if (fromGuid)
                {
                    Guid[] guids = await APIUtils.LoadIDs(logger, httpClient, datasUri);
                    foreach (Guid guid in guids)
                        ids.Add(guid);
                }
                else
                {
                    List<MetaInfo> metaInfos = await APIUtils.LoadMetaInfos(logger, httpClient, datasUri);
                    foreach (MetaInfo m in metaInfos)
                        ids.Add(m.ID);
                }

                if (ids != null)
                {
                    T data = null;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        data = await APIUtils.LoadData<T>(logger, httpClient, datasUri, ids[i]);
                        if (data != null)
                            dataList.Add(data);
                    }
                    if (dataList.Count != ids.Count)
                        logger.LogWarning($"Inconsistent count of data-loaded IDs of type of type {typeof(T).Name} and loaded datas.");
                    success = true;
                }
                else
                {
                    logger.LogWarning($"Impossible to get datas of type {typeof(T).Name} from controller");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to load datas of type {typeof(T).Name}");
            }
            if (success)
            {
                logger.LogInformation($"Loaded datas of type {typeof(T).Name} successfully");
                return dataList;
            }
            else
            {
                logger.LogWarning($"Impossible to load datas of type {typeof(T).Name}");
                return null;
            }
        }

        public static async Task<bool> PostData(ILogger logger, HttpClient httpClient, string datasUri, string content)
        {
            bool success = false;
            try
            {
                StringContent jsonContent = new StringContent(content, Encoding.UTF8, "application/json");
                var a = await httpClient.PostAsync(datasUri, jsonContent);
                if (a.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to post data of given json content");
            }
            if (success)
            {
                logger.LogInformation($"Posted data of given json content");
                return true;
            }
            else
            {
                logger.LogWarning($"Impossible to post data data of given json content");
                return false;
            }
        }

        public static async Task<bool> PutData(ILogger logger, HttpClient httpClient, string datasUri, string content, Guid ID)
        {
            bool success = false;
            try
            {
                StringContent jsonContent = new StringContent(content, Encoding.UTF8, "application/json");
                var a = await httpClient.PutAsync(datasUri + ID.ToString(), jsonContent);
                if (a.IsSuccessStatusCode)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Impossible to put data of given ID");
            }
            if (success)
            {
                logger.LogInformation($"Put data of given ID successfully");
                return true;
            }
            else
            {
                logger.LogWarning($"Impossible to put data of given ID");
                return false;
            }
        }

        public static async Task<bool> DeleteData(ILogger logger, HttpClient httpClient, string datasUri, Guid ID)
        {
            bool success = false;
            try
            {
                var a = await httpClient.DeleteAsync(datasUri + ID.ToString());
                success = a.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Impossible to delete data of given ID");
            }
            if (success)
            {
                logger.LogInformation("Deleted the data given ID successfully");
                return true;
            }
            else
            {
                logger.LogWarning("Impossible to delete data of given ID");
                return false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Utility functions to load and parse data of type double from one tab of given xls/csv files //
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public static async Task<DataTableCollection> LoadFile(IBrowserFile file)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            await using FileStream writeStream = new(tempPath, FileMode.Create);
            using var readStream = file.OpenReadStream();
            var bytesRead = 0;
            var totalRead = 0;
            var buffer = new byte[1024 * 10];

            while ((bytesRead = await readStream.ReadAsync(buffer)) != 0)
            {
                totalRead += bytesRead;

                await writeStream.WriteAsync(buffer, 0, bytesRead);

                //progressPercent = Decimal.Divide(totalRead, file.Size);
                //StateHasChanged();
            }

            using (var reader = ExcelReaderFactory.CreateReader(writeStream))
            {
                var result = reader.AsDataSet();
                return result.Tables;
            }
        }

        /// <summary>
        /// DataTable parser that retrieves from a tab of DataTable, a list of arrays of type List<double[]> (variable number of rows and fixed number of columns)
        /// - data are parsed according to their header, defined by its given label
        /// - each row of data consists of a fixed number of table columns that must be contiguous
        /// - the dimension of each data row should match the dimension of the input array of labels
        /// </summary>
        /// <param name="tab">the DataTable to read from</param>
        /// <param name="dataList">the output list data represented as arrays of variable number of rows and fixed number of columns</param>
        /// <returns>true if parsing went ok</returns>
        public static bool ParseDataTable(DataTable tab, out List<List<double[]>> dataList, string[] labels)
        {
            dataList = new List<List<double[]>>();
            int nCol = labels.Length;
            List<int[]> startIndices = new List<int[]>();
            int minMeasCount = 3;
            // in the framework of the current program, it is assumed that there must be a minimum of 3 records to define a valid rheogram
            for (int i = 0; i < tab.Rows.Count - (minMeasCount + 1); ++i)
            {
                // parsing the whole table, that can comprise blank rows or columns and multiple tables
                for (int j = 0; j < tab.Columns.Count - 1; ++j)
                {
                    bool allMatch = true;
                    for (int l = 0; l < nCol; ++l)
                    {
                        // trying to discover headers that match labels. Matching columns must be both contiguous and ordered the same as labels
                        if (!MatchesLabel(tab.Rows[i][j + l], labels[l]))
                        {
                            allMatch = false;
                            break;
                        }
                    }
                    if (allMatch)
                        startIndices.Add(new int[] { i, j }); // all labels have been discovered, indices are stored as starting indices
                }
            }

            //then, iterate over the multiple tables discovered within the table
            for (int k = 0; k < startIndices.Count; ++k)
            {
                //for each of them, find the indices where numeric values start (a unit header row is allowed but ignored since it is assumed units are set by the user externally)
                int i0 = startIndices.ElementAt(k)[0];
                int j0 = startIndices.ElementAt(k)[1];

                bool allDouble = true;
                double[] headerRow = new double[nCol];
                for (int l = 0; l < nCol; ++l)
                {
                    if (!double.TryParse(tab.Rows[i0 + 1][j0 + l].ToString(), out headerRow[l]))
                    {
                        allDouble = false;
                        break;
                    }

                }
                if (allDouble)
                {
                    i0 += 1; //no unit header has been found for record starting at row,col i0,j0
                }
                else
                {
                    allDouble = true;
                    headerRow = new double[nCol];
                    for (int l = 0; l < nCol; ++l)
                    {
                        if (!double.TryParse(tab.Rows[i0 + 2][j0 + l].ToString(), out headerRow[l]))
                        {
                            allDouble = false;
                            break;
                        }
                    }
                    if (allDouble)
                    {
                        i0 += 2; //a unit header has been found for record starting at row,col i0,j0, it is ignored since units are managed based on user settings
                    }
                    else
                    {
                        continue;
                    }
                }

                //now the header has been dimensioned, parse numerical data
                int i = 0;
                List<double[]> dataArray = new List<double[]>();
                while (i < tab.Rows.Count - i0)
                {
                    bool allNum = true;
                    double[] row = new double[nCol];
                    for (int l = 0; l < nCol; ++l)
                    {
                        if (!double.TryParse(tab.Rows[i0 + i][j0 + l].ToString(), out row[l]))
                        {
                            allNum = false;
                            break;
                        }
                    }
                    if (!allNum)
                        break;
                    dataArray.Add(row);
                    i++;
                }
                if (i >= minMeasCount)
                    dataList.Add(dataArray); //meeting the last above condition
            }
            return dataList.Count > 0;
        }

        private static bool MatchesLabel(object value, string label)
        {
            if (value.ToString().Equals(label))
                return true;
            return false;
        }

    }
}
