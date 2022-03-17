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
                // POST the YPLCorrection into YPLCorrectionsTable
                YPLCorrection yplCorrection = yplCorrectionManager_.Get(value.ID);
                if (yplCorrection == null)
                {
                    try
                    {
                        yplCorrectionManager_.Add(value);
                    }
                    catch (Exception ex)
                    {
                        logger_.LogError(ex, "Impossible to post the given YPLCorrection");
                    }
                    
                }
                // else do nothing because the Post method is not supposed to work on existing on existing YPLCorrections
            }
        }

        // PUT api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] YPLCorrection value)
        {
            if (value != null)
            {
                try
                {
                    YPLCorrection yplCorrection = yplCorrectionManager_.Get(id);
                    if (yplCorrection != null)
                    {
                        yplCorrectionManager_.Update(id, value);
                    }
                    // else do nothing because the Put method should only be called on existing YPLCorrections
                }
                catch (Exception ex)
                {
                    logger_.LogError(ex, "Impossible to put the given YPLCorrection");
                }
            }
        }

        // DELETE api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            yplCorrectionManager_.Remove(id);
        }
    }
}
