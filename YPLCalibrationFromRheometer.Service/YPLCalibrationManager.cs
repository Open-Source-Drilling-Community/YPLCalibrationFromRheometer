using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;

namespace YPLCalibrationFromRheometer.Service
{
    public class YPLCalibrationManager : DataManager
    {
        private readonly RheogramManager rheogramManager_;

        public YPLCalibrationManager(ILoggerFactory loggerFactory, RheogramManager rheogramManager) : base(loggerFactory)
        {
            rheogramManager_ = rheogramManager;
        }

        public int Count
        {
            get
            {
                int count = 0;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT COUNT(*) FROM YPLCalibrationsTable";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            count = (int)reader.GetInt64(0);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to count records in the YPLCalibrationsTable");
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
                        command.CommandText = @"DELETE FROM YPLCalibrationsTable";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        logger_.LogError(ex, "Impossible to clear the YPLCalibrationsTable or the YPLModelOutputsTable");
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
                command.CommandText = @"SELECT COUNT(*) FROM YPLCalibrationsTable WHERE ID = '" + guid + "'";
                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = (int)reader.GetInt64(0);
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to count rows from YPLCalibrationsTable");
                }
            }
            return count >= 1;
        }

        public List<Guid> GetIDs()
        {
            List<Guid> ids = new List<Guid>();
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID FROM YPLCalibrationsTable";
                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            ids.Add(reader.GetGuid(0));
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to get IDs from YPLCalibrationsTable");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
            }
            return ids;
        }

        public YPLCalibration Get(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = null;
                if (connection_ != null)
                {
                    Guid inputID = Guid.Empty;
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, RheogramInputID, Data FROM YPLCalibrationsTable " +
                        "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string name = reader.GetString(0);
                            string description = reader.GetString(1);
                            inputID = reader.GetGuid(2);
                            string json= reader.GetString(3);
                            yplCalibration = JsonConvert.DeserializeObject<YPLCalibration>(json);
                            if(yplCalibration == null ||
                                !yplCalibration.ID.Equals(guid) ||
                                !yplCalibration.Name.Equals(name) ||
                                inputID == Guid.Empty)
                                throw (new SQLiteException("SQLite database corrupted: YPLCalibration has been jsonified with the wrong ID or Name."));
                            Rheogram rheogram = GetRheogram(inputID);
                            yplCalibration.RheogramInput = rheogram;
                            return yplCalibration;
                        }
                        else
                        {
                            logger_.LogInformation("No YPLCalibration in the database");
                            return null;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to get the YPLCalibration with the given ID from YPLCalibrationsTable");
                        return null;
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCalibration ID is null or empty");
            }
            return null;
        }

        /// <summary>
        /// performs calculations on the given YPLCalibration and adds it to the database
        /// </summary>
        public bool Add(YPLCalibration yplCalibration)
        {
            // every YPLCalibration added to the database should have both RheogramInput and YPLModelOutput different from null and with non empty ID
            if (yplCalibration != null && yplCalibration.RheogramInput != null && yplCalibration.YPLModelKelessidis != null && yplCalibration.YPLModelMullineux != null &&
                !yplCalibration.RheogramInput.ID.Equals(Guid.Empty) && !yplCalibration.YPLModelKelessidis.ID.Equals(Guid.Empty) && !yplCalibration.YPLModelMullineux.ID.Equals(Guid.Empty))
            {
                // first apply calculations
                if (!yplCalibration.CalculateYPLModelMullineux() || !yplCalibration.CalculateYPLModelKelessidis() || !yplCalibration.CalculateYPLLevenbergMarquardt())
                {
                    logger_.LogWarning("Impossible to calculate outputs for the given YPLCalibration");
                    return false;
                }

                // then update to database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using SQLiteTransaction transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string json = JsonConvert.SerializeObject(yplCalibration);
                            // first add the YPLCalibration to the YPLCalibrationsTable
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO YPLCalibrationsTable " +
                                "(ID, Name, Description, RheogramInputID, Data) VALUES (" +
                                "'" + yplCalibration.ID.ToString() + "', " +
                                "'" + yplCalibration.Name + "', " +
                                "'" + yplCalibration.Description + "', " +
                                "'" + yplCalibration.RheogramInput.ID.ToString() + "', " +
                                "'" + json + "' " +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count == 1)
                            {
                                logger_.LogWarning("Inserted the YPLCalibration into the YPLCalibrationsTable");
                                success = true;
                            }
                            else
                            {
                                logger_.LogWarning("Impossible to insert the YPLCalibration into the YPLCalibrationsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to add the YPLCalibration into YPLCalibrationsTable");
                            success = false;
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Added the YPLCalibration of given ID into the YPLCalibrationsTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
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
                logger_.LogWarning("The YPLCalibration ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// removes the YPLCalibration of given guid from the database (YPLModel output children are also removed)
        /// </summary>
        public bool Remove(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = Get(guid);

                // every YPLCalibration added to the database should have both RheogramInput and YPLModelOutput different from null
                if (yplCalibration != null && yplCalibration.RheogramInput != null && yplCalibration.YPLModelKelessidis != null && yplCalibration.YPLModelMullineux != null)
                {
                    if (connection_ != null)
                    {
                        lock (lock_)
                        {
                            using var transaction = connection_.BeginTransaction();
                            bool success = true;
                            // first delete YPLCalibration from YPLCalibrationsTable
                            try
                            {
                                var command = connection_.CreateCommand();
                                command.CommandText = @"DELETE FROM YPLCalibrationsTable WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count < 0)
                                {
                                    logger_.LogWarning("Impossible to delete the YPLCalibration of given ID from the YPLCalibrationsTable");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to delete the YPLCalibration of given ID from YPLCalibrationsTable");
                                success = false;
                            }
                            if (success)
                            {
                                transaction.Commit();
                                logger_.LogInformation("Removed the YPLCalibration of given ID from the YPLCalibrationsTable successfully");
                            }
                            else
                            {
                                transaction.Rollback();
                            }
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
                    logger_.LogWarning("Impossible to remove the YPLCalibration of given ID because it is null or one of its attributes is");
                }
            }
            else
            {
                logger_.LogWarning("The YPLCalibration ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// removes all YPLCalibrations referencing the Rheogram of given ID
        /// </summary>
        public bool RemoveReferences(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    // first select all YPLCalibrations referencing the Rheogram as their input identified by the given ID from YPLCalibrationsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT YPLCalibrationsTable.ID " +
                        "FROM YPLCalibrationsTable, RheogramInputsTable " +
                        "WHERE YPLCalibrationsTable.RheogramInputID = RheogramInputsTable.ID " +
                        "AND RheogramInputsTable.ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid parentId = reader.GetGuid(0);
                            if (!parentId.Equals(Guid.Empty))
                                parentIds.Add(parentId);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to retrieve YPLCalibrations referencing the Rheogram of given ID from YPLCalibrationsTable");
                        return false;
                    }

                    // then delete all of them through the use of the YPLCalibrationManager (which ensures their children outputs are properly deleted)
                    foreach (Guid parentId in parentIds)
                    {
                        if (!Remove(parentId))
                        {
                            logger_.LogWarning("Impossible to delete one of the YPLCalibrations referencing the Rheogram of given ID from YPLCalibrationsTable");
                            return false;
                        }
                    }
                    // Finalizing
                    logger_.LogInformation("Removed all YPLCalibrations referencing the Rheogram of given ID from the YPLCalibrationsTable successfully");
                    return true;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The Rheogram ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// performs calculations on the given YPLCalibration and updates the YPLCalibration of given ID in the database
        /// </summary>
        public bool Update(Guid guid, YPLCalibration updatedYplCalibration)
        {
            bool success = true;
            if (!guid.Equals(Guid.Empty) && updatedYplCalibration != null && !updatedYplCalibration.ID.Equals(Guid.Empty) &&
                updatedYplCalibration.RheogramInput != null && updatedYplCalibration.YPLModelKelessidis != null && updatedYplCalibration.YPLModelMullineux != null)
            {
                // first apply calculations
                if (!updatedYplCalibration.CalculateYPLModelMullineux() || !updatedYplCalibration.CalculateYPLModelKelessidis() || !updatedYplCalibration.CalculateYPLLevenbergMarquardt())
                {
                    logger_.LogWarning("Impossible to calculate outputs for the given YPLCalibration");
                    return false;
                }

                // then update to database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using SQLiteTransaction transaction = connection_.BeginTransaction();
                        // first update the relevant fields in YPLCalibrationsTable (other fields never change)
                        try
                        {
                            string json = JsonConvert.SerializeObject(updatedYplCalibration);
                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE YPLCalibrationsTable SET " +
                                "Name = '" + updatedYplCalibration.Name + "', " +
                                "Description = '" + updatedYplCalibration.Description + "', " +
                                "RheogramInputID = '" + updatedYplCalibration.RheogramInput.ID.ToString() + "', " +
                                "Data = '" + json + "' " +
                                "WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to update the YPLCalibration");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to update the YPLCalibration");
                            success = false;
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Updated the given YPLCalibration successfully");
                            return true;
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The YPLCalibration ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// updates all YPLCalibrations referencing the Rheogram of given ID
        /// </summary>
        public bool UpdateReferences(Guid guid, Rheogram updatedRheogram)
        {
            if (!guid.Equals(Guid.Empty) && updatedRheogram != null)
            {
                if (connection_ != null)
                {
                    // select all YPLCalibrations referencing the Rheogram identified by the given ID from YPLCalibrationsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command2 = connection_.CreateCommand();
                    command2.CommandText = @"SELECT YPLCalibrationsTable.ID " +
                        "FROM YPLCalibrationsTable, RheogramInputsTable " +
                        "WHERE YPLCalibrationsTable.RheogramInputID = RheogramInputsTable.ID " +
                        "AND RheogramInputsTable.ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command2.ExecuteReader();
                        while (reader.Read())
                        {
                            Guid parentId = reader.GetGuid(0);
                            if (!parentId.Equals(Guid.Empty))
                                parentIds.Add(parentId);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to access to the YPLCalibration referencing the Rheogram of given ID from YPLCalibrationsTable");
                        return false;
                    }

                    // then update all of them
                    foreach (Guid parentId in parentIds)
                    {
                        YPLCalibration yplCalibration = Get(parentId);
                        if (yplCalibration != null)
                        {
                            yplCalibration.RheogramInput = updatedRheogram;
                            if (!Update(parentId, yplCalibration))
                            {
                                logger_.LogWarning("Impossible to delete one of the YPLCalibrations referencing the Rheogram of given ID");
                                return false;
                            }
                        }
                        else
                        {
                            logger_.LogWarning("Impossible to get one of the YPLCalibrations referencing the Rheogram of given ID");
                            return false;
                        }
                    }
                    // Finalizing
                    logger_.LogWarning("Updated YPLCalibrations referencing the Rheogram of given ID successfully");
                    return true;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to update the YPLCalibrations referencing the Rheogram which is null or badly identified");
            }
            return false;
        }
    }
}