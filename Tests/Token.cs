using System;

namespace SqlTools.Common
{
    public enum TokenTypes {
        //Symbols
        OPEN_BRACKET = 0,       //(
        CLOSE_BRACKET,          //)
        OPEN_SQUARE_BRACE,      //[
        CLOSE_SQUARE_BRACE,     //]
        DOT,                    //.
        SEMICOLON,              //;
        //Keywords
        USE,
        GO,
        BEGIN,                  
        END,                    
        CREATE,
        ALTER,
        PROCEDURE,
        FUNCTION,
        AS,
        EXEC,
        //
        IDENTIFIER,             //ID        
        DELIMITER               //,
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
