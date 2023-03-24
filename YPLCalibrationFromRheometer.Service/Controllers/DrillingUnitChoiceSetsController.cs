using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.UnitConversion.Conversion.DrillingEngineering;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class DrillingUnitChoiceSetsController : ControllerBase
    {
        private readonly ILogger logger_;
        private readonly DrillingUnitChoiceSetsManager drillingUnitChoiceSetsManager_;

        public DrillingUnitChoiceSetsController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<DrillingUnitChoiceSetsController>();
            drillingUnitChoiceSetsManager_ = new DrillingUnitChoiceSetsManager(loggerFactory);
        }

        // GET api/DrillingUnitChoiceSets
        [HttpGet]
        public IEnumerable<MetaInfo> Get()
        {
            List<MetaInfo> ids = drillingUnitChoiceSetsManager_.GetIDs();
            return ids;
        }

        // GET api/DrillingUnitChoiceSets/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpGet("{id}")]
        public DrillingUnitChoiceSet Get(Guid id)
        {
            return drillingUnitChoiceSetsManager_.Get(id);
        }

        // POST api/DrillingUnitChoiceSets
        [HttpPost]
        public void Post([FromBody] DrillingUnitChoiceSet value)
        {
            if (value != null && value.ID != null && value.ID != Guid.Empty)
            {
                DrillingUnitChoiceSet baseData1 = drillingUnitChoiceSetsManager_.Get(value.ID);
                if (baseData1 == null)
                {
                    drillingUnitChoiceSetsManager_.Add(value);
                }
                else
                {
                    logger_.LogWarning("The given DrillingUnitChoiceSet already exists and will not be updated");
                }
            }
            else
            {
                logger_.LogWarning("The given DrillingUnitChoiceSet is null or its ID is null or empty");
            }
        }

        // PUT api/DrillingUnitChoiceSets/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] DrillingUnitChoiceSet value)
        {
            if (value != null && value.ID != null && value.ID != Guid.Empty)
            {
                DrillingUnitChoiceSet baseData1 = drillingUnitChoiceSetsManager_.Get(id);
                if (baseData1 != null)
                {
                    drillingUnitChoiceSetsManager_.Update(id, value);
                }
                else
                {
                    logger_.LogWarning("The given DrillingUnitChoiceSet cannot be retrieved from the database");
                }
            }
            else
            {
                logger_.LogWarning("The given DrillingUnitChoiceSet is null or its ID is null or empty");
            }
        }

        // DELETE api/DrillingUnitChoiceSets/7f70fe4f-f4a4-4fdf-a95d-241b0a6f4a4e
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            if (id != null && !id.Equals(Guid.Empty))
            {
                drillingUnitChoiceSetsManager_.Remove(id);
            }
            else
            {
                logger_.LogWarning("The given DrillingUnitChoiceSet ID is null or empty");
            }
        }
    }
}
