using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    /// <summary>
    /// Objects implementing this interface will be cloned when used
    /// remotely. The target computer will not use a remote proxy but rather
    /// an exact copy of the remote object.
    /// </summary>
    public interface IRemotelyCloneable
    {
        /// <summary>
        /// Creates a serialized representation of the object in order to
        /// transfer it to the remote system and make a clone there.
        /// </summary>
        /// <remarks>
        /// This method should be called on the original object which should
        /// be remotely cloned.</remarks>
        /// <returns>Serialized representation of the remote object</returns>
        string SerializeClone();

        /// <summary>
        /// Initialized a clone of the remote object from the serialized
        /// representation.
        /// </summary>
        /// <remarks>
        /// This method is not a factory method and is not intended for direct
        /// usage, rather it provides a part of the implementation of cloning.
        /// It is called upon and operates on a uninitialized object and fills
        /// it with the information from the serialized form.
        /// </remarks>
        /// <param name="serialized">Serialized representation of the remote
        /// object.</param>
        void DeserializeClone(string serialized);
    }
}
