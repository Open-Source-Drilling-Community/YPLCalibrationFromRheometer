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
                // POST the YPLCalibration into YPLCalibrationsTable (no need to PUT its RheogramInput since by construction, the webapp creates YPLCalibration's from already existing Rheogram's)
                YPLCalibration yplCalibration = yplCalibrationManager_.Get(value.ID);
                if (yplCalibration == null)
                {
                    try
                    {
                        yplCalibrationManager_.Add(value);
                    }
                    catch (Exception ex)
                    {
                        logger_.LogError(ex, "Impossible to post the given YPLCalibration");
                    }
                }
                // else do nothing because the Post method is not supposed to work on existing on existing YPLCorrections
            }
        }

        // PUT api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] YPLCalibration value)
        {
            if (value != null)
            {
                try
                {
                    YPLCalibration yplCalibration = yplCalibrationManager_.Get(id);
                    if (yplCalibration != null)
                    {
                        yplCalibrationManager_.Update(id, value);
                    }
                    // else do nothing because the Put method should only be called on existing YPLCalibrations
                }
                catch (Exception ex)
                {
                    logger_.LogError(ex, "Impossible to put the given YPLCalibration");
                }
            }
        }

        // DELETE api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            yplCalibrationManager_.Remove(id);
        }
    }
}
