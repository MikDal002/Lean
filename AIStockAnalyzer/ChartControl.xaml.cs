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
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace AIStockAnalyzer
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl, INotifyPropertyChanged
    {
        private List<string> _labels = new List<string>();
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

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; private set; }
        public List<string> Labels
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

        public void AddDataWithDispatcher(TradeBar tradeBar)
        {
            AddDataWithDispatcher(
                (double)tradeBar.Open,
                (double)tradeBar.High,
                (double)tradeBar.Low,
                (double)tradeBar.Close,
                (double)tradeBar.Volume,
                tradeBar.EndTime);
        }

        private void AddDataWithDispatcher(double open, double high, double low, double close, double volume, DateTime date)
        {
            Dispatcher.Invoke(
                () =>
                {
                    StockValues.Add(new OhlcPoint(open, high, low, close));
                    StockVolumen.Add(volume);
                    Labels.Add(date.ToShortDateString());
                });
        }

        public void SetMarkOnChart(TradeBar tradeBar, int value)
        {
            Dispatcher.Invoke(
                () =>
                {
                    Chart.VisualElements.Add(new VisualElement
                    {
                        AxisY = 2,
                        // tak aby wyświetliło sie nad ostatnią testowaną próbką 
                        X = StockValues.Count - value - 1,
                        Y = value,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        UIElement = new TextBlock
                        {
                            Text = $"{value}",
                            FontWeight = FontWeights.Bold,
                            FontSize = 16
                        }
                    });

                    // zaznaczamy próbki poddane analizie
                    for (int i = value - 1; i > 0; i--)
                    {
                        Chart.VisualElements.Add(new VisualElement
                        {
                            AxisY = 2,
                            // tak aby wyświetliło sie nad ostatnią testowaną próbką 
                            X = StockValues.Count - value - 1 - i,
                            Y = value,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            UIElement = new TextBlock
                            {
                                Text = "–",
                                FontWeight = FontWeights.Bold,
                                FontSize = 16
                            }
                        });
                    }
                });
        }
    }
}

