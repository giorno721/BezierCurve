using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurve
{
    internal interface ICoefMatrixFactory
    {
        public double[,] CreateCoefMatrix(int n);

    }
}
