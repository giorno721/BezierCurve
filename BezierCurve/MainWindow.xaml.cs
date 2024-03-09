using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BezierCurve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Point> Points { get; set; }
        bool Inited = false;
        private bool HyperLines = false;
        public MainWindow()
        {
            InitializeComponent();
            Points = new ObservableCollection<Point>();
            PointsDataGrid.ItemsSource = Points;
            DataContext = this;
            Points.CollectionChanged += (sender, args) => FillCanvans();
            Inited = true;
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddCoordinateGrid();
        }
        private void AddRandomPoints()
        {
            int maxH = (int)canvas.ActualHeight;
            int maxW = (int)canvas.ActualWidth;
            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                Points.Add(new Point(random.Next(0, maxW), random.Next(0, maxH)));
            }
        }
        private void FillCanvans()
        {
            canvas.Children.Clear();
            AddCoordinateGrid();
            DrawPoints();
            DrawLines();
            DrawCurve();

        }
        private void DrawCurve()
        {
            if (Points.Count < 3)
            {
                return;
            }
            int numPoints = 50;
            Point[] points = BezierCurveWindowHelper.CalculatePoints(HyperLines ? 1000 : numPoints, Points);
            DrawCurveLine(points);
        }

        private void DrawCurveLine(Point[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                Line line = new()
                {
                    X1 = points[i - 1].X,
                    Y1 = points[i - 1].Y,
                    X2 = points[i].X,
                    Y2 = points[i].Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = 3
                };
                canvas.Children.Add(line);
            }
        }
        private void DrawLines()
        {
            for (int i = 1; i < Points.Count; i++)
            {
                Line line = new Line
                {
                    X1 = Points[i - 1].X,
                    Y1 = Points[i - 1].Y,
                    X2 = Points[i].X,
                    Y2 = Points[i].Y,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }
        }
        private void DrawPoints()
        {
            foreach (var point in Points)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Black
                };
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
            }
        }
        private void GenerateRandomBezierCurve(object sender, RoutedEventArgs e)
        {
            Points.Clear();
            canvas.Children.Clear();
            AddRandomPoints();
        }
        private void AddCoordinateGrid()
        {
            // Vertical lines (X-axis)
            for (double x = 100; x <= canvas.ActualWidth; x += 50) // Змінено крок на 50
            {
                Line line = new Line
                {
                    X1 = x,
                    Y1 = canvas.ActualHeight - 100, // Змінено на висоту площини мінус 100
                    X2 = x,
                    Y2 = canvas.ActualHeight - 90, // Змінено на висоту площини мінус 90 (змініть, якщо потрібно)
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }

            // Horizontal lines (Y-axis)
            for (double y = canvas.ActualHeight - 100; y >= 0; y -= 50) // Змінено крок на 50
            {
                Line line = new Line
                {
                    X1 = 100, // Змінено на ширину площини
                    Y1 = y,
                    X2 = 110, // Змінено на ширину площини плюс 10 (змініть, якщо потрібно)
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }
        }


    }
}
