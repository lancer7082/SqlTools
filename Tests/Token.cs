using System;

namespace SqlTools.Common
{
    public enum TokenTypes {
        OPEN_PARENTHESIS = 0,   //(
        CLOSE_PARENTHESIS,      //)
        BEGIN_BLOCK,            //{
        END_BLOCK               //{
        //KEYWORD = 0, IDENTIFIER, DELIMITER, NEWLINE
    }
    public class Token
    {
        public string Value { get; set; }

        public TokenTypes Type { get; set; }

        public Token()
        {
        }
    }
}
