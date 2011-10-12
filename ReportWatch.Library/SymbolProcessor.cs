using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class SymbolProcessor : IDisposable
    {

        private ReportWatchEntities _entities;
        private DateTime _reportDate = new DateTime();
        private List<string> symbolNameList = null;
        public delegate void Completed();
        public event Completed OnCompleted;

        public SymbolProcessor(DateTime reportDate)
        {
            this._entities = new ReportWatchEntities();
            this._reportDate = reportDate;
        }

        public void Process()
        {
            try
            {
                SymbolDownloader symbolDownloader = new SymbolDownloader(DateTime.Parse("2011-10-11"));
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
                    symbol.DateReport = _reportDate;
                }

                _entities.SaveChanges();

                // Keep track of progress on the list of symbols
                symbolNameList = (from s in symbolEnumerable select s.SymbolName).ToList<string>();

                // Retrieve the report history
                foreach (Symbol symbol in symbolEnumerable)
                {
                    ReportDownloader reportDownloader = new ReportDownloader(symbol.SymbolName);
                    reportDownloader.OnLoadDataComplete += new ReportDownloader.LoadDataCompleted(reportDownloader_OnLoadDataComplete);
                    ReportDownloaderQueue reportDownloaderQueue = ReportDownloaderQueue.Instance(4000);
                    reportDownloaderQueue.Enqueue(reportDownloader);
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

                // Get the DayPrice history
                DayPriceDownloader dayPriceDownloader = new DayPriceDownloader(reportDownloader.SymbolName);
                dayPriceDownloader.OnLoadDataComplete += new DayPriceDownloader.LoadDataCompleted(dayPriceDownloader_OnLoadDataComplete);
                DayPriceDownloaderQueue dayPriceDownloaderQueue = DayPriceDownloaderQueue.Instance(6000);
                dayPriceDownloaderQueue.Enqueue(dayPriceDownloader);
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
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
                        System.Threading.Thread.Sleep(5); // Don't go too fast, or GUIDs are not unique.
                        _entities.DayPriceSet.AddObject(dayPrice);
                    }
                }

                _entities.SaveChanges();

                // Check for process completion
                symbolNameList.Remove(dayPriceDownloader.SymbolName);
                if (symbolNameList.Count == 0)
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

        #region IDisposable Members

        public void Dispose()
        {
            _entities.Dispose();
        }

        #endregion
    }
}
