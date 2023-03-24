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
        public IEnumerable<MetaInfo> Get(int option)
        {
            if (option == 0)
            {
                List<PhysicalQuantity> quantities = new List<PhysicalQuantity>();
                // Adding base Conversion quantities
                quantities.AddRange(PhysicalQuantity.AvailableQuantities);
                // and quantities specific to Conversion.DrillingEngineering
                quantities.AddRange(DrillingPhysicalQuantity.AvailableQuantities);
                List<MetaInfo> ids = new List<MetaInfo>();
                if (quantities != null)
                {
                    foreach (PhysicalQuantity quantity in quantities)
                    {
                        MetaInfo metaInfo = new MetaInfo
                        {
                            ID = quantity.ID,
                            Name = quantity.Name
                        };
                        ids.Add(metaInfo);
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
            // GetQuantity(Guid id) method looks into Conversion quantities and Conversion.DrillingEngineering
            PhysicalQuantity quantity = DrillingPhysicalQuantity.GetQuantity(id);
            return quantity;
        }
    }
}