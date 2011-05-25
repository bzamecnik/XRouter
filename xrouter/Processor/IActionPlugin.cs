using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Processor
{
    public interface IActionPlugin : IDisposable
    {
        void Initialize(IProcessorServiceForAction processorService);

        void Evaluate(Token token);
    }
}
