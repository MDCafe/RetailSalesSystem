using RetailManagementSystem.Command;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel.Reports
{
    public class ReportBusinessBaseVM : ReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public Company SelectedCompany { get; set; }
        public Category SelectedCategory { get; set; }
        public Product SelectedProduct { get; set; }

        public IEnumerable<Company> Companies { get; set; }

        public IEnumerable<Category> ProductCategories { get; set; }
        public IEnumerable<Product> ProductList { get; set; }

        public ReportBusinessBaseVM(string reportTitle) : base(false, false, reportTitle)
        {
            var sysDate = RMSEntitiesHelper.GetServerDate();
            FromDate = sysDate;
            ToDate = sysDate;
        }

        override internal void Clear()
        {
            SelectedCategory = null;
            SelectedCompany = null;
            SelectedProduct = null;
            var sysDate = RMSEntitiesHelper.GetServerDate();
            FromDate = sysDate;
            ToDate = sysDate;
        }

        #region Print Command
        RelayCommand<Window> _printCommand = null;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new RelayCommand<Window>((w) => OnPrint(w));
                }

                return _printCommand;
            }
        }

        public virtual void OnPrint(Window window)
        {

        }
        #endregion
    }
}
