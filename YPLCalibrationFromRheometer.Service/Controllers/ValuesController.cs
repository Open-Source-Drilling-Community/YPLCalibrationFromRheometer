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
    public class ValuesController : ControllerBase
    {
        private readonly ILogger logger_;
        private readonly YPLCalibrationManager yplCalibrationManager_;

        public ValuesController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<ValuesController>();
            yplCalibrationManager_ = new YPLCalibrationManager(
                loggerFactory,
                new RheogramManager(loggerFactory));
        }

        // GET api/Values
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = yplCalibrationManager_.GetIDs();
            return ids;
        }

        // GET api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpGet("{id}")]
        public YPLCalibration Get(Guid id)
        {
            return yplCalibrationManager_.Get(id);
        }

        // POST api/Values
        [HttpPost]
        public void Post([FromBody] YPLCalibration value)
        {
            if (value != null && value.RheogramInput != null && !value.RheogramInput.ID.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = yplCalibrationManager_.Get(value.ID);
                if (yplCalibration == null)
                {
                    yplCalibrationManager_.Add(value);
                }
                else
                {
                    logger_.LogWarning("The given YPLCalibration already exists and will not be updated");
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCalibration is null or its ID is null or empty");
            }
        }

        // PUT api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] YPLCalibration value)
        {
            if (value != null && value.ID != null && !value.ID.Equals(Guid.Empty))
            {
                YPLCalibration yplCalibration = yplCalibrationManager_.Get(id);
                if (yplCalibration != null)
                {
                    yplCalibrationManager_.Update(id, value);
                }
                else
                {
                    logger_.LogWarning("The given YPLCalibration cannot be retrieved from the database");
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCalibration is null or its ID is null or empty");
            }
        }

        // DELETE api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            if (id != null && !id.Equals(Guid.Empty))
            {
                yplCalibrationManager_.Remove(id);
            }
            else
            {
                logger_.LogWarning("The given YPLCalibration ID is null or empty");
            }
        }
    }
}
