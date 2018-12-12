using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace SqlTools.MSSqlParser
{
    public class SqlParser
    {
        public class TokenInfo
        {
            public int Start { get; set; }
            public int End { get; set; }
            public bool IsPairMatch { get; set; }
            public bool IsExecAutoParamHelp { get; set; }
            public string Sql { get; set; }
            public Tokens Token { get; set; }
        }

        public void Parse()
        {
            var opt = new ParseOptions("GO");
            var scanner = new Scanner(opt);
            var sb = new StringBuilder();
            sb.Append("CREATE PROCEDURE [BackOffice].[Accounts:View]");
            sb.Append("GO");
            var sql = sb.ToString();
            scanner.SetSource(sql, 0);

            int token,
                start = 0,
                end = 0,
                state = 0,
                lastTokenEnd = -1;

            bool isPairMatch = false, isExecAutoParamHelp = false;

            List<TokenInfo> tokens = new List<TokenInfo>();

            while ((token = scanner.GetNext(ref state, out start, out end, out isPairMatch, out isExecAutoParamHelp)) != (int)Tokens.EOF)
            {
                TokenInfo tokenInfo =
                      new TokenInfo()
                      {
                          Start = start,
                          End = end,
                          IsPairMatch = isPairMatch,
                          IsExecAutoParamHelp = isExecAutoParamHelp,
                          Sql = sql.Substring(start, end - start + 1),
                          Token = (Tokens)token,
                      };

                tokens.Add(tokenInfo);

                lastTokenEnd = end;
            };
        }    
    }
}
