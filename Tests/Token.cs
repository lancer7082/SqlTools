using System;

namespace SqlTools.Common
{
    public enum TokenTypes {
        //Symbols
        OPEN_BRACKET = 0,       //(
        CLOSE_BRACKET,          //)
        SQUARE_BRACE_OPEN,      //[
        SQUARE_BRACE_CLOSE,     //]
        DOT,                    //.
        SEMICOLON,              //;
        SINGLE_QUOTE,           //'
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
        GRANT,
        ON,
        TO,
        //
        //OBJECT,                 //Procedure of function
        //SCRIPT,                 //Other
        IDENTIFIER,             //ID        
        DELIMITER,              //,
        STRING,
        COMMENT
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
