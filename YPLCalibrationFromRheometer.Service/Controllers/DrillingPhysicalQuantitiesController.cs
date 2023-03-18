using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.UnitConversion.Conversion.DrillingEngineering;
using OSDC.UnitConversion.Conversion;
using System.Collections.Generic;
using System;

namespace YPLCalibrationFromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class DrillingPhysicalQuantitiesController : ControllerBase
    {
        private readonly ILogger logger_;

        public DrillingPhysicalQuantitiesController(ILoggerFactory loggerFactory)
        {
            logger_ = loggerFactory.CreateLogger<DrillingPhysicalQuantitiesController>();
        }

        // GET api/DrillingPhysicalQuantities
        [HttpGet]
        public IEnumerable<MetaID> Get(int option)
        {
            if (option == 0)
            {
                List<PhysicalQuantity> quantities = DrillingPhysicalQuantity.AvailableQuantities;
                List<MetaID> ids = new List<MetaID>();
                if (quantities != null)
                {
                    foreach (PhysicalQuantity quantity in quantities)
                    {
                        ids.Add(new MetaID(quantity.ID, quantity.Name));
                    }
                }
                return ids;
            }
            else
            {
                return null;
            }
        }

        // GET api/DrillingPhysicalQuantities/c0d965b2-a153-420a-9d03-7a2a272d619e
        [HttpGet("{id}")]
        public PhysicalQuantity Get(Guid id)
        {
            PhysicalQuantity quantity = DrillingPhysicalQuantity.GetQuantity(id);
            return quantity;
        }
    }
}