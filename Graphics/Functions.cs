using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace гравики_и_производные
{
    namespace basic
    {
        public class Line : IFunction
        {
            public double get(double x)
            {
                return x;
            }
            public bool isConst()
            {
                return false;
            }
            public IFunction Simplify() { return this; }
        }
        public class Const : IFunction
        {
            public double c;
            public Const(double c)
            {
                this.c = c;
            }
            public double get(double x)
            {
                return c;
            }
            public bool isConst()
            {
                return true;
            }
            public IFunction Simplify() { return this; }
        }
        public class Summ : IFunction
        {
            public IFunction f1;
            public IFunction f2;
            public Summ(IFunction f1, IFunction f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
            public double get(double x)
            {
                return f1.get(x) + f2.get(x);
            }
            public bool isConst()
            {
                return f1.isConst() && f2.isConst();
            }
            public IFunction Simplify() 
            {
                f1 = f1.Simplify();
                f2 = f2.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Diff : IFunction
        {
            public IFunction f1;
            public IFunction f2;
            public Diff(IFunction f1, IFunction f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
            public double get(double x)
            {
                return f1.get(x) - f2.get(x);
            }
            public bool isConst()
            {
                return f1.isConst() && f2.isConst();
            }
            public IFunction Simplify()
            {
                f1 = f1.Simplify();
                f2 = f2.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Mult : IFunction
        {
            public IFunction f1;
            public IFunction f2;
            public Mult(IFunction f1, IFunction f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
            public double get(double x)
            {
                return f1.get(x) * f2.get(x);
            }
            public bool isConst()
            {
                return f1.isConst() && f2.isConst();
            }
            public IFunction Simplify()
            {
                f1 = f1.Simplify();
                f2 = f2.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Share : IFunction
        {
            public IFunction f1;
            public IFunction f2;
            public Share(IFunction f1, IFunction f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
            public double get(double x)
            {
                return f1.get(x) / f2.get(x);
            }
            public bool isConst()
            {
                return f1.isConst() && f2.isConst();
            }
            public IFunction Simplify()
            {
                f1 = f1.Simplify();
                f2 = f2.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Pow : IFunction
        {
            public IFunction f1;
            public IFunction f2;
            public Pow(IFunction f1, IFunction f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
            public double get(double x)
            {
                return Math.Pow(f1.get(x), f2.get(x));
            }
            public bool isConst()
            {
                return f1.isConst() && f2.isConst();
            }
            public IFunction Simplify()
            {
                f1 = f1.Simplify();
                f2 = f2.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Exp : IFunction
        {
            public IFunction f;
            public Exp(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Exp(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Ln : IFunction
        {
            public IFunction f;
            public Ln(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Log(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Sin : IFunction
        {
            public IFunction f;
            public Sin(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Sin(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Cos : IFunction
        {
            public IFunction f;
            public Cos(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Cos(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Tg : IFunction
        {
            public IFunction f;
            public Tg(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Tan(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Arcsin : IFunction
        {
            public IFunction f;
            public Arcsin(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Asin(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Arccos : IFunction
        {
            public IFunction f;
            public Arccos(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Acos(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Arctg : IFunction
        {
            public IFunction f;
            public Arctg(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                return Math.Atan(f.get(x));
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class Sgn : IFunction
        {
            public IFunction f;
            public Sgn(IFunction f)
            {
                this.f = f;
            }
            public double get(double x)
            {
                if (f.get(x) < 0) return -1;
                if (f.get(x) > 0) return 1;
                return 0;
            }
            public bool isConst()
            {
                return f.isConst();
            }
            public IFunction Simplify()
            {
                f = f.Simplify();
                if (isConst()) return new Const(get(0));
                return this;
            }
        }
        public class NaN : IFunction
        {
            public double get(double x)
            {
                return Double.NaN;
            }
            public bool isConst()
            {
                return true;
            }
            public IFunction Simplify()
            {
                return this;
            }
        }
    }
}
