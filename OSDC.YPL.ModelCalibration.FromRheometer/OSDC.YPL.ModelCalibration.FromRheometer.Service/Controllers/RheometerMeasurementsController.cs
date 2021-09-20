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
            RheometerMeasurement measurement = IdentifiedObjectManager<RheometerMeasurement>.Instance.Get(value.ID);
            if (!measurement.IsUndefined())
            {
                IdentifiedObjectManager<RheometerMeasurement>.Instance.Update(value.ID, value);
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
            IdentifiedObjectManager<RheometerMeasurement>.Instance.Remove(id);
        }
    }
}
