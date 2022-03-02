using System;
using System.IO;
using System.Data.SQLite;

namespace YPLCalibrationFromRheometer.Service
{
    /// <summary>
    /// A manager for the sql database connection. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class SQLConnectionManager
    {
        private static SQLConnectionManager instance_ = null;

        private SQLiteConnection connection_ = null;

        private object lock_ = new object();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private SQLConnectionManager()
        {

        }

        public static SQLConnectionManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new SQLConnectionManager();
                }
                return instance_;
            }
        }

        public SQLiteConnection Connection
        {
            get
            {
                if (connection_ == null)
                {
                    Initialize();
                }
                return connection_;
            }
        }

        private void ManageYPLCalibrationFromRheometerDatabases()
        {
            #region YPLCalibrationsTable
            var command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM YPLCalibrationsTable";
            long count = -1;
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("YPLCalibrationsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE YPLCalibrationsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Description text, " +
                    "RheogramInputID text, " +
                    "YPLModelMullineuxID text," +
                    "YPLModelKelessidisID text," +
                    "TimeStamp" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to create YPLCalibrationsTable and will be dropped.");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("YPLCalibrationsTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX YPLCalibrationsTableIndex ON YPLCalibrationsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLCalibrationsTable has been successfully indexed.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to index YPLCalibrationsTable and will be dropped.");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE YPLCalibrationsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLCalibrationsTable has been successfully dropped.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to drop YPLCalibrationsTable.");
                        success = false;
                    }
                }
            }
            #endregion

            #region YPLCorrectionsTable
            command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM YPLCorrectionsTable";
            count = -1;
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("YPLCorrectionsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE YPLCorrectionsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Description text, " +
                    "R1 double precision, " +
                    "R2 double precision, " +
                    "RheogramInputID text, " +
                    "RheogramShearRateCorrectedID text," +
                    "TimeStamp" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to create YPLCorrectionsTable and will be dropped.");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("YPLCorrectionsTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX YPLCorrectionsTableIndex ON YPLCorrectionsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLCorrectionsTable has been successfully indexed.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to index YPLCorrectionsTable and will be dropped.");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE YPLCorrectionsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLCorrectionsTable has been successfully dropped.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to drop YPLCorrectionsTable.");
                        success = false;
                    }
                }
            }
            #endregion

            #region RheogramInputsTable
            command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM RheogramInputsTable";
            count = -1;
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("RheogramInputsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE RheogramInputsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Rheogram text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to create RheogramInputsTable and will be dropped.");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("RheogramInputsTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX RheogramInputsTableIndex ON RheogramInputsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("RheogramInputsTable has been successfully indexed.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to index RheogramInputsTable and will be dropped.");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE RheogramInputsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("RheogramInputsTable has been successfully dropped.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to drop RheogramInputsTable.");
                        success = false;
                    }
                }
            }
            #endregion

            #region YPLModelOutputsTable
            command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM YPLModelOutputsTable";
            count = -1;
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("YPLModelOutputsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE YPLModelOutputsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "YPLModel text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to create YPLModelOutputsTable and will be dropped.");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("YPLModelOutputsTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX YPLModelOutputsTableIndex ON YPLModelOutputsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLModelOutputsTable has been successfully indexed.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to index YPLModelOutputsTable and will be dropped.");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE YPLModelOutputsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("YPLModelOutputsTable has been successfully dropped.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to drop YPLModelOutputsTable.");
                        success = false;
                    }
                }
            }
            #endregion

            #region RheogramOutputsTable
            command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM RheogramOutputsTable";
            count = -1;
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("RheogramOutputsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE RheogramOutputsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Rheogram text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    Console.WriteLine("Impossible to create RheogramOutputsTable and will be dropped.");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("RheogramOutputsTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX RheogramOutputsTableIndex ON RheogramOutputsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("RheogramOutputsTable has been successfully indexed.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to index RheogramOutputsTable and will be dropped.");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE RheogramOutputsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        Console.WriteLine("RheogramOutputsTable has been successfully dropped.");
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Impossible to drop RheogramOutputsTable.");
                        success = false;
                    }
                }
            }
            #endregion
        }

        private void Initialize()
        {
            string homeDirectory = ".." + Path.DirectorySeparatorChar + "home";
            if (!Directory.Exists(homeDirectory))
            {
                try
                {
                    Directory.CreateDirectory(homeDirectory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible to create home directory for local storage.");
                }
            }
            if (Directory.Exists(homeDirectory))
            {
                string connectionString = @"URI=file:" + homeDirectory + Path.DirectorySeparatorChar + "YPLCalibrationFromRheometer.db";
                connection_ = new SQLiteConnection(connectionString);
                connection_.Open();
                ManageYPLCalibrationFromRheometerDatabases();
            }
        }

    }
}
