
namespace XRouter.Common
{
    public interface IComponentController
    {
        void StartComponent();
        void StopComponent();

        bool IsComponentRunning();
    }
}
