using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace RetailManagementSystem.View.Graphs
{
    /// <summary>
    /// Interaction logic for SalesGraphToolTip.xaml
    /// </summary>
    public partial class SalesGraphToolTip : IChartTooltip
    {
        private TooltipData _data;

        public SalesGraphToolTip()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
