using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace ObjectRemoter.BasicSystemTest.Server
{
    public interface IConsoleService : IRemotelyReferable
    {
        void WriteLine(string text);

        void WriteLine(Token token);
    }
}
