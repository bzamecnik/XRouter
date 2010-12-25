using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.ComponentWatching
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class HideReferenceAttribute : Attribute
    {
        public HideReferenceAttribute()
        {
        }
    }
}
