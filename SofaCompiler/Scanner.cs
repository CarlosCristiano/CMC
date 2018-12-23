using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SofaCompiler
{
    public class Scanner
    {
        private SourceFile source;
        private char current;
        private StringBuilder currentTok = new StringBuilder();

        public Scanner(SourceFile source)
        {
            this.source = source;
            current = source.GetSource();
        }

        private void ReceiveCurrent()
        {
            currentTok.Append(current);
            current = source.GetSource();
        }

        private bool IsLetter(char letter)
        {
            return Regex.IsMatch(letter.ToString(), @"^[a-zA-Z]+$");
        }

        private bool IsDigit(char number)
        {
            return Regex.IsMatch(number.ToString(), @"^\d$");
        }

        private void ScanSeparator()
        {
            switch (current)
            {
                case '#':
                    ReceiveCurrent();
                    while (current != SourceFile.EOL && current != SourceFile.EOT)
                        ReceiveCurrent();

                    if (current == SourceFile.EOL)
                        ReceiveCurrent();
                    break;

                case ' ':
                case '\n':
                case '\t':
                    ReceiveCurrent();
                    break;
            }
        }

        private byte ScanToken()
        {
            if (IsLetter(current))
            {
                ReceiveCurrent();
                while (IsLetter(current) || IsDigit(current))
                    ReceiveCurrent();

                return Token.IDENTIFIER;

            }
            else if (IsDigit(current))
            {
                ReceiveCurrent();
                while (IsDigit(current))
                    ReceiveCurrent();

                return Token.INTEGER;

            }
            switch (current)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '=':
                    ReceiveCurrent();
                    return Token.OPERATOR;

                case ',':
                    ReceiveCurrent();
                    return Token.COMMA;

                case ';':
                    ReceiveCurrent();
                    return Token.SEMICOLON;

                case ':':
                    ReceiveCurrent();
                    return Token.COLON;

                case '{':
                    ReceiveCurrent();
                    return Token.LEFT_BRAKET;

                case '}':
                    ReceiveCurrent();
                    return Token.RIGHT_BRAKET;

                /*case SourceFile.EOT:
                    return Token.EOT;
                    */

                default:
                    ReceiveCurrent();
                    return Token.ERROR;
            }
        }

        public Token Scan()
        {
            while (current == '#' || 
                   current == '\n' ||
                   current == '\t' ||
                   current == ' ')
                ScanSeparator();

            currentTok = new StringBuilder("");
            byte kind = ScanToken();
            string result = currentTok.ToString();

            return new Token(kind, result);
        }
    }
}
