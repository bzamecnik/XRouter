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
        /// GUID of target object server
        /// </summary>
        public string ServerGuid { get; private set; }        

        /// <summary>
        /// Object identification
        /// </summary>
        public int ObjectID { get; private set; }

        internal RemoteObjectAddress(ServerAddress serverAddress, string serverGuid, int objectID)
        {
            ServerAddress = serverAddress;
            ServerGuid = serverGuid;
            ObjectID = objectID;
        }

        /// <summary>
        /// Serializes this address into string.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string result = string.Format("{0}|{1}|{2}", ServerAddress.Serialize(), ServerGuid, ObjectID);
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
            string serverGuid = parts[1];
            int objectID = int.Parse(parts[2]);
            var result = new RemoteObjectAddress(serverAddress, serverGuid, objectID);
            return result;
        }

        public override int GetHashCode()
        {
            return ServerGuid.GetHashCode() ^ ObjectID;
        }

        public override bool Equals(object obj)
        {
            if (obj is RemoteObjectAddress) {
                var other = (RemoteObjectAddress)obj;
                bool result = (other.ServerGuid == ServerGuid) && (other.ObjectID == ObjectID);
                return result;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}", ServerAddress.Url, ServerGuid, ObjectID);
        }
    }
}
