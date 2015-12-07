using System;
using System.Collections.Generic;

namespace SqlTools.Common
{
    public class SqlParser
    {
        private SqlLexer lexer;
        private Token currentToken;
        
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
        /// Выражение: USE, EXEC
        /// </summary>
        private bool StatementBase()
        {
            //System.Console.WriteLine("StatementBase: %s", currentToken.Type);
            //seek end of batch
            Token token;
            //bool isValid = false;
            while ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.GO ||
                    token.Type == TokenTypes.SEMICOLON)
                {
                    return true;
                }
            }
            throw new ApplicationException("End of batch not found");
            //return true;
        }

        /// <summary>
        /// ALTER | EXEC OBJECT (PROCEDURE | FUNCTION)
        /// </summary>
        private bool StatementAlterCreate()
        {
            //System.Console.WriteLine("StatementAlterCreate: %s", currentToken.Type);
            Token token;
            //bool isValid = false;
            while ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.GO ||
                    token.Type == TokenTypes.SEMICOLON)
                {
                    return true;
                }
            }
            throw new ApplicationException("End of batch not found");
        }

        /// <summary>
        /// Выражение
        /// </summary>
        /// <returns></returns>
        private bool Statement()
        {
            var token = lexer.Next();
            if (token != null)
            {
                currentToken = token;
                switch (token.Type)
                {
                    case TokenTypes.USE:
                    case TokenTypes.EXEC:
                        StatementBase();
                        break;
                    case TokenTypes.ALTER:
                    case TokenTypes.CREATE:
                        StatementAlterCreate();
                        break;
                    default:                      
                        throw new ApplicationException("Syntax error");
                }
                return true;
            }
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
