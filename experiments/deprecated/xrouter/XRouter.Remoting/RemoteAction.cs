using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    internal class RemoteAction : IRemoteAction
    {
        private Action action;

        public RemoteAction(Action action)
        {
            this.action = action;
        }

        public void Invoke()
        {
            action();
        }
    }

    internal class RemoteAction<T1> : IRemoteAction<T1>
    {
        private Action<T1> action;

        public RemoteAction(Action<T1> action)
        {
            this.action = action;
        }

        public void Invoke(T1 arg1)
        {
            action(arg1);
        }
    }

    internal class RemoteAction<T1, T2> : IRemoteAction<T1, T2>
    {
        private Action<T1, T2> action;

        public RemoteAction(Action<T1, T2> action)
        {
            this.action = action;
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            action(arg1, arg2);
        }
    }

    internal class RemoteAction<T1, T2, T3> : IRemoteAction<T1, T2, T3>
    {
        private Action<T1, T2, T3> action;

        public RemoteAction(Action<T1, T2, T3> action)
        {
            this.action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            action(arg1, arg2, arg3);
        }
    }

    internal class RemoteAction<T1, T2, T3, T4> : IRemoteAction<T1, T2, T3, T4>
    {
        private Action<T1, T2, T3, T4> action;

        public RemoteAction(Action<T1, T2, T3, T4> action)
        {
            this.action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            action(arg1, arg2, arg3, arg4);
        }
    }
}
