using System;
using System.IO;
using System.Text;

namespace SqlTools.Common
{
    public class SqlLexer
    {
        //private readonly string text;
        //private StringReader reader;
        private CustomStreamReader reader;

        /// <summary>
        /// Текущая позиция
        /// </summary>
        public long CurrentPosition { get { return reader != null ? reader.Position : 0; } }

        /// <summary>
        /// Последний токен
        /// </summary>
        public Token LastToken { get; set; }

        public SqlLexer(MemoryStream ms)
        {
            //this.text = text;
            reader = new CustomStreamReader(ms);
        }    
        
        /// <summary>
        /// Пропуск пробелов
        /// </summary>
        private void PassWhiteSpace()
        {
            while (Char.IsWhiteSpace((char)reader.Peek())) 
            {
                reader.Read();
            };
        }

        /// <summary>
        /// Строка
        /// </summary>
        /// <returns></returns>
        private Token ParseStringConstant()
        {
            if (reader.Peek() != -1 && (char)reader.Peek() == '\'')
            {
                reader.Read();
                var sb = new StringBuilder();
                while (reader.Peek() != -1)
                {
                    if ((char)reader.Peek() == '\'')
                    {
                        reader.Read();
                        if ((char)reader.Peek() != '\'') return new Token { Type = TokenTypes.STRING, Value = sb.ToString() };
                    }
                    sb.Append((char)reader.Read());
                }
            }
            throw new ApplicationException("Syntax error");
        }

        /// <summary>
        /// Многострочный комментарий или "/"
        /// </summary>
        /// <returns></returns>
        private Token ParseSlashOrComment()
        {
            if (reader.Peek() != -1 && (char)reader.Peek() == '/')
            {
                var c = (char)reader.Read();
                var pos = reader.Position;
                if (reader.Peek() != -1)
                {
                    if ((char)reader.Peek() == '*')   // /*
                    {
                        reader.Read();
                        //search for the end of comment
                        var sb = new StringBuilder();
                        while (reader.Peek() != -1)
                        {
                            if ((char)reader.Peek() == '*')
                            {
                                reader.Read();
                                if ((char)reader.Peek() == '/')
                                {
                                    reader.Read();
                                    return new Token { Type = TokenTypes.COMMENT, Value = sb.ToString(), Position = pos };
                                }
                            }
                            sb.Append((char)reader.Read());
                        }
                    }
                    else
                    {
                        reader.Read();
                        return new Token { Type = TokenTypes.DELIMITER, Value = c.ToString() };
                    }
                }
            }
            throw new ApplicationException("Syntax error");
        }

        /// <summary>
        /// Однострочный комментарий или "-"
        /// </summary>
        /// <returns></returns>
        private Token ParseMinusOrComment()
        {
            if (reader.Peek() != -1 && (char)reader.Peek() == '-')
            {
                var c = (char)reader.Read();
                var pos = reader.Position;
                if (reader.Peek() != -1)
                {
                    if ((char)reader.Peek() == '-')   // --
                    {
                        reader.Read();
                        //search for the end of line
                        var sb = new StringBuilder();
                        while (reader.Peek() != -1)
                        {
                            if ((char)reader.Peek() == '\r')
                            {
                                return new Token { Type = TokenTypes.COMMENT, Value = sb.ToString(), Position = pos };
                            }
                            sb.Append((char)reader.Read());
                        }
                    }
                    else
                    {
                        reader.Read();
                        return new Token { Type = TokenTypes.DELIMITER, Value = c.ToString() };
                    }
                }
            }
            throw new ApplicationException("Syntax error");
        }

        public Token Next()
        {
            var token = ReadNextToken();
            LastToken = token;
            return token;
        }

        private Token ReadNextToken()
        {
            PassWhiteSpace();
            if (reader.Peek() != -1)
            {
                //Token token;
                var c = (char)reader.Peek();
                var pos = reader.Position;
                switch (c)
                {
                    case '(':
                        reader.Read();
                        return new Token { Type = TokenTypes.OPEN_BRACKET, Value = c.ToString() };
                    case ')':
                        reader.Read();
                        return new Token { Type = TokenTypes.CLOSE_BRACKET, Value = c.ToString() };
                    case '[':
                        reader.Read();
                        return new Token { Type = TokenTypes.SQUARE_BRACE_OPEN, Value = c.ToString() };
                    case ']':
                        reader.Read();
                        return new Token { Type = TokenTypes.SQUARE_BRACE_CLOSE, Value = c.ToString() };
                    case '.':
                        reader.Read();
                        return new Token { Type = TokenTypes.DOT, Value = c.ToString() };
                    case ';':
                        reader.Read();
                        return new Token { Type = TokenTypes.SEMICOLON, Value = c.ToString() };
                    case '\'':
                        return ParseStringConstant();
                    case '-':
                        return ParseMinusOrComment();
                    case '/':
                        return ParseSlashOrComment();
                    default:
                        if (Char.IsLetter(c))
                        {
                            var sb = new StringBuilder();
                            while (Char.IsLetterOrDigit((char)reader.Peek()))
                            {
                                sb.Append((char)reader.Read());
                            }

                            var token = sb.ToString();
                            switch (token.ToUpper())
                            {
                                case "USE":
                                    return new Token { Type = TokenTypes.USE };
                                case "GO":
                                    return new Token { Type = TokenTypes.GO };
                                case "BEGIN":
                                    return new Token { Type = TokenTypes.BEGIN };
                                case "END":
                                    return new Token { Type = TokenTypes.END };
                                case "CREATE":
                                    return new Token { Type = TokenTypes.CREATE, Position = pos };
                                case "ALTER":
                                    return new Token { Type = TokenTypes.ALTER, Position = pos };
                                case "PROCEDURE":
                                    return new Token { Type = TokenTypes.PROCEDURE };
                                case "FUNCTION":
                                    return new Token { Type = TokenTypes.FUNCTION };
                                case "AS":
                                    return new Token { Type = TokenTypes.AS };
                                case "EXEC":
                                    return new Token { Type = TokenTypes.EXEC };
                                case "GRANT":
                                    return new Token { Type = TokenTypes.GRANT };
                                case "ON":
                                    return new Token { Type = TokenTypes.ON };
                                case "TO":
                                    return new Token { Type = TokenTypes.TO };
                                default:
                                    return new Token { Type = TokenTypes.IDENTIFIER, Value = token };
                            }
                        }
                        else
                        {
                            reader.Read();
                            return new Token { Type = TokenTypes.DELIMITER, Value = c.ToString() };
                        }
                }
            }
            return null;
        }

        public Token LookAhead()
        {
            var pos = reader.Position;
            var token = Next();
            reader.Seek(pos);
            return token;
        }

        public void Reset()
        {
            if (reader != null)
                reader.Seek(0);
        }

        public bool Eof()
        {
            return (reader != null ? reader.EndOfStream : true);
        }
    }
}