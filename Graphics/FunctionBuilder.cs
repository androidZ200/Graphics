using System;
using System.Collections.Generic;
using System.Text;

namespace гравики_и_производные
{
    public sealed class FunctionBuilder
    {
        private FunctionBuilder() { }
        private class MathObject
        {
            public Type type;
            public string text;
            public MathObject(Type type, string text)
            {
                this.type = type;
                this.text = text;
            }
        }

        private static string[] keyWords = { "sin", "cos", "tg", "ln", "arcsin", "arccos", "arctg", "sgn" };
        private static char[] operators = { '+', '-', '*', '/', '^' };
        private enum Type { Const, KeyWord, X, Operator, Open, Close }

        private static int[,] Table =
        {
            { 1,  1,  3,  3,  5,  5, 10},   // 0-9
            { 6,  4, -1,  4, -1,  9,  6},   // E
            { 6,  9, -1,  9, -1,  9,  6},   // a-z
            { 7,  9, -1,  9,  5,  9, 10},   // +-
            {-1,  2, -1, -1, -1, -1, -1},   // .
            { 8,  9, -1,  9, -1,  9, 10},   // ()
            { 7,  9, -1,  9, -1,  9, 10}    // */^
        };

        public static IFunction Create(string text)
        {
            var line = Read(text);
            if (line.Length == 0) throw new Exception();
            IFunction f = operatorLow(new Iterator(line));
            f = f.Simplify();
            return f;
        }
        private static int Converter(char symbol)
        {
            if (symbol >= '0' && symbol <= '9') return 0;
            if (symbol == 'E') return 1;
            if (symbol >= 'a' && symbol <= 'z' || symbol >= 'A' && symbol <= 'Z') return 2;
            if (symbol == '+' || symbol == '-') return 3;
            if (symbol == '.') return 4;
            if (symbol == '(' || symbol == ')') return 5;
            if (symbol == '*' || symbol == '/' || symbol == '^') return 6;
            return -1;
        }
        private static double Conv(string text)
        {
            if (text == "e") return Math.E;
            if (text == "pi") return Math.PI;
            return Convert.ToDouble(text);
        }
        private static bool ChecKeyWords(string text)
        {
            foreach (var x in keyWords)
                if (x == text)
                    return true;
            return false;
        }
        private static MathObject[] Read(string text)
        {
            List<MathObject> obj = new List<MathObject>();
            int state = 0;
            StringBuilder currentText = new StringBuilder();
            int index = 0;
            do
            {
                int convertSymbol = Converter(text[index]);
                if (convertSymbol == -1) throw new Exception();
                state = Table[convertSymbol, state];
                if (state == -1) throw new Exception();
                switch (state)
                {
                    case 7:
                        currentText.Append(text[index]);
                        obj.Add(new MathObject(Type.Operator, currentText.ToString()));
                        ++index;
                        break;
                    case 8:
                        currentText.Append(text[index]);
                        string t = currentText.ToString();
                        if(t == "(") obj.Add(new MathObject(Type.Open, t));
                        else obj.Add(new MathObject(Type.Close, t));
                        ++index;
                        break;
                    case 9:
                        obj.Add(new MathObject(Type.Const, currentText.ToString()));
                        break;
                    case 10:
                        t = currentText.ToString();
                        if(ChecKeyWords(t)) obj.Add(new MathObject(Type.KeyWord, t));
                        else if (t == "e" || t == "pi") obj.Add(new MathObject(Type.Const, t));
                        else if (t == "x") obj.Add(new MathObject(Type.X, t));
                        else throw new Exception();
                        break;
                    default:
                        currentText.Append(text[index]);
                        ++index;
                        break;
                }
                if(state > 6)
                {
                    state = 0;
                    currentText.Clear();
                }
            } while (index < text.Length);

            if(currentText.Length > 0)
            {
                if (state == 4 || state == 2) throw new Exception();
                if (state < 6) obj.Add(new MathObject(Type.Const, currentText.ToString()));
                else if (state == 6)
                {
                    string t = currentText.ToString();
                    foreach (var x in keyWords)
                        if (x == t)
                            obj.Add(new MathObject(Type.KeyWord, t));
                    if (t == "e" || t == "pi") obj.Add(new MathObject(Type.Const, t));
                    else if (t == "x") obj.Add(new MathObject(Type.X, t));
                    else throw new Exception();
                }
            }
            return obj.ToArray();
        }

        private static IFunction operatorLow(Iterator enumerator)
        {
            IFunction f1 = operatorHigh(enumerator);
            while(!enumerator.isEnd() && (enumerator.Current.text == "+" || enumerator.Current.text == "-"))
            {
                bool isPlus = enumerator.Current.text == "+";
                enumerator.MoveNext();
                if (enumerator.isEnd()) throw new Exception();
                IFunction f2 = operatorHigh(enumerator);
                if (isPlus) f1 = new basic.Summ(f1, f2);
                else f1 = new basic.Diff(f1, f2);
            }
            return f1;
        }
        private static IFunction operatorHigh(Iterator enumerator)
        {
            IFunction f1 = operatorPow(enumerator);
            while (!enumerator.isEnd() && (enumerator.Current.text == "*" || enumerator.Current.text == "/"))
            {
                bool isMult = enumerator.Current.text == "*";
                enumerator.MoveNext();
                if (enumerator.isEnd()) throw new Exception();
                IFunction f2 = operatorPow(enumerator);
                if (isMult) f1 = new basic.Mult(f1, f2);
                else f1 = new basic.Share(f1, f2);
            }
            return f1;
        }
        private static IFunction operatorPow(Iterator enumerator)
        {
            IFunction f1 = operatorMult(enumerator);
            if(!enumerator.isEnd() && enumerator.Current.text == "^")
            {
                enumerator.MoveNext();
                if (enumerator.isEnd()) throw new Exception();
                IFunction f2 = operatorMult(enumerator);
                f1 = new basic.Pow(f1, f2);
            }
            return f1;
        }
        private static IFunction operatorMult(Iterator enumerator)
        {
            if (enumerator.Current.type == Type.Const)
            {
                var t = enumerator.clone();
                enumerator.MoveNext();
                if (enumerator.isEnd() || enumerator.Current.type == Type.Operator ||
                    enumerator.Current.type == Type.Close) return new basic.Const(Conv(t.Current.text));
                else
                {
                    IFunction f2 = operatorHigh(enumerator);
                    return new basic.Mult(new basic.Const(Conv(t.Current.text)), f2);
                }
            }
            else return operatorKeyWords(enumerator);
        }
        private static IFunction operatorKeyWords(Iterator enumerator)
        {
            if (enumerator.Current.type == Type.KeyWord)
            {
                var t = enumerator.clone();
                enumerator.MoveNext();
                if (enumerator.isEnd()) throw new Exception();
                switch (t.Current.text)
                {
                    case "sin":
                        return new basic.Sin(operatorBrackets(enumerator));
                    case "cos":
                        return new basic.Cos(operatorBrackets(enumerator));
                    case "tg":
                        return new basic.Tg(operatorBrackets(enumerator));
                    case "ln":
                        return new basic.Ln(operatorBrackets(enumerator));
                    case "arcsin":
                        return new basic.Arcsin(operatorBrackets(enumerator));
                    case "arccos":
                        return new basic.Arccos(operatorBrackets(enumerator));
                    case "arctg":
                        return new basic.Arctg(operatorBrackets(enumerator));
                    case "sgn":
                        return new basic.Sgn(operatorBrackets(enumerator));
                }
            }
            return operatorBrackets(enumerator);
        }
        private static IFunction operatorBrackets(Iterator enumerator)
        {
            if(enumerator.Current.type == Type.Open)
            {
                enumerator.MoveNext();
                if (enumerator.isEnd()) throw new Exception();
                var f1 = operatorLow(enumerator);
                if (!enumerator.isEnd() && enumerator.Current.type != Type.Close) throw new Exception();
                enumerator.MoveNext();
                return f1;
            }
            var t = enumerator.clone();
            enumerator.MoveNext();
            if (t.Current.type == Type.X) return new basic.Line();
            if (t.Current.type == Type.Const) return new basic.Const(Conv(t.Current.text));
            if (t.Current.text == "-") return new basic.Mult(new basic.Const(-1), operatorHigh(enumerator));
            throw new Exception();
        }


        private class Iterator
        {
            private MathObject[] array;
            private int index;

            public Iterator(MathObject[] mathObjects)
            {
                array = mathObjects;
                index = 0;
            }
            public MathObject Current { get { return array[index]; } set { array[index] = value; } }
            public bool MoveNext()
            {
                if (isEnd()) return false;
                index++;
                if (isEnd()) return false;
                else return true;
            }
            public bool MoveBack()
            {
                if (index == 0) return false;
                index--;
                return true;
            }
            public bool isEnd()
            {
                return index == array.Length;
            }
            public Iterator clone()
            {
                Iterator t = new Iterator(array);
                t.index = index;
                return t;
            }
        }
    }
}
