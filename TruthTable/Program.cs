using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruthTable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 200;
            //declare variables
            string expression;
            bool stop = false;
            Console.WriteLine("\t\tTRUTH TABLE GENERATOR");
            Console.WriteLine("\t\tBy Nathan Beattie");
            Console.WriteLine("Type \"guide\" for instructions on how to use this guide, \"exit\" to quit, or type your expression below");
            //get input
            while (!stop)
            {
                Console.Write("Please enter an expression: ");
                expression = Console.ReadLine();
                if (expression == "guide")
                {
                    DisplayGuide();
                }
                else if (expression == "exit")
                {
                    return;
                }
                else
                {
                    Table truthTable = new Table();
                    LogicExpression logEx = new LogicExpression(expression, 0, truthTable);
                    truthTable.TestStuff();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Would you like to test another expression? (y for yes, anything else for no)");
                    string s = Console.ReadLine();
                    if (s != "y" && s != "Y")
                    {
                        stop = true;
                    }
                }
            }
        }

        static void DisplayGuide()
        {
            Console.Clear();
            Console.WriteLine("\tHOW TO USE THIS TRUTH TABLE");
            Console.WriteLine();
            Console.WriteLine("\t\tSYBMOLS\n");
            Console.WriteLine("T\tTRUE");
            Console.WriteLine("F\tFALSE");
            Console.WriteLine("~\tNOT");
            Console.WriteLine("v\tAND");
            Console.WriteLine("u\tOR");
            Console.WriteLine(">\tIF");
            Console.WriteLine("=\tIF AND ONLY IF");
            Console.WriteLine("( )\tBRACKETS\n");
            Console.WriteLine("TO USE '~' CORRECTLY, PLACE THE NOT IN BRACKETS WITH THE VARIABLE YOU WISH TO INVERT");
            Console.WriteLine("ie. ~pvq WILL RETURN 'NOT (p AND q)', WHEREAS (~p)vq WILL RETURN 'NOT p AND q");
        }
    }

    class LogicExpression
    {
        private Table truthTable;
        public LogicExpression(string exp, int level, Table table)
        {
            _Expression = exp;
            _Level = level;
            truthTable = table;
            truthTable.AddBranch(this);

            ParseExpression();
        }

        private string _Expression;
        public string Expression
        {
            get { return _Expression; }
            set { _Expression = value; }
        }

        private int _Level;
        public int Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        
        private bool _IsVariable;
        public bool IsVariable
        {
            get { return _IsVariable; }
        }

        private bool exp1Result = false;
        public bool Exp1Result
        {
            get { return exp1Result; }
            set { exp1Result = value; }
        }
        private bool exp2Result = false;
        public bool Exp2Result
        {
            get { return exp2Result; }
            set { exp2Result = value; }
        }

        public void ParseExpression()
        {
            if (_Expression[0] == '(' && _Expression[_Expression.Length - 1] == ')')
            {
                _Expression = _Expression.Substring(1, _Expression.Length - 2);
            }
            bool _Brackets = false;
            for (int x = 0; x < _Expression.Length; x++)
            {
                if (_Expression[x] == '(')
                {
                    if (exp1 == null)
                    {
                        exp1 = _Expression.Substring(x + 1, CloseBracketCalculation(_Expression.Substring(x + 1)) - (x));
                    }
                    else
                    {
                        exp2 = _Expression.Substring(x + 1, CloseBracketCalculation(_Expression.Substring(x + 1)));
                    }
                    x = CloseBracketCalculation(_Expression.Substring(_Expression.IndexOf('(') + 1));
                    _Brackets = true;
                }
                else if (_Expression[x] == 'u' || _Expression[x] == 'v' || _Expression[x] == '>' || _Expression[x] == '~' || _Expression[x] == '=') //
                {
                    {
                        if (_Expression[x] == 'u')
                        {
                            modifier = 'u';
                            SetChildren(x);
                            break;
                        }
                        else if (_Expression[x] == 'v')
                        {
                            modifier = 'v';
                            SetChildren(x);
                            break;
                        }
                        else if (_Expression[x] == '>')
                        {
                            modifier = '>';
                            SetChildren(x);
                            break;
                        }
                        else if (_Expression[x] == '=')
                        {
                            modifier = '=';
                            SetChildren(x);
                            break;
                        }
                        else if (_Expression[x] == '~')
                        {
                            modifier = '~';
                            
                            if (exp1 == null)
                            {
                                exp1 = _Expression.Substring(1, _Expression.Length - 1);
                            }
                            else
                            {
                                exp2 = _Expression.Substring(1, _Expression.Length - 1);
                            }
                            
                            IsNot = true;
                            break;
                        }
                    }
                }
            }
            if (modifier == 'A' && _Brackets == false)
            {
                exp1 = _Expression;
                _IsVariable = true;
                if (_Expression != "T" && _Expression != "F")
                {
                    truthTable.AddVariable(exp1);
                }
            }

            if (!_IsVariable)
            {
                Part1 = new LogicExpression(exp1, _Level + 1, truthTable);
                if (!IsNot)
                {
                    Part2 = new LogicExpression(exp2, _Level + 1, truthTable);
                }
            }
            
        }
        private void SetChildren(int index)
        {
            if (exp1 == null)
            {
                exp1 = _Expression.Substring(0, index);
            }
            if (exp2 == null)
            {
                exp2 = _Expression.Substring(index + 1);
            }
        }

        private int CloseBracketCalculation(string exp)
        {
            int endBracket = 1;
            int index = 0;
            for (int i = 0; i < exp.Length; i++)
            {
                if (exp[i] == '(')
                {
                    endBracket++;
                }
                else if (exp[i] == ')')
                {
                    endBracket--;
                }
                if (endBracket == 0)
                {
                    index = i;
                    return index;
                }
            }
            return index = 0;
        }

        public bool Evaluate()
        {
            if(Part1 != null)
                exp1Result = Part1.Evaluate();
            if(Part2 != null)
                exp2Result = Part2.Evaluate();
            return LogicRules(modifier);
        }

        public bool LogicRules(char mod)
        {
            //1 variable
            if (mod == 'A')
            {
                    //true
                if (exp1Result == true)
                {
                    return true;
                }
                    //false
                else
                {
                    return false;
                }
            }
            //1 variable with NOT
            if (mod == '~')
            {
                return !exp1Result;
            }

            //AND
            if (mod == 'v')
            {
                    //Both true
                if (exp1Result == true && exp2Result == true)
                {
                    return true;
                }
                    //one or less true
                else
                {
                    return false;
                }
            }
            //OR
            if (mod == 'u')
            {
                if (exp1Result == false && exp2Result == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            //IF
            if (mod == '>')
            {
                if (exp1Result == false)
                {
                    return true;
                }
                else if (exp1Result == true && exp2Result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //If and only IF
            if (mod == '=')
            {
                if (exp1Result == true && exp2Result == true)
                {
                    return true;
                }
                else if (exp1Result == false && exp2Result == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            return false;
        }

        private string exp1 = null;
        private string exp2 = null;
        private char modifier = 'A';
        private bool IsNot = false;
        LogicExpression Part1 = null;
        LogicExpression Part2 = null;
    }

    class Table{
        private string[] varNames = new string[5];
        private bool[] varTrueFalse = new bool[5];
        private List<LogicExpression> ExpressionList = new List<LogicExpression>();
        private bool exists = false;
        private int varNumber = 0;

        public void AddVariable(string var)
        {
            exists = false;
            foreach (string s in varNames)
            {
                if (s == var)
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                varNames[varNumber] = var;
                varNumber++;
            }
        }

        public void AddBranch(LogicExpression l)
        {
            ExpressionList.Add(l);
        }

        public void CreateTable()
        {
            
            foreach (string s in varNames)
            {
                if(s != null)
                Console.WriteLine(s);
            }
        }

        public void TestStuff()
        {
            bool tableSetUp = false;
            //set variables to false
            varTrueFalse[0] = false;
            varTrueFalse[1] = false;
            varTrueFalse[2] = false;
            varTrueFalse[3] = false;
            varTrueFalse[4] = false;

            Console.WriteLine();
            for (int i = 0; i < Math.Pow(2, varNumber); i++)
            {
                //flip T/F at appropriate time
                varTrueFalse[0] = !varTrueFalse[0];
                if (i % 2 == 0)
                {
                    varTrueFalse[1] = !varTrueFalse[1];
                }
                if (i % 4 == 0)
                {
                    varTrueFalse[2] = !varTrueFalse[2];
                }
                if (i % 8 == 0)
                {
                    varTrueFalse[3] = !varTrueFalse[3];
                }
                if (i % 16 == 0)
                {
                    varTrueFalse[4] = !varTrueFalse[4];
                }

                int level = 0;
                //change the necessary variable, then evaluate
                foreach (LogicExpression l in ExpressionList)
                {
                    if (l.IsVariable)
                    {
                        if (l.Level > level)
                        {
                            level = l.Level;
                        }
                        if (varNumber > 0)
                        {
                            for (int j = 0; j < varNumber; j++)
                            {
                                if (l.Expression == varNames[j])
                                {
                                    l.Exp1Result = varTrueFalse[j];
                                }
                                else if (l.Expression == "T")
                                {
                                    l.Exp1Result = true;
                                }
                                else if (l.Expression == "F")
                                {
                                    l.Exp1Result = false;
                                }
                            }
                        }
                        else
                        {
                            if (l.Expression == "T")
                            {
                                l.Exp1Result = true;
                            }
                            else if (l.Expression == "F")
                            {
                                l.Exp1Result = false;
                            }
                        }
                    }
                    
                }

                List<string> usedVarNames = new List<string>(); //ensures that variables that appear multiple times in the expression are not written multiple times
                //setup table columns

                if (!tableSetUp)
                {
                    for (int k = level; k >= 0; k--)
                    {
                        foreach (LogicExpression l in ExpressionList)
                        {
                            if (l.Level == k)
                            {
                                if (l.IsVariable && usedVarNames.Contains(l.Expression) == true)
                                {
                                }
                                else if (l.IsVariable && usedVarNames.Contains(l.Expression) == false)
                                {
                                    usedVarNames.Add(l.Expression);
                                    Console.Write("{0}\t|\t", l.Expression.ToString());
                                }
                                else
                                {
                                    Console.Write("{0}\t|\t", l.Expression.ToString());
                                }
                            }
                        }
                    }
                    tableSetUp = true;
                }

                usedVarNames = new List<string>();
                Console.WriteLine();
                Console.WriteLine(new string('-', 200));
                

                //populate table
                for (int k = level; k >= 0; k--)
                {
                    foreach (LogicExpression l in ExpressionList)
                    {
                        if (l.Level == k)
                        {
                            if (l.IsVariable && usedVarNames.Contains(l.Expression) == true)
                            {  
                            }
                            else if (l.IsVariable && usedVarNames.Contains(l.Expression) == false)
                            {
                                usedVarNames.Add(l.Expression);
                                Console.Write("{0}\t|\t", l.Evaluate().ToString());                                
                            }
                            else
                            {
                                Console.Write("{0}\t|\t", l.Evaluate().ToString());   
                            }
                        }
                    }
                }
            }
            
        }

    }
}
