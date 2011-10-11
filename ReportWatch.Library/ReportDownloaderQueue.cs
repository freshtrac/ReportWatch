using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class ReportDownloaderQueue : Queue<ReportDownloader>
    {

        #region Constructor (Singleton)

        private static ReportDownloaderQueue _instance;
        System.Timers.Timer _timer = null;
        private int _milliseconds = 1000;
        private bool _ready = true;

        protected ReportDownloaderQueue()
        {
            StartPolling();
        }

        public static ReportDownloaderQueue Instance(int milliseconds)
        {
            if (_instance == null)
            {
                _instance = new ReportDownloaderQueue();
            }

            _instance._milliseconds = milliseconds;

            return _instance;
        }

        #endregion

        # region Polling

        public void StartPolling()
        {
            try
            {
                // Start the polling timer.
                _timer = new System.Timers.Timer(_milliseconds);
                _timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
                _timer.Start();
            }
            catch
            {
                throw;
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_ready)
            {
                if (this.Count > 0)
                {
                    _ready = false; // Wait until this one is complete before doing the next one
                    ReportDownloader downloader = this.Dequeue();
                    downloader.OnLoadDataComplete += new ReportDownloader.LoadDataCompleted(downloader_OnLoadDataComplete);
                    downloader.OnError += new ReportDownloader.Error(downloader_OnError);
                    downloader.Download();
                }
            }
        }

        void downloader_OnError(ReportDownloader ReportDownloader, Exception ex)
        {
            ReportDownloader.Dispose();
            _ready = true; // Skip it and move on
        }

        void downloader_OnLoadDataComplete(ReportDownloader ReportDownloader, List<Report> ReportList)
        {
            _ready = true; // Ready to process another one
        }

        #endregion

    }
}
