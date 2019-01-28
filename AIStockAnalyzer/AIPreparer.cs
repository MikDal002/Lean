using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using QuantConnect.Data.Market;

namespace AIStockAnalyzer
{
    class AIPreparer
    {
        enum Relation
        {
            NextIsGreater,
            NextIsLower,
            Equal
        }


        private List<TradeBar> _analyzedBars;
        private List<TradeBar> _testBars;

        public int Count { get; private set; }

        public AIPreparer(int count, string path)
        {
            Count = count;
            _analyzedBars = new List<TradeBar>(Count);
            _testBars = new List<TradeBar>(Count);
        }

        public bool AddToAnalyze(TradeBar data)
        {
            if (_analyzedBars.Count < Count)
            {
                _analyzedBars.Add(data);
                return false;
            }

            if (_testBars.Count < Count)
            {
                _testBars.Add(data);
                return false;
            }


            bool satisfaied = true;
            Relation previousRelation = Relation.Equal;

            for (int i = 1; i < _analyzedBars.Count; i++)
            {
                if (_analyzedBars[i].Close > _analyzedBars[i - 1].Close && (previousRelation == Relation.Equal || previousRelation == Relation.NextIsGreater))
                {
                    previousRelation = Relation.NextIsGreater;
                }
                else if (_analyzedBars[i].Close < _analyzedBars[i - 1].Close && (previousRelation == Relation.Equal || previousRelation == Relation.NextIsLower))
                {
                    previousRelation = Relation.NextIsLower;
                }
                else if (_analyzedBars[i].Close == _analyzedBars[i - 1].Close)
                {
                    previousRelation = Relation.Equal;
                }
                else
                {
                    satisfaied = false;
                    break;
                }
            }

            switch (previousRelation)
            {
                case Relation.NextIsGreater:
                    satisfaied &= _analyzedBars.Last().Close < _testBars.OrderBy(x => x.Close).Skip(_testBars.Count() / 2).First().Close;
                    break;
                case Relation.NextIsLower:
                    try
                    {
                        satisfaied &= _analyzedBars
                                        .Last()
                                        .Close 
                                        >
                                      _testBars
                                          .OrderBy(x => x.Close).Skip(_testBars.Count() / 2).First()
                                          .Close;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("ho hoho ");
                    }
                    break;
                case Relation.Equal:
                    satisfaied = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (satisfaied && _analyzedBars.Count == 3)
            {
                Debug.WriteLine($"Potwierdzono sekwencję w dniu {data.EndTime.ToShortDateString()}");
                Debug.WriteLine($"Próbki poddane analizie to ({_analyzedBars.Count}) w zakresie od {_analyzedBars.First().EndTime.ToShortDateString()} do {_analyzedBars.Last().EndTime.ToShortDateString()}:");
                foreach (var foo in _analyzedBars)
                {
                    Debug.WriteLine($"\t {foo.EndTime.ToShortDateString()} z ceną zamknięcia {foo.Close}");
                }
                Debug.WriteLine($"Próbki testowe to ({_testBars.Count}) w zakresie od {_testBars.First().EndTime.ToShortDateString()} do {_testBars.Last().EndTime.ToShortDateString()}:");
                foreach (var foo in _testBars)
                {
                    Debug.WriteLine($"\t {foo.EndTime.ToShortDateString()} z ceną zamknięcia {foo.Close}");
                }
                Debug.WriteLine($"Próbki pozostawały w relacji {previousRelation}");
            }

            _analyzedBars.RemoveAt(0);
            _analyzedBars.Add(_testBars[0]);
            _testBars.RemoveAt(0);
            _testBars.Add(data);

            return satisfaied;
        }
    }
}
