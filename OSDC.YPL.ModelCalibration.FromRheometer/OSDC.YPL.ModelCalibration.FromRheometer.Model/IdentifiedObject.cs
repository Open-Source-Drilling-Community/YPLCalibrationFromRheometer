using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public class IdentifiedObject<T> : IIdentifiable, ICopyable<IdentifiedObject<T>>
    {
        /// <summary>
        ///  accessor to the object
        /// </summary>
        public T Object { get; set; }
        /// <summary>
        /// identification
        /// </summary>
        public int ID { get; set; } = -1;
        /// <summary>
        /// default constructor
        /// </summary>
        public IdentifiedObject()
        {
        }
        /// <summary>
        /// parametrized constructor
        /// </summary>
        /// <param name="source"></param>
        public IdentifiedObject(T source)
        {
            Object = source;
        }
        /// <summary>
        /// fully parametrized constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="id"></param>
        public IdentifiedObject(T source, int id)
        {
            Object = source;
            ID = id;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source"></param>
        public IdentifiedObject(IdentifiedObject<T> source, bool complete = false)
        {
            if (source != null)
            {
                Object = source.Object;
                if (complete)
                {
                    ID = source.ID;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool Copy(IdentifiedObject<T> target)
        {
            if (target != null)
            {
                target.Object = Object;
                target.ID = ID;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
