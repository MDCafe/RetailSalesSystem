using Microsoft.Reporting.WinForms;
using RetailManagementSystem.Command;
using RetailManagementSystem.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RetailManagementSystem.ViewModel
{
    internal class ReportViewModel : ReportBaseViewModel
    {
        private ReportViewer _rptViewer;
        private ReportDataSource[] _rptDataSource;
        private string _reportPath;

        public ReportViewModel(bool showResctricteCustomers, string title, ReportDataSource[] rptDataSource,
                              string reportPath) 
            : base(showResctricteCustomers)
        {
            this.Title = title;
            _rptDataSource = rptDataSource;
            _reportPath = reportPath;
        }

        public ReportViewer RptViewer
        {
            get
            {
                return _rptViewer;
            }

            set
            {
                _rptViewer = value;

                foreach (var dataSource in _rptDataSource)
                {
                    _rptViewer.LocalReport.DataSources.Add(dataSource);
                }
                _rptViewer.LocalReport.ReportPath = _reportPath;

                _rptViewer.RefreshReport();
            }
        }
    }
}
