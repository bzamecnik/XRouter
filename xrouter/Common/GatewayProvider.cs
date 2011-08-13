using XRouter.Common.ComponentInterfaces;

namespace XRouter.Common
{
    /// <summary>
    /// An object which holds both a reference to a gateway component instance
    /// and its name.
    /// </summary>
    public class GatewayProvider
    {
        /// <summary>
        /// Identifier of the gateway component instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the gateway component instance.
        /// </summary>
        public IGatewayService Gateway{ get; private set; }

        public GatewayProvider(string name, IGatewayService gateway)
        {
            Name = name;
            Gateway = gateway;
        }
    }
}
