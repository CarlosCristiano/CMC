using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SofaCompiler.AST;

namespace SofaCompiler {

    public class Lexer {
        private readonly string _filePath;

        private readonly Ast _ast = new Ast();

        public Lexer(string filePath) {
            _filePath = filePath;
        }

        private readonly List<string> _outList = new List<string>();

        private readonly List<Ast.Variable> _sofaVarList = new List<Ast.Variable>();

        private readonly List<Ast.ListVariable> _listsVariables = new List<Ast.ListVariable>();


        private string[] open_file(string filepath) {
            string[] content = File.ReadAllLines(filepath, Encoding.UTF8).ToArray();
            List<string> temp = content.ToList();

            return temp.ToArray();
        }

        //Lex
        private void Lex(string[] dataList) {
            foreach (string data in dataList) {
                if (data == "") continue;
                string j = data + "\n";
                //  j += "\n";
                if (!j.Contains(";"))
                    if (!j.Contains("{") || !j.Contains(":")) {
                        Console.WriteLine("Missing semicolon near [" + data + "]");

                        return;
                    }

                //     outInterpreter(j);
                Interpreter2(j);
            }
        }


        private void Interpreter2(string j) {
            bool isOut = false;
            bool isSofa = false;
            bool numberExpr = false;
            string tok = "";
            string result = "";
            string numbers = "";
            int state = 0; // 1 = Inside of quotes , 0 = outside of quotes

            bool isDust = false;
            bool hasEquals = false;
            string dustName = "";
            string dustValue = "";

            bool isLetterStart = false;
            bool isNop = false;
            string varName = "";
            string nopName = "";
            string nopValue = "";

            bool isList = false;
            bool listIdt = false;
            string listName = "";

            Ast.Variable var = new Ast.Variable {Type = Ast.Variable.Types.numr};

            foreach (char c in j) {
                tok += c;
                if (tok == " ") {
                    if (state == 0)
                        tok = "";
                    else
                        tok = " ";
                }

                if (tok == "tv.output" && !isOut) { //Outprints
                    isOut = true;
                    tok = "";
                }
                else if (tok == "sofa" && !isSofa) { //Assignments
                    isSofa = true;
                    tok = "";
                }

                if (isOut) {
                    if (tok == "\"") {
                        if (state == 0) {
                            state = 1;
                        }
                        else if (state == 1) {
                            _outList.Add("TEXT:" + result + "\"");
                            result = "";
                            state = 0;
                            tok = "";
                            isOut = false;
                        }
                    }
                    else if (state == 1) {
                        result += tok;
                        tok = "";
                    }

                    if (state == 0 && tok == ";") {
                        if (!numberExpr && numbers != "") {
                            _outList.Add("NUMR:" + numbers);

                            return;
                        }

                        if (numberExpr && numbers != "") {
                            _outList.Add("EXPR:" + numbers);

                            return;
                        }

                        if (isLetterStart) {
                            _outList.Add("VAR:" + varName);

                            return;
                        }

                        tok = "";
                    }
                    //Check if letter
                    else if (Regex.IsMatch(tok, @"^[a-zA-Z]+$") && state == 0 && !isLetterStart) {
                        isLetterStart = true;
                        varName += tok;
                        tok = "";
                    }
                    //Check for numbers
                    else if (tok == "0" && !isLetterStart || tok == "1" && !isLetterStart ||
                             tok == "2" && !isLetterStart || tok == "3" && !isLetterStart ||
                             tok == "4" && !isLetterStart || tok == "5" && !isLetterStart ||
                             tok == "6" && !isLetterStart || tok == "7" && !isLetterStart ||
                             tok == "8" && !isLetterStart || tok == "9" && !isLetterStart) {
                        numbers += tok;

                        tok = "";
                    }

                    else if (_ast.OperationsTokens.Contains(tok) && !isLetterStart) {
                        numberExpr = true;
                        numbers += tok;
                        tok = "";
                    }
                    else if (isLetterStart & (state == 0)) {
                        varName += tok;
                        tok = "";
                    }
                }

                else if (isSofa) {
                    if (tok == "dust" && !isDust && !isList) {
                        isDust = true;
                        tok = "";
                    }
                    else if (tok == "nop" && !isNop && !isList) {
                        isNop = true;
                        tok = "";
                    }
                    else if (tok == "list" && !isList) {
                        isList = true;
                        tok = "";
                    }

                    if (isDust) {
                        if (tok != "=" && !hasEquals) {
                            dustName += tok;
                            tok = "";
                        }

                        else if (tok == "=" && !hasEquals) {
                            hasEquals = true;
                            tok = "";
                        }
                        else if (hasEquals) {
                            if (tok == "\"") {
                                if (state == 0) {
                                    state = 1;
                                }
                                else if (state == 1) {
                                    var.Name = dustName;
                                    var.Value = dustValue + "\"";
                                    var.Type = Ast.Variable.Types.dust;

                                    _sofaVarList.Add(var);

                                    return;
                                }
                            }
                            else if (state == 1) {
                                dustValue += tok;
                                tok = "";
                            }
                            else if (tok == "tv.input") {
                                var.Name = dustName;
                                var.Value = "";
                                var.Type = Ast.Variable.Types.input;
                                _outList.Add("INPUT:" + dustName + "$");
                                _sofaVarList.Add(var);

                                return;
                            }
                        }
                    }
                    else if (isNop) {
                        if (tok != "=" && !hasEquals) {
                            nopName += tok;
                            tok = "";
                        }
                        else if (tok == "=" && !hasEquals) {
                            hasEquals = true;
                            tok = "";
                        }

                        else if (tok == ";" && hasEquals) {
                            var.Name = nopName;
                            var.Value = nopValue;

                            _sofaVarList.Add(var);

                            return;
                        }
                        else if (hasEquals) {
                            if (tok == "0" || tok == "1" || tok == "2" || tok == "3" || tok == "4" || tok == "5" ||
                                tok == "6" || tok == "7" || tok == "8" || tok == "9") {
                                nopValue += tok;
                                tok = "";
                            }
                            else if (_ast.OperationsTokens.Contains(tok)) {
                                nopValue += tok;
                                var.Type = Ast.Variable.Types.expr;
                                tok = "";
                            }
                        }
                    }
                    else if (isList) {
                        if (tok == "<" && !hasEquals && !listIdt) {
                            listIdt = true;
                            tok = "";
                        }
                        else if (tok == ">" && !hasEquals && listIdt) {
                            listIdt = false;
                            tok = "";
                        }
                        else if (tok == "=" && !hasEquals) {
                            hasEquals = true;
                            tok = "";
                        }
                        else if (hasEquals) {
                            if (tok == ";" && state == 0) {
                                var.Name = listName;
                                if (var.Type == Ast.Variable.Types.listDust) {
                                    var.Value = new List<string>();
                                    foreach (Ast.ListVariable variable in _listsVariables)
                                        ((List<string>) var.Value).Add(variable.Value as string);
                                }
                                else if (var.Type == Ast.Variable.Types.listNop) {
                                    var.Value = new List<int>();
                                    foreach (Ast.ListVariable variable in _listsVariables)
                                        ((List<int>) var.Value).Add((int) variable.Value);
                                }


                                _sofaVarList.Add(var);
                            }

                            else if (state == 0 && tok == ",") {
                                if (nopValue != "") {
                                    _listsVariables.Add(new Ast.ListVariable
                                        {Value = int.Parse(nopValue)});
                                    nopValue = "";
                                }

                                tok = "";
                            }
                            else if (var.Type == Ast.Variable.Types.listDust) {
                                if (tok == "\"") {
                                    if (state == 0) {
                                        state = 1;
                                    }
                                    else if (state == 1) {
                                        _listsVariables.Add(new Ast.ListVariable
                                            {Value = result + "\""});
                                        result = "";
                                        state = 0;
                                        tok = "";
                                    }
                                }
                                else if (state == 1) {
                                    result += tok;
                                    tok = "";
                                }
                            }
                            else if (var.Type == Ast.Variable.Types.listNop) {
                                if (tok == "0" || tok == "1" || tok == "2" || tok == "3" || tok == "4" || tok == "5" ||
                                    tok == "6" || tok == "7" || tok == "8" || tok == "9") {
                                    nopValue += tok;
                                    tok = "";
                                }
                            }
                        }
                        else if (!hasEquals && !listIdt) { //Name
                            listName += tok;
                            tok = "";
                        }
                        else if (listIdt) { //identifier
                            if (tok == "dust") {
                                var.Type = Ast.Variable.Types.listDust;
                                tok = "";
                            }
                            else if (tok == "nop") {
                                var.Type = Ast.Variable.Types.listNop;
                                tok = "";
                            }
                        }
                    }
                }
            }
        }

        private void Parser() {
            foreach (string dictionary in _outList)
                if (dictionary.Contains("TEXT:")) {
                    string toPrint = dictionary.Replace("TEXT:", "");

                    Console.WriteLine(toPrint);
                }
                else if (dictionary.Contains("EXPR:")) {
                    string toPrint = dictionary.Replace("EXPR:", "");

                    string result = new DataTable().Compute(toPrint, null).ToString();

                    Console.WriteLine(double.Parse(result));
                }
                else if (dictionary.Contains("NUMR:")) {
                    string toPrint = dictionary.Replace("NUMR:", "");

                    Console.WriteLine(toPrint);
                }
                else if (dictionary.Contains("INPUT:") && dictionary.Contains("$")) {
                    string toString = dictionary.Replace("INPUT:", "");

                    toString = toString.Replace("$", "");

                    Ast.Variable sofaVar = _sofaVarList.FirstOrDefault(p => p.Name == toString);
                    if (sofaVar != null)
                        sofaVar.Value = Console.ReadLine();
                }
                else if (dictionary.Contains("VAR:")) {
                    string varName = dictionary.Replace("VAR:", "");

                    Ast.Variable sofaVar = _sofaVarList.FirstOrDefault(p => p.Name == varName);
                    if (sofaVar != null) {
                        if (sofaVar.Type == Ast.Variable.Types.dust || sofaVar.Type == Ast.Variable.Types.numr) {
                            Console.WriteLine(sofaVar.Value);

                            continue;
                        }

                        if (sofaVar.Type == Ast.Variable.Types.input) {
                            Console.WriteLine(sofaVar.Value);

                            continue;
                        }

                        if (sofaVar.Type == Ast.Variable.Types.expr) {
                            string result = new DataTable().Compute(sofaVar.Value as string, null).ToString();
                            Console.WriteLine(double.Parse(result));

                            continue;
                        }

                        if (sofaVar.Type == Ast.Variable.Types.listDust) {
                            ((List<string>) sofaVar.Value).ForEach(p => Console.Write(p));
                            Console.WriteLine();
                        }
                    }
                }
        }

        public void Run() {
            string[] data = open_file(_filePath);
            Lex(data);
            Parser();
        }

    }

}