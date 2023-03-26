using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace YPLCalibrationFromRheometer.Service
{
    public class CouetteRheometerManager : DataManager
    {
        private readonly RheogramManager rheogramManager_;

        public CouetteRheometerManager(ILoggerFactory loggerFactory, RheogramManager rheogramManager) : base(loggerFactory) 
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
                    command.CommandText = @"SELECT COUNT(*) FROM CouetteRheometersTable";
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
                        logger_.LogError(ex, "Impossible to count records in the CouetteRheometersTable");
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
                        command.CommandText = @"DELETE FROM CouetteRheometersTable";
                        int count = command.ExecuteNonQuery();
                        transaction.Commit();
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        logger_.LogError(ex, "Impossible to clear the CouetteRheometersTable");
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
            return ContainsRheometer(guid);
        }

        public List<Guid> GetIDs()
        {
            return GetRheometerIDs();
        }

        public CouetteRheometer Get(Guid guid)
        {
            return GetCouetteRheometer(guid);
        }

        public bool Remove(CouetteRheometer rheometer)
        {
            if (rheometer != null)
            {
                return Remove(rheometer.ID);
            }
            else
            {
                logger_.LogWarning("The given Couette rheometer is null");
                return false;
            }
        }

        public bool Remove(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    // delete the Couette rheometer identified by the given guid from CouetteRheometersTable
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            var command = connection_.CreateCommand();
                            command.CommandText = @"DELETE FROM CouetteRheometersTable WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count < 0)
                            {
                                logger_.LogWarning("Impossible to delete the Couette rheometer of given ID from the CouetteRheometersTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to delete the Couette rheometer of given ID from CouetteRheometersTable");
                            success = false;
                        }

                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Removed the Couette rheometer of given ID from the CouetteRheometersTable successfully");
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
                logger_.LogWarning("The Couette rheometer ID is null or empty");
            }
            return false;
        }

        public bool Update(Guid guid, CouetteRheometer rheometer)
        {
            if (!guid.Equals(Guid.Empty) && rheometer != null)
            {
                if (connection_ != null)
                {
                    // update the Couette rheometer identified by the given ID from CouetteRheometersTable
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            // make sure the ID stored in the json string matches the expected one
                            rheometer.ID = guid;
                            string json = JsonConvert.SerializeObject(rheometer);

                            var command = connection_.CreateCommand();
                            command.CommandText = @"UPDATE CouetteRheometersTable SET " +
                                "Name = '" + rheometer.Name + "', " +
                                "Description = '" + rheometer.Description + "', " +
                                "Data = '" + json + "' " +
                                "WHERE ID = '" + guid.ToString() + "'";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to update the Couette rheometer of given ID");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to update Couette rheometer of given ID");
                            success = false;
                        }
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Updated the Couette rheometer of given ID successfully");
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
            }
            else
            {
                logger_.LogWarning("The Couette rheometer is null or its ID is null or empty");
            }
            return false;
        }

     }
}
