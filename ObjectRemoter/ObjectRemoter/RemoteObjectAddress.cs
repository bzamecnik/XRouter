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

        internal RemoteObjectAddress(ServerAddress serverAddress, int objectID)
        {
            ServerAddress = serverAddress;
            ObjectID = objectID;
        }

        /// <summary>
        /// Object identification
        /// </summary>
        public int ObjectID { get; private set; }

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

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RemoteObjectAddress)obj;
            return object.Equals(ServerAddress, other.ServerAddress) &&
                (ObjectID == other.ObjectID);
        }

        public override string ToString()
        {
            // TODO: no test coverage
            return string.Format("{0}|{1}", ServerAddress.Url, ObjectID);
        }
    }
}
