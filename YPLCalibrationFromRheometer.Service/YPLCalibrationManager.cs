using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;

namespace YPLCalibrationFromRheometer.Service
{
    public class YPLCalibrationManager
    {
        private readonly ILogger logger_;
        private readonly object lock_ = new object();
        private readonly RheogramManager rheogramManager_;
        private readonly SQLiteConnection connection_;

        public YPLCalibrationManager(ILoggerFactory loggerFactory, RheogramManager rheogramManager)
        {
            logger_ = loggerFactory.CreateLogger<YPLCalibrationManager>();
            connection_ = SQLConnectionManager.GetConnection(loggerFactory);
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
                        // first empty YPLModelOutputsTable
                        var command = connection_.CreateCommand();
                        command.CommandText = @"DELETE FROM YPLModelOutputsTable";
                        command.ExecuteNonQuery();

                        // then empty YPLCalibrationsTable
                        command = connection_.CreateCommand();
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
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = null;
                if (connection_ != null)
                {
                    Guid inputID = Guid.Empty;
                    Guid yplModelKelessidisID = Guid.Empty;
                    Guid yplModelMullineuxID = Guid.Empty;
                    Guid yplModelLevenbergID = Guid.Empty;

                    // first retrieve the YPLCalibration itself
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, RheogramInputID, YPLModelMullineuxID, YPLModelKelessidisID, YPLModelLevenbergID FROM YPLCalibrationsTable " +
                        "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            yplCalibration = new YPLCalibration
                            {
                                ID = guid,
                                Name = reader.GetString(0),
                                Description = reader.GetString(1)
                            };
                            inputID = reader.GetGuid(2);
                            yplModelMullineuxID = reader.GetGuid(3);
                            yplModelKelessidisID = reader.GetGuid(4);
                            yplModelLevenbergID = reader.GetGuid(5);
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

                    // then retrieve its RheogramInput with the RheogramManager
                    Rheogram rheogram = rheogramManager_.Get(inputID);
                    if (rheogram != null)
                    {
                        yplCalibration.RheogramInput = rheogram;

                        // then retrieve its Mullineux YPLModelOutput directly from YPLModelOutputsTable
                        command = connection_.CreateCommand();
                        command.CommandText = @"SELECT Name, YPLModel " +
                            "FROM YPLModelOutputsTable WHERE ID = '" + yplModelMullineuxID.ToString() + "'";
                        try
                        {
                            using SQLiteDataReader reader = command.ExecuteReader();
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                string name = reader.GetString(0);
                                string json = reader.GetString(1);
                                YPLModel yplModel = JsonConvert.DeserializeObject<YPLModel>(json);
                                if (yplModel == null || !yplModel.ID.Equals(yplModelMullineuxID) || !yplModel.Name.Equals(name))
                                    throw (new SQLiteException("SQLite database corrupted: YPLModel has been jsonified with the wrong ID or Name."));
                                yplCalibration.YPLModelMullineux = yplModel;
                            }
                            else
                            {
                                logger_.LogWarning("No such YPLModel associated to the Rheogram in the YPLModelOutputsTable");
                                return null;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to get such YPLModel from YPLModelOutputsTable");
                            return null;
                        }

                        // then retrieve its Kelessidis YPLModelOutput directly from YPLModelOutputsTable
                        command = connection_.CreateCommand();
                        command.CommandText = @"SELECT Name, YPLModel " +
                            "FROM YPLModelOutputsTable WHERE ID = '" + yplModelKelessidisID.ToString() + "'";
                        try
                        {
                            using SQLiteDataReader reader = command.ExecuteReader();
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                string name = reader.GetString(0);
                                string json = reader.GetString(1);
                                YPLModel baseData2 = JsonConvert.DeserializeObject<YPLModel>(json);
                                if (baseData2 == null || !baseData2.ID.Equals(yplModelKelessidisID) || !baseData2.Name.Equals(name))
                                    throw (new SQLiteException("SQLite database corrupted: YPLModel has been jsonified with the wrong ID or Name."));
                                yplCalibration.YPLModelKelessidis = baseData2;
                            }
                            else
                            {
                                logger_.LogWarning("No such YPLModel associated to the Rheogram in the YPLModelOutputsTable");
                                return null;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to get such YPLModel from YPLModelOutputsTable");
                            return null;
                        }

                        // then retrieve its Levenberg YPLModelOutput directly from YPLModelOutputsTable
                        command = connection_.CreateCommand();
                        command.CommandText = @"SELECT Name, YPLModel " +
                            "FROM YPLModelOutputsTable WHERE ID = '" + yplModelLevenbergID.ToString() + "'";
                        try
                        {
                            using SQLiteDataReader reader = command.ExecuteReader();
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                string name = reader.GetString(0);
                                string json = reader.GetString(1);
                                YPLModel baseData2 = JsonConvert.DeserializeObject<YPLModel>(json);
                                if (baseData2 == null || !baseData2.ID.Equals(yplModelLevenbergID) || !baseData2.Name.Equals(name))
                                    throw (new SQLiteException("SQLite database corrupted: YPLModel has been jsonified with the wrong ID or Name."));
                                yplCalibration.YPLModelLevenbergMarquardt = baseData2;
                            }
                            else
                            {
                                logger_.LogWarning("No such YPLModel associated to the Rheogram in the YPLModelOutputsTable");
                                return null;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to get such YPLModel from YPLModelOutputsTable");
                            return null;
                        }
                        // Finalizing
                        logger_.LogInformation("Returning the YPLCalibration of given ID");
                        return yplCalibration;
                    }
                    else
                    {
                        logger_.LogWarning("No RheogramInput associated to the YPLCalibration of given ID");
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
                            // first add the YPLCalibration to the YPLCalibrationsTable
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO YPLCalibrationsTable " +
                                "(ID, Name, Description, RheogramInputID, YPLModelMullineuxID, YPLModelKelessidisID, YPLModelLevenbergID, TimeStamp) VALUES (" +
                                "'" + yplCalibration.ID.ToString() + "', " +
                                "'" + yplCalibration.Name + "', " +
                                "'" + yplCalibration.Description + "', " +
                                "'" + yplCalibration.RheogramInput.ID.ToString() + "', " +
                                "'" + yplCalibration.YPLModelMullineux.ID.ToString() + "', " +
                                "'" + yplCalibration.YPLModelKelessidis.ID.ToString() + "', " +
                                "'" + yplCalibration.YPLModelLevenbergMarquardt.ID.ToString() + "', " +
                                "'" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count == 1)
                            {
                                // then add Mullineux YPLModelOutput to the YPLModelOutputsTable
                                YPLModel yplModel = yplCalibration.YPLModelMullineux;
                                if (yplModel != null)
                                {
                                    string json = JsonConvert.SerializeObject(yplModel);
                                    command = connection_.CreateCommand();
                                    command.CommandText = @"INSERT INTO YPLModelOutputsTable (ID, Name, YPLModel) " +
                                        "VALUES (" +
                                        "'" + yplModel.ID.ToString() + "', " +
                                        "'" + yplModel.Name + "', " +
                                        "'" + json + "'" +
                                        ")";
                                    count = command.ExecuteNonQuery();
                                    if (count != 1)
                                    {
                                        logger_.LogWarning("Impossible to insert the calculated Mullineux YPLModel into the YPLModelOutputsTable");
                                        success = false;
                                    }
                                }
                                else
                                {
                                    logger_.LogWarning("Impossible to get the YPLModel");
                                    success = false;
                                }
                                if (success)
                                {
                                    // then add Kelessidis YPLModelOutput to the YPLModelOutputsTable
                                    yplModel = yplCalibration.YPLModelKelessidis;
                                    if (yplModel != null)
                                    {
                                        string json = JsonConvert.SerializeObject(yplModel);
                                        command = connection_.CreateCommand();
                                        command.CommandText = @"INSERT INTO YPLModelOutputsTable (ID, Name, YPLModel) " +
                                            "VALUES (" +
                                            "'" + yplModel.ID.ToString() + "', " +
                                            "'" + yplModel.Name + "', " +
                                            "'" + json + "'" +
                                            ")";
                                        count = command.ExecuteNonQuery();
                                        if (count != 1)
                                        {
                                            logger_.LogWarning("Impossible to insert the calculated Kelessidis YPLModel into the YPLModelOutputsTable");
                                            success = false;
                                        }
                                    }
                                    else
                                    {
                                        logger_.LogWarning("Impossible to get the YPLModel");
                                        success = false;
                                    }
                                    if (success)
                                    {
                                        // then add Levenberg YPLModelOutput to the YPLModelOutputsTable
                                        yplModel = yplCalibration.YPLModelLevenbergMarquardt;
                                        if (yplModel != null)
                                        {
                                            string json = JsonConvert.SerializeObject(yplModel);
                                            command = connection_.CreateCommand();
                                            command.CommandText = @"INSERT INTO YPLModelOutputsTable (ID, Name, YPLModel) " +
                                                "VALUES (" +
                                                "'" + yplModel.ID.ToString() + "', " +
                                                "'" + yplModel.Name + "', " +
                                                "'" + json + "'" +
                                                ")";
                                            count = command.ExecuteNonQuery();
                                            if (count != 1)
                                            {
                                                logger_.LogWarning("Impossible to insert the calculated Levenberg-Marquardt YPLModel into the YPLModelOutputsTable");
                                                success = false;
                                            }
                                        }
                                        else
                                        {
                                            logger_.LogWarning("Impossible to get the YPLModel");
                                            success = false;
                                        }
                                    }
                                }
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
        /// creates a YPLCalibration from the given Rheogram and adds it to the database 
        /// </summary>
        public bool Add(Rheogram rheogram)
        {
            if (rheogram != null && !rheogram.ID.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = new YPLCalibration
                {
                    ID = Guid.NewGuid()
                };
                yplCalibration.Name = "DefaultName-" + yplCalibration.ID.ToString()[..8];
                yplCalibration.Description = "DefaultDescription-" + yplCalibration.ID.ToString()[..8];
                yplCalibration.RheogramInput = rheogram;
                yplCalibration.RheogramInput.ID = rheogram.ID;
                yplCalibration.RheogramInput.Name = yplCalibration.Name + "-input";
                yplCalibration.YPLModelKelessidis = new YPLModel
                {
                    ID = Guid.NewGuid(),
                    Name = yplCalibration.Name + "-calculated-Kelessidis"
                };
                yplCalibration.YPLModelMullineux = new YPLModel
                {
                    ID = Guid.NewGuid(),
                    Name = yplCalibration.Name + "-calculated-Mullineux"
                };
                yplCalibration.YPLModelLevenbergMarquardt = new YPLModel
                {
                    ID = Guid.NewGuid(),
                    Name = yplCalibration.Name + "-calculated-Levenberg"
                };

                if (Add(yplCalibration))
                {
                    logger_.LogInformation("Created a YPLCalibration from the given Rheogram successfully");
                    return true;
                }
                else
                {
                    logger_.LogWarning("Impossible to add a YPLCalibration from the given Rheogram");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to create a YPLCalibration from the given Rheogram which is null or badly identified");
            }
            return false;
        }

        /// <summary>
        /// removes the YPLCalibration of given guid from the database (YPLModel output children are also removed)
        /// </summary>
        public bool Remove(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
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
                                // then delete Mullineux YPLModelOutput from YPLModelOutputsTable
                                try
                                {
                                    var command = connection_.CreateCommand();
                                    command.CommandText = @"DELETE FROM YPLModelOutputsTable WHERE ID = '" + yplCalibration.YPLModelMullineux.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    if (count < 0)
                                    {
                                        logger_.LogWarning("Impossible to delete the calculated Mullineux YPLModel from the YPLModelOutputsTable");
                                        success = false;
                                    }
                                }
                                catch (SQLiteException ex)
                                {
                                    logger_.LogError(ex, "Impossible to delete the calculated Mullineux YPLModel from the YPLModelOutputsTable");
                                    success = false;
                                }
                            }
                            if (success)
                            {
                                // then delete Kelessidis YPLModelOutput from YPLModelOutputsTable
                                try
                                {
                                    var command = connection_.CreateCommand();
                                    command.CommandText = @"DELETE FROM YPLModelOutputsTable WHERE ID = '" + yplCalibration.YPLModelKelessidis.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    if (count < 0)
                                    {
                                        logger_.LogWarning("Impossible to delete the calculated Kelessidis YPLModel from the YPLModelOutputsTable");
                                        success = false;
                                    }
                                }
                                catch (SQLiteException ex)
                                {
                                    logger_.LogError(ex, "Impossible to delete the calculated Kelessidis YPLModel from the YPLModelOutputsTable");
                                    success = false;
                                }
                            }
                            if (success)
                            {
                                // then delete Levenberg YPLModelOutput from YPLModelOutputsTable
                                try
                                {
                                    var command = connection_.CreateCommand();
                                    command.CommandText = @"DELETE FROM YPLModelOutputsTable WHERE ID = '" + yplCalibration.YPLModelLevenbergMarquardt.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    if (count < 0)
                                    {
                                        logger_.LogWarning("Impossible to delete the calculated Levenberg YPLModel from the YPLModelOutputsTable");
                                        success = false;
                                    }
                                }
                                catch (SQLiteException ex)
                                {
                                    logger_.LogError(ex, "Impossible to delete the calculated Levenberg YPLModel from the YPLModelOutputsTable");
                                    success = false;
                                }
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
            if (guid != null && !guid.Equals(Guid.Empty))
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
                            if (parentId != null && !parentId.Equals(Guid.Empty))
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
        /// removes all YPLCalibrations older than the given date
        /// </summary>
        public bool Remove(DateTime old)
        {
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID FROM YPLCalibrationsTable WHERE TimeStamp < '" + (old - DateTime.MinValue).TotalSeconds.ToString() + "'";
                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Guid guid = reader.GetGuid(0);
                        if (Remove(guid))
                        {
                            logger_.LogInformation("An old YPLCalibration has been cleaned from the YPLCalibrationsTable successfully");
                            return true;
                        }
                        else
                        {
                            logger_.LogWarning("Impossible to clean an old YPLCalibration from the YPLCalibrationsTable");
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to clean old YPLCalibrations from the YPLCalibrationsTable");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
            }
            return false;
        }

        /// <summary>
        /// performs calculations on the given YPLCalibration and updates the YPLCalibration of given ID in the database
        /// </summary>
        public bool Update(Guid guid, YPLCalibration updatedYplCalibration)
        {
            bool success = true;
            if (guid != null && !guid.Equals(Guid.Empty) && updatedYplCalibration != null && !updatedYplCalibration.ID.Equals(Guid.Empty) &&
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
                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE YPLCalibrationsTable SET " +
                                "Name = '" + updatedYplCalibration.Name + "', " +
                                "Description = '" + updatedYplCalibration.Description + "', " +
                                "RheogramInputID = '" + updatedYplCalibration.RheogramInput.ID.ToString() + "', " +
                                "TimeStamp = '" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "' " +
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

                        // then update Mullineux YPLModelOutput (which may have changed after calculation) in YPLModelOutputsTable 
                        if (success)
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(updatedYplCalibration.YPLModelMullineux);
                                var command = connection_.CreateCommand();
                                command.CommandText = @"UPDATE YPLModelOutputsTable SET " +
                                    "Name = '" + updatedYplCalibration.YPLModelMullineux.Name + "', " +
                                    "YPLModel = '" + json + "' " +
                                    "WHERE ID = '" + updatedYplCalibration.YPLModelMullineux.ID.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count != 1)
                                {
                                    logger_.LogWarning("Impossible to update the calculated Mullineux YPLModel associated to the given YPLCalibration");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to update the calculated Mullineux YPLModel associated to the given YPLCalibration");
                                success = false;
                            }
                        }

                        // then update Kelessidis YPLModelOutput (which may have changed after calculation) in YPLModelOutputsTable 
                        if (success)
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(updatedYplCalibration.YPLModelKelessidis);
                                var command = connection_.CreateCommand();
                                command.CommandText = @"UPDATE YPLModelOutputsTable SET " +
                                    "Name = '" + updatedYplCalibration.YPLModelKelessidis.Name + "', " +
                                    "YPLModel = '" + json + "' " +
                                    "WHERE ID = '" + updatedYplCalibration.YPLModelKelessidis.ID.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count != 1)
                                {
                                    logger_.LogWarning("Impossible to update the calculated Kelessidis YPLModel associated to the given YPLCalibration");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to update the calculated Kelessidis YPLModel associated to the given YPLCalibration");
                                success = false;
                            }
                        }
                        // then update Levenberg YPLModelOutput (which may have changed after calculation) in YPLModelOutputsTable 
                        if (success)
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(updatedYplCalibration.YPLModelLevenbergMarquardt);
                                var command = connection_.CreateCommand();
                                command.CommandText = @"UPDATE YPLModelOutputsTable SET " +
                                    "Name = '" + updatedYplCalibration.YPLModelLevenbergMarquardt.Name + "', " +
                                    "YPLModel = '" + json + "' " +
                                    "WHERE ID = '" + updatedYplCalibration.YPLModelLevenbergMarquardt.ID.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count != 1)
                                {
                                    logger_.LogWarning("Impossible to update the calculated Levenberg-Marquardt YPLModel associated to the given YPLCalibration");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to update the calculated Levenberg-Marquardt YPLModel associated to the given YPLCalibration");
                                success = false;
                            }
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
            if (guid != null && !guid.Equals(Guid.Empty) && updatedRheogram != null)
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
                            if (parentId != null && !parentId.Equals(Guid.Empty))
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