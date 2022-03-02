using System;
using System.Collections.Generic;
using System.Threading;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service
{
    /// <summary>
    /// A manager for YPLCorrection. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class YPLCorrectionManager
    {
        private static YPLCorrectionManager instance_ = null;

        private Random random_ = new Random();

        private object lock_ = new object();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private YPLCorrectionManager()
        {
            // first initiate a call to the database to make sure all its tables are initialized and in the same time, make sure default Rheogram's are created
            RheogramManager.Instance.GetIDs();

            // then launch a garbage collector which automatically removes old YPLCorrection's from the database
            Thread thread = new Thread(new ThreadStart(GC));
            thread.Start();
        }

        public static YPLCorrectionManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new YPLCorrectionManager();
                }
                return instance_;

            }
        }

        /// <summary>
        /// database garbage collector
        /// </summary>
        private void GC()
        {
            while (true)
            {
                Remove(DateTime.UtcNow - TimeSpan.FromSeconds(360000));
                Thread.Sleep(10000);
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
                    command.CommandText = @"SELECT COUNT(*) FROM YPLCorrectionsTable";
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
                            // first empty RheogramOutputsTable
                            var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                            command.CommandText = @"DELETE FROM RheogramOutputsTable";
                            command.ExecuteNonQuery();

                            // then empty YPLCorrectionsTable
                            command = SQLConnectionManager.Instance.Connection.CreateCommand();
                            command.CommandText = @"DELETE FROM YPLCorrectionsTable";
                            command.ExecuteNonQuery();

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
                command.CommandText = @"SELECT COUNT(*) FROM YPLCorrectionsTable WHERE ID = '" + guid + "'";
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
                    Console.WriteLine("Impossible to count rows from YPLCorrectionsTable");
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
                command.CommandText = @"SELECT ID FROM YPLCorrectionsTable";
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
                    Console.WriteLine("Impossible to get IDs from YPLCorrectionsTable");
                }
            }
            return ids;
        }

        public YPLCorrection Get(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                YPLCorrection calculationData = null;
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    Guid inputID = Guid.Empty;
                    Guid shearRateCorrectedID = Guid.Empty;

                    // first retrieve the YPLCorrection itself
                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, R1, R2, RheogramInputID, RheogramShearRateCorrectedID FROM YPLCorrectionsTable " +
                        "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                calculationData = new YPLCorrection();
                                calculationData.ID = guid;
                                calculationData.Name = reader.GetString(0);
                                calculationData.Description = reader.GetString(1);
                                calculationData.R1 = reader.GetDouble(2);
                                calculationData.R2 = reader.GetDouble(3);
                                inputID = reader.GetGuid(4);
                                shearRateCorrectedID = reader.GetGuid(5);
                            }
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to get the YPLCorrection with the given guid from YPLCorrectionsTable");
                    }

                    // then retrieve its RheogramInput with the RheogramManager
                    Rheogram baseData1 = RheogramManager.Instance.Get(inputID);
                    if (baseData1 != null)
                    {
                        calculationData.RheogramInput = baseData1;

                        // then retrieve its RheogramShearRateCorrected directly from RheogramOutputsTable
                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                        command.CommandText = @"SELECT Name, Rheogram " +
                            "FROM RheogramOutputsTable WHERE ID = '" + shearRateCorrectedID.ToString() + "'";
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read() && !reader.IsDBNull(0))
                                {
                                    string name = reader.GetString(0);
                                    string json = reader.GetString(1);
                                    Rheogram baseData2 = JsonConvert.DeserializeObject<Rheogram>(json);
                                    if (baseData2 == null || !baseData2.ID.Equals(shearRateCorrectedID) || !baseData2.Name.Equals(name))
                                        throw (new SQLiteException("SQLite database corrupted: Rheogram has been jsonified with the wrong ID or Name."));
                                    calculationData.RheogramShearRateCorrected = baseData2;
                                }
                            }
                        }
                        catch (SQLiteException e)
                        {
                            Console.WriteLine("Impossible to get RheogramShearRateCorrectedID from RheogramOutputsTable");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error while getting the RheogramInput of the YPLCorrection for the given guid from the RheogramInputsTable. Should not be null.");
                    }
                }
                return calculationData;
            }
            else
            {
                return null;
            }
        }

        public bool Add(YPLCorrection calculationData)
        {
            bool result = false;
            // every YPLCorrection added to the database should have both RheogramInput and RheogramShearRateCorrected different from null and with non empty ID
            if (calculationData != null && calculationData.RheogramInput != null && calculationData.RheogramShearRateCorrected != null &&
                !calculationData.RheogramInput.ID.Equals(Guid.Empty) && !calculationData.RheogramShearRateCorrected.ID.Equals(Guid.Empty))
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    lock (lock_)
                    {
                        using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                        {
                            bool success = true;
                            try
                            {
                                // first add the YPLCorrection to the YPLCorrectionsTable
                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"INSERT INTO YPLCorrectionsTable " +
                                    "(ID, Name, Description, R1, R2, RheogramInputID, RheogramShearRateCorrectedID, TimeStamp) VALUES (" +
                                    "'" + calculationData.ID.ToString() + "', " +
                                    "'" + calculationData.Name + "', " +
                                    "'" + calculationData.Description + "', " +
                                    "'" + calculationData.R1.ToString() + "', " +
                                    "'" + calculationData.R2.ToString() + "', " +
                                    "'" + calculationData.RheogramInput.ID.ToString() + "', " +
                                    "'" + calculationData.RheogramShearRateCorrected.ID.ToString() + "', " +
                                    "'" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "'" +
                                    ")";
                                int count = command.ExecuteNonQuery();
                                result = count == 1;
                                if (result)
                                {
                                    // then add YPLModelOutput to the RheogramOutputsTable
                                    Rheogram baseData2 = calculationData.RheogramShearRateCorrected;
                                    if (baseData2 == null)
                                    {
                                        success = false;
                                        Console.WriteLine("Impossible to add RheogramShearRateCorrected into RheogramOutputsTable because it is null");
                                    }
                                    else
                                    {
                                        string json = JsonConvert.SerializeObject(baseData2);
                                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"INSERT INTO RheogramOutputsTable (ID, Name, Rheogram) " +
                                            "VALUES (" +
                                            "'" + baseData2.ID.ToString() + "', " +
                                            "'" + baseData2.Name + "', " +
                                            "'" + json + "'" +
                                            ")";
                                        count = command.ExecuteNonQuery();
                                        result = count == 1;
                                        if (result)
                                        {
                                            success = true;
                                        }
                                        else
                                        {
                                            success = false;
                                            Console.WriteLine("Impossible to insert RheogramShearRateCorrected into RheogramOutputsTable");
                                        }

                                    }
                                }
                                else
                                {
                                    success = false;
                                    Console.WriteLine("Impossible to insert YPLCorrection into YPLCorrectionsTable");
                                }
                            }
                            catch (SQLiteException e)
                            {
                                success = false;
                                Console.WriteLine("Error while inserting YPLCorrection into YPLCorrectionsTable");
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
            return result;
        }

        /// <summary>
        /// create a YPLCorrection from the given Rheogram as RheogramInput, let the remaining parameters the same and perform the calculation
        /// </summary>
        public bool Add(Rheogram baseData1)
        {
            bool success = false;
            if (baseData1 != null && !baseData1.ID.Equals(Guid.Empty))
            {
                YPLCorrection calculationData = new YPLCorrection();
                calculationData.ID = Guid.NewGuid();
                calculationData.Name = "DefaultName-" + calculationData.ID.ToString().Substring(0, 8);
                calculationData.Description = "DefaultDescription-" + calculationData.ID.ToString().Substring(0, 8);
                // R1 and R2 are set to their default value
                calculationData.RheogramInput = baseData1;
                calculationData.RheogramInput.ID = baseData1.ID;
                calculationData.RheogramInput.Name = calculationData.Name + "-input";
                calculationData.RheogramShearRateCorrected= new Rheogram();
                calculationData.RheogramShearRateCorrected.ID = Guid.NewGuid();
                calculationData.RheogramShearRateCorrected.Name = calculationData.Name + "-calculated-shearRateCorrected";

                calculationData.CalculateRheogramShearRateCorrected();

                Add(calculationData);

                success = true;
            }

            return success;
        }

        public bool Remove(YPLCorrection calculationData)
        {
            bool result = false;
            if (calculationData != null)
            {
                result = Remove(calculationData.ID);
            }
            return result;
        }

        /// <summary>
        /// remove the YPLCorrection of the given guid and its Rheogram output children
        /// </summary>
        public bool Remove(Guid guid)
        {
            bool result = false;
            if (!guid.Equals(Guid.Empty))
            {
                YPLCorrection calculationData = Get(guid);

                // every YPLCorrection added to the database should have both RheogramInput and YPLModelOutput different from null
                if (calculationData != null && calculationData.RheogramInput != null && calculationData.RheogramShearRateCorrected != null)
                {
                    if (SQLConnectionManager.Instance.Connection != null)
                    {
                        lock (lock_)
                        {
                            using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                            {
                                bool success = true;
                                // first delete YPLCorrection from YPLCorrectionsTable
                                try
                                {
                                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"DELETE FROM YPLCorrectionsTable WHERE ID = '" + guid.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    result = count >= 0;
                                    if (!result)
                                    {
                                        success = false;
                                    }
                                }
                                catch (SQLiteException e)
                                {
                                    Console.WriteLine("Impossible to delete YPLCorrection with the given guid from YPLCorrectionsTable");
                                    success = false;
                                }

                                // then delete RheogramShearRateCorrected from RheogramOutputsTable
                                Guid calculatedID = calculationData.RheogramShearRateCorrected.ID;
                                if (success && !calculatedID.Equals(Guid.Empty))
                                {
                                    try
                                    {
                                        var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"DELETE FROM RheogramOutputsTable WHERE ID = '" + calculatedID.ToString() + "'";
                                        int count = command.ExecuteNonQuery();
                                        result = count >= 0;
                                        if (!result)
                                        {
                                            success = false;
                                        }
                                    }
                                    catch (SQLiteException e)
                                    {
                                        success = false;
                                        Console.WriteLine("Impossible to delete the RheogramShearRateCorrected with the given calculatedID from RheogramOutputsTable");
                                    }
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

        /// <summary>
        /// remove all YPLCorrection and their Rheogram output children older than the given date
        /// </summary>
        public bool Remove(DateTime old)
        {
            bool success = true;
            if (SQLConnectionManager.Instance.Connection != null)
            {
                lock (lock_)
                {
                    using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                    {
                        Guid guid = Guid.Empty;
                        var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                        command.CommandText = @"SELECT ID FROM YPLCorrectionsTable WHERE TimeStamp < '" + (old - DateTime.MinValue).TotalSeconds.ToString() + "'";
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read() && !reader.IsDBNull(0))
                                {
                                    guid = reader.GetGuid(0);
                                }
                            }
                        }
                        catch (SQLiteException e)
                        {
                            success = false;
                            Console.WriteLine("Impossible to retrieve old RheogramShearRateCorrectedID from YPLCorrectionsTable");
                        }

                        if (success)
                        {
                            Remove(guid);
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            return success;
        }

        public bool Update(Guid guid, YPLCorrection updatedCalculation)
        {
            bool success = true;
            if (guid != null && !guid.Equals(Guid.Empty) && updatedCalculation != null && !updatedCalculation.ID.Equals(Guid.Empty) &&
                updatedCalculation.RheogramInput != null && updatedCalculation.RheogramShearRateCorrected != null)
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    lock (lock_)
                    {
                        using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                        {
                            // first update the relevant fields in YPLCorrectionsTable (other fields never change)
                            try
                            {
                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"UPDATE YPLCorrectionsTable SET " +
                                    "Name = '" + updatedCalculation.Name + "', " +
                                    "Description = '" + updatedCalculation.Description + "', " +
                                    "R1 = '" + updatedCalculation.R1 + "', " +
                                    "R2 = '" + updatedCalculation.R2 + "', " +
                                    "RheogramInputID = '" + updatedCalculation.RheogramInput.ID.ToString() + "', " +
                                    "TimeStamp = '" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "' " +
                                    "WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                success = count == 1;
                            }
                            catch (SQLiteException e)
                            {
                                success = false;
                                Console.WriteLine("Impossible to update YPLCorrectionsTable");
                            }

                            // then update RheogramShearRateCorrected (which may have changed after calculation) in RheogramOutputsTable 
                            if (success)
                            {
                                try
                                {
                                    string json = JsonConvert.SerializeObject(updatedCalculation.RheogramShearRateCorrected);
                                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"UPDATE RheogramOutputsTable SET " +
                                        "Name = '" + updatedCalculation.RheogramShearRateCorrected.Name + "', " +
                                        "Rheogram = '" + json + "' " +
                                        "WHERE ID = '" + updatedCalculation.RheogramShearRateCorrected.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    success = count == 1;
                                }
                                catch (SQLiteException e)
                                {
                                    transaction.Rollback();
                                }
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
            return success;
        }

        /// <summary>
        /// update a YPLCorrection of the given parentId with the given Rheogram as RheogramInput, let the remaining parameters the same and perform the calculation
        /// </summary>
        public bool Update(Guid parentId, Rheogram baseData1)
        {
            bool success = false;

            if (baseData1 != null && !baseData1.ID.Equals(Guid.Empty))
            {
                YPLCorrection calculationData = Get(parentId);
                if (calculationData != null)
                {
                    calculationData.RheogramInput = baseData1;

                    calculationData.CalculateRheogramShearRateCorrected();

                    if (Update(parentId, calculationData))
                        success = true;
                }
            }

            return success;
        }
    }
}