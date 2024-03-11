using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BezierCurve
{
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        public Matrix<double>  CoefMatrix;
        public InformationWindow(Matrix<double> coefMatrix)
        {
            InitializeComponent();
            CoefMatrix = coefMatrix;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder result = new StringBuilder();
            int zeroCount = 0;
            double firstRowSum = 0;
            double firstColumnSum = 0;

            for (int i = 0; i < CoefMatrix.RowCount; i++)
            {
                for (int j = 0; j < CoefMatrix.ColumnCount; j++)
                {
                    double value = CoefMatrix[i, j];
                    result.Append($"{value} ");

                    if (Math.Abs(value) < 1e-10) // Assume zero if value is very close to zero
                        zeroCount++;

                    if (i == 0)
                        firstRowSum += value;

                    if (j == 0)
                        firstColumnSum += value;
                }
                result.AppendLine();
            }

            // Append zero count information
            result.AppendLine($"Число нульових елементів: {zeroCount}");

            // Append sum of elements in the first row and column
            result.AppendLine($"Сума елементів першого рядка: {firstRowSum}");
            result.AppendLine($"Сума елементів першої колонки: {firstColumnSum}");

            ResultTextBlock.Text = result.ToString();
        }
    }
}
