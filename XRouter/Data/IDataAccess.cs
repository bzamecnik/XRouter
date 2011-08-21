using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Data
{
    public interface IDataAccess
    {
        void Initialize(string connectionString);

        void SaveConfiguration(string configXml);
        string LoadConfiguration();

        void SaveToken(Guid tokenGuid, string tokenXml);
        void SaveToken(Token token);
        string LoadToken(Guid tokenGuid);
        IEnumerable<string> LoadTokens(int pageSize, int pageNumber);
        IEnumerable<string> LoadMatchingTokens(string xpath);
    }
}
