using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using RetailManagementSystem.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace RetailManagementSystem.ViewModel.Reports.Sales
{
    public class ProductSalesSummaryViewModel : ReportBusinessBaseVM
    {
        public ProductSalesSummaryViewModel(bool forEmail) : base("Product Sales Summary")
        {
            ReportPath = @"View\Reports\Sales\ProductSalesSummary.rdl";
            if (forEmail) return;

            using (var rmsEntities = new RMSEntities())
            {
                ProductList = rmsEntities.Products.OrderBy(p => p.Name).ToList();
                //Companies = rmsEntities.Companies.OrderBy(c => c.Name).ToList();
                ProductCategories = rmsEntities.Categories.OrderBy(ct => ct.name).ToList();
            }


        }


        public override void OnPrint(Window window)
        {
            if (FromDate.CompareTo(ToDate) > 0)
            {
                Utilities.Utility.ShowErrorBox("From date can't be more than To date");
                return;
            }

            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1"
            };

            var query = " Select p.Id,P.Name,sum(sd.qty) SoldQuantity,c.name CategoryName, sum(sd.qty) *pd.SellingPrice SalesAmount,pd.SellingPrice, cp.Name CompanyName " +
                        " from saleDetails sd " +
                        " join products p on(sd.ProductId = p.id) " +
                        " join Category c on(p.CategoryId = c.Id) " +
                        " join companies cp on (cp.Id = p.companyId) " +
                        " Join PriceDetails pd on(pd.PriceId = sd.PriceId and pd.ProductId = P.id) " +
                        " where date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate ";

            //"    where sd.ProductId = @ProductId and date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate";

            var fromSqlParam = new MySqlParameter("FromDate", MySqlDbType.Date)
            {
                Value = FromDate.ToString("yyyy-MM-dd")
            };

            var toSqlParam = new MySqlParameter("ToDate", MySqlDbType.Date)
            {
                Value = ToDate.ToString("yyyy-MM-dd")
            };

            if (SelectedProduct != null && SelectedProduct.Id != 0)
            {
                var productIdSqlParam = new MySqlParameter("ProductId", MySqlDbType.Int32)
                {
                    Value = SelectedProduct.Id
                };

                query += " and p.Id = @ProductId group by P.id  order by c.Name,p.Name  ";

                _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[3]
                                                {
                                                     productIdSqlParam,fromSqlParam,toSqlParam
                                                },
                                                CommandType.Text);
            }
            else if (SelectedCategory != null && SelectedCategory.Id != 0)
            {
                var categoryIdSqlParam = new MySqlParameter("CategoryId", MySqlDbType.Int32)
                {
                    Value = SelectedCategory.Id
                };

                query += " and c.Id = @CategoryId group by P.id  order by c.Name,p.Name ";

                _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[3]
                                            {
                                                categoryIdSqlParam,fromSqlParam,toSqlParam
                                            },
                                            CommandType.Text);
            }
            //else if (SelectedCompany != null && SelectedCompany.Id != 0)
            //{
            //    var companyIdSqlParam = new MySqlParameter("CompanyId", MySqlDbType.Int32)
            //    {
            //        Value = SelectedCompany.Id
            //    };

            //    query += " and c.Id = @CompanyId group by P.id  order by c.Name ";

            //    _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[3]
            //                                {
            //                                    companyIdSqlParam,fromSqlParam,toSqlParam
            //                                },
            //                                CommandType.Text);
            //}
            else
            {
                query += " group by P.id  order by c.Name,p.Name ";

                _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[2]
                                            {
                                                fromSqlParam,toSqlParam
                                            },
                                            CommandType.Text);
            }

            ReportParameters = new List<ReportParameter>(1)
            {
                new ReportParameter("DateRange", fromSqlParam.Value + " to " + toSqlParam.Value)
            };

            Workspace.This.OpenReport(this);
            CloseWindow(window);
        }

        public ReportDataSource[] GetReportDataSources()
        {
            return _rptDataSource;
        }

        public List<ReportParameter> GenerateReportForEmail()
        {
            _rptDataSource[0] = new ReportDataSource
            {
                Name = "DataSet1"
            };

            var query = " Select p.Id,P.Name,sum(sd.qty) SoldQuantity,c.name CategoryName, sum(sd.qty) *pd.SellingPrice SalesAmount,pd.SellingPrice, cp.Name CompanyName " +
                        " from saleDetails sd " +
                        " join products p on(sd.ProductId = p.id) " +
                        " join Category c on(p.CategoryId = c.Id) " +
                        " join companies cp on (cp.Id = p.companyId) " +
                        " Join PriceDetails pd on(pd.PriceId = sd.PriceId and pd.ProductId = P.id) " +
                        " where date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate group by P.id  order by c.Name,p.Name";

            //"    where sd.ProductId = @ProductId and date(sd.AddedOn) >= @FromDate and date(sd.addedon) <= @ToDate";

            var fromSqlParam = new MySqlParameter("FromDate", MySqlDbType.Date)
            {
                Value = FromDate.ToString("yyyy-MM-dd")
            };

            var toSqlParam = new MySqlParameter("ToDate", MySqlDbType.Date)
            {
                Value = ToDate.ToString("yyyy-MM-dd")
            };

            _rptDataSource[0].Value = GetDataTable(query, new MySqlParameter[2]
                                        {
                                            fromSqlParam,toSqlParam
                                        },
                                        CommandType.Text);


            _rptDataSource[1] = new ReportDataSource
            {
                Name = "DataSet2",
                Value = GetApplicationDetails()
            };

            ReportParameters = new List<ReportParameter>(1)
            {
                new ReportParameter("DateRange", fromSqlParam.Value + " to " + toSqlParam.Value)
            };
            return ReportParameters;
        }

        public static DataTable GetApplicationDetails()
        {
            using (var conn = MySQLDataAccess.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (var appDt = new DataTable())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from applicationDetails";
                        cmd.CommandType = CommandType.Text;
                        conn.Open();
                        using (var dt = new DataTable())
                        {
                            using (var rdr = cmd.ExecuteReader())
                            {
                                dt.Load(rdr);
                            }
                            return dt;
                        }
                    }
                }
            }
        }
    }
}
