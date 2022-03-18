using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;

namespace YPLCalibrationFromRheometer.Service
{
    public class YPLCorrectionManager
    {
        private readonly ILogger logger_;
        private readonly object lock_ = new object();
        private readonly RheogramManager rheogramManager_;
        private readonly SQLiteConnection connection_;

        public YPLCorrectionManager(ILoggerFactory loggerFactory, RheogramManager rheogramManager)
        {
            logger_ = loggerFactory.CreateLogger<YPLCorrectionManager>();
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
                    command.CommandText = @"SELECT COUNT(*) FROM YPLCorrectionsTable";
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
                        logger_.LogError(ex, "Impossible to count records in the YPLCorrectionsTable");
                    }
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
                    using SQLiteTransaction transaction = connection_.BeginTransaction();
                    try
                    {
                        // first empty RheogramOutputsTable
                        var command = connection_.CreateCommand();
                        command.CommandText = @"DELETE FROM RheogramOutputsTable";
                        command.ExecuteNonQuery();

                        // then empty YPLCorrectionsTable
                        command = connection_.CreateCommand();
                        command.CommandText = @"DELETE FROM YPLCorrectionsTable";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        logger_.LogError(ex, "Impossible to clear the YPLCalibrationsTable or the RheogramOutputsTable");
                    }
                }
                return success;
            }
            else
            {
                return false;
            }
        }

        public bool Contains(Guid guid)
        {
            int count = 0;
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM YPLCorrectionsTable WHERE ID = '" + guid + "'";
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
                    logger_.LogError(ex, "Impossible to count rows from YPLCorrectionsTable");
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
                command.CommandText = @"SELECT ID FROM YPLCorrectionsTable";
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
                    logger_.LogError(ex, "Impossible to get IDs from YPLCorrectionsTable");
                }
            }
            return ids;
        }

        public YPLCorrection Get(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = null;
                if (connection_ != null)
                {
                    Guid inputID = Guid.Empty;
                    Guid shearRateCorrectedID = Guid.Empty;

                    // first retrieve the YPLCorrection itself
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, R1, R2, RheogramInputID, RheogramShearRateCorrectedID FROM YPLCorrectionsTable " +
                        "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            yplCorrection = new YPLCorrection()
                            {
                                ID = guid,
                                Name = reader.GetString(0),
                                Description = reader.GetString(1),
                                R1 = reader.GetDouble(2),
                                R2 = reader.GetDouble(3)
                            };
                            inputID = reader.GetGuid(4);
                            shearRateCorrectedID = reader.GetGuid(5);
                        }
                        else
                        {
                            logger_.LogInformation("No YPLCorrection in the database");
                            return null;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to get the YPLCorrection with the given ID from YPLCorrectionsTable");
                        return null;
                    }

                    // then retrieve its RheogramInput with the rheogramManager_
                    Rheogram rheogram = rheogramManager_.Get(inputID);
                    if (rheogram != null)
                    {
                        yplCorrection.RheogramInput = rheogram;

                        // then retrieve its RheogramShearRateCorrected directly from RheogramOutputsTable
                        command = connection_.CreateCommand();
                        command.CommandText = @"SELECT Name, Rheogram " +
                            "FROM RheogramOutputsTable WHERE ID = '" + shearRateCorrectedID.ToString() + "'";
                        try
                        {
                            using SQLiteDataReader reader = command.ExecuteReader();
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                string name = reader.GetString(0);
                                string json = reader.GetString(1);
                                Rheogram baseData2 = JsonConvert.DeserializeObject<Rheogram>(json);
                                if (baseData2 == null || !baseData2.ID.Equals(shearRateCorrectedID) || !baseData2.Name.Equals(name))
                                    throw (new SQLiteException("SQLite database corrupted: Rheogram has been jsonified with the wrong ID or Name."));
                                yplCorrection.RheogramShearRateCorrected = baseData2;
                            }
                            else
                            {
                                logger_.LogWarning("No such Rheogram output associated to the Rheogram input in the RheogramOutputsTable");
                                return null;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to get such Rheogram output from RheogramOutputsTable");
                            return null;
                        }
                        // Finalizing
                        logger_.LogInformation("Returning the YPLCorrection of given ID");
                        return yplCorrection;
                    }
                    else
                    {
                        logger_.LogWarning("No RheogramInput associated to the YPLCorrection of given ID");
                        return null;
                    }
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                    return null;
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCorrection ID is null or empty");
                return null;
            }
        }

        /// <summary>
        /// performs calculations on the given YPLCorrection and adds it to the database
        /// </summary>
        public bool Add(YPLCorrection yplCorrection)
        {
            // every YPLCorrection added to the database should have both RheogramInput and RheogramShearRateCorrected different from null and with non empty ID
            if (yplCorrection != null && yplCorrection.RheogramInput != null && yplCorrection.RheogramShearRateCorrected != null &&
                !yplCorrection.RheogramInput.ID.Equals(Guid.Empty) && !yplCorrection.RheogramShearRateCorrected.ID.Equals(Guid.Empty))
            {
                // first apply calculations
                if (!yplCorrection.CalculateRheogramShearRateCorrected())
                {
                    logger_.LogWarning("Impossible to calculate outputs for the given YPLCorrection");
                    return false;
                }

                // then update to database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            // first add the YPLCorrection to the YPLCorrectionsTable
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO YPLCorrectionsTable " +
                                "(ID, Name, Description, R1, R2, RheogramInputID, RheogramShearRateCorrectedID, TimeStamp) VALUES (" +
                                "'" + yplCorrection.ID.ToString() + "', " +
                                "'" + yplCorrection.Name + "', " +
                                "'" + yplCorrection.Description + "', " +
                                "'" + yplCorrection.R1.ToString() + "', " +
                                "'" + yplCorrection.R2.ToString() + "', " +
                                "'" + yplCorrection.RheogramInput.ID.ToString() + "', " +
                                "'" + yplCorrection.RheogramShearRateCorrected.ID.ToString() + "', " +
                                "'" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count == 1)
                            {
                                // then add YPLModelOutput to the RheogramOutputsTable
                                Rheogram rheogram = yplCorrection.RheogramShearRateCorrected;
                                if (rheogram != null)
                                {
                                    string json = JsonConvert.SerializeObject(rheogram);
                                    command = connection_.CreateCommand();
                                    command.CommandText = @"INSERT INTO RheogramOutputsTable (ID, Name, Rheogram) " +
                                        "VALUES (" +
                                        "'" + rheogram.ID.ToString() + "', " +
                                        "'" + rheogram.Name + "', " +
                                        "'" + json + "'" +
                                        ")";
                                    count = command.ExecuteNonQuery();
                                    if (count != 1)
                                    {
                                        logger_.LogWarning("Impossible to insert the calculated Rheogram into the RheogramOutputsTable");
                                        success = false;
                                    }
                                }
                                else
                                {
                                    logger_.LogWarning("Impossible to get the Rheogram");
                                    success = false;
                                }
                            }
                            else
                            {
                                logger_.LogWarning("Impossible to insert the YPLCorrection into the YPLCorrectionsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to add the YPLCorrection into YPLCorrectionsTable");
                            success = false;
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Added the YPLCorrection of given ID into the YPLCorrectionsTable successfully");
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
                    return false;
                }
            }
            else
            {
                logger_.LogWarning("The YPLCorrection ID or the ID of some of its attributes are null or empty");
                return false;
            }
        }

        /// <summary>
        /// creates a YPLCorrection from the given Rheogram and adds it to the database 
        /// </summary>
        public bool Add(Rheogram rheogram)
        {
            if (rheogram != null && !rheogram.ID.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = new YPLCorrection()
                {
                    ID = Guid.NewGuid()
                };
                yplCorrection.Name = "DefaultName-" + yplCorrection.ID.ToString()[..8];
                yplCorrection.Description = "DefaultDescription-" + yplCorrection.ID.ToString()[..8];
                // R1 and R2 are set to their default value
                yplCorrection.RheogramInput = rheogram;
                yplCorrection.RheogramInput.ID = rheogram.ID;
                yplCorrection.RheogramInput.Name = yplCorrection.Name + "-input";
                yplCorrection.RheogramShearRateCorrected = new Rheogram
                {
                    ID = Guid.NewGuid(),
                    Name = yplCorrection.Name + "-calculated-shearRateCorrected"
                };

                if (Add(yplCorrection))
                {
                    logger_.LogInformation("Created a YPLCorrection from the given Rheogram successfully");
                    return true;
                }
                else
                {
                    logger_.LogWarning("Impossible to add a YPLCorrection from the given Rheogram");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to create a YPLCorrection from the given Rheogram which is null or badly identified");
            }
            return false;
        }

        /// <summary>
        /// removes the YPLCorrection of given guid from the database (Rheogram output children are also removed)
        /// </summary>
        public bool Remove(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = Get(guid);

                // every YPLCorrection added to the database should have both RheogramInput and YPLModelOutput different from null
                if (yplCorrection != null && yplCorrection.RheogramInput != null && yplCorrection.RheogramShearRateCorrected != null)
                {
                    if (connection_ != null)
                    {
                        lock (lock_)
                        {
                            using SQLiteTransaction transaction = connection_.BeginTransaction();
                            bool success = true;
                            // first delete YPLCorrection from YPLCorrectionsTable
                            try
                            {
                                var command = connection_.CreateCommand();
                                command.CommandText = @"DELETE FROM YPLCorrectionsTable WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count < 0)
                                {
                                    logger_.LogWarning("Impossible to delete the YPLCorrection of given ID from the YPLCorrectionsTable");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to delete the YPLCorrection of given ID from YPLCorrectionsTable");
                                success = false;
                            }
                            if (success)
                            {
                                // then delete RheogramShearRateCorrected from RheogramOutputsTable
                                try
                                {
                                    var command = connection_.CreateCommand();
                                    command.CommandText = @"DELETE FROM RheogramOutputsTable WHERE ID = '" + yplCorrection.RheogramShearRateCorrected.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    if (count < 0)
                                    {
                                        logger_.LogWarning("Impossible to delete the calculated Rheogram from the RheogramOutputsTable");
                                        success = false;
                                    }
                                }
                                catch (SQLiteException ex)
                                {
                                    logger_.LogError(ex, "Impossible to delete the calculated Rheogram from the RheogramOutputsTable");
                                    success = false;
                                }
                            }
                            if (success)
                            {
                                transaction.Commit();
                                logger_.LogInformation("Removed the YPLCorrection of given ID from the YPLCorrectionsTable successfully");
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
                    logger_.LogWarning("Impossible to remove the YPLCorrection of given ID because it is null or one of its attributes is");
                }
            }
            else
            {
                logger_.LogWarning("The YPLCorrection ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// removes all YPLCorrections referencing the Rheogram of given ID
        /// </summary>
        public bool RemoveReferences(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    // first select all YPLCorrections referencing the Rheogram as their input identified by the given ID from YPLCalibrationsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT YPLCorrectionsTable.ID " +
                        "FROM YPLCorrectionsTable, RheogramInputsTable " +
                        "WHERE YPLCorrectionsTable.RheogramInputID = RheogramInputsTable.ID " +
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
                        logger_.LogError(ex, "Impossible to retrieve YPLCorrections referencing the Rheogram of given ID from YPLCorrectionsTable");
                        return false;
                    }

                    // then delete all of them through the use of the YPLCorrectionManager (which ensures their children outputs are properly deleted)
                    foreach (Guid parentId in parentIds)
                    {
                        if (!Remove(parentId))
                        {
                            logger_.LogWarning("Impossible to delete one of the YPLCorrections referencing the Rheogram of given ID from YPLCorrectionsTable");
                            return false;
                        }
                    }
                    // Finalizing
                    logger_.LogInformation("Removed all YPLCorrections referencing the Rheogram of given ID from the YPLCorrectionsTable successfully");
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
        /// removes all YPLCorrections older than the given date
        /// </summary>
        public bool Remove(DateTime old)
        {
            Guid guid;
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID FROM YPLCorrectionsTable WHERE TimeStamp < '" + (old - DateTime.MinValue).TotalSeconds.ToString() + "'";
                try
                {
                    using SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        guid = reader.GetGuid(0);
                        if (Remove(guid))
                        {
                            logger_.LogInformation("An old YPLCorrection has been cleaned from the YPLCorrectionsTable successfully");
                            return true;
                        }
                        else
                        {
                            logger_.LogWarning("Impossible to clean an old YPLCorrection from the YPLCorrectionsTable");
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to clean old YPLCorrections from the YPLCorrectionsTable");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to access the SQLite database");
            }
            return false;
        }

        /// <summary>
        /// performs calculations on the given YPLCorrection and updates the YPLCorrection of given ID in the database
        /// </summary>
        public bool Update(Guid guid, YPLCorrection updatedYplCorrection)
        {
            bool success = true;
            if (guid != null && !guid.Equals(Guid.Empty) && updatedYplCorrection != null && !updatedYplCorrection.ID.Equals(Guid.Empty) &&
                updatedYplCorrection.RheogramInput != null && updatedYplCorrection.RheogramShearRateCorrected != null)
            {
                // first apply calculations
                if (!updatedYplCorrection.CalculateRheogramShearRateCorrected())
                {
                    logger_.LogWarning("Impossible to calculate outputs for the given YPLCorrection");
                    return false;
                }

                // then update to database
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using SQLiteTransaction transaction = connection_.BeginTransaction();
                        // first update the relevant fields in YPLCorrectionsTable (other fields never change)
                        try
                        {
                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE YPLCorrectionsTable SET " +
                                "Name = '" + updatedYplCorrection.Name + "', " +
                                "Description = '" + updatedYplCorrection.Description + "', " +
                                "R1 = '" + updatedYplCorrection.R1 + "', " +
                                "R2 = '" + updatedYplCorrection.R2 + "', " +
                                "RheogramInputID = '" + updatedYplCorrection.RheogramInput.ID.ToString() + "', " +
                                "TimeStamp = '" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "' " +
                                "WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to update the YPLCorrection");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to update the YPLCorrection");
                            success = false;
                        }

                        // then update RheogramShearRateCorrected (which may have changed after calculation) in RheogramOutputsTable 
                        if (success)
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(updatedYplCorrection.RheogramShearRateCorrected);
                                var command = connection_.CreateCommand();
                                command.CommandText = @"UPDATE RheogramOutputsTable SET " +
                                    "Name = '" + updatedYplCorrection.RheogramShearRateCorrected.Name + "', " +
                                    "Rheogram = '" + json + "' " +
                                    "WHERE ID = '" + updatedYplCorrection.RheogramShearRateCorrected.ID.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count != 1)
                                {
                                    logger_.LogWarning("Impossible to update the calculated Rheogram associated to the given YPLCorrection");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to update the calculated Rheogram associated to the given YPLCorrection");
                                success = false;
                            }
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Updated the given YPLCorrection successfully");
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
                logger_.LogWarning("The YPLCorrection ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// updates all YPLCorrections referencing the Rheogram of given ID
        /// </summary>
        public bool UpdateReferences(Guid guid, Rheogram updatedRheogram)
        {
            if (guid != null && !guid.Equals(Guid.Empty) && updatedRheogram != null)
            {
                if (connection_ != null)
                {
                    // select all YPLCorrections referencing the Rheogram identified by the given ID from YPLCorrectionsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command2 = connection_.CreateCommand();
                    command2.CommandText = @"SELECT YPLCorrectionsTable.ID " +
                        "FROM YPLCorrectionsTable, RheogramInputsTable " +
                        "WHERE YPLCorrectionsTable.RheogramInputID = RheogramInputsTable.ID " +
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
                        logger_.LogError(ex, "Impossible to access to the YPLCorrection referencing the Rheogram of given ID from YPLCorrectionsTable");
                        return false;
                    }

                    // then update all of them
                    foreach (Guid parentId in parentIds)
                    {
                        YPLCorrection yplCorrection = Get(parentId);
                        if (yplCorrection != null)
                        {
                            yplCorrection.RheogramInput = updatedRheogram;
                            if (!Update(parentId, yplCorrection))
                            {
                                logger_.LogWarning("Impossible to delete one of the YPLCorrections referencing the Rheogram of given ID");
                                return false;
                            }
                        }
                        else
                        {
                            logger_.LogWarning("Impossible to get one of the YPLCorrections referencing the Rheogram of given ID");
                            return false;
                        }
                    }
                    // Finalizing
                    logger_.LogWarning("Updated YPLCorrections referencing the Rheogram of given ID successfully");
                    return true;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("Impossible to update the YPLCorrections referencing the Rheogram which is null or badly identified");
            }
            return false;
        }
    }
}