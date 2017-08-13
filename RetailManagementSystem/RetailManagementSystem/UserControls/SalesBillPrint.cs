using System;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using RetailManagementSystem.Model;
using System.Text;
using log4net;

namespace RetailManagementSystem.UserControls
{
    internal class SalesBillPrint
    {
        PrintDocument _pdoc;
        ApplicationDetail _appDetail;
        string _customerName;
        IEnumerable<SaleDetailExtn> _saleDetails;
        Sale _billSales;
        decimal? _amountPaid, _balanceAmt;

        static readonly ILog _log = LogManager.GetLogger(typeof(SalesBillPrint));

        public SalesBillPrint()
        {
            PrintDialog pd = new PrintDialog();
            string strDefaultPrinter = pd.PrinterSettings.PrinterName;//Code to get default printer name  
            _pdoc = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Courier New", 15);//set default font for page
                                                    //PaperSize psize = new PaperSize("Custom", 212, 130);//set paper size sing code
            //PaperSize psize = new PaperSize("Custom", 212, 100);
            pd.Document = _pdoc;
            //pd.Document.DefaultPageSettings.PaperSize = psize;
            _pdoc.PrintPage += new PrintPageEventHandler(PrintPage);

            string defaultPrinterName = ps.PrinterName; // Code to get default printer

            ps.PrinterName = defaultPrinterName;//Code to set default printer name
            pd.PrinterSettings.PrinterName = defaultPrinterName;//Code to set default printer name 

            pd.PrinterSettings.PrintToFile = true;
            pd.PrinterSettings.PrintFileName = @"E:\PosPrint.pdf";

            RMSEntitiesHelper.Instance.RMSEntities.ApplicationDetails.Count();
            _appDetail = RMSEntitiesHelper.Instance.RMSEntities.ApplicationDetails.FirstOrDefault();

        }

        public void print(string customerName,IEnumerable<SaleDetailExtn> saleDetails,Sale billSales,decimal? amountPaid,decimal? balanceAmt)
        {
            try
            {
                _customerName = customerName;
                _billSales = billSales;
                _saleDetails = saleDetails;
                _billSales = billSales;
                _amountPaid = amountPaid;
                _balanceAmt = balanceAmt; 
                _pdoc.Print();
            }
            catch (Exception ex)
            {
                Utilities.Utility.ShowErrorBox("Error while Printing..!!" + ex.Message);
                _log.Info("Error while Printing..!! - "  + _billSales.BillId, ex);
            }
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {   
            int startX = 5;// Position of x-axis
            int startY = 2;//starting position of y-axis
            int Offset = 0;
            //**********************Header***************************************************************************************//
            var headerFont = new Font("Maiandra GD", 8, FontStyle.Bold);
            var itemFont = new Font("Times New Roman", 9, FontStyle.Regular);
            float fontHeight = itemFont.GetHeight();
            var solidBrush = new SolidBrush(Color.Black);

            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            int headerStartX = startX + 130;
            //drawFormat.FormatFlags = StringFormatFlags.;

            e.Graphics.DrawString(_appDetail.Name, new Font("Maiandra GD", 12, FontStyle.Bold), solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 16;
            e.Graphics.DrawString(_appDetail.Address, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 16;
            e.Graphics.DrawString(_appDetail.City, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 16;
            e.Graphics.DrawString(_appDetail.Lan_Line_No + "," + _appDetail.Mobile_No, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 16;
            e.Graphics.DrawString(_appDetail.EmailAddress, headerFont, solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 25;

            RectangleF marginBounds = e.MarginBounds;
            RectangleF printableArea = e.PageSettings.PrintableArea;
            int availableWidth = (int)Math.Floor(_pdoc.OriginAtMargins ? marginBounds.Width : (e.PageSettings.Landscape
                                                    ? printableArea.Height
                                                    : printableArea.Width));

            var rightAlignformat = new StringFormat() { Alignment = StringAlignment.Far };
            var rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);

            e.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yy HH:mm"), itemFont, solidBrush, startX, startY + Offset);
            e.Graphics.DrawString("Bill No: " + _billSales.RunningBillNo, itemFont, solidBrush, rect, rightAlignformat);
            //drawFormat.Alignment = StringAlignment.Far;
            Offset = Offset + 20;

            if(!string.IsNullOrWhiteSpace(_billSales.CustomerOrderNo))
            {
                e.Graphics.DrawString("Order No : " + _billSales.CustomerOrderNo, itemFont, solidBrush, startX, startY + Offset);
                Offset = Offset + 20;
            }

            //********************************************ENd OF Header********************************************

            drawFormat.Alignment = StringAlignment.Center;
            //Customer Name
            e.Graphics.DrawString(_customerName, new Font("Times New Roman", 11, FontStyle.Bold), solidBrush, headerStartX, startY + Offset, drawFormat);
            Offset = Offset + 25;


            rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
            
            e.Graphics.DrawString("Qty", itemFont, solidBrush, startX, startY + Offset);
            e.Graphics.DrawString("Unit", itemFont, solidBrush, rect, drawFormat);
            e.Graphics.DrawString("Total", itemFont, solidBrush, rect, rightAlignformat);

            Offset = Offset + 5;
            e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
            Offset = Offset + 20;


            //********************************************Items*****************************************************************
            var itemDiscountAmount = 0.00M;
            foreach (var item in _saleDetails)
            {
                var product = RMSEntitiesHelper.Instance.RMSEntities.Products.Find(item.ProductId);
                e.Graphics.DrawString(product.Name, itemFont, solidBrush, startX, startY + Offset);
                Offset = Offset + 20;

                var qtyString = item.Qty.Value.ToString("G").Replace('.', '-');
                //var itemValue = qtyString[0];

                var itemValueRest = qtyString + product.MeasuringUnit.unit + "              ";

                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);

                //var x = startX + qtyString[0].Length + 10;
                drawFormat.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(itemValueRest, itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString(item.SellingPrice.Value.ToString("N2"), itemFont, solidBrush, rect,drawFormat);

                //e.Graphics.DrawString(".", new Font("Times New Roman", 10, FontStyle.Bold), solidBrush, x, startY + Offset); //new Font("Times New Roman", 10,FontStyle.Bold)
                //e.Graphics.DrawString(itemValueRest, itemFont, solidBrush, x + 5, startY + Offset);

                var totalValue = item.Qty.Value * item.SellingPrice.Value;
                
                e.Graphics.DrawString(totalValue.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                Offset += 20;

                itemDiscountAmount += item.Discount.HasValue ? item.Discount.Value : 0.0M;
            }
            Offset += -15;

            //********************************************End of Items*************************************
            drawFormat.Alignment = StringAlignment.Far;
            //drawFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
            Offset = Offset + 20;

            rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
            //Total
            e.Graphics.DrawString(_saleDetails.Sum(a => a.Qty.Value * a.SellingPrice.Value).ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
            e.Graphics.DrawString("Total", itemFont, solidBrush, startX, startY + Offset);

            Offset = Offset + 20;

            var discount = _billSales.Discount.HasValue ? _billSales.Discount.Value : 0.00M;
            if (discount != 0.00M || itemDiscountAmount !=0.0M)
            {
                discount += itemDiscountAmount;

                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                e.Graphics.DrawString("Discount", itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString(discount.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                Offset = Offset + 5;
                e.Graphics.DrawString("_______________________________________", itemFont, solidBrush, startX, startY + Offset);
                Offset = Offset + 20;

                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                e.Graphics.DrawString("Total", itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString(_billSales.TotalAmount.Value.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                Offset = Offset + 20;
            }

            if (_amountPaid.HasValue && _amountPaid.Value != 0.00M)
            { 
                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                e.Graphics.DrawString("Amount Paid", itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString(_amountPaid.Value.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                Offset = Offset + 20;
            }

            if (_balanceAmt.HasValue && _amountPaid.Value != 0.00M)
            {
                rect = new RectangleF(startX, startY + Offset, availableWidth - 10, fontHeight);
                e.Graphics.DrawString("Balance Amount", itemFont, solidBrush, startX, startY + Offset);
                e.Graphics.DrawString(_balanceAmt.Value.ToString("N2"), itemFont, solidBrush, rect, rightAlignformat);
                Offset = Offset + 20;
            }

            if(_billSales.PaymentMode == "1")
            {
                e.Graphics.DrawString("Customer's Sign : ______________________", itemFont, solidBrush, startX, startY + Offset);
                Offset = Offset + 20;
            }

            drawFormat.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Thank You!", headerFont, solidBrush, headerStartX, startY + Offset,drawFormat);

            Offset = 0;

        }

        string AddWhiteSpaceBasedOnLength(string s,int length)
        {
            if (s.Length == length) return s;

            StringBuilder sb = new StringBuilder(s);

            for (int i = s.Length; i < length; i++)
            {
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
