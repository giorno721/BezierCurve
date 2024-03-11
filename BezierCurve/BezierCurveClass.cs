using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace BezierCurve
{
    internal class BezierCurveClass
    {
        private readonly ICoefMatrixFactory _coefMatrixFactory;
        private readonly List<Point> _controlPoints;
        private Matrix<double> _coefMatrix;
        private Matrix<double> _pointMatrix;

        public BezierCurveClass(ICoefMatrixFactory coefMatrixFactory, IEnumerable<Point> controlPoints)
        {
            _coefMatrixFactory = coefMatrixFactory;
            _controlPoints = controlPoints.ToList();

            var doubleMatrix = _coefMatrixFactory.CreateCoefMatrix(_controlPoints.Count);
            _coefMatrix = Matrix<double>.Build.DenseOfArray(doubleMatrix);

            _pointMatrix = CreatePointMatrix();
        }

        public Point GetPoint(double t)
        {
            var tVector = Vector<double>.Build.Dense(_controlPoints.Count);
            for (int i = 0; i < _controlPoints.Count; i++)
            {
                tVector[i] = Math.Pow(t, i);
            }

            var point = tVector.ToRowMatrix() * _coefMatrix * _pointMatrix;
            return new Point(point[0, 0], point[0, 1]);
        }

        public Matrix<double> GetCoefMatrix() { return _coefMatrix; }

        public Matrix<double> CreatePointMatrix()
        {
            var pointMatrix = Matrix<double>.Build.Dense(_controlPoints.Count, 2);
            for (int i = 0; i < _controlPoints.Count; i++)
            {
                pointMatrix[i, 0] = _controlPoints[i].X;
                pointMatrix[i, 1] = _controlPoints[i].Y;
            }
            return pointMatrix;
        }
        public void SetControlPoints(IEnumerable<Point> controlPoints)
        {
            _controlPoints.Clear();
            _controlPoints.AddRange(controlPoints);
            _pointMatrix = CreatePointMatrix();

            _coefMatrix = Matrix<double>.Build.DenseOfArray(_coefMatrixFactory.CreateCoefMatrix(_controlPoints.Count));
        }
    }
}
