using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Data
{
    public interface IDataAccess_New
    {
        void Initialize(string connectionString);

        void SaveConfiguration(string configXml);
        string LoadConfiguration();

        void SaveToken(Guid tokenGuid, string tokenXml);
        string LoadToken(Guid tokenGuid);
        IEnumerable<string> LoadTokens(int pageSize, int pageNumber);
        IEnumerable<string> LoadMatchingTokens(string xpath);
    }
}
