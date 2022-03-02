using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("YPLCalibrationFromRheometer/api/[controller]")]
    [ApiController]
    public class YPLCorrectionsController : ControllerBase
    {
        // GET api/YPLCorrections
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = YPLCorrectionManager.Instance.GetIDs();
            return ids;
        }

        // GET api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpGet("{id}")]
        public YPLCorrection Get(Guid id)
        {
            return YPLCorrectionManager.Instance.Get(id);
        }

        // GET api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{parentId}/{childId}")]
        public Rheogram Get(Guid parentId, Guid childId)
        {
            YPLCorrection calculationData = YPLCorrectionManager.Instance.Get(parentId);
            if (calculationData != null && calculationData.RheogramShearRateCorrected != null && calculationData.RheogramShearRateCorrected.ID.Equals(childId))
            {
                return calculationData.RheogramShearRateCorrected;
            }
            return null;
        }

        // POST api/YPLCorrections
        [HttpPost]
        public void Post([FromBody] YPLCorrection value)
        {
            if (value != null && value.RheogramInput != null && !value.RheogramInput.ID.Equals(Guid.Empty))
            {
                bool success = true;

                // POST the YPLCorrection into YPLCorrectionsTable (no need to PUT its RheogramInput since by construction, the webapp creates YPLCorrection's from already existing Rheogram's)
                YPLCorrection calculationData = YPLCorrectionManager.Instance.Get(value.ID);
                if (success && calculationData == null)
                {
                    try
                    {
                        value.CalculateRheogramShearRateCorrected();
                    }
                    catch (Exception e)
                    {

                    }
                    YPLCorrectionManager.Instance.Add(value);
                }
            }
        }

        // POST api/YPLCorrections/input
        [HttpPost("input")]
        public void Post([FromBody] Rheogram value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                // first PUT (!) the given Rheogram into the database
                bool success = true;
                if (RheogramManager.Instance.Get(value.ID) != null)
                {
                    success = RheogramManager.Instance.Update(value.ID, value);
                }
                else
                {
                    success = RheogramManager.Instance.Add(value);
                }
                if (success)
                {
                    // then POST (!) a new YPLCorrection using the given Rheogram as RheogramInput (calculation is performed withing the Add method)
                    YPLCorrectionManager.Instance.Add(value);
                }
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
                    value.CalculateRheogramShearRateCorrected();
                }
                catch (Exception e)
                {

                }
                YPLCorrection calculationData = YPLCorrectionManager.Instance.Get(id);
                if (calculationData != null)
                {
                    YPLCorrectionManager.Instance.Update(id, value);
                }
                else
                {
                    YPLCorrectionManager.Instance.Add(value);
                }
            }
        }

        // PUT api/YPLCorrections/input/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpPut("input/{parentId}")]
        public void Put(Guid parentId, [FromBody] Rheogram value)
        {
            if (value != null && !value.ID.Equals(Guid.Empty))
            {
                // first PUT the given Rheogram into the database
                bool success = true;
                if (RheogramManager.Instance.Get(value.ID) != null)
                {
                    success = RheogramManager.Instance.Update(value.ID, value);
                }
                else
                {
                    success = RheogramManager.Instance.Add(value);
                }
                if (success)
                {
                    // then PUT its parent YPLCorrection into the database (calculation is performed withing the Update and Add methods)
                    YPLCorrection calculationData = YPLCorrectionManager.Instance.Get(parentId);
                    if (calculationData != null)
                    {
                        YPLCorrectionManager.Instance.Update(parentId, value);
                    }
                    else
                    {
                        YPLCorrectionManager.Instance.Add(value);
                    }
                }
            }
        }

        // DELETE api/YPLCorrections/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            YPLCorrectionManager.Instance.Remove(id);
        }
    }
}
