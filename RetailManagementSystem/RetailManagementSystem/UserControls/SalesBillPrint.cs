using System;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using RetailManagementSystem.Model;
using System.Text;
using log4net;
using System.Configuration;

namespace RetailManagementSystem.UserControls
{
    internal class SalesBillPrint
    {
        readonly PrintDocument _pdoc;
        readonly ApplicationDetail _appDetail;
        string _customerName;
        IEnumerable<SaleDetailExtn> _saleDetails;
        Sale _billSales;
        decimal? _amountPaid, _balanceAmt;
        decimal _totalAmount;
        bool _showRestrictedCustomers;
        static readonly ILog _log = LogManager.GetLogger(typeof(SalesBillPrint));
        readonly RMSEntities _rmsEntities;

        public SalesBillPrint(RMSEntities rmsEntities)
        {
            _rmsEntities = rmsEntities;
            using (PrintDialog pd = new PrintDialog())
            {
                string strDefaultPrinter = pd.PrinterSettings.PrinterName;//Code to get default printer name  
                _pdoc = new PrintDocument();
                PrinterSettings ps = new PrinterSettings();

                //Font font = new Font("Courier New", 15);//set default font for page
                                                        //PaperSize psize = new PaperSize("Custom", 212, 130);//set paper size sing code
                                                        //PaperSize psize = new PaperSize("Custom", 212, 100);
                pd.Document = _pdoc;
                //pd.Document.DefaultPageSettings.PaperSize = psize;
                _pdoc.PrintPage += new PrintPageEventHandler(PrintPage);


                var printFileName = ConfigurationManager.AppSettings["PrintFileName"];
                var printerName = ConfigurationManager.AppSettings["BillPrinter"];
                if (!string.IsNullOrWhiteSpace(printFileName))
                {
                    pd.PrinterSettings.PrintToFile = true;
                    pd.PrinterSettings.PrintFileName = printFileName;
                }
                else if (!string.IsNullOrWhiteSpace(printerName))
                {
                    //string defaultPrinterName = ps.PrinterName; // Code to get default printer
                    //var printerNamePc1 = "\\\\pc01\\EPSON LX-300+ /II";
                    //ps.PrinterName = printerName;
                    pd.PrinterSettings.PrinterName = printerName;
                }
                else
                {
                    Utilities.Utility.ShowErrorBox("Billing printer name is not configured");
                }
            }
            _rmsEntities.ApplicationDetails.Count();
            _appDetail = _rmsEntities.ApplicationDetails.FirstOrDefault();
        }

        public void Print(string customerName,IEnumerable<SaleDetailExtn> saleDetails,Sale billSales,decimal totalAmount, decimal? amountPaid,decimal? balanceAmt,
                          bool showRestrictedCustomers)
        {
            try
            {                
                _customerName = customerName;
                _saleDetails = saleDetails;
                _billSales = billSales;
                _amountPaid = amountPaid;
                _balanceAmt = balanceAmt;
                _showRestrictedCustomers = showRestrictedCustomers;
                _totalAmount = totalAmount;

                _pdoc.Print();
            }
            catch (Exception ex)
            {
                Utilities.Utility.ShowErrorBox("Error while Printing..!!" + ex.Message);
                _log.Error("Error while Printing..!! - "  + _billSales.BillId, ex);
            }
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {
            //using (System.Security.Principal.WindowsImpersonationContext wic = System.Security.Principal.WindowsIdentity.Impersonate(IntPtr.Zero))
            //{
            try
            {
                int startX = 5;// Position of x-axis
                int startY = 2;//starting position of y-axis
                int Offset = 0;
                //**********************Header***************************************************************************************//
                var headerFont = new Font("Maiandra GD", 8, FontStyle.Bold);
                var itemFont = new Font("Arial", 9, FontStyle.Regular);
                float fontHeight = itemFont.GetHeight();
                var solidBrush = new SolidBrush(Color.Black);

                StringFormat drawFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };
                int headerStartX = startX + 130;
                //drawFormat.FormatFlags = StringFormatFlags.;

                e.Graphics.DrawString(_appDetail.Name, new Font("Maiandra GD", 12, FontStyle.Bold), solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 16;
                e.Graphics.DrawString(_appDetail.Address, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 12;
                e.Graphics.DrawString(_appDetail.City, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 11;
                e.Graphics.DrawString(_appDetail.Lan_Line_No + "," + _appDetail.Mobile_No, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 11;
                e.Graphics.DrawString(_appDetail.EmailAddress, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 20;

                RectangleF marginBounds = e.MarginBounds;
                RectangleF printableArea = e.PageSettings.PrintableArea;
                int availableWidth = (int)Math.Floor(_pdoc.OriginAtMargins ? marginBounds.Width : (e.PageSettings.Landscape
                                                        ? printableArea.Height
                                                        : printableArea.Width));

                var rightAlignformat = new StringFormat() { Alignment = StringAlignment.Far };
                var rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);

                e.Graphics.DrawString(_billSales.AddedOn.Value.ToString("dd/MM/yy HH:mm"), itemFont, solidBrush, startX, startY + Offset);
                if (_showRestrictedCustomers)
                    e.Graphics.DrawString("Bill No:C " + _billSales.RunningBillNo, itemFont, solidBrush, rect, rightAlignformat);
                else
                    e.Graphics.DrawString("Bill No: " + _billSales.RunningBillNo, itemFont, solidBrush, rect, rightAlignformat);
                //drawFormat.Alignment = StringAlignment.Far;
                Offset += 20;

                if (!string.IsNullOrWhiteSpace(_billSales.CustomerOrderNo))
                {
                    e.Graphics.DrawString("Order No : " + _billSales.CustomerOrderNo, itemFont, solidBrush, startX, startY + Offset);
                    Offset += 20;
                }

                //********************************************ENd OF Header********************************************
                string paymentMode;

                if(_billSales.PaymentMode == "0")
                    paymentMode = "Cash";
                else
                    paymentMode = "Credit";

                //drawFormat.Alignment = StringAlignment.Near;
                //Payment Mode
                e.Graphics.DrawString(paymentMode, itemFont, solidBrush, startX, startY + Offset);

                drawFormat.Alignment = StringAlignment.Center;
                //Customer Name
                e.Graphics.DrawString(_customerName, new Font("Arial", 11, FontStyle.Bold), solidBrush, headerStartX, startY + Offset, drawFormat);
                Offset += 25;


                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);

                e.Graphics.DrawString("Qty", itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString("Unit Price", itemFont, solidBrush, rect, drawFormat);
                e.Graphics.DrawString("Total", itemFont, solidBrush, rect, rightAlignformat);

                Offset += 5;
                e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
                Offset += 20;


                //********************************************Items*****************************************************************
                var itemDiscountAmount = 0.00M;
                foreach (var item in _saleDetails)
                {
                    var product = _rmsEntities.Products.Find(item.ProductId);
                    if(product == null)
                    {
                        var msg = "Product not found." + item.ProductId;
                        //Utilities.Utility.ShowErrorBox(msg);
                        _log.Error(msg);
                        continue;
                    }
                    e.Graphics.DrawString(product.Name, itemFont, solidBrush, startX, startY + Offset);
                    Offset += 20;

                    var qtyString = item.Qty.Value.ToString("G").Replace('.', '-');
                    //var itemValue = qtyString[0];

                    var itemValueRest = qtyString + " " + product.MeasuringUnit.unit + "              ";

                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);

                    //var x = startX + qtyString[0].Length + 10;
                    drawFormat.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(itemValueRest, itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(item.SellingPrice.Value.ToString("N2"), itemFont, solidBrush, rect, drawFormat);

                    //e.Graphics.DrawString(".", new Font("Times New Roman", 10, FontStyle.Bold), solidBrush, x, startY + Offset); //new Font("Times New Roman", 10,FontStyle.Bold)
                    //e.Graphics.DrawString(itemValueRest, itemFont, solidBrush, x + 5, startY + Offset);

                    var totalValue = item.Qty.Value * item.SellingPrice.Value;

                    e.Graphics.DrawString(totalValue.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 20;

                    itemDiscountAmount += item.Discount ?? 0.0M;
                }
                Offset += -10;

                //********************************************End of Items*************************************
                drawFormat.Alignment = StringAlignment.Far;
                //drawFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
                Offset += 20;

                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                //Total
                e.Graphics.DrawString(_saleDetails.Sum(a => a.ProductId !=0 ?  (a.Qty.Value * a.SellingPrice.Value): 0).ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                e.Graphics.DrawString("Total", itemFont, solidBrush, startX, startY + Offset);

                Offset += 20;

                var isTransportOrDiscountAvailable = false;

                //Transport Amount
                var transport = _billSales.TransportCharges ?? 0.00M;
                if (transport != 0.0M)
                {
                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                    e.Graphics.DrawString("Transport", itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(transport.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 20;
                    isTransportOrDiscountAvailable = true;
                }

                var discount = _billSales.Discount ?? 0.00M;
                if (discount != 0.00M || itemDiscountAmount != 0.0M)
                {
                    discount += itemDiscountAmount;

                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                    e.Graphics.DrawString("Discount", itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(discount.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 5;
                    e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
                    Offset += 20;
                    isTransportOrDiscountAvailable = true;
                }

                if (isTransportOrDiscountAvailable)
                {
                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                    e.Graphics.DrawString("Total", itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(_totalAmount.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 20;
                }

                if (_amountPaid.HasValue && _amountPaid.Value != 0.00M)
                {
                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                    e.Graphics.DrawString("Amount Paid", itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(_amountPaid.Value.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 20;
                }

                if (_balanceAmt.HasValue && _balanceAmt.Value != 0.00M)
                {
                    rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                    e.Graphics.DrawString("Balance Amount", itemFont, solidBrush, startX, startY + Offset);
                    e.Graphics.DrawString(_balanceAmt.Value.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                    Offset += 20;
                }

                if (_billSales.PaymentMode == "1" || _billSales.PaymentMode == "Credit")
                {
                    e.Graphics.DrawString("Customer's Sign : ______________________", itemFont, solidBrush, startX, startY + Offset + 10);
                    Offset += 20;
                }

                e.Graphics.DrawString("Packed By ________  Checked By ________", itemFont, solidBrush, startX, startY + Offset + 10);
                Offset += 20;

                drawFormat.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("Thank You", headerFont, solidBrush, headerStartX, startY + Offset + 10, drawFormat);

                Offset = 0;
            }
            catch (Exception ex) 
            {
                _log.Error("Error while Printing..!! - " + _billSales.BillId, ex);
                throw;
            }
            //}
        }
    }
}
