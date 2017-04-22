using System.ComponentModel;

namespace RetailManagementSystem.ViewModel.Extensions
{
    public interface IExtensions : INotifyPropertyChanged
    {
                    
        decimal Calculate(decimal amount);
        void Clear();
        void SetValues(params decimal[] extensionValues);
        decimal GetPropertyValue(string propertyName);

    }
}