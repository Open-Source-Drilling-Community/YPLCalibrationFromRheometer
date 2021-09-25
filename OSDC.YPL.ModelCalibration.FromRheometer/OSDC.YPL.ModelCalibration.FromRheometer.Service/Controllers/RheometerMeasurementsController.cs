using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("YPLCalibrationFromRheometer/api/[controller]")]
    [ApiController]
    public class RheometerMeasurementsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<int> Get()
        {
            return IdentifiedObjectManager<RheometerMeasurement>.Instance.GetIDs();
        }

        // GET api/RheometerMeasurements/5
        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public RheometerMeasurement Get(int id)
        {
            RheometerMeasurement measurement = IdentifiedObjectManager<RheometerMeasurement>.Instance.Get(id);
            return measurement;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] RheometerMeasurement value)
        {
            if (value.ID < 0 || !IdentifiedObjectManager<RheometerMeasurement>.Instance.Contains(value.ID))
            {
                IdentifiedObjectManager<RheometerMeasurement>.Instance.Add(value);
                if (value.ParentID >= 0)
                {
                    Rheogram rheogram = RheogramManager.Instance.Get(value.ParentID);
                    if (rheogram != null && rheogram.Measurements != null)
                    {
                        // make sure that there is not a measurement with the same ID
                        RheometerMeasurement exist = null;
                        foreach (RheometerMeasurement measurement in rheogram.Measurements)
                        {
                            if (measurement.ID == value.ID)
                            {
                                exist = measurement;
                                break;
                            }
                        }
                        if (exist != null)
                        {
                            // update with new values:
                            exist.ShearRate = value.ShearRate;
                            exist.ShearStress = value.ShearStress;
                        }
                        else
                        {
                            bool added = false;
                            for (int i = 0; i < rheogram.Measurements.Count; i++)
                            {
                                if (value.ShearRate < rheogram.Measurements[i].ShearRate)
                                {
                                    rheogram.Measurements.Insert(i, value);
                                    added = true;
                                    break;
                                }
                            }
                            if (!added)
                            {
                                rheogram.Measurements.Add(value);
                            }
                            CleanNullValues(rheogram);
                        }
                    }
                }
            }
        }

        private void CleanNullValues(Rheogram rheogram)
        {
            if (rheogram != null && rheogram.Measurements != null)
            {
                bool found = false;
                do
                {
                    for (int i = 0; i < rheogram.Measurements.Count; i++)
                    {
                        if (rheogram.Measurements[i] == null)
                        {
                            rheogram.Measurements.RemoveAt(i);
                            found = true;
                            break;
                        }
                    }
                } while (found);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] RheometerMeasurement value)
        {
            if (!value.IsUndefined())
            {
                RheometerMeasurement measurement = IdentifiedObjectManager<RheometerMeasurement>.Instance.Get(value.ID);
                if (!measurement.IsUndefined())
                {
                    IdentifiedObjectManager<RheometerMeasurement>.Instance.Update(id, value);
                }
                else
                {
                    IdentifiedObjectManager<RheometerMeasurement>.Instance.Add(value);
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            RheometerMeasurement measurement = Get(id);
            if (measurement != null && measurement.ParentID >= 0)
            {
                Rheogram rheogram = RheogramManager.Instance.Get(measurement.ParentID);
                if (rheogram != null && rheogram.Measurements != null)
                {
                    for (int i = 0; i < rheogram.Measurements.Count; i++)
                    {
                        if (rheogram.Measurements[i].ID == id)
                        {
                            rheogram.Measurements.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            IdentifiedObjectManager<RheometerMeasurement>.Instance.Remove(id);
        }
    }
}
