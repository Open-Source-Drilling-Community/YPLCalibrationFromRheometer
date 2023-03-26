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
    public class RheogramsController : ControllerBase
    {
        private readonly ILogger logger_;
        private readonly RheogramManager rheogramManager_;
        private readonly YPLCalibrationManager yplCalibrationManager_;
        private readonly YPLCorrectionManager yplCorrectionManager_;

        public RheogramsController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<RheogramsController>();
            rheogramManager_ = new RheogramManager(loggerFactory);
            yplCalibrationManager_ = new YPLCalibrationManager(loggerFactory, rheogramManager_);
            yplCorrectionManager_ = new YPLCorrectionManager(loggerFactory, rheogramManager_);
        }

        // GET api/Rheograms
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = rheogramManager_.GetIDs();
            return ids;
        }

        // GET api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{id}")]
        public Rheogram Get(Guid id)
        {
            return rheogramManager_.Get(id);
        }

        // POST api/Rheograms
        [HttpPost]
        public void Post([FromBody] Rheogram value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                Rheogram baseData1 = rheogramManager_.Get(value.ID);
                if (baseData1 == null)
                {
                    rheogramManager_.Add(value);
                }
                else
                {
                    logger_.LogWarning("The given Rheogram already exists and will not be updated");
                }
            }
            else
            {
                logger_.LogWarning("The given Rheogram is null or its ID is null or empty");
            }
        }

        // PUT api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] Rheogram value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                Rheogram baseData1 = rheogramManager_.Get(id);
                if (baseData1 != null)
                {
                    rheogramManager_.Update(id, value);
                    yplCalibrationManager_.UpdateReferences(id, value);
                    yplCorrectionManager_.UpdateReferences(id, value);
                }
                else
                {
                    logger_.LogWarning("The given Rheogram cannot be retrieved from the database");
                }
            }
            else
            {
                logger_.LogWarning("The given Rheogram is null or its ID is null or empty");
            }
        }

        // DELETE api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                if (yplCalibrationManager_.RemoveReferences(id))
                {
                    if (yplCorrectionManager_.RemoveReferences(id))
                        rheogramManager_.Remove(id);
                }
            }
            else
            {
                logger_.LogWarning("The given Rheogram ID is null or empty");
            }
        }
    }
}
