using System.Collections.Generic;

namespace SofaCompiler.AST {

    public class Ast {

        public class Variable {
            public string Name { get; set; }
            public object Value { get; set; }
            public Types Type { get; set; }

            public enum Types {
                numr,

                dust,

                expr,

                input,

                listNop,

                listDust
            }
        }

        public class ListVariable {

            public object Value { get; set; }
        }

        public readonly List<string> OperationsTokens = new List<string> {"+", "-", "*", "/", "="};


    }

}