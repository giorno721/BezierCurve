using MathNet.Numerics.LinearAlgebra;
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
        Matrix<double> coefMatrix;
        public MainWindow()
        {
            InitializeComponent();
            Points = new ObservableCollection<Point>();
            PointsDataGrid.ItemsSource = Points;
            DataContext = this;
            Points.CollectionChanged += (sender, args) => FillCanvans();
            Inited = true;
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
            new Point(MainCanvans.ActualWidth, Center.Y),
            new Point(MainCanvans.ActualWidth - 10, Center.Y - 5),
            new Point(MainCanvans.ActualWidth - 10, Center.Y + 5)
            }
            ),
                Fill = Brushes.Black
            };
            MainCanvans.Children.Add(arrowX);

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
            MainCanvans.Children.Add(arrowY);
        }

        private void DrawNames()
        {
            Point point = new(MainCanvans.ActualWidth - 20, Center.Y + 10);
            Label label = new()
            {
                Content = "X",
                Foreground = Brushes.Black
            };
            MainCanvans.Children.Add(label);
            Canvas.SetLeft(label, point.X);
            Canvas.SetTop(label, point.Y);

            point = new Point(Center.X - 20, 5);
            label = new()
            {
                Content = "Y",
                Foreground = Brushes.Black
            };
            MainCanvans.Children.Add(label);
            Canvas.SetLeft(label, point.X);
            Canvas.SetTop(label, point.Y);
        }

        private void DrawTicks()
        {
            // Позначки на осі X
            for (double x = Scale; x < Center.X; x += Scale)
            {
                if (2 * x < MainCanvans.ActualWidth - 20)
                {
                    Line tickRight = new()
                    {
                        X1 = Center.X + x,
                        Y1 = Center.Y - 5,
                        X2 = Center.X + x,
                        Y2 = Center.Y + 5,
                        Stroke = Brushes.Black
                    };
                    MainCanvans.Children.Add(tickRight);
                }

                Line tickLeft = new()
                {
                    X1 = Center.X - x,
                    Y1 = Center.Y - 5,
                    X2 = Center.X - x,
                    Y2 = Center.Y + 5,
                    Stroke = Brushes.Black
                };
                MainCanvans.Children.Add(tickLeft);
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
                MainCanvans.Children.Add(tickDown);

                if (2 * y < MainCanvans.ActualHeight - 20)
                {
                    Line tickUp = new()
                    {
                        X1 = Center.X - 5,
                        Y1 = Center.Y - y,
                        X2 = Center.X + 5,
                        Y2 = Center.Y - y,
                        Stroke = Brushes.Black
                    };
                    MainCanvans.Children.Add(tickUp);
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
                X2 = MainCanvans.ActualWidth,
                Y2 = Center.Y,
                Stroke = Brushes.Black
            };
            MainCanvans.Children.Add(xAxis);

            // Ось Y
            Line yAxis = new()
            {
                X1 = Center.X,
                Y1 = 0,
                X2 = Center.X,
                Y2 = MainCanvans.ActualHeight,
                Stroke = Brushes.Black
            };
            MainCanvans.Children.Add(yAxis);
        }

        private void AddRandomPoints()
        {
            int maxH = (int)MainCanvans.ActualHeight;
            int maxW = (int)MainCanvans.ActualWidth;
            Random random = new Random();
            for (int i = 0; i < 7; i++)
            {
                Point randomPoint = new Point(random.Next(0, maxW), random.Next(0, maxH));
                Point rawVirtualPoint = ScreenCoordsToVirtualCords(randomPoint);
                Point virtualPoint = new Point(Math.Round(rawVirtualPoint.X, 2), Math.Round(rawVirtualPoint.Y, 2));
                Points.Add(virtualPoint);
            }
        }

        private void FillCanvans()
        {
            MainCanvans.Children.Clear();
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
            (Point[] virtualPoints,coefMatrix) = BezierCurveWindowHelper.CalculatePoints(HyperLines ? 1000 : numPoints, Points);
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
                    Fill = Brushes.Red
                };
                MainCanvans.Children.Add(ellipse);
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
                MainCanvans.Children.Add(line);
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
                MainCanvans.Children.Add(line);
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
                MainCanvans.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, screenPoint.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, screenPoint.Y - ellipse.Height / 2);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Center = new Point(MainCanvans.ActualWidth / 2, MainCanvans.ActualHeight / 2);
            AddRandomPoints();
            DrawAxes();
        }
        private Point VirtualCoordsToScreenCords(Point vitualPoint)
        {
            return new Point(Center.X + vitualPoint.X * Scale,Center.Y + vitualPoint.Y * Scale);
        }
        private Point ScreenCoordsToVirtualCords(Point screenPoint)
        {
            return new Point((screenPoint.X - Center.X) / Scale, (screenPoint.Y - Center.Y) / Scale);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Points.Clear();
            MainCanvans.Children.Clear();
            AddRandomPoints();
        }

        private void AddNewPoint(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(xCoordinate.Text, out double x) && double.TryParse(YCoordinate.Text, out double y))
            {

                Points.Add(new Point { X = x, Y = -y });

            }
            else
            {
                MessageBox.Show("Неправильний ввід!");
            }
        }

        private void TabulateClick(object sender, RoutedEventArgs e)
        {
            TabulateWindow tabulateWindow = new TabulateWindow(Points);
            tabulateWindow.Show();
        }

        private void InformationClick(object sender, RoutedEventArgs e)
        {
           InformationWindow informationWindow = new InformationWindow(coefMatrix);
            informationWindow.Show();
        }
    }
}
