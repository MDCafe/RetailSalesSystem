using LiveCharts;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailManagementSystem.View.Graphs
{
    /// <summary>
    /// Interaction logic for Graphs.xaml
    /// </summary>
    public partial class Graphs : UserControl
    {
        //private ZoomingOptions _zoomingMode;

        public Graphs()
        {
            InitializeComponent();

            //var gradientBrush = new LinearGradientBrush
            //{
            //    StartPoint = new Point(0, 0),
            //    EndPoint = new Point(0, 1)
            //};
            //gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(33, 148, 241), 0));
            //gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));

            //SeriesCollection = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Values = GetData(),
            //        Fill = gradientBrush,
            //        StrokeThickness = 1,
            //        PointGeometrySize = 0
            //    }
            //};

            //ZoomingMode = ZoomingOptions.X;

            //XFormatter = val => new DateTime((long)val).ToString("dd MMM");
            //YFormatter = val => val.ToString("C");

            //this.DataContextChanged += Graphs_DataContextChanged;
            
        }

        //private void Graphs_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    //DataContext = new SalesGraphViewModel();
        //}

        //public SeriesCollection SeriesCollection { get; set; }
        //public Func<double, string> XFormatter { get; set; }
        //public Func<double, string> YFormatter { get; set; }

        //public ZoomingOptions ZoomingMode
        //{
        //    get { return _zoomingMode; }
        //    set
        //    {
        //        _zoomingMode = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private void ToogleZoomingMode(object sender, RoutedEventArgs e)
        //{
        //    switch (ZoomingMode)
        //    {
        //        case ZoomingOptions.None:
        //            ZoomingMode = ZoomingOptions.X;
        //            break;
        //        case ZoomingOptions.X:
        //            ZoomingMode = ZoomingOptions.Y;
        //            break;
        //        case ZoomingOptions.Y:
        //            ZoomingMode = ZoomingOptions.Xy;
        //            break;
        //        case ZoomingOptions.Xy:
        //            ZoomingMode = ZoomingOptions.None;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        //private ChartValues<DateTimePoint> GetData()
        //{
        //    var dataTable = new DataTable();

        //    using (MySqlConnection con = Utilities.MySQLDataAccess.GetConnection())
        //    {
        //        con.Open();
        //        using (MySqlCommand cmd = new MySqlCommand())
        //        {
        //            //cmd.CommandText = "select totalamount,addedon from sales";

        //            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter("select totalamount,addedon from sales",con))
        //            {
        //                dataAdapter.Fill(dataTable);
        //            }
        //        }
        //    }

        //        var r = new Random();
        //    //var trend = 100;
        //    var values = new ChartValues<DateTimePoint>();

        //    //for (var i = 1; i < 10; i++)
        //    //{
        //    //    //var seed = r.NextDouble();
        //    //    //if (seed > .8) trend += seed > .9 ? 50 : -50;
        //    //    values.Add(new DateTimePoint(DateTime.Now.AddDays(i), i* 100));
        //    //}

        //    //for (var i = 0; i < dataTable.Rows.Count; i++)
        //    //{                
        //    //    values.Add(new DateTimePoint(Convert.ToDateTime(dataTable.Rows[i].ItemArray.GetValue(1)), Convert.ToDouble(dataTable.Rows[i].ItemArray.GetValue(0))));
        //    //}

        //    return values;
        //}

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName = null)
        //{
        //    if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        //{
        //    //Use the axis MinValue/MaxValue properties to specify the values to display.
        //    //use double.Nan to clear it.

        //    //X.MinValue = double.NaN;
        //    //X.MaxValue = double.NaN;
        //    //Y.MinValue = double.NaN;
        //    //Y.MaxValue = double.NaN;
        //}
    }

    public class ZoomingModeCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ZoomingOptions)value)
            {
                case ZoomingOptions.None:
                    return "None";
                case ZoomingOptions.X:
                    return "X";
                case ZoomingOptions.Y:
                    return "Y";
                case ZoomingOptions.Xy:
                    return "XY";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
