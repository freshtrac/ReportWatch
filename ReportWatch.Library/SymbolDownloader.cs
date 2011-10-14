using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class SymbolDownloader : IDisposable
    {
        private WebClient _webClient;
        private const string Pattern1 = @"<td>(?<Company>.+?)</td><td><a href=\""http://finance.yahoo.com/q\?s=(?<Ticker>[a-z0-9\.]+?)\"">(?<Ticker2>[A-Z0-9\.]+?)</a></td><td\s*align=\""*center\""*>(?<Eps>.+?)</td><td\s*align=\""*center\""*><small>(?<Time>.+?)</small></td>";
        private const string Pattern2 = @"<td>(?<Company>.+?)</td><td><a href=\""http://finance.yahoo.com/q\?s=(?<Ticker>[a-z0-9\.]+?)\"">(?<Ticker2>[A-Z0-9\.]+?)</a></td><td\s*align=\""*center\""*><small>(?<Time>.+?)</small></td></tr>";

        private DateTime _reportDate = new DateTime();
        public DateTime ReportDate
        {
            get
            {
                return _reportDate;
            }
            set
            {
                _reportDate = value;
            }
        }

        public delegate void Started();
        public event Started OnStart;

        public delegate void Error(SymbolDownloader symbolDownloader, Exception ex);
        public event Error OnError;

        public delegate void LoadDataCompleted(SymbolDownloader symbolDownloader, List<Symbol> symbolList);
        public event LoadDataCompleted OnLoadDataComplete;

        public SymbolDownloader(DateTime reportDate)
        {
            this.ReportDate = reportDate;

            _webClient = new WebClient();
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebClientDownloadStringCompleted);
        }

        public void Download()
        {
            try
            {
                if (OnStart != null)
                {
                    OnStart();
                }

                // Get the earnings Symbol from Yahoo Finance.
                String uriString = String.Format("http://biz.yahoo.com/research/earncal/{0:0000}{1:00}{2:00}.html",
                                                    ReportDate.Year,
                                                    ReportDate.Month,
                                                    ReportDate.Day);

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
                    string s = e.Result.ToString().Replace(Environment.NewLine,string.Empty);
                    List<Symbol> symbolList = new List<Symbol>();

                    Regex re = null;
                    if (ReportDate >= DateTime.Today)
                    {
                        re = new Regex(Pattern1, RegexOptions.Multiline);
                    }
                    else
                    {
                        re = new Regex(Pattern2, RegexOptions.Multiline);
                    }
                    
                    MatchCollection mc = re.Matches(s);
                    {
                        foreach (Match m in mc)
                        {
                            Symbol symbol = new Symbol();
                            symbol.CompanyName = m.Groups["Company"].Value.ToString();
                            symbol.SymbolName = m.Groups["Ticker2"].Value.ToString();
                            if (m.Groups["Time"].Value.ToString().Equals("After Market Close"))
                            {
                                symbol.ReportDate = ReportDate.AddDays(1);
                            }
                            else
                            {
                                symbol.ReportDate = ReportDate;
                            }
                            symbolList.Add(symbol);
                        }
                    }

                    if (OnLoadDataComplete != null)
                    {
                        OnLoadDataComplete(this, symbolList);
                    }
                }
                else
                {
                    if (OnError != null) { OnError(this, e.Error); }
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
