using System;
using System.Text;
using System.Collections.Generic;

namespace SqlTools.Common
{
    public class SqlParser
    {
        private SqlLexer lexer;
        //private Token currentToken;
        private List<Token> comments;
        
        public List<string> Objects { get; set; }

        public SqlParser(string text)
        {
            lexer = new SqlLexer(text);
            comments = new List<Token>();
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
            if (SeekToken(TokenTypes.GO))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Поиск 1-го токена по типу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool SeekToken(TokenTypes type)
        {
            Token token;
            while ((token = lexer.Next()) != null)
            {
 
                if (token.Type == type)
                    return true;
                else if (token.Type == TokenTypes.ALTER || 
                    token.Type == TokenTypes.CREATE ||
                    token.Type == TokenTypes.GRANT)
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Разбор процедуры
        /// </summary>
        /// <returns></returns>
        private bool MatchProcedure()
        {
            string objectName = "";
            if (MatchObjectName(ref objectName))
            {
                if (SeekToken(TokenTypes.AS))
                {
                    if (SeekToken(TokenTypes.GO))
                    {
                        return true;
                    }
                }
            }
            return false; 
        }

        /// <summary>
        /// Разбор функции
        /// </summary>
        /// <returns></returns>
        private bool MatchFunction()
        {
            string objectName = "";
            if (MatchObjectName(ref objectName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Название объекта БД (схема, таблица, поле и т.д.)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool MatchDatabaseIdentifier(ref string name)
        {
            Token token;
            var sb = new StringBuilder();
            if ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.SQUARE_BRACE_OPEN)
                {
                    sb.Append(token.Value);
                    if (((token = lexer.Next()) != null) && (token.Type == TokenTypes.IDENTIFIER))
                    {
                        sb.Append(token.Value);
                        if (((token = lexer.Next()) != null) && (token.Type == TokenTypes.SQUARE_BRACE_CLOSE))
                        {
                            sb.Append(token.Value);
                            name = sb.ToString();
                            return true;
                        }
                    }
                }
                else if (token.Type == TokenTypes.IDENTIFIER)
                {
                    name = token.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Разбор названия объекта с учетом схемы
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        private bool MatchObjectName(ref string objectName)
        {
            Token token;
            string name = "";
            if ((token = lexer.Next()) != null)
            {
                if (MatchDatabaseIdentifier(ref name))
                {
                    objectName = name;
                    if (((token = lexer.LookAhead()) != null) && (token.Type == TokenTypes.DOT))
                    {
                        token = lexer.Next();
                        if (MatchDatabaseIdentifier(ref name))
                        {
                            objectName += name;
                            return true;
                        }
                    }
                    else 
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ALTER | EXEC OBJECT (PROCEDURE | FUNCTION)
        /// </summary>
        private bool StatementAlterCreate()
        {
            Token token;
            if ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.PROCEDURE)
                {
                    if (MatchProcedure())
                        return true;
                }
                else if (token.Type == TokenTypes.FUNCTION)
                {
                    if (MatchFunction())
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// GRANT Permission ON Object TO Principal
        /// </summary>
        private bool MatchGrantPermission()
        {
            return false;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //private void PushComment(Token token)
        //{
        //    comments.Add(token)
        //}

        /// <summary>
        /// Выражение
        /// </summary>
        /// <returns></returns>
        private bool Statement()
        {
            var token = lexer.Next();
            if (token != null)
            {
                //if ((token.Type != TokenTypes.COMMENT) && (token.Type != ))
                //currentToken = token;
                switch (token.Type)
                {
                    case TokenTypes.ALTER:
                    case TokenTypes.CREATE:
                        StatementAlterCreate();
                        break;
                    case TokenTypes.GRANT:
                        MatchGrantPermission();
                        break;
                    case TokenTypes.COMMENT:
                        comments.Add(token);
                        //PushComment(token);
                        break;
                    //case TokenTypes.USE:
                    //case TokenTypes.EXEC:
                    //    StatementBase();
                    //    break;
                    //default:                      
                    //    throw new ApplicationException("Syntax error");
                    default:
                        StatementBase();
                        break;
                }
                if (token.Type != TokenTypes.COMMENT)
                {
                    comments.Clear();
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
