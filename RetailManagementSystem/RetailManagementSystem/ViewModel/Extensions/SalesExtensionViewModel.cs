using System.ComponentModel;

namespace RetailManagementSystem.ViewModel.Extensions
{
    public class SalesExtensionViewModel : IExtensions
    {
        decimal? _transportCharges;
        decimal? _transportChargesOldValue;

        public decimal? TransportCharges
        {
            get { return _transportCharges; }
            set
            {
                _transportChargesOldValue = _transportCharges;
                _transportCharges = value;
                OnPropertyChanged("TransportCharges");
            }
        }

        public decimal? TransportChargesOldValue
        {
            get { return _transportChargesOldValue; }
            set
            {
                _transportChargesOldValue = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal Calculate(decimal value)
        {
            var totalAmount = value + (TransportCharges.HasValue ? TransportCharges.Value : 0);
            return totalAmount;                        
        }

        public void Clear()
        {
            TransportCharges = 0;
        }

        public decimal GetPropertyValue(string propertyName, out decimal? oldValue)
        {
            oldValue = 0.0M;
            switch (propertyName)
            {
                case "TransportCharges":
                    {
                        oldValue = _transportChargesOldValue.HasValue ? _transportChargesOldValue.Value : 0;
                        return TransportCharges.HasValue ? TransportCharges.Value : 0;
                    }
            }
            return 0;
        }

        public void SetValues(params decimal[] extensionValues)
        {
            if (extensionValues != null && extensionValues.Length > 0)
            {
                TransportCharges = extensionValues[0];
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            bool flag = propertyChanged != null;
            if (flag)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
