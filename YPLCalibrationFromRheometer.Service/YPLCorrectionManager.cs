using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace YPLCalibrationFromRheometer.Service
{
    public class YPLCorrectionManager : DataManager
    {
        private readonly RheogramManager rheogramManager_;

        public YPLCorrectionManager(ILoggerFactory loggerFactory, RheogramManager rheogramManager) : base(loggerFactory)
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
                        // then empty YPLCorrectionsTable
                        var command = connection_.CreateCommand();
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
            if (!guid.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = null;
                if (connection_ != null)
                {
                    Guid inputID = Guid.Empty;
                    // retrieve the YPLCorrection 
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, " +
                                           "RheogramInputID, Data " +
                                           "FROM YPLCorrectionsTable " +
                                           "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            Guid ID = guid;
                            string name = reader.GetString(0);
                            string description = reader.GetString(1);
                            inputID = reader.GetGuid(2);
                            string json = reader.GetString(3);
                            yplCorrection = JsonConvert.DeserializeObject<YPLCorrection>(json);
                            if (yplCorrection == null ||
                                !yplCorrection.ID.Equals(guid) ||
                                !yplCorrection.Name.Equals(name) ||
                                inputID == Guid.Empty)
                                throw (new SQLiteException("SQLite database corrupted: yplCorrection has been jsonified with the wrong ID or Name."));
                            Rheogram rheogram = GetRheogram(inputID);
                            yplCorrection.RheogramInput = rheogram;
                            return yplCorrection;
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
            // every YPLCorrection added to the database should have both Rheogram input and outputs different from null and with non empty ID
            if (yplCorrection != null &&
                yplCorrection.RheogramInput != null &&
                yplCorrection.RheogramFullyCorrected != null &&
                yplCorrection.RheogramShearRateCorrected != null &&
                yplCorrection.RheogramShearStressCorrected != null &&
                !yplCorrection.RheogramInput.ID.Equals(Guid.Empty))
            {
                // first apply calculations
                if (!yplCorrection.CalculateInputRheogram() ||
                    !yplCorrection.CalculateFullyCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt) ||
                    !yplCorrection.CalculateShearRateCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt) ||
                    !yplCorrection.CalculateShearStressCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt))
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
                            string json = JsonConvert.SerializeObject(yplCorrection);
                            // first add the YPLCorrection to the YPLCorrectionsTable
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO YPLCorrectionsTable " +
                                "(ID, Name, Description, " +
                                "RheogramInputID, Data) " +
                                "VALUES " +
                                "(" +
                                "'" + yplCorrection.ID.ToString() + "', " +
                                "'" + yplCorrection.Name + "', " +
                                "'" + yplCorrection.Description + "', " +
                                "'" + yplCorrection.RheogramInput.ID.ToString() + "', " +
                                "'" + json + "' " +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count == 1)
                            {
                                success = true;
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
        /// removes the YPLCorrection of given guid from the database (Rheogram output children are also removed)
        /// </summary>
        public bool Remove(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = Get(guid);

                // every YPLCorrection added to the database should have both Rheogram input and outputs different from null
                if (yplCorrection != null && yplCorrection.RheogramInput != null && yplCorrection.RheogramFullyCorrected != null && yplCorrection.RheogramShearRateCorrected != null && yplCorrection.RheogramShearStressCorrected != null)
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
            if (!guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        // delete all YPLCorrections referencing the Rheogram as their input identified by the given ID from YPLCalibrationsTable 
                        try
                        {
                            var command = connection_.CreateCommand();
                            command.CommandText = @"DELETE FROM YPLCorrectionsTable WHERE RheogramInputID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count < 0)
                            {
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to delete YPLCorrections referencing the Rheogram of given ID from YPLCorrectionsTable");
                            success = false;
                        }
                        if (success)
                        {
                            logger_.LogInformation("Removed all YPLCorrections referencing the Rheogram of given ID successfully");
                            transaction.Commit();
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
                logger_.LogWarning("The Rheogram ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// performs calculations on the given YPLCorrection and updates the YPLCorrection of given ID in the database
        /// </summary>
        public bool Update(Guid guid, YPLCorrection updatedYplCorrection)
        {
            bool success = true;
            if (!guid.Equals(Guid.Empty) && updatedYplCorrection != null && !updatedYplCorrection.ID.Equals(Guid.Empty) && updatedYplCorrection.RheogramInput != null &&
                updatedYplCorrection.RheogramFullyCorrected != null && updatedYplCorrection.RheogramShearRateCorrected != null && updatedYplCorrection.RheogramShearStressCorrected != null)
            {
                // first apply calculations
                if (!updatedYplCorrection.CalculateInputRheogram() ||
                    !updatedYplCorrection.CalculateFullyCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt) ||
                    !updatedYplCorrection.CalculateShearRateCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt) ||
                    !updatedYplCorrection.CalculateShearStressCorrected(Rheogram.CalibrationMethodEnum.LevenbergMarquardt))
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
                            string json = JsonConvert.SerializeObject(updatedYplCorrection);
                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE YPLCorrectionsTable SET " +
                                "Name = '" + updatedYplCorrection.Name + "', " +
                                "Description = '" + updatedYplCorrection.Description + "', " +
                                "RheogramInputID = '" + updatedYplCorrection.RheogramInput.ID.ToString() + "', " +
                                "Data = '" + json + "' " +
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
            if (!guid.Equals(Guid.Empty) && updatedRheogram != null)
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
                            if (!parentId.Equals(Guid.Empty))
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
                                logger_.LogWarning("Impossible to update one of the YPLCorrections referencing the Rheogram of given ID");
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