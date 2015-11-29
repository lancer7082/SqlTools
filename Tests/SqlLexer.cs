using System;
using System.IO;

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
            while (reader.Peek() != -1)
            {
                var c = (char)reader.Peek();
                switch (c)
                {
                    case '(':
                        return new Token { Type = TokenTypes.OPEN_PARENTHESIS };
                    case ')':
                        return new Token { Type = TokenTypes.CLOSE_PARENTHESIS };
                    default:
                        if (Char.IsLetter(c))
                        {
                            //var token = ParseKeyword();
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