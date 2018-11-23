using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace RetailManagementSystem.ViewModel.Graphs
{
    class SalesGraphViewModel : GraphsViewModel
    {
        //public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public SalesGraphViewModel()
        {
            Title = "Sales Graph";

            GraphSeriesCollection = new SeriesCollection();
            var cashQuery = "select sum(totalamount),date_format(addedOn,'%b-%y') SalesMonth from sales  where paymentMode=0 group by year(AddedOn), Month(AddedOn) ";
            var labelsList = GetSalesData(cashQuery,"Cash");
            Labels = labelsList.ToArray();

            var creditQuery = "SELECT " +
                                        "SUM(totalamount), " +
                                        "DATE_FORMAT(s.addedOn, '%b-%y') SalesMonth " +
                                    "FROM " +
                                        "sales s, Customers c " +
                                     "WHERE " +
                                        "paymentMode = 1 " +
                                        "and s.CustomerId = c.id " +
                                        "and c.CustomerTypeId != 7 " +
                                    "GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn) ";
            labelsList = GetSalesData(creditQuery, "Credit");
            Labels = labelsList.ToArray();

            var hotelCustomersQuery =  "SELECT " +
                                        "SUM(totalamount), " +
                                        "DATE_FORMAT(s.addedOn, '%b-%y') SalesMonth " +
                                    "FROM " +
                                        "sales s, Customers c " +
                                     "WHERE " +
                                        "paymentMode = 1 " +
                                        "and s.CustomerId = c.id " +
                                        "and c.CustomerTypeId = 7 " +
                                    "GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn) ";
            
            GetSalesData(hotelCustomersQuery, "Hotels");

            //GraphSeriesCollection = new SeriesCollection
            //{
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {4, 5, 6, 8},
            //        StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
            //        DataLabels = true
            //    },
            //    new StackedColumnSeries
            //    {
            //        Values = new ChartValues<double> {2, 5, 6, 7},
            //        StackMode = StackMode.Values,
            //        DataLabels = true
            //    }
            //};

            ////adding series updates and animates the chart
            //GraphSeriesCollection.Add(new StackedColumnSeries
            //{
            //    Values = new ChartValues<double> { 6, 2, 7 },
            //    StackMode = StackMode.Values
            //});

            ////adding values also updates and animates
            ////GraphSeriesCollection[2].Values.Add(4d);

            //Labels = new[] { "Chrome", "Mozilla", "Opera", "IE" };
            Formatter = value => value.ToString("N");

            //DataContext = this;
        }

        public List<string> GetSalesData(string query,string seriesTitle)
        {
            List<string> lst = new List<string>();
            using (var conn = MySQLDataAccess.GetConnection())
            {              
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;                    

                    using (var rdr = cmd.ExecuteReader())
                    {                                                
                        //LineSeries LnSeries = new LineSeries();
                        var yAxisChartValues = new ChartValues<decimal>();                        
                        //var xAxisChartValues = new List<DateTimePoint>();
                        while (rdr.Read())
                        {
                            //yAxisChartValues.Add();
                            var dbl = rdr.GetDecimal(0);
                            yAxisChartValues.Add(dbl);
                            lst.Add(rdr.GetString(1));
                        }
                        //LnSeries.Values = yAxisChartValues;

                        var stackColSeries = new StackedColumnSeries
                        {
                            Values = yAxisChartValues, //new ChartValues<double> { 2, 5, 6, 7 },
                            StackMode = StackMode.Values,                            
                            Title = seriesTitle,
                            DataLabels = true                            
                        };
                        GraphSeriesCollection.Add(stackColSeries);
                        //XAxisLabels = xAxisChartValues;
                    }
                }                
            }
            return lst;
        }
    }
}
