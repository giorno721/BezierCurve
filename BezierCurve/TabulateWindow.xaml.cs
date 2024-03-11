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
    /// Interaction logic for TabulateWindow.xaml
    /// </summary>
    public partial class TabulateWindow : Window
    {
        IEnumerable<Point> Points;
        Matrix<double> coefMatrix;
        public TabulateWindow(IEnumerable<Point> points)
        {
            Points = points;
            InitializeComponent();
        }
        private void TabulateButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(IntervalsTextBox.Text, out int intervals) &&
                double.TryParse(IntervalTextBox.Text.Split(' ')[0], out double start) &&
                double.TryParse(IntervalTextBox.Text.Split(' ')[1], out double end))
            {
                StringBuilder result = new();
                (var points, coefMatrix) = BezierCurveWindowHelper.CalculatePoints(intervals, Points, start, end);
                double step = (end - start) / (intervals - 1);
                for (int i = 0; i < intervals; i++)
                {
                    result.AppendLine($"t = {start + i * step} x = {points[i].X} y = {points[i].Y}");
                }

                ResultTextBlock.Text = result.ToString();
            }
            else
            {
                ResultTextBlock.Text = "Invalid input";
            }

        }
    }
}
