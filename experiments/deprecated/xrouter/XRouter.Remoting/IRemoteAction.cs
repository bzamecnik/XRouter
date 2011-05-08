using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    internal interface IRemoteAction : IRemotelyReferable
    {
        void Invoke();
    }

    internal interface IRemoteAction<T1> : IRemotelyReferable
    {
        void Invoke(T1 arg1);
    }

    internal interface IRemoteAction<T1, T2> : IRemotelyReferable
    {
        void Invoke(T1 arg1, T2 arg2);
    }

    internal interface IRemoteAction<T1, T2, T3> : IRemotelyReferable
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }

    internal interface IRemoteAction<T1, T2, T3, T4> : IRemotelyReferable
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
}
