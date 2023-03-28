using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.WebApp.Client.Shared
{
    public class APIUtils
    {
        public static readonly string HostName = YPLCalibrationFromRheometer.WebApp.Client.Configuration.YPLCalibrationHostURL;
        public static readonly string HostBasePath = "YPLCalibrationFromRheometer/api/";
        public static readonly HttpClient HttpClientYPLCalibrationFromRheometer = APIUtils.SetHttpClient(HostName, HostBasePath);

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

        public static async Task<List<T>> LoadDatas<T>(ILogger logger, HttpClient httpClient, string datasUri) where T : class
        {
            bool success = false;
            List<T> dataList = new List<T>();
            try
            {
                Guid[] ids = await APIUtils.LoadIDs(logger, httpClient, datasUri);
                if (ids != null)
                {
                    T data = null;
                    for (int i = 0; i < ids.Length; i++)
                    {
                        data = await APIUtils.LoadData<T>(logger, httpClient, datasUri, ids[i]);
                        if (data != null)
                            dataList.Add(data);
                    }
                    if (dataList.Count != ids.Length)
                        throw new Exception($"Inconsistent count of data-loaded IDs of type of type {typeof(T).Name} and loaded datas.");
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
    }
}
