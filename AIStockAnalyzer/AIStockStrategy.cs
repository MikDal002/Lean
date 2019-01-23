using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuantConnect;
using QuantConnect.Data.Market;
using QuantConnect.Algorithm;
using QuantConnect.Indicators;
using QuantConnect.Securities;

namespace AIStockAnalyzer
{
    /// <summary>
    /// Uses daily data and a simple moving average cross to place trades and an ema for stop placement
    /// </summary>
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="indicators" />
    /// <meta name="tag" content="trading and orders" />
    public class AIStockStrategy : QCAlgorithm
    {
        private readonly Symbol _ibm = QuantConnect.Symbol.Create("IBM", SecurityType.Equity, Market.USA);
        private readonly Symbol _spy = QuantConnect.Symbol.Create("SPY", SecurityType.Equity, Market.USA);

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        public override void Initialize()
        {
            //SetStartDate(2006, 01, 01);  //Set Start Date
            //SetEndDate(2018, 01, 01);    //Set End Date

            SetStartDate(DateTime.Now.Subtract(TimeSpan.FromDays(30)));
            SetEndDate(DateTime.Now);
            SetCash(100000);             //Set Strategy Cash

            // Find more symbols here: http://quantconnect.com/data
            //AddSecurity(SecurityType.Equity, "IBM", Resolution.Hour);
            //AddSecurity(SecurityType.Equity, "SPY", Resolution.Daily);
            AddCfd("XAUGBP", Resolution.Daily, Market.Oanda);


            //_macd = MACD(_spy, 12, 26, 9, MovingAverageType.Wilders, Resolution.Daily, Field.Close);
            //_ema = EMA(_ibm, 15 * 6, Resolution.Hour, Field.SevenBar);

            //Securities[_ibm].SetLeverage(1.0m);
        }

        /// <summary>
        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// </summary>
        /// <param name="data">TradeBars IDictionary object with your stock data</param>
        public void OnData(TradeBars data)
        {
            TradeBar current = data["XAUGPB"];

        }
    }

    public class CsvWriter
    {
        private readonly string _filePath;

        public CsvWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void AddData(TradeBar data)
        {
            
        }
    }
}
