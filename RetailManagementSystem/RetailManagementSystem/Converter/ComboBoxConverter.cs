using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RetailManagementSystem.Converter
{
    public class ComboBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //var products = values[0] as IList<Product>;
            var selectedItem = values[0] as Product;
            if (selectedItem == null) return null;

            //var selectedGlobalResourceView = values[2] as ResourceViewOption;
            return RMSEntitiesHelper.Instance.RMSEntities.PriceDetails.Where(pr => pr.ProductId == selectedItem.Id).OrderByDescending(s => s.ModifiedOn).Take(3).ToList();
            //if (personnelType == null)
            //    return allResources;
            //if (selectedGlobalResourceView.ResourceViewTitle == "Regional")
            //    return allResources.Where(r => r.Head == personnelType.Head && r.Obsolete == false && r.Location.Region.RegionID.Trim() == SettingsManager.OpsMgrSettings.Region.Trim()).OrderBy(r => r.Name).ToList();
            //if (selectedGlobalResourceView.ResourceViewTitle == "Local")
            //    return allResources.Where(r => r.Head == personnelType.Head && r.Obsolete == false && r.LocnID.Trim() == SettingsManager.OpsMgrSettings.LOCNCODE.Trim()).OrderBy(r => r.Name).ToList();

            //return null;// allResources.Where(r => r.Head == personnelType.Head && r.Obsolete == false).OrderBy(r => r.Name).ToList();

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
