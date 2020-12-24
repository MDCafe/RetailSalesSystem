using System.Windows.Controls;

namespace RetailManagementSystem.Validation
{
    public class ValidateComboRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is Customer)
                return new ValidationResult(true, null);


            return new ValidationResult(false, "Invalid Customer");
        }
    }

    public class ValidateCompanies : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is Company)
                return new ValidationResult(true, null);


            return new ValidationResult(false, "Invalid Supplier");
        }
    }
}
