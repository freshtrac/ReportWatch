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

        public SymbolProcessor(DateTime reportDate)
        {
            this._entities = new ReportWatchEntities();
            this._reportDate = reportDate;
            Process();
        }

        private void Process()
        {
            SymbolDownloader symbolDownloader = new SymbolDownloader(DateTime.Parse("2011-10-11"));
            symbolDownloader.OnLoadDataComplete += new SymbolDownloader.LoadDataCompleted(symbolDownloader_OnLoadDataComplete);
            SymbolDownloaderQueue symbolDownloaderQueue = SymbolDownloaderQueue.Instance(1000);
            symbolDownloaderQueue.Enqueue(symbolDownloader);
        }

        void symbolDownloader_OnLoadDataComplete(SymbolDownloader symbolDownloader, List<Symbol> symbolList)
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

            // Retrieve the report history
            foreach (Symbol symbol in symbolEnumerable)
            {
                ReportDownloader reportDownloader = new ReportDownloader(symbol.SymbolName);
                reportDownloader.OnLoadDataComplete += new ReportDownloader.LoadDataCompleted(reportDownloader_OnLoadDataComplete);
                ReportDownloaderQueue reportDownloaderQueue = ReportDownloaderQueue.Instance(4000);
                reportDownloaderQueue.Enqueue(reportDownloader);
            }

        }

        void reportDownloader_OnLoadDataComplete(ReportDownloader ReportDownloader, List<Report> reportList)
        {
            foreach (Report report in reportList)
            {

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
