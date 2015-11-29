using System;
using System.Collections.Generic;

namespace SqlTools.Common
{
    public class SqlParser
    {
        private SqlLexer lexer;
        
        public List<string> Objects { get; set; }

        public SqlParser(string text)
        {
            lexer = new SqlLexer(text);
        }

        public void Parse()
        {
            StatementsList();
        }

        /// <summary>
        /// Выражение
        /// </summary>
        /// <returns></returns>
        private bool Statement()
        {
            return false;
        }

        /// <summary>
        /// Список выражений
        /// </summary>
        private bool StatementsList()
        {
            while (Statement()) { };
            return true;
        }
    }
}
