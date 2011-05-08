using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Remoting.RemoteCommunication;

namespace XRouter.Remoting
{
    public class RemoteObjectAddress
    {
        public ServerAddress ServerAddress { get; private set; }

        public string ServerGuid { get; private set; }        

        public int ObjectID { get; private set; }

        public RemoteObjectAddress(ServerAddress serverAddress, string serverGuid, int objectID)
        {
            ServerAddress = serverAddress;
            ServerGuid = serverGuid;
            ObjectID = objectID;
        }

        public string Serialize()
        {
            string result = ServerAddress.Serialize() + "|" + ServerGuid + "|" + ObjectID.ToString();
            return result;
        }

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
