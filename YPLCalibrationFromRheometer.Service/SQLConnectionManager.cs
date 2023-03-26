using System;
using System.IO;
using System.Data.SQLite;
using Microsoft.Extensions.Logging;

namespace YPLCalibrationFromRheometer.Service
{
    public class SQLConnectionManager
    {
        private static ILogger logger_;
        private static SQLiteConnection connection_;
        private static object lock_ = new object();

        public static SQLiteConnection GetConnection(ILoggerFactory loggerFactory)
        {
            lock (lock_)
            {
                if (connection_ == null)
                    Initialize(loggerFactory);
            }
            return connection_;
        }

        private static void ManageYPLCalibrationFromRheometerDatabase()
        {
            #region CouetteRheometerTable
            var command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM CouetteRheometersTable";
            long count = -1;
            try
            {
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    count = reader.GetInt64(0);
                }
            }
            catch (SQLiteException ex)
            {
                logger_.LogWarning(ex, "CouetteRheometersTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE CouetteRheometersTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Description text, " +
                    "Data text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to create CouetteRheometersTable and will be dropped");
                    success = false;
                }
                if (success)
                {
                    Console.WriteLine("CouetteRheometersTable has been successfully created and will be indexed.");
                    command.CommandText =
                        @"CREATE UNIQUE INDEX CouetteRheometersTableIndex ON CouetteRheometersTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        logger_.LogInformation("CouetteRheometersTable has been successfully created");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to index CouetteRheometersTable and will be dropped");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE CouetteRheometersTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        logger_.LogWarning("CouetteRheometersTable has been successfully dropped");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to drop CouetteRheometersTable");
                    }
                }
            }
            #endregion

            #region YPLCalibrationsTable
            command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM YPLCalibrationsTable";
            count = -1;
            try
            {
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    count = reader.GetInt64(0);
                }
            }
            catch (SQLiteException ex)
            {
                logger_.LogWarning(ex, "YPLCalibrationsTable does not exist and will be created");
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
                    "Data text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to create YPLCalibrationsTable and will be dropped");
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
                        logger_.LogInformation("YPLCalibrationsTable has been successfully created");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to index YPLCalibrationsTable and will be dropped");
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
                        logger_.LogWarning("YPLCalibrationsTable has been successfully dropped");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to drop YPLCalibrationsTable");
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
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    count = reader.GetInt64(0);
                }
            }
            catch (SQLiteException ex)
            {
                logger_.LogWarning(ex, "YPLCorrectionsTable does not exist and will be created");
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
                    "RheogramInputID text, " +
                    "Data text" +
                    ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to create YPLCorrectionsTable and will be dropped");
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
                        logger_.LogInformation("YPLCorrectionsTable has been successfully created");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to index YPLCorrectionsTable and will be dropped");
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
                        logger_.LogWarning("YPLCorrectionsTable has been successfully dropped");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to drop YPLCorrectionsTable");
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
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    count = reader.GetInt64(0);
                }
            }
            catch (SQLiteException ex)
            {
                logger_.LogWarning(ex, "RheogramInputsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE RheogramInputsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Description text, " +
                    "CouetteRheometerID text, " +
                    "Data text" +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to create RheogramInputsTable and will be dropped");
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
                        logger_.LogInformation("RheogramInputsTable has been successfully created");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to index RheogramInputsTable and will be dropped");
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
                        logger_.LogWarning("RheogramInputsTable has been successfully dropped");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to drop RheogramInputsTable");
                    }
                }
            }
            #endregion

            ManageDrillingUnitChoiceSets();
        }

        private static void ManageDrillingUnitChoiceSets()
        {
            #region DrillingUnitChoiceSetsTable
            var command = connection_.CreateCommand();
            command.CommandText = @"SELECT count(*) FROM DrillingUnitChoiceSetsTable";
            long count = -1;
            try
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    count = reader.GetInt64(0);
                }
            }
            catch (SQLiteException ex)
            {
                logger_.LogWarning(ex, "DrillingUnitChoiceSetsTable does not exist and will be created");
            }
            if (count < 0)
            {
                bool success = true;
                // table does no exist
                command.CommandText =
                    @"CREATE TABLE DrillingUnitChoiceSetsTable (" +
                    "ID text primary key, " +
                    "Name text, " +
                    "Description text, " +
                    "IsDefault integer, " +
                    "Data text " +
                   ")";
                try
                {
                    int res = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    logger_.LogError(ex, "Impossible to create DrillingUnitChoiceSetsTable and will be dropped");
                    success = false;
                }
                if (success)
                {
                    command.CommandText =
                        @"CREATE UNIQUE INDEX DrillingUnitChoiceSetsTableIndex ON DrillingUnitChoiceSetsTable (ID)";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        logger_.LogInformation("DrillingUnitChoiceSetsTable has been successfully created");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to index DrillingUnitChoiceSetsTable and will be dropped");
                        success = false;
                    }
                }
                if (!success)
                {
                    command.CommandText =
                        @"DROP TABLE DrillingUnitChoiceSetsTable";
                    try
                    {
                        int res = command.ExecuteNonQuery();
                        logger_.LogWarning("DrillingUnitChoiceSetsTable has been successfully dropped");
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to drop DrillingUnitChoiceSetsTable");
                    }
                }
            }
            #endregion
        }
        public static void Initialize(ILoggerFactory loggerFactory)
        {
            if (logger_ == null)
                logger_ = loggerFactory.CreateLogger<SQLConnectionManager>();
            string homeDirectory = ".." + Path.DirectorySeparatorChar + "home";
            if (!Directory.Exists(homeDirectory))
            {
                try
                {
                    Directory.CreateDirectory(homeDirectory);
                }
                catch (Exception ex)
                {
                    logger_.LogError(ex, "Impossible to create home directory for local storage");
                }
            }
            if (Directory.Exists(homeDirectory) && connection_ == null)
            {
                string connectionString = @"URI=file:" + homeDirectory + Path.DirectorySeparatorChar + "YPLCalibrationFromRheometer.db";
                connection_ = new SQLiteConnection(connectionString);
                connection_.Open();
                ManageYPLCalibrationFromRheometerDatabase();
            }
        }

    }
}
