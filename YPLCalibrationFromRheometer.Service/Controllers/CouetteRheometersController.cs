using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class CouetteRheometersController : ControllerBase
    {
        private readonly ILogger logger_;
        private readonly CouetteRheometerManager rheometerManager_;

        public CouetteRheometersController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<CouetteRheometersController>();
            rheometerManager_ = new CouetteRheometerManager(loggerFactory, new RheogramManager(loggerFactory));
        }

        // GET api/CouetteRheometers
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = rheometerManager_.GetIDs();
            return ids;
        }

        // GET api/CouetteRheometers/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{id}")]
        public CouetteRheometer Get(Guid id)
        {
            return rheometerManager_.Get(id);
        }

        // POST api/CouetteRheometers
        [HttpPost]
        public void Post([FromBody] CouetteRheometer value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                CouetteRheometer baseData1 = rheometerManager_.Get(value.ID);
                if (baseData1 == null)
                {
                    rheometerManager_.Add(value);
                }
                else
                {
                    logger_.LogWarning("The given Couette Rheometer already exists and will not be updated");
                }
            }
            else
            {
                logger_.LogWarning("The given Couette Rheometer is null or its ID is null or empty");
            }
        }

        // PUT api/CouetteRheometers/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] CouetteRheometer value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                CouetteRheometer baseData1 = rheometerManager_.Get(id);
                if (baseData1 != null)
                {
                    rheometerManager_.Update(id, value);
                }
                else
                {
                    logger_.LogWarning("The given Couette Rheometer cannot be retrieved from the database");
                }
            }
            else
            {
                logger_.LogWarning("The given Couette Rheometer is null or its ID is null or empty");
            }
        }

        // DELETE api/CouetteRheometers/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                rheometerManager_.Remove(id);
            }
            else
            {
                logger_.LogWarning("The given Couette Rheometer ID is null or empty");
            }
        }
    }
}
