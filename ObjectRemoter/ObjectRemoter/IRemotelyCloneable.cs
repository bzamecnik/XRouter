using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// Objects which implement this interface will be cloned when used remotely. So, a target computer will not use a proxy but a an exact copy.
    /// </summary>
    public interface IRemotelyCloneable
    {
        /// <summary>
        /// Creates a serialized representation which will be used to create a clone.
        /// </summary>
        /// <returns></returns>
        string SerializeClone();

        /// <summary>
        /// Creates a clone from serialized representation.
        /// </summary>
        /// <param name="serialized"></param>
        void DeserializeClone(string serialized);
    }
}
