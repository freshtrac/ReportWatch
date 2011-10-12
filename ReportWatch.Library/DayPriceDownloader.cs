using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ReportWatch.Database;
using System.Text.RegularExpressions;
using ReportWatch.Library;

namespace ReportWatch.Library
{
    public class DayPriceDownloader : IDisposable
    {
        private WebClient _webClient;
        private const string Pattern = @"^(?<Year>\d{4})-(?<Month>\d{2})-(?<Day>\d{2}),(?<Open>[\d\.]+?),(?<High>[\d\.]+?),(?<Low>[\d\.]+?),(?<Close>[\d\.]+?),(?<Volume>[\d]+?),(?<AdjClose>[\d\.]+)";

        private String _symbolName = string.Empty;
        public String SymbolName
        {
            get
            {
                return _symbolName;
            }
            set
            {
                _symbolName = value;
            }
        }

        private DateTime _dateStart = DateTime.Today.AddYears(-3);
        public DateTime DateStart
        {
            get
            {
                return _dateStart;
            }
            set
            {
                _dateStart = value;
            }
        }

        private DateTime _dateEnd = DateTime.Today;
        public DateTime DateEnd
        {
            get
            {
                return _dateEnd;
            }
            set
            {
                _dateEnd = value;
            }
        }

        public delegate void Started();
        public event Started OnStart;

        public delegate void Error(DayPriceDownloader DayPriceDownloader, Exception ex);
        public event Error OnError;

        public delegate void LoadDataCompleted(DayPriceDownloader DayPriceDownloader, List<DayPrice> DayPriceList);
        public event LoadDataCompleted OnLoadDataComplete;

        public DayPriceDownloader(String symbolName)
        {
            this.SymbolName = symbolName;

            _webClient = new WebClient();
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebClientDownloadStringCompleted);
        }

        public void Download()
        {
            if (OnStart != null)
            {
                OnStart();
            }

            try
            {
                // Get the historical quotes from Yahoo Finance.
                String uriString = String.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=d&ignore=.csv",
                                                SymbolName,
                                                DateStart.Month - 1,
                                                DateStart.Day,
                                                DateStart.Year,
                                                DateEnd.Month - 1,
                                                DateEnd.Day,
                                                DateEnd.Year);

                Uri uri = new Uri(uriString);
                _webClient.DownloadStringAsync(uri);
            }
            catch (Exception ex)
            {
                if (OnError != null) { OnError(this, ex); }
            }
        }

        private void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    string s = e.Result.ToString();
                    List<DayPrice> DayPriceList = new List<DayPrice>();

                    Regex re = new Regex(Pattern, RegexOptions.Multiline);
                    MatchCollection mc = re.Matches(s);
                    {
                        foreach (Match m in mc)
                        {
                            DayPrice DayPrice = new DayPrice();
                            DayPrice.DayPriceId = Guid.NewGuid();
                            DayPrice.SymbolName = this.SymbolName;
                            int year = Conversion.StringToInt32(m.Groups["Year"].Value.ToString());
                            int month = Conversion.StringToInt32(m.Groups["Month"].Value.ToString());
                            int day = Conversion.StringToInt32(m.Groups["Day"].Value.ToString());
                            DayPrice.DayPriceDate = new DateTime(year, month, day);
                            DayPrice.DayPriceOpen = Conversion.StringToDecimal(m.Groups["Open"].Value.ToString());
                            DayPrice.DayPriceHigh = Conversion.StringToDecimal(m.Groups["High"].Value.ToString());
                            DayPrice.DayPriceLow = Conversion.StringToDecimal(m.Groups["Low"].Value.ToString());
                            DayPrice.DayPriceClose = Conversion.StringToDecimal(m.Groups["Close"].Value.ToString());
                            DayPrice.DayPriceVolume = Conversion.StringToInt64(m.Groups["Volume"].Value.ToString());
                            DayPrice.DayPriceAdjustedClose = Conversion.StringToDecimal(m.Groups["AdjClose"].Value.ToString());
                            DayPriceList.Add(DayPrice);
                        }
                    }

                    if (OnLoadDataComplete != null)
                    {
                        OnLoadDataComplete(this, DayPriceList);
                    }
                }
                else
                {
                    if (OnError != null){OnError(this, e.Error);}
                }
            }
            catch (Exception ex)
            {
                if (OnError != null) { OnError(this, ex); }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _webClient.Dispose();
        }

        #endregion
    }
}
