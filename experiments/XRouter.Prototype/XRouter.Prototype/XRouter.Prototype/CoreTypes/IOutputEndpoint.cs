using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Prototype.CoreTypes
{
    interface IOutputEndpoint
    {
        void SendMessage(Token messageToken, Action<Token> resultCallback);
    }
}
