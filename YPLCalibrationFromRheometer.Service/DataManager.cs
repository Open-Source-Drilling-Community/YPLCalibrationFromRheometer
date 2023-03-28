using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using YPLCalibrationFromRheometer.Model;
using YPLCalibrationFromRheometer.Service.Controllers;

namespace YPLCalibrationFromRheometer.Service
{
    public class DataManager
    {
        protected readonly ILogger logger_;
        protected readonly object lock_ = new object();
        protected readonly SQLiteConnection connection_;

        public DataManager(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<DataManager>();
            connection_ = SQLConnectionManager.GetConnection(loggerFactory);
            List<Guid> rheometerIDs = GetRheometerIDs();
            // then create some default Couette Rheometer's
            if (!rheometerIDs.Any())
                FillDefaultRheometers();
            List<Guid> rheogramIDs = GetRheogramIDs();
            // then create some default Rheogram's
            if (!rheogramIDs.Any())
                FillDefaultRheograms();
        }

        protected List<Guid> GetRheometerIDs()
        {
            List<Guid> ids = new List<Guid>();
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT ID FROM CouetteRheometersTable";
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
                    logger_.LogError(ex, "Impossible to get IDs from CouetteRheometersTable");
                }
            }
            return ids;
        }


        protected bool ContainsRheometer(Guid guid)
        {
            int count = 0;
            if (connection_ != null)
            {
                var command = connection_.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM CouetteRheometersTable WHERE ID = '" + guid.ToString() + "'";
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
                    logger_.LogError(ex, "Impossible to count rows from CouetteRheometersTable");
                }
            }
            return count >= 1;
        }

        protected CouetteRheometer GetCouetteRheometer(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                CouetteRheometer rheometer = null;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, Data " +
                                           "FROM CouetteRheometersTable WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string name = reader.GetString(0);
                            string description = reader.GetString(1);
                            string json = reader.GetString(2);
                            rheometer = JsonConvert.DeserializeObject<CouetteRheometer>(json);
                            if (!rheometer.ID.Equals(guid) ||
                                (!string.IsNullOrEmpty(rheometer.Name) && !rheometer.Name.Equals(name)) ||
                                (!string.IsNullOrEmpty(rheometer.Description) && !rheometer.Description.Equals(description)))
                                throw (new SQLiteException("SQLite database corrupted: Couette rheometer has been jsonified with the wrong ID, Name or description."));
                        }
                        else
                        {
                            logger_.LogInformation("No such couette rheometer in the database");
                            return null;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        logger_.LogError(ex, "Impossible to get the couette rheometer with the given ID from CouetteRheometersTable");
                        return null;
                    }
                    // Finalizing
                    logger_.LogInformation("Returning the Couette rheometer of given ID from CouetteRheometersTable");
                    return rheometer;
                }
                else
                {
                    logger_.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                logger_.LogWarning("The given Couette rheometer ID is null or empty");
            }
            return null;
        }

        public bool Add(CouetteRheometer rheometer)
        {
            if (rheometer != null && !rheometer.ID.Equals(Guid.Empty))
            {
                if (connection_ != null)
                {
                    lock (lock_)
                    {
                        using var transaction = connection_.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string json = JsonConvert.SerializeObject(rheometer);
                            var command = connection_.CreateCommand();
                            command.CommandText = @"INSERT INTO CouetteRheometersTable (ID, Name, Description, Data) " +
                                "VALUES (" +
                                "'" + rheometer.ID.ToString() + "', " +
                                "'" + rheometer.Name + "', " +
                                "'" + rheometer.Description + "', " +
                                "'" + json + "'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                logger_.LogWarning("Impossible to insert the Couette Rheometer into the CouetteRheometersTable");
                                success = false;
                            }
                        }
                        catch (SQLiteException ex)
                        {
                            logger_.LogError(ex, "Impossible to add the Couette rheometer into CouetteRheometersTable");
                            success = false;
                        }
                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            logger_.LogInformation("Added the Couette rheometer of given ID into the CouetteRheometersTable successfully");
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
                logger_.LogWarning("The Couetter rheometer is null or its ID is null or empty");
            }
            return false;
        }

        protected List<Guid> GetRheogramIDs()
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

        public Rheogram GetRheogram(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                Rheogram rheogram = null;
                if (connection_ != null)
                {
                    var command = connection_.CreateCommand();
                    command.CommandText = @"SELECT Name, Description, CouetteRheometerID, Data " +
                        "FROM RheogramInputsTable WHERE ID = '" + guid.ToString() + "'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string name = reader.GetString(0);
                            string desc = reader.GetString(1);
                            string couetteRheometerID = reader.GetString(2);
                            string json = reader.GetString(3);
                            rheogram = JsonConvert.DeserializeObject<Rheogram>(json);
                            if (rheogram == null ||
                                !rheogram.ID.Equals(guid) ||
                                !rheogram.Name.Equals(name) ||
                                string.IsNullOrEmpty(couetteRheometerID))
                                throw (new SQLiteException("SQLite database corrupted: RheometerMeasurement has been jsonified with the wrong ID or Name."));
                            Guid rheometerID = new Guid(couetteRheometerID);
                            CouetteRheometer rheometer = GetCouetteRheometer(rheometerID);
                            rheogram.SetRheometer(rheometer);
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
            if (rheogram != null &&
                !rheogram.ID.Equals(Guid.Empty) &&
                !rheogram.CouetteRheometerID.Equals(Guid.Empty))
            {
                CouetteRheometer rheometer = GetCouetteRheometer(rheogram.CouetteRheometerID);
                if (rheometer != null)
                {
                    rheogram.SetRheometer(rheometer);
                    rheogram.Calculate();
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
                                command.CommandText = @"INSERT INTO RheogramInputsTable (ID, Name, Description, CouetteRheometerID, Data) " +
                                    "VALUES (" +
                                    "'" + rheogram.ID.ToString() + "', " +
                                    "'" + rheogram.Name + "', " +
                                    "'" + rheogram.Description + "', " +
                                    "'" + rheogram.CouetteRheometerID.ToString() + "', " +
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
                    logger_.LogWarning("Couldn't upload the rheometer");
                }
            }
            else
            {
                logger_.LogWarning("The Rheogram is null or its ID is null or empty");
            }
            return false;
        }


        /// <summary>
        /// populates database with a few default couette rheometers
        /// </summary>
        private void FillDefaultRheometers()
        {
            List<Guid> ids = GetRheometerIDs();
            if (!ids.Any())
            {
                CouetteRheometer fann35_R1B1_6speeds = new CouetteRheometer();
                fann35_R1B1_6speeds.ID = new Guid("1cea479d-fec4-4887-83e4-86bd8d2537d6");
                fann35_R1B1_6speeds.Name = "Model 35 R1B1 with 6 speeds";
                fann35_R1B1_6speeds.Description = "The standard (R1B1) oil-field rheometer with 6 speeds: 3, 6, 100, 200, 300, 600 RPM";
                fann35_R1B1_6speeds.Gap = 0.00117;
                fann35_R1B1_6speeds.BobLength = 0.0381;
                fann35_R1B1_6speeds.BobRadius = 0.017245;
                fann35_R1B1_6speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B1_6speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B1_6speeds.MeasurementPrecision = 0.25;
                fann35_R1B1_6speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B1_6speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B1_6speeds.UseISOConvention = false;
                Add(fann35_R1B1_6speeds);
                CouetteRheometer fann35_R1B2_6speeds = new CouetteRheometer();
                fann35_R1B2_6speeds.ID = new Guid("4ecaaa1e-ce3c-4c29-b455-2a54a4e11336");
                fann35_R1B2_6speeds.Name = "Model 35 R1B2 with 6 speeds";
                fann35_R1B2_6speeds.Description = "An oil-field rheometer in the configuration R1B2 with 6 speeds: 3, 6, 100, 200, 300, 600 RPM";
                fann35_R1B2_6speeds.Gap = 0.00614;
                fann35_R1B2_6speeds.BobLength = 0.0381;
                fann35_R1B2_6speeds.BobRadius = 0.012276;
                fann35_R1B2_6speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B2_6speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B2_6speeds.MeasurementPrecision = 0.25;
                fann35_R1B2_6speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B2_6speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B2_6speeds.UseISOConvention = false;
                Add(fann35_R1B2_6speeds);
                CouetteRheometer fann35_R1B3_6speeds = new CouetteRheometer();
                fann35_R1B3_6speeds.ID = new Guid("605dcd87-3737-4c0c-a465-d72771e78e46");
                fann35_R1B3_6speeds.Name = "Model 35 R1B3 with 6 speeds";
                fann35_R1B3_6speeds.Description = "An oil-field rheometer in the configuration R1B3 with 6 speeds: 3, 6, 100, 200, 300, 600 RPM";
                fann35_R1B3_6speeds.Gap = 0.00979;
                fann35_R1B3_6speeds.BobLength = 0.0381;
                fann35_R1B3_6speeds.BobRadius = 0.008622;
                fann35_R1B3_6speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B3_6speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B3_6speeds.MeasurementPrecision = 0.25;
                fann35_R1B3_6speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B3_6speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B3_6speeds.UseISOConvention = false;
                Add(fann35_R1B3_6speeds);
                CouetteRheometer fann35_R1B5_6speeds = new CouetteRheometer();
                fann35_R1B5_6speeds.ID = new Guid("5ec48b12-b2f3-48c9-8213-426c5a7e58a6");
                fann35_R1B5_6speeds.Name = "Model 35 R1B5 with 6 speeds";
                fann35_R1B5_6speeds.Description = "An oil-field rheometer in the configuration R1B5 with 6 speeds: 3, 6, 100, 200, 300, 600 RPM";
                fann35_R1B5_6speeds.Gap = 0.00243;
                fann35_R1B5_6speeds.BobLength = 0.0381;
                fann35_R1B5_6speeds.BobRadius = 0.015987;
                fann35_R1B5_6speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B5_6speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B5_6speeds.MeasurementPrecision = 0.25;
                fann35_R1B5_6speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B5_6speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B5_6speeds.UseISOConvention = false;
                Add(fann35_R1B5_6speeds);
                CouetteRheometer fann35_R1B1_8speeds = new CouetteRheometer();
                fann35_R1B1_8speeds.ID = new Guid("83a98898-0e80-4b78-a748-0a5cd28189df");
                fann35_R1B1_8speeds.Name = "Model 35 R1B1 with 8 speeds";
                fann35_R1B1_8speeds.Description = "The standard (R1B1) oil-field rheometer with 6 speeds: 3, 6, 30, 60, 100, 200, 300, 600 RPM";
                fann35_R1B1_8speeds.Gap = 0.00117;
                fann35_R1B1_8speeds.BobLength = 0.0381;
                fann35_R1B1_8speeds.BobRadius = 0.017245;
                fann35_R1B1_8speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B1_8speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 30.0 / 60.0, 60.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B1_8speeds.MeasurementPrecision = 0.25;
                fann35_R1B1_8speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B1_8speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B1_8speeds.UseISOConvention = false;
                Add(fann35_R1B1_8speeds);
                CouetteRheometer fann35_R1B2_8speeds = new CouetteRheometer();
                fann35_R1B2_8speeds.ID = new Guid("63e55c23-a9d2-4bc8-b560-d928e329bb81");
                fann35_R1B2_8speeds.Name = "Model 35 R1B2 with 8 speeds";
                fann35_R1B2_8speeds.Description = "An oil-field rheometer in the configuration R1B2 with 6 speeds: 3, 6, 30, 60, 100, 200, 300, 600 RPM";
                fann35_R1B2_8speeds.Gap = 0.00614;
                fann35_R1B2_8speeds.BobLength = 0.0381;
                fann35_R1B2_8speeds.BobRadius = 0.012276;
                fann35_R1B2_8speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B2_8speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 30.0 / 60.0, 60.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B2_8speeds.MeasurementPrecision = 0.25;
                fann35_R1B2_8speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B2_8speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B2_8speeds.UseISOConvention = false;
                Add(fann35_R1B2_8speeds);
                CouetteRheometer fann35_R1B3_8speeds = new CouetteRheometer();
                fann35_R1B3_8speeds.ID = new Guid("55d993e9-3403-4101-b527-bdd1006f6578");
                fann35_R1B3_8speeds.Name = "Model 35 R1B3 with 8 speeds";
                fann35_R1B3_8speeds.Description = "An oil-field rheometer in the configuration R1B3 with 6 speeds: 3, 6, 30, 60, 100, 200, 300, 600 RPM";
                fann35_R1B3_8speeds.Gap = 0.00979;
                fann35_R1B3_8speeds.BobLength = 0.0381;
                fann35_R1B3_8speeds.BobRadius = 0.008622;
                fann35_R1B3_8speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B3_8speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 30.0 / 60.0, 60.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B3_8speeds.MeasurementPrecision = 0.25;
                fann35_R1B3_8speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B3_8speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B3_8speeds.UseISOConvention = false;
                Add(fann35_R1B3_8speeds);
                CouetteRheometer fann35_R1B5_8speeds = new CouetteRheometer();
                fann35_R1B5_8speeds.ID = new Guid("bdb1bb1f-4d5a-427b-afd7-647dd2bd7d45");
                fann35_R1B5_8speeds.Name = "Model 35 R1B5 with 8 speeds";
                fann35_R1B5_8speeds.Description = "An oil-field rheometer in the configuration R1B5 with 6 speeds: 3, 6, 30, 60, 100, 200, 300, 600 RPM";
                fann35_R1B5_8speeds.Gap = 0.00243;
                fann35_R1B5_8speeds.BobLength = 0.0381;
                fann35_R1B5_8speeds.BobRadius = 0.015987;
                fann35_R1B5_8speeds.ConicalAngle = Math.PI / 6.0;
                fann35_R1B5_8speeds.FixedSpeedList = new List<double>() { 3.0 / 60.0, 6.0 / 60.0, 30.0 / 60.0, 60.0 / 60.0, 100.0 / 60.0, 200.0 / 60.0, 300.0 / 60.0, 600.0 / 60.0 };
                fann35_R1B5_8speeds.MeasurementPrecision = 0.25;
                fann35_R1B5_8speeds.NewtonianEndEffectCorrection = 1.064;
                fann35_R1B5_8speeds.RheometerType = CouetteRheometer.RheometerTypeEnum.RotorBob;
                fann35_R1B5_8speeds.UseISOConvention = false;
                Add(fann35_R1B5_8speeds);
                CouetteRheometer antonPaarMCR301Gap1_131mm = new CouetteRheometer();
                antonPaarMCR301Gap1_131mm.ID = new Guid("0cd3114b-bb22-402d-8df6-67c27118c1c2");
                antonPaarMCR301Gap1_131mm.Name = "Anton Paar MCR 301 gap 1.131mm";
                antonPaarMCR301Gap1_131mm.Description = "A laboratory rheometer with a gap of 1.131mm ";
                antonPaarMCR301Gap1_131mm.Gap = 0.001131;
                antonPaarMCR301Gap1_131mm.BobLength = 0.040;
                antonPaarMCR301Gap1_131mm.BobRadius = 0.013329;
                antonPaarMCR301Gap1_131mm.ConicalAngle = Math.PI / 6.0;
                antonPaarMCR301Gap1_131mm.FixedSpeedList = null;
                antonPaarMCR301Gap1_131mm.MeasurementPrecision = 0.02;
                antonPaarMCR301Gap1_131mm.NewtonianEndEffectCorrection = 1.1;
                antonPaarMCR301Gap1_131mm.RheometerType = CouetteRheometer.RheometerTypeEnum.RotatingBob;
                antonPaarMCR301Gap1_131mm.UseISOConvention = true;
                Add(antonPaarMCR301Gap1_131mm);
                CouetteRheometer antonPaarMCR302Gap1_11285mm = new CouetteRheometer();
                antonPaarMCR302Gap1_11285mm.ID = new Guid("d01b5626-c4bc-4d4f-85a8-a89f3206c554");
                antonPaarMCR302Gap1_11285mm.Name = "Anton Paar MCR 302 gap 1.1285mm";
                antonPaarMCR302Gap1_11285mm.Description = "A laboratory rheometer with a gap of 1.1285mm ";
                antonPaarMCR302Gap1_11285mm.Gap = 0.0011285;
                antonPaarMCR302Gap1_11285mm.BobLength = 0.040;
                antonPaarMCR302Gap1_11285mm.BobRadius = 0.0133265;
                antonPaarMCR302Gap1_11285mm.ConicalAngle = Math.PI / 6.0;
                antonPaarMCR302Gap1_11285mm.FixedSpeedList = null;
                antonPaarMCR302Gap1_11285mm.MeasurementPrecision = 0.02;
                antonPaarMCR302Gap1_11285mm.NewtonianEndEffectCorrection = 1.1;
                antonPaarMCR302Gap1_11285mm.RheometerType = CouetteRheometer.RheometerTypeEnum.RotatingBob;
                antonPaarMCR302Gap1_11285mm.UseISOConvention = true;
                Add(antonPaarMCR302Gap1_11285mm);
            }
        }

        /// <summary>
        /// populates database with a few default Rheograms
        /// </summary>
        private void FillDefaultRheograms()
        {
            List<Guid> ids = GetRheogramIDs();
            Guid defaultMCR301RheometerID = new Guid("0cd3114b-bb22-402d-8df6-67c27118c1c2");
            if (!ids.Any() && ContainsRheometer(defaultMCR301RheometerID))
            {
                CouetteRheometer defaultMCR301Rheometer = GetCouetteRheometer(defaultMCR301RheometerID);
                if (defaultMCR301Rheometer != null)
                {
                    //////////////////////////////////
                    // Example Rheogram #1 //
                    //////////////////////////////////
                    Rheogram rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Herschel-bulkley fluid",
                        Description = "Yield stress 2.000Pa, consistency index 0.750Pa.s^N and flow behavior index 0.500",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.750, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2, 3.061, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(4, 3.5, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(8, 4.121, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(16, 5.000, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(32, 6.243, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(64, 8, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(128, 10.485, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(256, 14, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(512, 18.971, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();

                    Add(rheogram);

                    //////////////////////////////////
                    // Example Rheogram #2 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Quemada fluid",
                        Description = "Zero viscosity infinite, infinite viscosity 0.025Pa.s, reference shear rate 300.000 1/s and flow behavior index 0.400",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.911, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2, 3.545, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(4, 4.387, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(8, 5.538, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(16, 7.157, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(32, 9.51, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(64, 13.043, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(128, 18.523, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(256, 27.304, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(512, 41.818, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    //////////////////////////////////
                    // Example Rheogram #3 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 10degC",
                        Description = "Versatec 1.37sg @ 10degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.5196, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.6578, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.8146, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.9859, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 3.1716, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 3.3837, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 3.6294, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 3.8974, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 4.2086, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 4.5639, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 4.9672, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 5.4318, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 5.9677, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 6.5878, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 7.3079, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 8.1477, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 9.1328, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 10.292, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 11.664, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 13.298, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 15.25, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #4 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 20degC",
                        Description = "Versatec 1.37sg @ 20degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.0577, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.1715, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.3005, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.4491, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.5985, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.7706, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 2.976, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 3.1883, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.4419, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.7294, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 4.0526, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 4.4228, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.8514, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 5.3455, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 5.916, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 6.5789, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 7.3552, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 8.2668, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 9.3414, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 10.617, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 12.134, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    
                    // Example Rheogram #5 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 30degC",
                        Description = "Versatec 1.37sg @ 30degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 1.9866, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.0875, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.1994, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.3232, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.4576, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.607, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 2.7778, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 2.9645, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.1781, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.4188, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.6904, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 4, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.3546, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.7617, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 5.2302, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.7717, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 6.4018, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 7.138, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 8.0034, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 9.0279, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 10.24, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #6 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 40degC",
                        Description = "Versatec 1.37sg @ 40degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.02, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.231, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.2144, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.3735, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.4619, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.6592, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 2.8225, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 2.947, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.1174, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.3399, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.5762, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 3.8344, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.156, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.5129, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 4.914, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.3743, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 5.9095, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 6.5353, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 7.2604, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 8.1156, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 9.1217, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #7 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 50degC",
                        Description = "Versatec 1.37sg @ 50degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.1151, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.2041, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.3, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.404, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.5159, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.6406, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 2.7797, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 2.9318, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.1039, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.2972, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.5141, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 3.7603, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.0382, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.3559, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 4.7191, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.1348, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 5.6128, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 6.1653, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 6.8067, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 7.5556, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 8.429, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #8 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 60degC",
                        Description = "Versatec 1.37sg @ 60degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.1926, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.2792, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.3734, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.4857, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.5831, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.7021, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 2.8448, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 2.9796, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.1456, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.3319, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.5339, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 3.7605, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.0212, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.3181, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 4.6522, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.0344, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 5.4749, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 5.9814, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 6.5664, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 7.2479, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 8.0389, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #9 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 70degC",
                        Description = "Versatec 1.37sg @ 70degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.4179, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.5159, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.5989, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.6989, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 2.8048, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 2.9275, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 3.0499, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 3.194, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.3459, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.5194, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.7138, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 3.9399, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.1783, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.4605, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 4.7778, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.1391, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 5.5528, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 6.0282, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 6.5753, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 7.2109, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 7.9457, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                    // Example Rheogram #10 //
                    //////////////////////////////////
                    rheogram = new Rheogram
                    {
                        ID = Guid.NewGuid(),
                        Name = "Versatec 1.37sg @ 80degC",
                        Description = "Versatec 1.37sg @ 80degC Anton Paar MCR301",
                        CouetteRheometerID = defaultMCR301RheometerID,
                        RateSource = Rheogram.RateSourceEnum.ISONewtonianShearRate,
                        StressSource = Rheogram.StressSourceEnum.ISONewtonianShearStress
                    };
                    rheogram.SetRheometer(defaultMCR301Rheometer);
                    if (rheogram.Measurements == null)
                    {
                        rheogram.Measurements = new List<RheometerMeasurement>();
                    }
                    /// RheometerMeasurements
                    rheogram.Add(new RheometerMeasurement(1, 2.7059, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.26, 2.7957, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.58, 2.8917, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(1.99, 2.9946, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(2.51, 3.101, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.16, 3.2146, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(3.98, 3.3432, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(5.01, 3.4774, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(6.31, 3.6299, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(7.94, 3.7977, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(10.0, 3.983, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(12.6, 4.1903, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(15.8, 4.4243, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(19.9, 4.6882, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(25.1, 4.9866, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(31.6, 5.3251, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(39.8, 5.7119, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(50.1, 6.1548, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(63.1, 6.6623, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(79.4, 7.2505, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Add(new RheometerMeasurement(99.9, 7.9279, rheogram.RateSource, rheogram.StressSource));
                    rheogram.Calculate();
                    Add(rheogram);

                }
            }
        }

    }
}
