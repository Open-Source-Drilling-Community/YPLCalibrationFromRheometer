using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using System.Linq;
using OSDC.UnitConversion.Conversion.DrillingEngineering;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;

namespace YPLCalibrationFromRheometer.Service
{
    public class DrillingUnitChoiceSetsManager
    {
        private readonly ILogger logger_;
        private readonly object lock_ = new object();
        private readonly SQLiteConnection connection_;

        public DrillingUnitChoiceSetsManager(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<DrillingUnitChoiceSetsManager>();
            connection_ = SQLConnectionManager.GetConnection(loggerFactory);

            // first initiate a call to the database to make sure all its tables are initialized
            List<MetaID> unitChoiceSetIDs = GetIDs();

            // then create some default DrillingUnitChoiceSets'
            if (!unitChoiceSetIDs.Any())
            {
                FillDefault();
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT COUNT(*) FROM DrillingUnitChoiceSetsTable";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            count = (int)reader.GetInt64(0);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to count records in the DrillingUnitChoiceSetsTable");
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
                return count;
            }
        }

        public bool Clear()
        {
            if (connection_ != null)
            {
                bool success = false;
                lock (lock_)
                {
                    using var transaction = connection_.BeginTransaction();
                    try
                    {
                        var command = connection_.CreateCommand();
                        command.CommandText = @"DELETE FROM DrillingUnitChoiceSetsTable";
                        int count = command.ExecuteNonQuery();
                        transaction.Commit();
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        logger_.LogError(ex, "Impossible to clear the DrillingUnitChoiceSetsTable");
                    }
                }
                return success;
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
                return false;
            }
        }

        public bool Contains(Guid guid)
        {
            int count = 0;
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM DrillingUnitChoiceSetsTable WHERE ID = '" + guid.ToString() + "'";
                try
                {
                    using var reader = command.ExecuteReader();
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        count = (int)reader.GetInt64(0);
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to count rows from DrillingUnitChoiceSetsTable");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
            }
            return count >= 1;
        }

        public List<MetaID> GetIDs()
        {
            List<MetaID> ids = new List<MetaID>();
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID, Name, Description, IsDefault FROM DrillingUnitChoiceSetsTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int res = reader.GetInt32(3);
                            Dictionary<string, bool> flags = new Dictionary<string, bool>
                            {
                                { "IsDefault", res != 0 }
                            };
                            ids.Add(new MetaID(reader.GetGuid(0), reader.GetString(1), reader.GetString(2), flags));
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to get IDs and names from DrillingUnitChoiceSetsTable");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
            }
            return ids;
        }

        public DrillingUnitChoiceSet Get(Guid guid)
        {
            if (guid != null && guid != Guid.Empty)
            {
                if (connection_ != null)
                {
                    DrillingUnitChoiceSet result = null;
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Data " +
                        "FROM DrillingUnitChoiceSetsTable WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string json = reader.GetString(0);
                            result = JsonConvert.DeserializeObject<DrillingUnitChoiceSet>(json);
                            if (!result.ID.Equals(guid))
                                throw (new SQLiteException("SQLite database corrupted: DrillingUnitChoiceSetsTable has been jsonified with the wrong ID."));
                        }
                        else
                        {
                            logger_.LogInformation("No DrillingUnitChoiceSet in the database");
                            return null;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to get the DrillingUnitChoiceSet with the given ID from DrillingUnitChoiceSetsTable");
                        return null;
                    }
                    // Finalizing
                    logger_.LogInformation("Returning the DrillingUnitChoiceSet of given ID from DrillingUnitChoiceSetsTable");
                    return result;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The given DrillingUnitChoiceSet ID is null or empty");
            }
            return null;
        }

        public bool Add(DrillingUnitChoiceSet drillingUnitChoiceSet)
        {
            if (drillingUnitChoiceSet != null && drillingUnitChoiceSet.ID != null && !drillingUnitChoiceSet.ID.Equals(Guid.Empty))
            {
                // 1) the custom DrillingUnitChoiceSet is added to the database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string json = JsonConvert.SerializeObject(drillingUnitChoiceSet);
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO DrillingUnitChoiceSetsTable (ID, Name, Description, IsDefault, Data) " +
                                "VALUES (" +
                                "'" + drillingUnitChoiceSet.ID.ToString() + "', " +
                                "'" + drillingUnitChoiceSet.Name + "', " +
                                "'" + drillingUnitChoiceSet.Description + "', " +
                                "" + ((drillingUnitChoiceSet.IsDefault) ? 1 : 0).ToString() + ", " +
                                "'" + json + "'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to insert the DrillingUnitChoiceSet into the DrillingUnitChoiceSetsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to add the DrillingUnitChoiceSet into DrillingUnitChoiceSetsTable");
                            success = false;
                        }
                        // Finalizing addition to the database
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Added the DrillingUnitChoiceSet of given ID into the DrillingUnitChoiceSetsTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        // 2) the DrillingUnitChoiceSet must be statically added to the DrillingUnitChoiceSet class to be later accessed by consumers like class DataUnitConversionSet which calls the static method DrillingUnitChoiceSet.Get()
                        return (success && DrillingUnitChoiceSet.Add(drillingUnitChoiceSet));
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The DrillingUnitChoiceSet is null or its ID is null or empty");
            }
            return false;
        }

        public bool Remove(DrillingUnitChoiceSet drillingUnitChoiceSet)
        {
            if (drillingUnitChoiceSet != null)
            {
                return Remove(drillingUnitChoiceSet.ID);
            }
            else
            {
                logger_.LogWarning("The given drillingUnitChoiceSet is null");
                return false;
            }
        }

        public bool Remove(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            var command = connection_.CreateCommand();
                            command.CommandText = @"DELETE FROM DrillingUnitChoiceSetsTable WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count < 0)
                            {
                                logger_.LogWarning("Impossible to delete the DrillingUnitChoiceSet of given ID from the DrillingUnitChoiceSetsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to delete the DrillingUnitChoiceSet of given ID from DrillingUnitChoiceSetsTable");
                            success = false;
                        }

                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Removed the DrillingUnitChoiceSet of given ID from the DrillingUnitChoiceSetsTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        // Finalizing
                        return success;
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The DrillingUnitChoiceSet ID is null or empty");
            }
            return false;
        }

        public bool Update(Guid guid, DrillingUnitChoiceSet drillingUnitChoiceSet)
        {
            if (guid != null && !guid.Equals(Guid.Empty) && drillingUnitChoiceSet != null && guid.Equals(drillingUnitChoiceSet.ID))
            {
                // 1) the custom DrillingUnitChoiceSet is updated in the database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string json = JsonConvert.SerializeObject(drillingUnitChoiceSet);

                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE DrillingUnitChoiceSetsTable SET " +
                                "Name = '" + drillingUnitChoiceSet.Name + "', " +
                                "Description = '" + drillingUnitChoiceSet.Description + "', " +
                                "IsDefault = " + ((drillingUnitChoiceSet.IsDefault) ? 1 : 0).ToString() + ", " +
                                "Data = '" + json + "' " +
                                "WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to update the DrillingUnitChoiceSet of given ID");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to update DrillingUnitChoiceSet of given ID");
                            success = false;
                        }
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Updated the DrillingUnitChoiceSet of given ID successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        // 2) the DrillingUnitChoiceSet must be statically updated in the DrillingUnitChoiceSet class to be later accessed by consumers like class DataUnitConversionSet which calls the static method DrillingUnitChoiceSet.Get()
                        return (success && DrillingUnitChoiceSet.Update(drillingUnitChoiceSet));
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The DrillingUnitChoiceSet is null or its ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// populate databasewith a few default UnitConversionSets
        /// </summary>
        private void FillDefault()
        {
            DrillingUnitChoiceSet SI = DrillingUnitChoiceSet.DrillingSIUnitChoiceSet;
            if (Get(SI.ID) == null)
            {
                Add(SI);
            }
            else
            {
                Update(SI.ID, SI);
            }
            DrillingUnitChoiceSet metric = DrillingUnitChoiceSet.DrillingMetricUnitChoiceSet;
            if (Get(metric.ID) == null)
            {
                Add(metric);
            }
            else
            {
                Update(metric.ID, metric);
            }
            DrillingUnitChoiceSet US = DrillingUnitChoiceSet.DrillingUSUnitChoiceSet;
            if (Get(US.ID) == null)
            {
                Add(US);
            }
            else
            {
                Update(US.ID, US);
            }
            DrillingUnitChoiceSet imperial = DrillingUnitChoiceSet.DrillingImperialUnitChoiceSet;
            if (Get(imperial.ID) == null)
            {
                Add(imperial);
            }
            else
            {
                Update(imperial.ID, imperial);
            }
        }
    }
}
