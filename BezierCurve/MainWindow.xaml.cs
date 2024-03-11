using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private double Scale = 10;
        private Point Center;
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
            Center = new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2);
            AddRandomPoints();
            FillCanvans();
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
            DrawAxes();
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
            int numPoints = 100;
            Point[] virtualPoints = BezierCurveWindowHelper.CalculatePoints(HyperLines ? 1000 : numPoints, Points);
            Point[] screenPoints = virtualPoints.Select(VirtualCoordsToScreenCords).ToArray();
            DrawCurveLine(screenPoints);
            DrawCurvePoints(screenPoints);
        }

        private void DrawCurvePoints(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 3,
                    Height = 3,
                    Fill = Brushes.Black
                };
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, points[i].X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, points[i].Y - ellipse.Height / 2);
            }
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
                Point screenPoint1 = VirtualCoordsToScreenCords(Points[i - 1]);
                Point screenPoint2 = VirtualCoordsToScreenCords(Points[i]);
                Line line = new Line
                {
                    X1 = screenPoint1.X,
                    Y1 = screenPoint1.Y,
                    X2 = screenPoint2.X,
                    Y2 = screenPoint2.Y,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }
        }

        private void DrawPoints()
        {
            foreach (var virtualPoint in Points)
            {
                Point screenPoint = VirtualCoordsToScreenCords(virtualPoint);
                Ellipse ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Black
                };
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, screenPoint.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, screenPoint.Y - ellipse.Height / 2);
            }
        }
        private void GenerateRandomBezierCurve(object sender, RoutedEventArgs e)
        {
            Points.Clear();
            canvas.Children.Clear();
            AddRandomPoints();

        }
        private void DrawAxes()
        {
            DrawXYLines();
            DrawTicks();
            DrawNames();
            DrawArrows();
        }

        private void DrawArrows()
        {
            // Стрілочка на осі X
            Polygon arrowX = new()
            {
                Points = new PointCollection(new Point[]
                {
            new Point(canvas.ActualWidth, Center.Y),
            new Point(canvas.ActualWidth - 10, Center.Y - 5),
            new Point(canvas.ActualWidth - 10, Center.Y + 5)
            }
            ),
                Fill = Brushes.Black
            };
            canvas.Children.Add(arrowX);

            // Стрілочка на осі Y
            Polygon arrowY = new Polygon
            {
                Points = new PointCollection(new Point[] {
            new Point(Center.X, 0),
            new Point(Center.X - 5, 10),
            new Point(Center.X + 5, 10)
            }),
                Fill = Brushes.Black
            };
            canvas.Children.Add(arrowY);
        }

        private void DrawNames()
        {
            Point point = new(canvas.ActualWidth - 20, Center.Y + 10);
            Label label = new()
            {
                Content = "X",
                Foreground = Brushes.Black
            };
            canvas.Children.Add(label);
            Canvas.SetLeft(label, point.X);
            Canvas.SetTop(label, point.Y);

            point = new Point(Center.X - 20, 5);
            label = new()
            {
                Content = "Y",
                Foreground = Brushes.Black
            };
            canvas.Children.Add(label);
            Canvas.SetLeft(label, point.X);
            Canvas.SetTop(label, point.Y);
        }

        private void DrawTicks()
        {
            // Позначки на осі X
            for (double x = Scale; x < Center.X; x += Scale)
            {   
                if (2 * x < canvas.ActualWidth - 20)
                {
                    Line tickRight = new()
                    {
                        X1 = Center.X + x,
                        Y1 = Center.Y - 5,
                        X2 = Center.X + x,
                        Y2 = Center.Y + 5,
                        Stroke = Brushes.Black
                    };
                    canvas.Children.Add(tickRight);
                }

                Line tickLeft = new()
                {
                    X1 = Center.X - x,
                    Y1 = Center.Y - 5,
                    X2 = Center.X - x,
                    Y2 = Center.Y + 5,
                    Stroke = Brushes.Black
                };
                canvas.Children.Add(tickLeft);
            }



            // Позначки на осі Y
            for (double y = Scale; y < Center.Y; y += Scale)
            {
                Line tickDown = new()
                {
                    X1 = Center.X - 5,
                    Y1 = Center.Y + y,
                    X2 = Center.X + 5,
                    Y2 = Center.Y + y,
                    Stroke = Brushes.Black
                };  
                canvas.Children.Add(tickDown);

                if (2 * y < canvas.ActualHeight - 20)
                {
                    Line tickUp = new()
                    {
                        X1 = Center.X - 5,
                        Y1 = Center.Y - y,
                        X2 = Center.X + 5,
                        Y2 = Center.Y - y,
                        Stroke = Brushes.Black
                    };
                    canvas.Children.Add(tickUp);
                }
            }
        }

        private void DrawXYLines()
        {
            // Ось X
            Line xAxis = new()
            {
                X1 = 0,
                Y1 = Center.Y,
                X2 = canvas.ActualWidth,
                Y2 = Center.Y,
                Stroke = Brushes.Black
            };
            canvas.Children.Add(xAxis);

            // Ось Y
            Line yAxis = new()
            {
                X1 = Center.X,
                Y1 = 0,
                X2 = Center.X,
                Y2 = canvas.ActualHeight,
                Stroke = Brushes.Black
            };
            canvas.Children.Add(yAxis);
        }


        private Point VirtualCoordsToScreenCords(Point vitualPoint)
        {
            return new Point(Center.X + vitualPoint.X * Scale, Center.Y + vitualPoint.Y * Scale);
        }

    }
}
