using System;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.Model
{
    public class YPLCalibration : ICloneable
    {
        /// <summary>
        /// an ID for the YPLCalibration, typed as a string to support GUID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// name of the YPLCalibration
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the YPLCalibration
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the input Rheogram
        /// </summary>
        public Rheogram RheogramInput { get; set; }

        /// <summary>
        /// the YPLModel calculated with the Kelessidis method
        /// </summary>
        public YPLModel YPLModelKelessidis { get; set; }

        /// <summary>
        /// the YPLModel calculated with the Mullineux method
        /// </summary>
        public YPLModel YPLModelMullineux { get; set; }

        /// <summary>
        /// the YPLModel calculated with the Levenberg-Marquardt algortihm
        /// </summary>
        public YPLModel YPLModelLevenbergMarquardt { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public YPLCalibration() : base()
        {

        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public YPLCalibration(YPLCalibration src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }

        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(YPLCalibration dest)
        {
            if (dest != null)
            {
                dest.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                dest.Name = Name;
                dest.Description = Description;
                if (RheogramInput != null)
                {
                    if (dest.RheogramInput == null)
                        dest.RheogramInput = new Rheogram();
                    RheogramInput.Copy(dest.RheogramInput);
                    if (dest.RheogramInput.ID.Equals(Guid.Empty))
                        dest.RheogramInput.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelKelessidis != null)
                {
                    if (dest.YPLModelKelessidis == null)
                        dest.YPLModelKelessidis = new YPLModel();
                    YPLModelKelessidis.Copy(dest.YPLModelKelessidis);
                    if (dest.YPLModelKelessidis.ID.Equals(Guid.Empty))
                        dest.YPLModelKelessidis.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelLevenbergMarquardt != null)
                {
                    if (dest.YPLModelLevenbergMarquardt == null)
                        dest.YPLModelLevenbergMarquardt = new YPLModel();
                    YPLModelLevenbergMarquardt.Copy(dest.YPLModelLevenbergMarquardt);
                    if (dest.YPLModelLevenbergMarquardt.ID.Equals(Guid.Empty))
                        dest.YPLModelLevenbergMarquardt.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// cloning function (including the ID)
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            YPLCalibration copy = new YPLCalibration(this)
            {
                ID = ID
            };
            return copy;
        }

        /// <summary>
        ///  calculates the output YPLModel with Kelessidis method
        /// </summary>
        /// <returns></returns>
        public bool CalculateYPLModelKelessidis()
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.RheometerMeasurementList;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (YPLModelKelessidis == null)
                        YPLModelKelessidis = new YPLModel(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    if (YPLModelKelessidis.ID.Equals(Guid.Empty))
                        YPLModelKelessidis.ID = Guid.NewGuid();
                    if (YPLModelKelessidis.Name == null)
                        YPLModelKelessidis.Name = RheogramInput.Name + "-calculated-Kelessidis";
                    YPLModelKelessidis.FitToKelessidis(RheogramInput);
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        ///  calculates the output YPLModel with Mullineux method
        /// </summary>
        /// <returns></returns>
        public bool CalculateYPLModelMullineux()
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.RheometerMeasurementList;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (YPLModelMullineux == null)
                        YPLModelMullineux = new YPLModel(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    if (YPLModelMullineux.ID.Equals(Guid.Empty))
                        YPLModelMullineux.ID = Guid.NewGuid();
                    if (YPLModelMullineux.Name == null)
                        YPLModelMullineux.Name = RheogramInput.Name + "-calculated-Mullineux";
                    YPLModelMullineux.FitToMullineux(RheogramInput);
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        ///  calculates the output YPLModel with Levenberg-Marquardt algorithm
        /// </summary>
        /// <returns></returns>
        public bool CalculateYPLLevenbergMarquardt()
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.RheometerMeasurementList;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (YPLModelLevenbergMarquardt == null)
                        YPLModelLevenbergMarquardt = new YPLModel(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    if (YPLModelLevenbergMarquardt.ID.Equals(Guid.Empty))
                        YPLModelLevenbergMarquardt.ID = Guid.NewGuid();
                    if (YPLModelLevenbergMarquardt.Name == null)
                        YPLModelLevenbergMarquardt.Name = RheogramInput.Name + "-calculated-Levenberg";
                    YPLModelLevenbergMarquardt.FitToLevenbergMarquardt(RheogramInput);
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }
    }
}
