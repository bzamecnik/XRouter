using System;
using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace XRouter.Data.Clr
{
    public partial class XRouterUDFs
    {
        [SqlFunction(
            Name = "Split",
            FillRowMethodName = "FillRow",
            TableDefinition = "value nvarchar(MAX)"
        )]
        public static IEnumerable Split(SqlString input, SqlString delimeter, SqlBoolean removeEmpty)
        {
            if (input.IsNull || delimeter.IsNull || removeEmpty.IsNull)
            {
                return null;
            }

            StringSplitOptions stringSplitOptions;
            if (removeEmpty)
            {
                stringSplitOptions = StringSplitOptions.RemoveEmptyEntries;
            }
            else
            {
                stringSplitOptions = StringSplitOptions.None;
            }

            return input.Value.Split(delimeter.Value.ToCharArray(), stringSplitOptions);
        }

        public static void FillRow(object row, out SqlString input)
        {
            input = new SqlString((string)row);
        }
    };
}

