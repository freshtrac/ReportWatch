using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class DayPriceDownloaderQueue : Queue<DayPriceDownloader>
    {

        #region Constructor (Singleton)

        private static DayPriceDownloaderQueue _instance;
        System.Timers.Timer _timer = null;
        private int _milliseconds = 1000;
        private bool _ready = true;

        protected DayPriceDownloaderQueue()
        {
            StartPolling();
        }

        public static DayPriceDownloaderQueue Instance(int milliseconds)
        {
            if (_instance == null)
            {
                _instance = new DayPriceDownloaderQueue();
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
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_ready)
                {
                    if (this.Count > 0)
                    {
                        _ready = false; // Wait until this one is complete before doing the next one
                        DayPriceDownloader downloader = this.Dequeue();
                        downloader.OnLoadDataComplete += new DayPriceDownloader.LoadDataCompleted(downloader_OnLoadDataComplete);
                        downloader.OnError += new DayPriceDownloader.Error(downloader_OnError);
                        downloader.Download();
                    }
                }
            }
            catch (Exception ex)
            {
                LogOps.LogException(ex);
            }
        }

        void downloader_OnError(DayPriceDownloader DayPriceDownloader, Exception ex)
        {
            DayPriceDownloader.Dispose();
            LogOps.LogException(ex);
            _ready = true; // Skip it and move on
        }

        void downloader_OnLoadDataComplete(DayPriceDownloader DayPriceDownloader, List<DayPrice> DayPriceList)
        {
            _ready = true; // Ready to process another one
        }

        #endregion

    }
}