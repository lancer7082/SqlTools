using System.Text;
using System.Collections.Generic;
using System.IO;

namespace SqlTools.SimpleSqlParser
{
    public class SqlParser
    {
        private SqlLexer lexer;
        //private Token currentToken;
        private List<Token> comments;
        
        public List<ScriptObject> Objects { get; set; }

        public SqlParser(string text)
        {
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(text));
            lexer = new SqlLexer(ms);
            comments = new List<Token>();
            Objects = new List<ScriptObject>();
        }

        public void Parse()
        {
            if (lexer.Eof()) lexer.Reset();
            StatementsList();
        }

        /// <summary>
        /// Выражение: USE, EXEC
        /// </summary>
        private bool StatementBase()
        {
            if (SeekEndOfBatch())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ALTER | CREATE | DROP
        /// </summary>
        /// <returns></returns>
        private bool IsDDLInstruction(TokenTypes type)
        {
            return (
                type == TokenTypes.ALTER ||
                type == TokenTypes.CREATE ||
                type == TokenTypes.DROP);
        }

        /// <summary>
        /// GRANT | REVOKE
        /// </summary>
        /// <returns></returns>
        private bool IsDCLInstruction(TokenTypes type)
        {
            return (
                type == TokenTypes.GRANT ||
                type == TokenTypes.REVOKE);
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
                {
                    return true;
                }
                else if (IsDDLInstruction(token.Type))
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Поиск завершения инструкции 
        /// </summary>
        /// <returns></returns>
        private bool SeekEndOfBatch()
        {
            Token token;
            while ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.GO)
                {
                    return true;
                }
                else if (IsDDLInstruction(token.Type))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Разбор процедуры
        /// </summary>
        /// <returns></returns>
        private bool MatchProcedure(ref string objectName)
        {
            //string objectName = "";
            if (MatchObjectName(ref objectName))
            {
                if (SeekToken(TokenTypes.AS))
                {
                    if (SeekEndOfBatch())
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
        private bool MatchFunction(ref string objectName)
        {
            //string objectName = "";
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
            token = lexer.LastToken;
            if (token != null)
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
                        objectName += token.Value;
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

        //private long GetCurrentPosition

        /// <summary>
        /// ALTER | EXEC OBJECT (PROCEDURE | FUNCTION)
        /// </summary>
        private bool StatementAlterCreate()
        {
            Token token;
            string objectName = "";

            var lastToken = lexer.LastToken;

            if ((token = lexer.Next()) != null)
            {
                if (token.Type == TokenTypes.PROCEDURE)
                {
                    if (MatchProcedure(ref objectName))
                    {
                        Objects.Add(new ScriptObject { Name = objectName, PosStart = lastToken.Position, PosEnd = lexer.CurrentPosition });
                        return true;
                    }
                }
                else if (token.Type == TokenTypes.FUNCTION)
                {
                    if (MatchFunction(ref objectName))
                    {
                        Objects.Add(new ScriptObject { Name = objectName, PosStart = token.Position, PosEnd = lexer.CurrentPosition });
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// GRANT Permission ON Object TO Principal
        /// </summary>
        private bool MatchGrantRevoke()
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
                    case TokenTypes.REVOKE:
                        MatchGrantRevoke();
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
