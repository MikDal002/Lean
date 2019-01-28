using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using QuantConnect.Data.Market;

namespace AIStockAnalyzer
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl, INotifyPropertyChanged
    {
        private string[] _labels;
        ChartValues<OhlcPoint> StockValues = new ChartValues<OhlcPoint>();
        ChartValues<double> StockVolumen = new ChartValues<double>();
        public ChartControl()
        {
            InitializeComponent();
            
            var ohlcSeries = new OhlcSeries()
            {
                Values = StockValues,
                ScalesYAt = 0
            };
            var volumentSeries = new LineSeries()
            {
                Values = StockVolumen,
                Fill = Brushes.Transparent,
                ScalesYAt = 1
            };

            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(ohlcSeries);
            SeriesCollection.Add(volumentSeries);

            //Labels = new[]
            //{
            //    DateTime.Now.ToString("dd MMM"),
            //    DateTime.Now.AddDays(1).ToString("dd MMM"),
            //    DateTime.Now.AddDays(2).ToString("dd MMM"),
            //    DateTime.Now.AddDays(3).ToString("dd MMM"),
            //    DateTime.Now.AddDays(4).ToString("dd MMM"),
            //};
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; private set; }
        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged("Labels");
            }
        }

        private void UpdateAllOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();

            foreach (var point in SeriesCollection[0].Values.Cast<OhlcPoint>())
            {
                point.Open = r.Next((int)point.Low, (int)point.High);
                point.Close = r.Next((int)point.Low, (int)point.High);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddDataWithDispatcher(Bar bar, double volume)
        {
            AddDataWithDispatcher(
                (double)bar.Open,
                (double)bar.High,
                (double)bar.Low,
                (double)bar.Close,
                (double)volume);
        }

        public void AddDataWithDispatcher(TradeBar tradeBar)
        {
            AddDataWithDispatcher(
                (double) tradeBar.Open,
                (double) tradeBar.High,
                (double) tradeBar.Low,
                (double) tradeBar.Close,
                (double) tradeBar.Volume);
        }

        private void AddDataWithDispatcher(double open, double high, double low, double close, double volume)
        {
            Dispatcher.Invoke(
                () =>
                {
                    StockValues.Add(new OhlcPoint(open, high, low, close));
                    StockVolumen.Add(volume);
                });
        }
    }
}

