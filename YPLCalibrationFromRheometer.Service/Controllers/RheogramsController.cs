using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using YPLCalibrationFromRheometer.Model;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("YPLCalibrationFromRheometer/api/[controller]")]
    [ApiController]
    public class RheogramsController : ControllerBase
    {
        // GET api/Rheograms
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            var ids = RheogramManager.Instance.GetIDs();
            return ids;
        }
        // GET api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{id}")]
        public Rheogram Get(Guid id)
        {
            return RheogramManager.Instance.Get(id);
        }
        // POST api/Rheograms
        [HttpPost]
        public void Post([FromBody] Rheogram value)
        {
            if (value != null)
            {
                Rheogram baseData1 = RheogramManager.Instance.Get(value.ID);
                if (baseData1 == null)
                {
                    RheogramManager.Instance.Add(value);
                }
            }
        }
        // PUT api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] Rheogram value)
        {
            if (value != null)
            {
                Rheogram baseData1 = RheogramManager.Instance.Get(id);
                if (baseData1 != null)
                {
                    RheogramManager.Instance.Update(id, value);
                }
                else
                {
                    RheogramManager.Instance.Add(value);
                }
            }
        }
        // DELETE api/Rheograms/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            RheogramManager.Instance.Remove(id);
        }
    }
}
