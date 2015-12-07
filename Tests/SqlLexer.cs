using System;
using System.IO;
using System.Text;

namespace SqlTools.Common
{
    public class SqlLexer
    {
        private readonly string text;
        private StringReader reader;
        //public readonly char[] aWhiteSpace = { ' ', '\t' };
        //public readonly char[] aNewLine = { '\n', '\r' };

        public SqlLexer(string text)
        {
            this.text = text;
            reader = new StringReader(text);
        }    
        
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

        private Token ParseSlashOrComment()
        {
            if (reader.Peek() != -1 && (char)reader.Peek() == '/')
            {
                var c = (char)reader.Read();
                if (reader.Peek() != -1)
                {
                    if ((char)reader.Peek() == '*')   // /*
                    {
                        //search for end of comment
                        var sb = new StringBuilder();
                        while (reader.Peek() != -1)
                        {
                            if ((char)reader.Peek() == '*')
                            {
                                reader.Read();
                                if ((char)reader.Peek() == '/') return new Token { Type = TokenTypes.COMMENT, Value = sb.ToString() };
                            }
                            sb.Append((char)reader.Read());
                        }
                    }
                    else
                    {
                        return new Token { Type = TokenTypes.DELIMITER, Value = c.ToString() };
                    }
                }
            }
            throw new ApplicationException("Syntax error");
        }

        private Token ParseMinusOrComment()
        {
            return null;
        }

        public Token Next()
        {
            PassWhiteSpace();
            if (reader.Peek() != -1)
            {
                //Token token;
                var c = (char)reader.Peek();
                switch (c)
                {
                    case '(':
                        reader.Read();
                        return new Token { Type = TokenTypes.OPEN_BRACKET };
                    case ')':
                        reader.Read();
                        return new Token { Type = TokenTypes.CLOSE_BRACKET };
                    case '[':
                        reader.Read();
                        return new Token { Type = TokenTypes.SQUARE_BRACE_OPEN };
                    case ']':
                        reader.Read();
                        return new Token { Type = TokenTypes.SQUARE_BRACE_CLOSE };
                    case '.':
                        reader.Read();
                        return new Token { Type = TokenTypes.DOT };
                    case ';':
                        reader.Read();
                        return new Token { Type = TokenTypes.SEMICOLON };
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
                                    return new Token { Type = TokenTypes.CREATE };
                                case "ALTER":
                                    return new Token { Type = TokenTypes.ALTER };
                                case "PROCEDURE":
                                    return new Token { Type = TokenTypes.PROCEDURE };
                                case "FUNCTION":
                                    return new Token { Type = TokenTypes.FUNCTION };
                                case "AS":
                                    return new Token { Type = TokenTypes.AS };
                                case "EXEC":
                                    return new Token { Type = TokenTypes.EXEC };
                                default:
                                    //TODO
                                    return new Token { Type = TokenTypes.IDENTIFIER, Value = token };
                            }
                        }
                        else
                        {
                            reader.Read();
                            return new Token { Type = TokenTypes.DELIMITER, Value = c.ToString() };
                        }
                        //break;              
                }
            }
            return null;
        }

        //public Token LookAhead()
        //{
        //    return null;
        //}
    }
}