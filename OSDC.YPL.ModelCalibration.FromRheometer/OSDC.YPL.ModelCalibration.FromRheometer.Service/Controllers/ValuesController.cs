using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Controllers
{
    [Produces("application/json")]
    [Route("YPLCalibrationFromRheometer/api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<int> Get()
        {
            return RheogramManager.Instance.GetIDs();
        }

        // GET api/values/5
        /// <summary>
        /// default uses Mullineux' method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public YPLModel Get(int id)
        {
            Rheogram rheogram = RheogramManager.Instance.Get(id);
            if (rheogram != null)
            {
                YPLModel model = new YPLModel();
                model.Rheogram = rheogram;
                model.FitToMullineux(rheogram);
                return model;
            }
            else
            {
                return null;
            }
        }
        // GET api/values/5
        /// <summary>
        /// default uses Mullineux' method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/{method}")]
        public YPLModel Get(int id, string method)
        {
            Rheogram rheogram = RheogramManager.Instance.Get(id);
            if (rheogram != null)
            {
                YPLModel model = new YPLModel();
                model.Rheogram = rheogram;
                switch (method)
                {
                    case "Kelessidis":
                        model.FitToKelessidis(rheogram);
                        break;
                    default:
                        model.FitToMullineux(rheogram);
                        break;
                }
                return model;
            }
            else
            {
                return null;
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Rheogram value)
        {
            if (value != null)
            {
                Rheogram rheogram = RheogramManager.Instance.Get(value.ID);
                if (rheogram == null)
                {
                    RheogramManager.Instance.Add(value);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Rheogram value)
        {
            if (value != null)
            {
                Rheogram rheogram = RheogramManager.Instance.Get(value.ID);
                if (rheogram != null)
                {
                    RheogramManager.Instance.Update(id, value);
                }
                else
                {
                    RheogramManager.Instance.Add(value);
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            RheogramManager.Instance.Remove(id);
        }
    }
}
