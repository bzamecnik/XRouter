using System.ServiceModel;

namespace XRouter.Examples.Restaurant.RestaurantService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void SaveOrder(Order order);

        [OperationContract]
        Receipt GetReceipt(Payment payment);  
    }    
}
