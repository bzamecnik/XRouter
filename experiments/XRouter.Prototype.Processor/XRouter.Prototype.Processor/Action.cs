using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Prototype.Processor
{
    class Action : INodeFunction
    {
        private Int32 step;

        private String name = null;

        private INodeFunction func = null;

        public Action(INodeFunction func, String name, Int32 step)
        {
            this.name = name;
            this.step = step;
            this.func = func;
        }

        public void Initialize()
        {
            Logger.LogInfo(String.Format("Init Action.{0}", name));
            this.func.Initialize();
            Logger.LogInfo(String.Format("Init Action.{0} done!", name));
        }

        public void Evaluate(Token token)
        {
            Logger.LogInfo(String.Format("Action.{0}", name));
            this.func.Evaluate(token);
            token.Step = step;
            Logger.LogInfo(String.Format("Action.{0} done!", name));
        }
    }
}
