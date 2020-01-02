using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace гравики_и_производные
{
    public interface IFunction
    {
        double get(double x);
        bool isConst();
        IFunction Simplify();
    }
}
