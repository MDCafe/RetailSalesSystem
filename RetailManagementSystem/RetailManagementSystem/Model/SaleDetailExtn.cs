using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace RetailManagementSystem.Model
{
    [ImplementPropertyChanged]
    public class SaleDetailExtn : SaleDetail
    {
        public decimal CostPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
