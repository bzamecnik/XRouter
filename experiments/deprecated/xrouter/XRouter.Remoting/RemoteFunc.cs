using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Remoting
{
    internal class RemoteFunc<R> : IRemoteFunc<R>
    {
        private Func<R> function;

        public RemoteFunc(Func<R> function)
        {
            this.function = function;
        }

        public R Invoke()
        {
            return function();
        }
    }

    internal class RemoteFunc<T1, R> : IRemoteFunc<T1, R>
    {
        private Func<T1, R> function;

        public RemoteFunc(Func<T1, R> function)
        {
            this.function = function;
        }

        public R Invoke(T1 arg1)
        {
            return function(arg1);
        }
    }

    internal class RemoteFunc<T1, T2, R> : IRemoteFunc<T1, T2, R>
    {
        private Func<T1, T2, R> function;

        public RemoteFunc(Func<T1, T2, R> function)
        {
            this.function = function;
        }

        public R Invoke(T1 arg1, T2 arg2)
        {
            return function(arg1, arg2);
        }
    }

    internal class RemoteFunc<T1, T2, T3, R> : IRemoteFunc<T1, T2, T3, R>
    {
        private Func<T1, T2, T3, R> function;

        public RemoteFunc(Func<T1, T2, T3, R> function)
        {
            this.function = function;
        }

        public R Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return function(arg1, arg2, arg3);
        }
    }

    internal class RemoteFunc<T1, T2, T3, T4, R> : IRemoteFunc<T1, T2, T3, T4, R>
    {
        private Func<T1, T2, T3, T4, R> function;

        public RemoteFunc(Func<T1, T2, T3, T4, R> function)
        {
            this.function = function;
        }

        public R Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return function(arg1, arg2, arg3, arg4);
        }
    }
}
