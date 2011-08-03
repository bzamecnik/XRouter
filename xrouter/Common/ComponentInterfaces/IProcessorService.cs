
namespace XRouter.Common.ComponentInterfaces
{
    public interface IProcessorService : IComponentService
    {
        void Start(string componentName, IBrokerServiceForProcessor brokerService);
        void Stop();

        double GetUtilization();

        void AddWork(Token token);
    }
}
