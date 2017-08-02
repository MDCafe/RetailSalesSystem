using System;

namespace RetailManagementSystem.Model
{
    class PurchaseDetailExtn : SaleDetailExtn
    {
        private decimal? _purchasePrice;
        private decimal? _freeIssue;
        private DateTime? _expiryDate;

        public decimal? PurchasePrice
        {
            get
            {
                return _purchasePrice;
            }

            set
            {
                if (Nullable.Equals<decimal>(_purchasePrice, value))
                {
                    return;
                }
                _purchasePrice = value;
                this.OnPropertyChanged("PurchasePrice");
            }
        }

        public decimal? FreeIssue
        {
            get
            {
                return _freeIssue;
            }

            set
            {
                if (Nullable.Equals<decimal>(_freeIssue, value))
                {
                    return;
                }
                _freeIssue = value;
                this.OnPropertyChanged("FreeIssue");
            }
        }

        public decimal? OldCostPrice { get; set; }
        public decimal? OldSellingPrice { get; set; }
        public decimal? Tax { get; set; }

        public DateTime? ExpiryDate
        {
            get
            {
                if (_expiryDate == null)
                {
                    return _expiryDate = DateTime.Now.AddMonths(1);
                }
                return _expiryDate;
            }

            set
            {   
                if (Nullable.Equals(_expiryDate, value))
                {
                    _expiryDate = DateTime.Now.AddMonths(1);
                }
                _expiryDate = value;
                this.OnPropertyChanged("ExpiryDate");
            }
        }
    }
}
