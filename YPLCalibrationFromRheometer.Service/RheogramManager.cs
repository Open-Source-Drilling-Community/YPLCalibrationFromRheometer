using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;
using Microsoft.Extensions.Logging;

namespace YPLCalibrationFromRheometer.Service
{
    public class RheogramManager
    {
        private readonly ILogger logger_;
        private readonly object lock_ = new object();
        private readonly SQLiteConnection connection_;

        public RheogramManager(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<RheogramManager>();
            connection_ = SQLConnectionManager.GetConnection(loggerFactory);

            // first initiate a call to the database to make sure all its tables are initialized
            List<Guid> baseData1Ids = GetIDs();

            // then create some default Rheogram's
            if (!baseData1Ids.Any())
                FillDefault();
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
            List<Guid> ids = new List<Guid>();
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID FROM RheogramInputsTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            ids.Add(reader.GetGuid(0));
                    }
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to get IDs from RheogramInputsTable");
                }
            }
            return ids;
        }

        public Rheogram Get(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                Rheogram rheogram = null;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Rheogram " +
                        "FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string name = reader.GetString(0);
                            string json = reader.GetString(1);
                            rheogram = JsonConvert.DeserializeObject<Rheogram>(json);
                            if (!rheogram.ID.Equals(guid) || !rheogram.Name.Equals(name))
                                throw (new SQLiteException("SQLite database corrupted: RheometerMeasurement has been jsonified with the wrong ID or Name."));
                        }
                        else
                        {
                            logger_.LogInformation("No such rheogram in the database");
                            return null;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to get the Rheogram with the given ID from RheogramInputsTable");
                        return null;
                    }
                    // Finalizing
                    logger_.LogInformation("Returning the Rheogram of given ID from RheogramInputsTable");
                    return rheogram;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The given Rheogram ID is null or empty");
            }
            return null;
        }

        public bool Add(Rheogram rheogram)
        {
            if (rheogram != null && rheogram.ID != null && !rheogram.ID.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string json = JsonConvert.SerializeObject(rheogram);
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO RheogramInputsTable (ID, Name, Rheogram) " +
                                "VALUES (" +
                                "'" + rheogram.ID.ToString() + "', " +
                                "'" + rheogram.Name + "', " +
                                "'" + json + "'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to insert the Rheogram into the RheogramInputsTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to add the Rheogram into RheogramInputsTable");
                            success = false;
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Added the Rheogram of given ID into the RheogramInputsTable successfully");
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
                logger_.LogWarning("The Rheogram is null or its ID is null or empty");
            }
            return false;
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
            if (guid != null && !guid.Equals(Guid.Empty))
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
            if (guid != null && !guid.Equals(Guid.Empty) && rheogram != null)
            {
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
                                "Rheogram = '" + json + "' " +
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
            }
            else
            {
                logger_.LogWarning("The Rheogram is null or its ID is null or empty");
            }
            return false;
        }

        /// <summary>
        /// populates database with a few default Rheograms
        /// </summary>
        private void FillDefault()
        {
            List<Guid> ids = GetIDs();
            if (!ids.Any())
            {
                //////////////////////////////////
                // Example Rheogram #1 //
                //////////////////////////////////
                Rheogram rheogram = new Rheogram
                {
                    ID = Guid.NewGuid(),
                    Name = "Herschel-bulkley fluid",
                    Description = "Yield stress 2.000Pa, consistency index 0.750Pa.s^N and flow behavior index 0.500",
                    ShearStressStandardDeviation = 0.01 // Pa
                };
                if (rheogram.RheometerMeasurementList == null)
                {
                    rheogram.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                /// RheometerMeasurements
                rheogram.Add(new RheometerMeasurement(1, 2.750));
                rheogram.Add(new RheometerMeasurement(2, 3.061));
                rheogram.Add(new RheometerMeasurement(4, 3.5));
                rheogram.Add(new RheometerMeasurement(8, 4.121));
                rheogram.Add(new RheometerMeasurement(16, 5.000));
                rheogram.Add(new RheometerMeasurement(32, 6.243));
                rheogram.Add(new RheometerMeasurement(64, 8));
                rheogram.Add(new RheometerMeasurement(128, 10.485));
                rheogram.Add(new RheometerMeasurement(256, 14));
                rheogram.Add(new RheometerMeasurement(512, 18.971));

                Add(rheogram);

                //////////////////////////////////
                // Example Rheogram #2 //
                //////////////////////////////////
                rheogram = new Rheogram
                {
                    ID = Guid.NewGuid(),
                    Name = "Quemada fluid",
                    Description = "Zero viscosity infinite, infinite viscosity 0.025Pa.s, reference shear rate 300.000 1/s and flow behavior index 0.400",
                    ShearStressStandardDeviation = 0.01 // Pa
                };
                if (rheogram.RheometerMeasurementList == null)
                {
                    rheogram.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                /// RheometerMeasurements
                rheogram.Add(new RheometerMeasurement(1, 2.911));
                rheogram.Add(new RheometerMeasurement(2, 3.545));
                rheogram.Add(new RheometerMeasurement(4, 4.387));
                rheogram.Add(new RheometerMeasurement(8, 5.538));
                rheogram.Add(new RheometerMeasurement(16, 7.157));
                rheogram.Add(new RheometerMeasurement(32, 9.51));
                rheogram.Add(new RheometerMeasurement(64, 13.043));
                rheogram.Add(new RheometerMeasurement(128, 18.523));
                rheogram.Add(new RheometerMeasurement(256, 27.304));
                rheogram.Add(new RheometerMeasurement(512, 41.818));

                Add(rheogram);
            }
        }
    }
}
