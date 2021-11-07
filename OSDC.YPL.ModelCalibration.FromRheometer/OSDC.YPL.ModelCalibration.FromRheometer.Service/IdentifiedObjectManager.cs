using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service
{

    public class IdentifiedObjectManager<T> where T : IIdentifiable, IParentIdentified, ICopyable<T>, IUndefinable, new()
    {
        private static IdentifiedObjectManager<T> instance_ = null;

        private Dictionary<int, T> data_ = new();
        private object lock_ = new();
        private Random random_ = new();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private IdentifiedObjectManager()
        {

        }

        public static IdentifiedObjectManager<T> Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new IdentifiedObjectManager<T>();
                }
                return instance_;

            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                lock (lock_)
                {
                    count = data_.Count;
                }
                return count;
            }
        }

        public bool Clear()
        {
            lock (lock_)
            {
                data_.Clear();
            }
            return true;
        }

        public bool Contains(int id)
        {
            bool contains = false;
            lock (lock_)
            {
                contains = data_.ContainsKey(id);
            }
            return contains;
        }
        public List<int> GetIDs()
        {
            List<int> ids = new();
            lock (lock_)
            {
                foreach (int key in data_.Keys)
                {
                    ids.Add(key);
                }
            }
            return ids;
        }

        public List<int> GetIDs(int parentID)
        {
            List<int> ids = new();
            if (parentID >= 0)
            {
                lock (lock_)
                {
                    foreach (T dat in data_.Values)
                    {
                        if (dat != null && dat.ID >= 0 && dat.ParentID == parentID)
                        {
                            ids.Add(dat.ID);
                        }
                    }
                }
            }
            return ids;
        }

        public T Get(int dataID)
        {
            if (dataID > 0)
            {
                T data;
                lock (lock_)
                {
                    data_.TryGetValue(dataID, out data);
                }
                return data;
            }
            else
            {
                T undefined = new();
                undefined.SetUndefined();
                return undefined;
            }
        }

        public bool Add(T data)
        {
            bool result = false;
            if (data != null && !data.IsUndefined())
            {
                if (data.ID < 0)
                {
                    data.ID = GetNextID();
                }
                lock (lock_)
                {
                    data_.Add(data.ID, data);
                    result = true;
                }
            }
            return result;
        }

        public bool Remove(T data)
        {
            bool result = false;
            if (data != null && data.ID >= 0)
            {
                result = Remove(data.ID);
            }
            return result;
        }

        public bool Remove(int dataID)
        {
            bool result = false;
            if (data_.ContainsKey(dataID))
            {
                lock (lock_)
                {
                    data_.Remove(dataID);
                    result = true;
                }
            }
            return result;
        }

        public bool Update(int dataID, T updatedData)
        {
            bool result = false;
            if (dataID > 0 && updatedData != null)
            {
                lock (lock_)
                {
                    T data = Get(dataID);
                    if (data == null || data.IsUndefined())
                    {
                        if (updatedData.ID < 0)
                        {
                            updatedData.ID = GetNextID();
                        }
                        result = Add(updatedData);
                    }
                    else
                    {
                        result = updatedData.Copy(data);
                    }
                }
            }
            return result;
        }

        public int GetNextID()
        {
            int id;
            bool exists;
            do
            {
                id = random_.Next();
                exists = data_.ContainsKey(id);
            }
            while (exists);
            return id;
        }
    }
}
