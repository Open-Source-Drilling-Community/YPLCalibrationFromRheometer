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
                }
            }
        }

    }
}
