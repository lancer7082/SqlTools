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

        public Token Next()
        {
            PassWhiteSpace();
            if (reader.Peek() != -1)
            {
                var c = (char)reader.Peek();
                switch (c)
                {
                    case '(':
                        return new Token { Type = TokenTypes.OPEN_BRACKET };
                    case ')':
                        return new Token { Type = TokenTypes.CLOSE_BRACKET };
                    case '[':
                        return new Token { Type = TokenTypes.OPEN_SQUARE_BRACE };
                    case ']':
                        return new Token { Type = TokenTypes.CLOSE_SQUARE_BRACE };
                    case '.':
                        return new Token { Type = TokenTypes.DOT};
                    case ';':
                        return new Token { Type = TokenTypes.SEMICOLON };
                    default:
                        if (Char.IsLetter(c))
                        {
                            var sb = new StringBuilder();
                            while (Char.IsLetterOrDigit((char)reader.Peek()))
                            {
                                sb.Append((char)reader.Read());
                            }

                            var token = sb.ToString();
                            switch(token.ToUpper())
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
                                default
                                    //TODO
                                    return null;
                            }
                        }
                        break;
                }
            }
            return null;
        }

        public Token LookAhead()
        {
            return null;
        }
    }
}