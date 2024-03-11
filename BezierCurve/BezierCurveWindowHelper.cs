using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BezierCurve
{
    internal class BezierCurveWindowHelper
    {
        public static (Point[], Matrix<double>) CalculatePoints(int numOfPoints, IEnumerable<Point> controlPoints)
        {
            return CalculatePoints(numOfPoints, controlPoints, 0, 1);
        }
        public static (Point[], Matrix<double>) CalculatePoints(int numOfPoints, IEnumerable<Point> controlPoints, double start, double end)
        {
            BezierCurveClass bezierCurve = new(new GeneralBazesCoefFactory(), controlPoints);
            var points = new Point[numOfPoints];
            double step = (end - start) / (numOfPoints - 1);
            for (int i = 0; i < numOfPoints; i++)
            {
                points[i] = bezierCurve.GetPoint(start + i * step);
            }
            return (points, bezierCurve.GetCoefMatrix());
        }

    }
}
