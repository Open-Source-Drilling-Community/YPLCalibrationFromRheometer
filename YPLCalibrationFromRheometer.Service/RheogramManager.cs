using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service
{
    /// <summary>
    /// A manager for Rheograms. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class RheogramManager
    {
        private static RheogramManager instance_ = null;

        private Random random_ = new Random();

        private object lock_ = new object();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private RheogramManager()
        {
            // first initiate a call to the database to make sure all its tables are initialized
            List<Guid> baseData1Ids = GetIDs();

            // then create some default Rheogram's
            if (!baseData1Ids.Any())
                FillDefault();
        }

        public static RheogramManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RheogramManager();
                }
                return instance_;

            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command.CommandText = @"SELECT COUNT(*) FROM RheogramInputsTable";
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                count = (int)reader.GetInt64(0);
                            }
                        }
                    }
                    catch (SQLiteException e)
                    {
                    }
                }
                return count;
            }
        }

        public bool Clear()
        {
            if (SQLConnectionManager.Instance.Connection != null)
            {
                bool success = false;
                lock (lock_)
                {
                    using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                    {
                        try
                        {
                            var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                            command.CommandText = @"DELETE FROM RheogramInputsTable";
                            int count = command.ExecuteNonQuery();
                            transaction.Commit();
                            success = true;
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();
                        }
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
            if (SQLConnectionManager.Instance.Connection != null)
            {
                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            count = (int)reader.GetInt64(0);
                        }
                    }
                }
                catch (SQLiteException e)
                {
                }
            }
            return count >= 1;
        }

        public List<Guid> GetIDs()
        {
            List<Guid> ids = new List<Guid>();
            if (SQLConnectionManager.Instance.Connection != null)
            {
                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                command.CommandText = @"SELECT ID FROM RheogramInputsTable";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                ids.Add(reader.GetGuid(0));
                        }
                    }
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to get IDs from RheogramInputsTable");
                }
            }
            return ids;
        }

        public Rheogram Get(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                Rheogram baseData1 = null;
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command.CommandText = @"SELECT Name, Rheogram " +
                        "FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                string name = reader.GetString(0);
                                string json = reader.GetString(1);
                                baseData1 = JsonConvert.DeserializeObject<Rheogram>(json);
                                if (!baseData1.ID.Equals(guid) || !baseData1.Name.Equals(name))
                                    throw (new SQLiteException("SQLite database corrupted: RheometerMeasurement has been jsonified with the wrong ID or Name."));
                            }
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to get ID from RheogramInputsTable");
                    }
                }
                return baseData1;
            }
            else
            {
                return null;
            }
        }

        public bool Add(Rheogram baseData1)
        {
            bool result = false;
            if (baseData1 != null && baseData1.ID != null && !baseData1.ID.Equals(Guid.Empty))
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    lock (lock_)
                    {
                        using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(baseData1);
                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"INSERT INTO RheogramInputsTable (ID, Name, Rheogram) " +
                                    "VALUES (" +
                                    "'" + baseData1.ID.ToString() + "', " +
                                    "'" + baseData1.Name + "', " +
                                    "'" + json + "'" +
                                    ")";
                                int count = command.ExecuteNonQuery();
                                result = count == 1;
                                if (result)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                            }
                            catch (SQLiteException e)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                }
            }
            return result;
        }

        public bool Remove(Rheogram baseData1)
        {
            bool result = false;
            if (baseData1 != null)
            {
                result = Remove(baseData1.ID);
            }
            return result;
        }

        public bool Remove(Guid guid)
        {
            bool result = false;
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    // first select all YPLCalibration referencing as their RheogramInput the Rheogram identified by the given guid from YPLCalibrationsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command.CommandText = @"SELECT YPLCalibrationsTable.ID " +
                        "FROM YPLCalibrationsTable, RheogramInputsTable " +
                        "WHERE YPLCalibrationsTable.RheogramInputID = RheogramInputsTable.ID " +
                        "AND RheogramInputsTable.ID = '" + guid.ToString() + "'";
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid parentId = reader.GetGuid(0);
                                if (parentId != null && !parentId.Equals(Guid.Empty))
                                    parentIds.Add(parentId);
                            }
                            result = true;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to retrieve YPLCalibration referencing as their RheogramInput the Rheogram with the given guid from YPLCalibrationsTable");
                        return result;
                    }

                    // then delete all of them through the use of the YPLCalibrationManager (which ensures their children outputs are properly deleted)
                    foreach (Guid parentId in parentIds)
                    {
                        result = YPLCalibrationManager.Instance.Remove(parentId);
                        if (!result)
                        {
                            Console.WriteLine("Impossible to delete YPLCalibration referencing as their RheogramInput the Rheogram with the given guid from YPLCalibrationsTable");
                            break;
                        }
                    }

                    // finally delete the Rheogram identified by the given guid from RheogramInputsTable
                    if (result)
                    {
                        lock (lock_)
                        {
                            using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                            {
                                bool success = true;
                                try
                                {
                                    command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"DELETE FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    result = count >= 0;
                                    if (result)
                                    {
                                        success = true;
                                    }
                                    else
                                    {
                                        success = false;
                                        Console.WriteLine("No Rheogram with the given guid found for deletion in RheogramInputsTable");
                                    }
                                }
                                catch (SQLiteException e)
                                {
                                    success = false;
                                    Console.WriteLine("Impossible to delete the Rheogram with the given guid from RheogramInputsTable");
                                }

                                if (success)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public bool Update(Guid guid, Rheogram updatedBaseData1)
        {
            bool result = false;
            if (guid != null && !guid.Equals(Guid.Empty) && updatedBaseData1 != null)
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    // first update the Rheogram identified by the given baseData1ID from RheogramInputsTable
                    lock (lock_)
                    {
                        using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                        {
                            try
                            {
                                // make sure the ID stored in the json string matches the expected one
                                updatedBaseData1.ID = guid;
                                string json = JsonConvert.SerializeObject(updatedBaseData1);

                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"UPDATE RheogramInputsTable SET " +
                                    "Name = '" + updatedBaseData1.Name + "', " +
                                    "Rheogram = '" + json + "' " +
                                    "WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                result = count == 1;
                                if (result)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    transaction.Rollback();
                                }
                            }
                            catch (SQLiteException e)
                            {
                                transaction.Rollback();
                            }
                        }
                    }

                    // then select all YPLCalibration referencing as their RheogramInput the Rheogram identified by the given baseData1ID from YPLCalibrationsTable 
                    List<Guid> parentIds = new List<Guid>();
                    var command2 = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command2.CommandText = @"SELECT YPLCalibrationsTable.ID " +
                        "FROM YPLCalibrationsTable, RheogramInputsTable " +
                        "WHERE YPLCalibrationsTable.RheogramInputID = RheogramInputsTable.ID " +
                        "AND RheogramInputsTable.ID = '" + guid.ToString() + "'";
                    try
                    {
                        using (var reader = command2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid parentId = reader.GetGuid(0);
                                if (parentId != null && !parentId.Equals(Guid.Empty))
                                    parentIds.Add(parentId);
                            }
                            result = true;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to retrieve YPLCalibration referencing as their RheogramInput the Rheogram with the given guid from YPLCalibrationsTable");
                        return result;
                    }

                    // finally update all of them through the use of the YPLCalibrationManager (which ensures calculations are properly udpated)
                    foreach (Guid parentId in parentIds)
                    {
                        YPLCalibration calculationData = YPLCalibrationManager.Instance.Get(parentId);
                        if (calculationData != null)
                        {
                            calculationData.RheogramInput = updatedBaseData1;
                            result = YPLCalibrationManager.Instance.Update(parentId, calculationData);
                            if (!result)
                            {
                                Console.WriteLine("Impossible to delete YPLCalibration referencing as their RheogramInput the Rheogram with the given guid from YPLCalibrationsTable");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Impossible to get YPLCalibration with the given parentId from YPLCalibrationsTable");
                            return false;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// populate databasewith a few default Rheograms
        /// </summary>
        private void FillDefault()
        {
            List<Guid> ids = GetIDs();
            if (!ids.Any())
            {
                //////////////////////////////////
                // Example Rheogram #1 //
                //////////////////////////////////
                Rheogram baseData1 = new Rheogram();
                baseData1.ID = Guid.NewGuid();
                baseData1.Name = "Herschel-bulkley fluid";
                baseData1.Description = "Yield stress 2.000Pa, consistency index 0.750Pa.s^N and flow behavior index 0.500";
                baseData1.ShearStressStandardDeviation = 0.01; // Pa
                if (baseData1.RheometerMeasurementList == null)
                {
                    baseData1.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                /// RheometerMeasurements
                baseData1.Add(new RheometerMeasurement(1, 2.750));
                baseData1.Add(new RheometerMeasurement(2, 3.061));
                baseData1.Add(new RheometerMeasurement(4, 3.5));
                baseData1.Add(new RheometerMeasurement(8, 4.121));
                baseData1.Add(new RheometerMeasurement(16, 5.000));
                baseData1.Add(new RheometerMeasurement(32, 6.243));
                baseData1.Add(new RheometerMeasurement(64, 8));
                baseData1.Add(new RheometerMeasurement(128, 10.485));
                baseData1.Add(new RheometerMeasurement(256, 14));
                baseData1.Add(new RheometerMeasurement(512, 18.971));

                Add(baseData1);

                //////////////////////////////////
                // Example Rheogram #2 //
                //////////////////////////////////
                baseData1 = new Rheogram();
                baseData1.ID = Guid.NewGuid();
                baseData1.Name = "Quemada fluid";
                baseData1.Description = "Zero viscosity infinite, infinite viscosity 0.025Pa.s, reference shear rate 300.000 1/s and flow behavior index 0.400";
                baseData1.ShearStressStandardDeviation = 0.01; // Pa
                if (baseData1.RheometerMeasurementList == null)
                {
                    baseData1.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                /// RheometerMeasurements
                baseData1.Add(new RheometerMeasurement(1, 2.911));
                baseData1.Add(new RheometerMeasurement(2, 3.545));
                baseData1.Add(new RheometerMeasurement(4, 4.387));
                baseData1.Add(new RheometerMeasurement(8, 5.538));
                baseData1.Add(new RheometerMeasurement(16, 7.157));
                baseData1.Add(new RheometerMeasurement(32, 9.51));
                baseData1.Add(new RheometerMeasurement(64, 13.043));
                baseData1.Add(new RheometerMeasurement(128, 18.523));
                baseData1.Add(new RheometerMeasurement(256, 27.304));
                baseData1.Add(new RheometerMeasurement(512, 41.818));

                Add(baseData1);
            }
        }
    }
}
