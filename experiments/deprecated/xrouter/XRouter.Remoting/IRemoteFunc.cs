using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    internal interface IRemoteFunc<R> : IRemotelyReferable
    {
        R Invoke();
    }

    internal interface IRemoteFunc<T1, R> : IRemotelyReferable
    {
        R Invoke(T1 arg1);
    }

    internal interface IRemoteFunc<T1, T2, R> : IRemotelyReferable
    {
        R Invoke(T1 arg1, T2 arg2);
    }

    internal interface IRemoteFunc<T1, T2, T3, R> : IRemotelyReferable
    {
        R Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    internal interface IRemoteFunc<T1, T2, T3, T4, R> : IRemotelyReferable
    {
        R Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
}
