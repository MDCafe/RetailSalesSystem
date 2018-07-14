using LiveCharts;
using RetailManagementSystem.ViewModel.Base;
using System.Collections.Generic;

namespace RetailManagementSystem.ViewModel.Graphs
{
    class GraphsViewModel : DocumentViewModel
    {
        public SeriesCollection GraphSeriesCollection { get; set; }
        public string YAxisTitle { get; set; }
        public string XAxisTitle { get; set; }
        public List<string> XAxisLabels { get; set; }

        public GraphsViewModel()
        {
           
        }
    }
}
