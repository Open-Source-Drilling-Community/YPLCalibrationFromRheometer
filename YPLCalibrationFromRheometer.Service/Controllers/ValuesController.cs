using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("YPLCalibrationFromRheometer/api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/Values
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = YPLCalibrationManager.Instance.GetIDs();
            return ids;
        }

        // GET api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpGet("{id}")]
        public YPLCalibration Get(Guid id)
        {
            return YPLCalibrationManager.Instance.Get(id);
        }

        // GET api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{parentId}/{childId}")]
        public YPLModel Get(Guid parentId, Guid childId)
        {
            YPLCalibration calculationData = YPLCalibrationManager.Instance.Get(parentId);
            if (calculationData != null && calculationData.YPLModelMullineux != null && calculationData.YPLModelMullineux.ID.Equals(childId))
            {
                return calculationData.YPLModelMullineux;
            }
            return null;
        }

        // POST api/Values
        [HttpPost]
        public void Post([FromBody] YPLCalibration value)
        {
            if (value != null && value.RheogramInput != null && !value.RheogramInput.ID.Equals(Guid.Empty))
            {
                bool success = true;

                // POST the YPLCalibration into YPLCalibrationsTable (no need to PUT its RheogramInput since by construction, the webapp creates YPLCalibration's from already existing Rheogram's)
                YPLCalibration calculationData = YPLCalibrationManager.Instance.Get(value.ID);
                if (success && calculationData == null)
                {
                    try
                    {
                        value.CalculateYPLModelMullineux();
                        value.CalculateYPLModelKelessidis();
                    }
                    catch (Exception e)
                    {

                    }
                    YPLCalibrationManager.Instance.Add(value);
                }
            }
        }

        // POST api/Values/input
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
                    // then POST (!) a new YPLCalibration using the given Rheogram as RheogramInput (calculation is performed withing the Add method)
                    YPLCalibrationManager.Instance.Add(value);
                }
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
                    value.CalculateYPLModelKelessidis();
                }
                catch (Exception e)
                {

                }
                YPLCalibration calculationData = YPLCalibrationManager.Instance.Get(id);
                if (calculationData != null)
                {
                    YPLCalibrationManager.Instance.Update(id, value);
                }
                else
                {
                    YPLCalibrationManager.Instance.Add(value);
                }
            }
        }

        // PUT api/Values/input/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
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
                    // then PUT its parent YPLCalibration into the database (calculation is performed withing the Update and Add methods)
                    YPLCalibration calculationData = YPLCalibrationManager.Instance.Get(parentId);
                    if (calculationData != null)
                    {
                        YPLCalibrationManager.Instance.Update(parentId, value);
                    }
                    else
                    {
                        YPLCalibrationManager.Instance.Add(value);
                    }
                }
            }
        }

        // DELETE api/Values/f29b357f-8b76-4abe-ad84-4ccd5ccef77e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            YPLCalibrationManager.Instance.Remove(id);
        }
    }
}
