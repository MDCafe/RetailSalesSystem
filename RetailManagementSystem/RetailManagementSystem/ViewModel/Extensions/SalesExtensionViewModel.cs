using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.ViewModel.Extensions
{
    public class SalesExtensionViewModel : IExtensions
    {
        decimal _transportCharges;
        public decimal TransportCharges
        {
            get { return _transportCharges; }
            set
            {
                _transportCharges = value;
                OnPropertyChanged("TransportCharges");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal Calculate(decimal value)
        {
            return value +=  TransportCharges;                        
        }

        public void Clear()
        {
            TransportCharges = 0;
        }

        public decimal GetPropertyValue(string propertyName)
        {
            switch(propertyName)
            {
                case "TransportCharges":
                    return TransportCharges;
            }
            return 0;
        }

        public void SetValues(params decimal[] extensionValues)
        {
            if(extensionValues !=null && extensionValues.Length > 1)
            TransportCharges = extensionValues[0];
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
