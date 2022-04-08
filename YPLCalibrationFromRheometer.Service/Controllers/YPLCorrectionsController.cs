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
    public class YPLCorrectionsController : ControllerBase
    {
        private readonly ILogger logger_;
        private readonly YPLCorrectionManager yplCorrectionManager_;

        public YPLCorrectionsController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<YPLCorrectionsController>();
            yplCorrectionManager_ = new YPLCorrectionManager(
                loggerFactory,
                new RheogramManager(loggerFactory));
        }

        // GET api/YPLCorrections
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = yplCorrectionManager_.GetIDs();
            return ids;
        }

        // GET api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpGet("{id}")]
        public YPLCorrection Get(Guid id)
        {
            return yplCorrectionManager_.Get(id);
        }

        // POST api/YPLCorrections
        [HttpPost]
        public void Post([FromBody] YPLCorrection value)
        {
            if (value != null && value.RheogramInput != null && !value.RheogramInput.ID.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = yplCorrectionManager_.Get(value.ID);
                if (yplCorrection == null)
                {
                    yplCorrectionManager_.Add(value);
                }
                else
                {
                    logger_.LogWarning("The given YPLCorrection already exists and will not be updated");
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCorrection is null or its ID is null or empty");
            }
        }

        // PUT api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] YPLCorrection value)
        {
            if (value != null && value.ID != null && !value.ID.Equals(Guid.Empty))
            {
                YPLCorrection yplCorrection = yplCorrectionManager_.Get(id);
                if (yplCorrection != null)
                {
                    yplCorrectionManager_.Update(id, value);
                }
                else
                {
                    logger_.LogWarning("The given YPLCorrection cannot be retrieved from the database");
                }
            }
            else
            {
                logger_.LogWarning("The given YPLCorrection is null or its ID is null or empty");
            }
        }

        // DELETE api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            if (id != null && !id.Equals(Guid.Empty))
            {
                yplCorrectionManager_.Remove(id);
            }
            else
            {
                logger_.LogWarning("The given YPLCorrection ID is null or empty");
            }
        }
    }
}
