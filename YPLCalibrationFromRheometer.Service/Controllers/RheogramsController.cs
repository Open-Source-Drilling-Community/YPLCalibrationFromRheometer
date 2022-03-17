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
        private readonly RheogramManager rheogramManager_;
        private readonly YPLCalibrationManager yplCalibrationManager_;
        private readonly YPLCorrectionManager yplCorrectionManager_;

        public RheogramsController(ILoggerFactory loggerFactory)
        {
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
            if (value != null)
            {
                Rheogram baseData1 = rheogramManager_.Get(value.ID);
                if (baseData1 == null)
                {
                    rheogramManager_.Add(value);
                }
            }
        }

        // PUT api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] Rheogram value)
        {
            if (value != null)
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
                    rheogramManager_.Add(value);
                }
            }
        }

        // DELETE api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            rheogramManager_.Remove(id);
            yplCalibrationManager_.RemoveReferences(id);
            yplCorrectionManager_.RemoveReferences(id);
        }
    }
}
