using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurve
{
    internal class GeneralBazesCoefFactory : ICoefMatrixFactory
    {
        private static long BinomCoefficient(long n, long k)
        {
            if (k > n) { return 0; }
            if (n == k) { return 1; } // only one way to chose when n == k
            if (k > n - k) { k = n - k; } // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
            long c = 1;
            for (long i = 1; i <= k; i++)
            {
                c *= n--;
                c /= i;
            }
            return c;
        }

        public double[,] CreateCoefMatrix(int n)
        {
            n--;
            double[,] res = new double[n + 1, n + 1];

            for (int i = 0; i < n + 1; i++)
            {
                for (int j = 0; j < n + 1; j++)
                {
                    long elem = (i + j > n) ? 0 : BinomCoefficient(n, j) * BinomCoefficient(n - j, n - (i + j)) * (((n - (i + j)) % 2 == 1) ? -1 : 1);

                    res[n - i, j] = elem;
                }
            }

            return res;
        }
    }
}
