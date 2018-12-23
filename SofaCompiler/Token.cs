using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofaCompiler
{
    public class Token
    {
        public byte kind;
        public string tok;
        public Token(byte kind, string tok)
        {
            this.kind = kind;
            this.tok = tok;

            if (kind == IDENTIFIER)
                for (byte i = 0; i < TOKENS.Length; i++)
                {
                    if (tok.Equals(TOKENS[i]))
                    {
                        this.kind = i;
                        break;
                    }
                }
        }

        private static string[] TOKENS =
        {
            "identifier",
            "integer",
            "operator",
            "sofa",
            "dust",
            "nop",
            "list<dust>",
            "list<nop>",
            "tv.input",
            "tv.output",

            "if",
            "then",
            "elseif",
            "else",
            "fi",

            "while",
            "do",
            "od",

            "foreach",
            "fe",

            "out",
            "void",

            "return",
            "=>",

            ",",
            ";",
            ":",
            "{",
            "}"
        };

        public const byte SOFA = 0;
        public const byte IDENTIFIER = 1;
        public const byte INTEGER = 2;
        public const byte OPERATOR = 3; 
        public const byte LIST_DUST = 4;
        public const byte LIST_NOP = 5;
        public const byte TV_INPUT = 6;
        public const byte TV_OUTPUT = 7;
        public const byte IF = 8;
        public const byte THEN = 9;
        public const byte ELSEIF = 10;
        public const byte ELSE = 11;
        public const byte FI = 12;
        public const byte WHILE = 13;
        public const byte DO = 14;
        public const byte OD = 15;
        public const byte FOREACH = 16;
        public const byte FE = 17;
        public const byte OUT = 18;
        public const byte VOID = 19;
        public const byte RETURN = 20;
        public const byte RETURN_LAMBDA = 21;
        public const byte COMMA = 22;
        public const byte SEMICOLON = 23;
        public const byte COLON = 24;
        public const byte LEFT_BRAKET = 25;
        public const byte RIGHT_BRAKET = 26;

        public const byte ERROR = 30; 


        private static string ADD_OPER = "+";
        private static string SUB_OPER = "-";
        private static string MUL_OPER = "*";
        private static string DIV_OPER = "/";
        private static string ASS_OPER = "="; 

    }
}
