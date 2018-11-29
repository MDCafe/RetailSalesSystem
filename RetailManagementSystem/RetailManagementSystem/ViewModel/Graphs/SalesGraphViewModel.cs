using LiveCharts;
using LiveCharts.Configurations;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Model.Graphs;
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
        public ChartValues<SalesGraphCustomModel> SalesGraphCustomModels { get; set; }

        public SalesGraphViewModel()
        {
            Title = "Sales Graph";

            var labelsList = GetSalesData("", "Sales");

            Labels = labelsList.ToArray();
            //GraphSeriesCollection = new SeriesCollection();
            //var cashQuery = "select sum(totalamount),date_format(addedOn,'%b-%y') SalesMonth from sales  where paymentMode=0 group by year(AddedOn), Month(AddedOn) ";
            //var labelsList = GetSalesData(cashQuery,"Cash");
            //Labels = labelsList.ToArray();

            //var creditQuery = "SELECT " +
            //                            "SUM(totalamount), " +
            //                            "DATE_FORMAT(s.addedOn, '%b-%y') SalesMonth " +
            //                        "FROM " +
            //                            "sales s, Customers c " +
            //                         "WHERE " +
            //                            "paymentMode = 1 " +
            //                            "and s.CustomerId = c.id " +
            //                            "and c.CustomerTypeId != 7 " +
            //                        "GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn) ";
            //labelsList = GetSalesData(creditQuery, "Credit");
            //Labels = labelsList.ToArray();

            //var hotelCustomersQuery =  "SELECT " +
            //                            "SUM(totalamount), " +
            //                            "DATE_FORMAT(s.addedOn, '%b-%y') SalesMonth " +
            //                        "FROM " +
            //                            "sales s, Customers c " +
            //                         "WHERE " +
            //                            "paymentMode = 1 " +
            //                            "and s.CustomerId = c.id " +
            //                            "and c.CustomerTypeId = 7 " +
            //                        "GROUP BY YEAR(s.AddedOn) , MONTH(s.AddedOn) ";

            //GetSalesData(hotelCustomersQuery, "Hotels");

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
            query = "GetSalesGraphReport";
            List<string> lst = new List<string>();
            using (var conn = MySQLDataAccess.GetConnection())
            {                              
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {                                                
                        //LineSeries LnSeries = new LineSeries();
                        var cashChartValues = new ChartValues<Decimal>();
                        //var creditChartValues = new ChartValues<Decimal>();

                        SalesGraphCustomModels = new ChartValues<SalesGraphCustomModel>();

                        while (rdr.Read())
                        {
                            SalesGraphCustomModels.Add(
                                new SalesGraphCustomModel()
                                {
                                    CashSales = rdr.GetDecimal(0),
                                    SaleYearMonth = rdr.GetString(1),
                                    CreditSales = rdr.GetDecimal(2),
                                    HotelSales = rdr.GetDecimal(3),
                                    TotalSales = rdr.GetDouble(4)                                    
                                }
                            );
                            //cashChartValues.Add(rdr.GetDecimal(4));
                            //creditChartValues.Add(rdr.GetDecimal(2));
                            lst.Add(rdr.GetString(1));
                        }

                        //var cashlineSeries = new LineSeries
                        //{
                        //    Values = salesChartValues, //new ChartValues<double> { 2, 5, 6, 7 },
                        //    //StackMode = StackMode.Values,
                        //    Title = seriesTitle,
                        //    DataLabels = true,
                        //    //LabelPoint = point => point.X + "Total" 
                        //};


                        var customerVmMapper = Mappers.Xy<SalesGraphCustomModel>()
                        .X((value, index) => index) // lets use the position of the item as X
                        .Y(value => value.TotalSales);

                       
                        //var creditlineSeries = new LineSeries
                        //{
                        //    Values = creditChartValues, 
                        //    DataLabels = true,
                        //    //LabelPoint = point => point.X + "Total" 
                        //};
                        //GraphSeriesCollection = new SeriesCollection();
                        //GraphSeriesCollection.Add(cashlineSeries);

                        //GraphSeriesCollection.Add(creditlineSeries);
                        Charting.For<SalesGraphCustomModel>(customerVmMapper);

                        //XAxisLabels = xAxisChartValues;
                    }
                }                
            }
            return lst;
        }
       
    }
}
