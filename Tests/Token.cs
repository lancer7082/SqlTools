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
        DROP,
        PROCEDURE,
        FUNCTION,
        AS,
        EXEC,
        GRANT,
        REVOKE,
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

        public long Position { get; set; }

        public Token()
        {
        }
    }

    public class ScriptObject
    {
        public string Name { get; set; }

        public long PosStart { get; set; }

        public long PosEnd { get; set; }
    }
}
