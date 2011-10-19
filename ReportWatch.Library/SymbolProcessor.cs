using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class SymbolProcessor : IDisposable
    {

        private static ReportWatchEntities _entities;
        private DateTime _reportDate = new DateTime();
        private List<string> _symbolNameList = null;
        private List<string> _symbolNameListReport = null;
        private List<string> _symbolNameListDayPrice = null;
        public delegate void Completed();
        public event Completed OnCompleted;

        public SymbolProcessor(DateTime reportDate)
        {
            _entities = new ReportWatchEntities();
            this._reportDate = reportDate;
        }

        public void Process()
        {
            try
            {
                SymbolDownloader symbolDownloader = new SymbolDownloader(_reportDate);
                symbolDownloader.OnLoadDataComplete += new SymbolDownloader.LoadDataCompleted(symbolDownloader_OnLoadDataComplete);
                SymbolDownloaderQueue symbolDownloaderQueue = SymbolDownloaderQueue.Instance(1000);
                symbolDownloaderQueue.Enqueue(symbolDownloader);
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        void symbolDownloader_OnLoadDataComplete(SymbolDownloader symbolDownloader, List<Symbol> symbolList)
        {
            try
            {
                IEnumerable<string> symbolNameEnumerable = from symbol in symbolList select symbol.SymbolName;

                // Consider only those symbols that are in major market indexes
                IEnumerable<Symbol> symbolEnumerable = _entities.SymbolSet.Where(symbol => symbolNameEnumerable.Contains(symbol.SymbolName));

                // Set the next report date
                foreach (Symbol symbol in symbolEnumerable)
                {
                    symbol.ReportDate = _reportDate;
                }

                _entities.SaveChanges();

                // Keep track of progress on the list of symbols
                _symbolNameList = (from s in symbolEnumerable select s.SymbolName).ToList<string>();
                _symbolNameListReport = (from s in symbolEnumerable select s.SymbolName).ToList<string>();
                _symbolNameListDayPrice = (from s in symbolEnumerable select s.SymbolName).ToList<string>();

                // Load the price history for the market indexes
                dayPriceDownloader_Enqueue("^DJI",3000);
                dayPriceDownloader_Enqueue("^IXIC",3000);
                dayPriceDownloader_Enqueue("^GSPC",3000);
                System.Threading.Thread.Sleep(10000);

                // Retrieve the report history
                foreach (String symbolName in _symbolNameList)
                {
                    ReportDownloader reportDownloader = new ReportDownloader(symbolName);
                    reportDownloader.OnLoadDataComplete += new ReportDownloader.LoadDataCompleted(reportDownloader_OnLoadDataComplete);
                    ReportDownloaderQueue reportDownloaderQueue = ReportDownloaderQueue.Instance(5000);
                    reportDownloaderQueue.Enqueue(reportDownloader);
                }

                if (_symbolNameList.Count() == 0)
                {
                    if (OnCompleted != null) OnCompleted();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        void reportDownloader_OnLoadDataComplete(ReportDownloader reportDownloader, List<Report> reportList)
        {
            try
            {
                // Check for existing reports
                List<DateTime> reportDateList = (from r in _entities.ReportSet where r.SymbolName.Equals(reportDownloader.SymbolName) select r.ReportDate).ToList<DateTime>();

                // Insert new reports
                foreach (Report report in reportList)
                {
                    if (!reportDateList.Contains(report.ReportDate))
                    {
                        System.Threading.Thread.Sleep(5); // Don't go too fast, or GUIDs are not unique.
                        _entities.ReportSet.AddObject(report);
                    }
                }

                _entities.SaveChanges();

                // Check for process completion
                if (_symbolNameListReport.Contains(reportDownloader.SymbolName)) _symbolNameListReport.Remove(reportDownloader.SymbolName);
                if (_symbolNameListReport.Count < 1)
                {
                    // Get the DayPrice history
                    foreach (String symbolName in _symbolNameList)
                    {
                        dayPriceDownloader_Enqueue(symbolName, 6000);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        private void dayPriceDownloader_Enqueue(String symbolName, int miliseconds)
        {
            DayPriceDownloader dayPriceDownloader = new DayPriceDownloader(symbolName);
            dayPriceDownloader.OnLoadDataComplete += new DayPriceDownloader.LoadDataCompleted(dayPriceDownloader_OnLoadDataComplete);
            DayPriceDownloaderQueue dayPriceDownloaderQueue = DayPriceDownloaderQueue.Instance(miliseconds);
            dayPriceDownloaderQueue.Enqueue(dayPriceDownloader);
        }

        void dayPriceDownloader_OnLoadDataComplete(DayPriceDownloader dayPriceDownloader, List<DayPrice> dayPriceList)
        {
            try
            {
                // Check for existing prices
                List<DateTime> dayPriceDateList = (from dp in _entities.DayPriceSet where dp.SymbolName.Equals(dayPriceDownloader.SymbolName) select dp.DayPriceDate).ToList<DateTime>();

                // Insert new reports
                foreach (DayPrice dayPrice in dayPriceList)
                {
                    if (!dayPriceDateList.Contains(dayPrice.DayPriceDate))
                    {
                        System.Threading.Thread.Sleep(10); // Don't go too fast, or GUIDs are not unique.
                        _entities.DayPriceSet.AddObject(dayPrice);
                    }
                }

                _entities.SaveChanges();

                // Check for process completion
                if(_symbolNameListDayPrice.Contains(dayPriceDownloader.SymbolName)) _symbolNameListDayPrice.Remove(dayPriceDownloader.SymbolName);
                if (_symbolNameListDayPrice.Count < 1)
                {
                    _entities.SetDayPriceHigh(_reportDate);

                    if (OnCompleted != null) OnCompleted();

                    System.Threading.Thread.Sleep(2000);

                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _entities.Dispose();
        }

        #endregion
    }
}
