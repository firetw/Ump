using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WebLoginer.Core
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            dcs.WriteObject(ms, source);
            ms.Seek(0, 0);
            T result = (T)dcs.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return result;
        }
    }
}
