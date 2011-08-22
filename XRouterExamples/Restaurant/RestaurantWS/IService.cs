using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace XRouterWS
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
