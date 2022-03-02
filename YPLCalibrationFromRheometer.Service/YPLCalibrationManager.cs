using System;
using System.Collections.Generic;
using System.Threading;
using System.Data.SQLite;
using Newtonsoft.Json;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service
{
    /// <summary>
    /// A manager for YPLCalibration. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class YPLCalibrationManager
    {
        private static YPLCalibrationManager instance_ = null;

        private Random random_ = new Random();

        private object lock_ = new object();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private YPLCalibrationManager()
        {
            // first initiate a call to the database to make sure all its tables are initialized and in the same time, make sure default Rheogram's are created
            RheogramManager.Instance.GetIDs();

            // then launch a garbage collector which automatically removes old YPLCalibration's from the database
            Thread thread = new Thread(new ThreadStart(GC));
            thread.Start();
        }

        public static YPLCalibrationManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new YPLCalibrationManager();
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
                    command.CommandText = @"SELECT COUNT(*) FROM YPLCalibrationsTable";
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
                            // first empty YPLModelOutputsTable
                            var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                            command.CommandText = @"DELETE FROM YPLModelOutputsTable";
                            command.ExecuteNonQuery();

                            // then empty YPLCalibrationsTable
                            command = SQLConnectionManager.Instance.Connection.CreateCommand();
                            command.CommandText = @"DELETE FROM YPLCalibrationsTable";
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
                command.CommandText = @"SELECT COUNT(*) FROM YPLCalibrationsTable WHERE ID = '" + guid + "'";
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
                    Console.WriteLine("Impossible to count rows from YPLCalibrationsTable");
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
                command.CommandText = @"SELECT ID FROM YPLCalibrationsTable";
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
                    Console.WriteLine("Impossible to get IDs from YPLCalibrationsTable");
                }
            }
            return ids;
        }

        public YPLCalibration Get(Guid guid)
        {
            if (guid != null && !guid.Equals(Guid.Empty))
            {
                YPLCalibration calculationData = null;
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    Guid inputID = Guid.Empty;
                    Guid yplModelKelessidisID = Guid.Empty;
                    Guid yplModelMullineuxID = Guid.Empty;

                    // first retrieve the YPLCalibration itself
                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, RheogramInputID, YPLModelMullineuxID, YPLModelKelessidisID FROM YPLCalibrationsTable " +
                        "WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                calculationData = new YPLCalibration();
                                calculationData.ID = guid;
                                calculationData.Name = reader.GetString(0);
                                calculationData.Description = reader.GetString(1);
                                inputID = reader.GetGuid(2);
                                yplModelMullineuxID = reader.GetGuid(3);
                                yplModelKelessidisID = reader.GetGuid(4);
                            }
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to get the YPLCalibration with the given guid from YPLCalibrationsTable");
                    }

                    // then retrieve its RheogramInput with the RheogramsManager
                    Rheogram baseData1 = RheogramManager.Instance.Get(inputID);
                    if (baseData1 != null)
                    {
                        calculationData.RheogramInput = baseData1;

                        // then retrieve its Mullineux YPLModelOutput directly from YPLModelOutputsTable
                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                        command.CommandText = @"SELECT Name, YPLModel " +
                            "FROM YPLModelOutputsTable WHERE ID = '" + yplModelMullineuxID.ToString() + "'";
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read() && !reader.IsDBNull(0))
                                {
                                    string name = reader.GetString(0);
                                    string json = reader.GetString(1);
                                    YPLModel baseData2 = JsonConvert.DeserializeObject<YPLModel>(json);
                                    if (baseData2 == null || !baseData2.ID.Equals(yplModelMullineuxID) || !baseData2.Name.Equals(name))
                                        throw (new SQLiteException("SQLite database corrupted: YPLModel has been jsonified with the wrong ID or Name."));
                                    calculationData.YPLModelMullineux = baseData2;
                                }
                            }
                        }
                        catch (SQLiteException e)
                        {
                            Console.WriteLine("Impossible to get YPLModelMullineuxID from YPLModelOutputsTable");
                        }

                        // then retrieve its Kelessidis YPLModelOutput directly from YPLModelOutputsTable
                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                        command.CommandText = @"SELECT Name, YPLModel " +
                            "FROM YPLModelOutputsTable WHERE ID = '" + yplModelKelessidisID.ToString() + "'";
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read() && !reader.IsDBNull(0))
                                {
                                    string name = reader.GetString(0);
                                    string json = reader.GetString(1);
                                    YPLModel baseData2 = JsonConvert.DeserializeObject<YPLModel>(json);
                                    if (baseData2 == null || !baseData2.ID.Equals(yplModelKelessidisID) || !baseData2.Name.Equals(name))
                                        throw (new SQLiteException("SQLite database corrupted: YPLModel has been jsonified with the wrong ID or Name."));
                                    calculationData.YPLModelKelessidis = baseData2;
                                }
                            }
                        }
                        catch (SQLiteException e)
                        {
                            Console.WriteLine("Impossible to get YPLModelKelessidisID from YPLModelOutputsTable");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error while getting the RheogramInput of the YPLCalibration for the given guid from the RheogramInputsTable. Should not be null.");
                    }
                }
                return calculationData;
            }
            else
            {
                return null;
            }
        }

        public bool Add(YPLCalibration calculationData)
        {
            bool result = false;
            // every YPLCalibration added to the database should have both RheogramInput and YPLModelOutput different from null and with non empty ID
            if (calculationData != null && calculationData.RheogramInput != null && calculationData.YPLModelKelessidis != null && calculationData.YPLModelMullineux != null &&
                !calculationData.RheogramInput.ID.Equals(Guid.Empty) && !calculationData.YPLModelKelessidis.ID.Equals(Guid.Empty) && !calculationData.YPLModelMullineux.ID.Equals(Guid.Empty))
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
                                // first add the YPLCalibration to the YPLCalibrationsTable
                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"INSERT INTO YPLCalibrationsTable " +
                                    "(ID, Name, Description, RheogramInputID, YPLModelMullineuxID, YPLModelKelessidisID, TimeStamp) VALUES (" +
                                    "'" + calculationData.ID.ToString() + "', " +
                                    "'" + calculationData.Name + "', " +
                                    "'" + calculationData.Description + "', " +
                                    "'" + calculationData.RheogramInput.ID.ToString() + "', " +
                                    "'" + calculationData.YPLModelMullineux.ID.ToString() + "', " +
                                    "'" + calculationData.YPLModelKelessidis.ID.ToString() + "', " +
                                    "'" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "'" +
                                    ")";
                                int count = command.ExecuteNonQuery();
                                result = count == 1;
                                if (result)
                                {
                                    // then add Mullineux YPLModelOutput to the YPLModelOutputsTable
                                    YPLModel baseData2 = calculationData.YPLModelMullineux;
                                    if (baseData2 == null)
                                    {
                                        success = false;
                                        Console.WriteLine("Impossible to add Mullineux YPLModelOutput into YPLModelOutputsTable because it is null");
                                    }
                                    else
                                    {
                                        string json = JsonConvert.SerializeObject(baseData2);
                                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"INSERT INTO YPLModelOutputsTable (ID, Name, YPLModel) " +
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
                                            Console.WriteLine("Impossible to insert Mullineux YPLModelOutput into YPLModelOutputsTable");
                                        }

                                    }

                                    // then add Kelessidis YPLModelOutput to the YPLModelOutputsTable
                                    baseData2 = calculationData.YPLModelKelessidis;
                                    if (baseData2 == null)
                                    {
                                        success = false;
                                        Console.WriteLine("Impossible to add Kelessidis YPLModelOutput into YPLModelOutputsTable because it is null");
                                    }
                                    else
                                    {
                                        string json = JsonConvert.SerializeObject(baseData2);
                                        command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"INSERT INTO YPLModelOutputsTable (ID, Name, YPLModel) " +
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
                                            Console.WriteLine("Impossible to insert Kelessidis YPLModelOutput into YPLModelOutputsTable");
                                        }

                                    }
                                }
                                else
                                {
                                    success = false;
                                    Console.WriteLine("Impossible to insert YPLCalibration into YPLCalibrationsTable");
                                }
                            }
                            catch (SQLiteException e)
                            {
                                success = false;
                                Console.WriteLine("Error while inserting YPLCalibration into YPLCalibrationsTable");
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
        /// create a YPLCalibration from the given Rheogram as RheogramInput, let the remaining parameters the same and perform the calculation
        /// </summary>
        public bool Add(Rheogram baseData1)
        {
            bool success = false;
            if (baseData1 != null && !baseData1.ID.Equals(Guid.Empty))
            {
                YPLCalibration calculationData = new YPLCalibration();
                calculationData.ID = Guid.NewGuid();
                calculationData.Name = "DefaultName-" + calculationData.ID.ToString().Substring(0, 8);
                calculationData.Description = "DefaultDescription-" + calculationData.ID.ToString().Substring(0, 8);
                calculationData.RheogramInput = baseData1;
                calculationData.RheogramInput.ID = baseData1.ID;
                calculationData.RheogramInput.Name = calculationData.Name + "-input";
                calculationData.YPLModelKelessidis = new YPLModel();
                calculationData.YPLModelKelessidis.ID = Guid.NewGuid();
                calculationData.YPLModelKelessidis.Name = calculationData.Name + "-calculated-Kelessidis";
                calculationData.YPLModelMullineux = new YPLModel();
                calculationData.YPLModelMullineux.ID = Guid.NewGuid();
                calculationData.YPLModelMullineux.Name = calculationData.Name + "-calculated-Mullineux";

                calculationData.CalculateYPLModelMullineux();
                calculationData.CalculateYPLModelKelessidis();

                Add(calculationData);

                success = true;
            }

            return success;
        }

        public bool Remove(YPLCalibration calculationData)
        {
            bool result = false;
            if (calculationData != null)
            {
                result = Remove(calculationData.ID);
            }
            return result;
        }

        /// <summary>
        /// remove the YPLCalibration of the given guid and its Rheogram output children
        /// </summary>
        public bool Remove(Guid guid)
        {
            bool result = false;
            if (!guid.Equals(Guid.Empty))
            {
                YPLCalibration calculationData = Get(guid);

                // every YPLCalibration added to the database should have both RheogramInput and YPLModelOutput different from null
                if (calculationData != null && calculationData.RheogramInput != null && calculationData.YPLModelKelessidis != null && calculationData.YPLModelMullineux != null)
                {
                    if (SQLConnectionManager.Instance.Connection != null)
                    {
                        lock (lock_)
                        {
                            using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                            {
                                bool success = true;
                                // first delete YPLCalibration from YPLCalibrationsTable
                                try
                                {
                                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"DELETE FROM YPLCalibrationsTable WHERE ID = '" + guid.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    result = count >= 0;
                                    if (!result)
                                    {
                                        success = false;
                                    }
                                }
                                catch (SQLiteException e)
                                {
                                    Console.WriteLine("Impossible to delete YPLCalibration with the given guid from YPLCalibrationsTable");
                                    success = false;
                                }

                                // then delete Mullineux YPLModelOutput from YPLModelOutputsTable
                                Guid calculatedID = calculationData.YPLModelMullineux.ID;
                                if (success && !calculatedID.Equals(Guid.Empty))
                                {
                                    try
                                    {
                                        var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"DELETE FROM YPLModelOutputsTable WHERE ID = '" + calculatedID.ToString() + "'";
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
                                        Console.WriteLine("Impossible to delete the Mullineux YPLModel with the given calculatedID from YPLModelOutputsTable");
                                    }
                                }

                                // then delete Kelessidis YPLModelOutput from YPLModelOutputsTable
                                calculatedID = calculationData.YPLModelKelessidis.ID;
                                if (success && !calculatedID.Equals(Guid.Empty))
                                {
                                    try
                                    {
                                        var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                        command.CommandText = @"DELETE FROM YPLModelOutputsTable WHERE ID = '" + calculatedID.ToString() + "'";
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
                                        Console.WriteLine("Impossible to delete the Kelessidis YPLModel with the given calculatedID from YPLModelOutputsTable");
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
        /// remove all YPLCalibration and their Rheogram output children older than the given date
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
                        command.CommandText = @"SELECT ID FROM YPLCalibrationsTable WHERE TimeStamp < '" + (old - DateTime.MinValue).TotalSeconds.ToString() + "'";
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
                            Console.WriteLine("Impossible to retrieve old records from YPLCalibrationsTable");
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

        public bool Update(Guid guid, YPLCalibration updatedCalculation)
        {
            bool success = true;
            if (guid != null && !guid.Equals(Guid.Empty) && updatedCalculation != null && !updatedCalculation.ID.Equals(Guid.Empty) &&
                updatedCalculation.RheogramInput != null && updatedCalculation.YPLModelKelessidis != null && updatedCalculation.YPLModelMullineux != null)
            {
                if (SQLConnectionManager.Instance.Connection != null)
                {
                    lock (lock_)
                    {
                        using (var transaction = SQLConnectionManager.Instance.Connection.BeginTransaction())
                        {
                            // first update the relevant fields in YPLCalibrationsTable (other fields never change)
                            try
                            {
                                var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                command.CommandText = @"UPDATE YPLCalibrationsTable SET " +
                                    "Name = '" + updatedCalculation.Name + "', " +
                                    "Description = '" + updatedCalculation.Description + "', " +
                                    "RheogramInputID = '" + updatedCalculation.RheogramInput.ID.ToString() + "', " +
                                    "TimeStamp = '" + (DateTime.UtcNow - DateTime.MinValue).TotalSeconds.ToString() + "' " +
                                    "WHERE ID = '" + guid.ToString() + "'";
                                int count = command.ExecuteNonQuery();
                                success = count == 1;
                            }
                            catch (SQLiteException e)
                            {
                                success = false;
                                Console.WriteLine("Impossible to update YPLCalibrationsTable");
                            }

                            // then update Mullineux YPLModelOutput (which may have changed after calculation) in YPLModelOutputsTable 
                            if (success)
                            {
                                try
                                {
                                    string json = JsonConvert.SerializeObject(updatedCalculation.YPLModelMullineux);
                                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"UPDATE YPLModelOutputsTable SET " +
                                        "Name = '" + updatedCalculation.YPLModelMullineux.Name + "', " +
                                        "YPLModel = '" + json + "' " +
                                        "WHERE ID = '" + updatedCalculation.YPLModelMullineux.ID.ToString() + "'";
                                    int count = command.ExecuteNonQuery();
                                    success = count == 1;
                                }
                                catch (SQLiteException e)
                                {
                                    transaction.Rollback();
                                }
                            }

                            // then update Kelessidis YPLModelOutput (which may have changed after calculation) in YPLModelOutputsTable 
                            if (success)
                            {
                                try
                                {
                                    string json = JsonConvert.SerializeObject(updatedCalculation.YPLModelKelessidis);
                                    var command = SQLConnectionManager.Instance.Connection.CreateCommand();
                                    command.CommandText = @"UPDATE YPLModelOutputsTable SET " +
                                        "Name = '" + updatedCalculation.YPLModelKelessidis.Name + "', " +
                                        "YPLModel = '" + json + "' " +
                                        "WHERE ID = '" + updatedCalculation.YPLModelKelessidis.ID.ToString() + "'";
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
        /// update a YPLCalibration of the given parentId with the given Rheogram as RheogramInput, let the remaining parameters the same and perform the calculation
        /// </summary>
        public bool Update(Guid parentId, Rheogram baseData1)
        {
            bool success = false;

            if (baseData1 != null && !baseData1.ID.Equals(Guid.Empty))
            {
                YPLCalibration calculationData = Get(parentId);
                if (calculationData != null)
                {
                    calculationData.RheogramInput = baseData1;

                    calculationData.CalculateYPLModelMullineux();

                    calculationData.CalculateYPLModelKelessidis();

                    if (Update(parentId, calculationData))
                        success = true;
                }
            }

            return success;
        }
    }
}