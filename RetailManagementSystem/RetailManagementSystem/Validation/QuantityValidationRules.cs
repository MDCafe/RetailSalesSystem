using RetailManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailManagementSystem.Validation
{
    class QuantityValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //var saleDetailExtn = (value as BindingGroup).Items[0] as SaleDetailExtn;

            BindingExpression expression = value as BindingExpression;
            var saleDetailExtn = expression.DataItem as SaleDetailExtn;
            
            if (saleDetailExtn.Qty > saleDetailExtn.AvailableStock)            
                return new ValidationResult(false, "Quantity can't be more than available stock");
                        
            return ValidationResult.ValidResult;            
        }
    }
}
