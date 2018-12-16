using System;

namespace SofaCompiler {

    internal class Program {

        public static void Main(string[] args) {
            Lexer lexer = new Lexer(@"sofaLanguage.lang");

            lexer.Run();
            Console.ReadLine();
        }
    }

}