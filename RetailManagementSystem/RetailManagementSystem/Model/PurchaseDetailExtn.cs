using System;

namespace RetailManagementSystem.Model
{
    class PurchaseDetailExtn : SaleDetailExtn
    {
        private decimal? _purchasePrice;
        private decimal? _freeIssue;
        
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
    }
}
