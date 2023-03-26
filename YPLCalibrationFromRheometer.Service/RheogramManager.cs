using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;

namespace YPLCalibrationFromRheometer.Service
{
    public class RheogramManager : DataManager
    {
        public RheogramManager(ILoggerFactory loggerFactory) :base(loggerFactory)
        {
        }

        public int Count
        {
            get
            {
                int count = 0;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT COUNT(*) FROM RheogramInputsTable";
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
                        logger_.LogError(ex, "Impossible to count records in the RheogramInputsTable");
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
                    using var transaction = connection_.BeginTransaction();
                    try
                    {
                        var command = connection_.CreateCommand();
                        command.CommandText = @"DELETE FROM RheogramInputsTable";
                        int count = command.ExecuteNonQuery();
                        transaction.Commit();
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        logger_.LogError(ex, "Impossible to clear the RheogramInputsTable");
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
                command.CommandText = @"SELECT COUNT(*) FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
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
                    logger_.LogError(ex, "Impossible to count rows from RheogramInputsTable");
                }
            }
            return count >= 1;
        }

        public List<Guid> GetIDs()
        {
            return GetRheogramIDs();
        }

        public Rheogram Get(Guid guid)
        {
            return GetRheogram(guid);
        }

        public bool Remove(Rheogram rheogram)
        {
            if (rheogram != null)
            {
                return Remove(rheogram.ID);
            }
            else
            {
                logger_.LogWarning("The given Rheogram is null");
                return false;
            }
        }

        public bool Remove(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    // delete the Rheogram identified by the given guid from RheogramInputsTable
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            var command = connection_.CreateCommand();
                            command.CommandText = @"DELETE FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count < 0)
                            {
                                logger_.LogWarning("Impossible to delete the Rheogram of given ID from the RheogramInputsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to delete the Rheogram of given ID from RheogramInputsTable");
                            success = false;
                        }

                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Removed the Rheogram of given ID from the RheogramInputsTable successfully");
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
                logger_.LogWarning("The Rheogram ID is null or empty");
            }
            return false;
        }

        public bool Update(Guid guid, Rheogram rheogram)
        {
            if (!guid.Equals(Guid.Empty) && 
                rheogram != null &&
                !rheogram.CouetteRheometerID.Equals(Guid.Empty))
            {
                CouetteRheometer rheometer = GetCouetteRheometer(rheogram.CouetteRheometerID);
                if (rheometer != null)
                {
                    rheogram.SetRheometer(rheometer);
                    rheogram.Calculate();
                    if (connection_ != null)
                    {
                        // update the Rheogram identified by the given ID from RheogramInputsTable
                        lock (lock_)
                        {
                            using var transaction = connection_.BeginTransaction();
                            bool success = true;
                            try
                            {
                                // make sure the ID stored in the json string matches the expected one
                                rheogram.ID = guid;
                                string json = JsonConvert.SerializeObject(rheogram);

                                var command = connection_.CreateCommand();
                                command.CommandText = @"UPDATE RheogramInputsTable SET " +
                                                       "Name = '" + rheogram.Name + "', " +
                                                       "Description = '" + rheogram.Description + "', " +
                                                       "CouetteRheometerID = '" + rheogram.CouetteRheometerID.ToString() + "', " +
                                                       "Data = '" + json + "' " +
                                                       "WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                if (count != 1)
                                {
                                    logger_.LogWarning("Impossible to update the Rheogram of given ID");
                                    success = false;
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                logger_.LogError(ex, "Impossible to update Rheogram of given ID");
                                success = false;
                            }
                            if (success)
                            {
                                transaction.Commit();
                                logger_.LogInformation("Updated the Rheogram of given ID successfully");
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        logger_.LogWarning("Impossible to access the SQLite database");
                    }
                }else
                {
                    logger_.LogWarning("Couldn't upload the rheometer");
                }
            }
            else
            {
                logger_.LogWarning("The Rheogram is null or its ID is null or empty");
            }
            return false;
        }

     }
}
