using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Utilities;
using System.Collections.Generic;
using System.Data;

namespace RetailManagementSystem.ViewModel.Graphs
{
    class SalesGraphViewModel : GraphsViewModel
    {
        public SalesGraphViewModel()
        {
            Title = "Sales Graph";
            GetSalesData();
        }

        public void GetSalesData()
        {
            using (var conn = MySQLDataAccess.GetConnection())
            {
                var query = "select totalamount,addedOn from sales ";// +
                            //"group by year(AddedOn),month(addedOn) " +
                            //"order by addedOn";
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                   

                    using (var rdr = cmd.ExecuteReader())
                    {
                        LineSeries LnSeries = new LineSeries();
                        var yAxisChartValues = new ChartValues<decimal>();
                        var xAxisChartValues = new List<DateTimePoint>();
                        while (rdr.Read())
                        {
                            //yAxisChartValues.Add();
                            var dbl = rdr.GetDouble(0);
                            xAxisChartValues.Add(new DateTimePoint(rdr.GetDateTime(1), dbl ));                            
                        }
                        LnSeries.Values = yAxisChartValues;
                        GraphSeriesCollection = new SeriesCollection();
                        GraphSeriesCollection.Add(LnSeries);
                        //XAxisLabels = xAxisChartValues;
                    }
                }
            }
        }
    }
}
