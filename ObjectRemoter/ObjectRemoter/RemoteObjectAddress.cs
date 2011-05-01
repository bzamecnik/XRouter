using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjectRemoter.RemoteCommunication;

namespace ObjectRemoter
{
    /// <summary>
    /// An address of a remote object.
    /// </summary>
    public class RemoteObjectAddress
    {
        /// <summary>
        /// Address of a remote communication server
        /// </summary>
        internal ServerAddress ServerAddress { get; private set; }

        /// <summary>
        /// Object identification
        /// </summary>
        public int ObjectID { get; private set; }

        internal RemoteObjectAddress(ServerAddress serverAddress, int objectID)
        {
            ServerAddress = serverAddress;
            ObjectID = objectID;
        }

        /// <summary>
        /// Serializes this address into string.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string result = string.Format("{0}|{1}", ServerAddress.Serialize(), ObjectID);
            return result;
        }

        /// <summary>
        /// Crate an address object from serialized string.
        /// </summary>
        /// <param name="serialized"></param>
        /// <returns></returns>
        public static RemoteObjectAddress Deserialize(string serialized)
        {
            string[] parts = serialized.Split('|');
            ServerAddress serverAddress = ServerAddress.Deserialize(parts[0]);
            int objectID = int.Parse(parts[1]);
            var result = new RemoteObjectAddress(serverAddress, objectID);
            return result;
        }

        public override int GetHashCode()
        {
            return ObjectID;
        }

        public override bool Equals(object obj)
        {
            if (obj is RemoteObjectAddress) {
                var other = (RemoteObjectAddress)obj;
                bool result = (other.ServerAddress.Url == ServerAddress.Url) && (other.ObjectID == ObjectID);
                return result;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", ServerAddress.Url, ObjectID);
        }
    }
}
