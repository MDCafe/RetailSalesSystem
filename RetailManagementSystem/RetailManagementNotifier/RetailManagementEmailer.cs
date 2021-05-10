﻿using log4net;
using System;
using System.IO;

namespace RetailManagementNotifier
{
    class RetailManagementEmailer
    {      
        static readonly ILog log = LogManager.GetLogger(typeof(RetailManagementEmailer));
        public void SendDailySalesReport(string[] args)
        {
            try
            {
                DateTime fromSalesDate, toSalesDate;

                if (args != null && args.Length == 2)
                {
                    var arg1 = args[0];
                    var arg2 = args[1];

                    log.Debug("Arg1:" + args[0]);
                    log.Debug("Arg2:" + args[1]);

                    if (DateTime.TryParse(arg1, out fromSalesDate) && DateTime.TryParse(arg1, out toSalesDate))
                    {
                        //
                    }
                    else
                    {
                        log.Fatal("No date available.Check the args");
                        throw new Exception("Invalid Date");
                    }
                }
                else
                {

                    fromSalesDate = DateTime.Today;
                    toSalesDate = DateTime.Today;
                }

                log.Debug("FromSalesDate:" + fromSalesDate.ToString("dd/MM/yyyy HH:mm"));
                log.Debug("ToSalesDate:" + toSalesDate.ToString("dd/MM/yyyy HH:mm"));
                log.Debug("SendDailySalesReport called..");

                using (var en = new RetailManagementSystem.RMSEntities())
                {
                    string[] fileAttachmentPaths = new string[1];

                    log.Debug("Generating report for : Product Sales Summary");

                    var productSalesSummary = new RetailManagementSystem.ViewModel.Reports.Sales.ProductSalesSummaryViewModel(true);
                    var executingAssemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    productSalesSummary.FromDate = fromSalesDate;
                    productSalesSummary.ToDate = toSalesDate;

                    var reportParams = productSalesSummary.GenerateReportForEmail();

                     WTechCommonProject.Utilities.Utility.SaveReportAsPDF(productSalesSummary.GetReportDataSources(), "ProductSalesSummary",
                                            executingAssemblyPath + "\\" + productSalesSummary.ReportPath, reportParams);

                    fileAttachmentPaths[0] = "ProductSalesSummary.pdf";

                    log.Debug("Report generated as ProductSalesSummary.pdf");

                    WTechCommonProject.Utilities.Notifier.EmailNotifier.Send("Sales Summary - Dated :" + toSalesDate.ToString("dd/MM/yyyy"), "", fileAttachmentPaths);

                    log.Info("Sales Summary generated and sent");
                }
            }
            catch (Exception ex)
            {
                log.Error("Sales Summary not generated and sent", ex);
            }
        }     
    }
}
