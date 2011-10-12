using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ReportWatch.Database;
using System.Text.RegularExpressions;

namespace ReportWatch.Library
{
    public class ReportDownloader : IDisposable
    {
        private WebClient _webClient;
        private const string Pattern = @"^<tr class=""bgWhite"">\s+<td align=""center"" width=""50""><nobr>(?<Ticker>.+)\s+[&#160;]*</nobr></td>\s+<td align=""center"">(?<Quarter>Q\d{1})[&#160;]*(?<Year>\d{4})</td>\s+<td align=""left"" width=""\*"">(?<Title>.+?)</td>\s+<td align=""center"">\$*\s*(?<Estimate>.+?)[&#160;]*</td>\s+<td align=""center"">\$*\s*(?<Actual>.+?)[&#160;]*</td>\s+<td align=""center"">\$*\s*(?<Previous>.+?)[&#160;]*</td>\s+<td align=""center""><nobr>(?<Date>\d+?\-.+?\-\d{2})\s*(?<Time>.*?)</nobr></td>\s+</tr>";

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

        public delegate void Started();
        public event Started OnStart;

        public delegate void Error(ReportDownloader ReportDownloader, Exception ex);
        public event Error OnError;

        public delegate void LoadDataCompleted(ReportDownloader ReportDownloader, List<Report> reportList);
        public event LoadDataCompleted OnLoadDataComplete;

        public ReportDownloader(String symbolName)
        {
            this.SymbolName = symbolName;

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

                // Scrape the Earnings Reports from earnings.com
                String uriString = String.Format("http://www.earnings.com/company.asp?client=cb&ticker={0}", this.SymbolName);

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
                    List<Report> reportList = new List<Report>();

                    Regex re = new Regex(Pattern, RegexOptions.Multiline);
                    MatchCollection mc = re.Matches(s);
                    {
                        foreach (Match m in mc)
                        {
                            Report report = new Report();
                            report.ReportId = Guid.NewGuid();
                            report.SymbolName = this.SymbolName;
                            report.ReportName = m.Groups["Quarter"].Value.ToString() + " " + m.Groups["Year"].Value.ToString();
                            report.ReportTitle = m.Groups["Title"].Value.ToString();
                            report.ReportDate = DateTime.Parse(m.Groups["Date"].Value.ToString());
                            if (m.Groups["Time"].Value.ToString().Contains("AMC")) report.ReportDate = report.ReportDate.AddDays(1);
                            report.ReportExpected = Conversion.StringToDecimal(m.Groups["Estimate"].Value.ToString());
                            report.ReportActual = Conversion.StringToDecimal(m.Groups["Actual"].Value.ToString());
                            report.ReportPreviousYear = Conversion.StringToDecimal(m.Groups["Previous"].Value.ToString());
                            reportList.Add(report);
                        }
                    }

                    if (OnLoadDataComplete != null)
                    {
                        OnLoadDataComplete(this, reportList);
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
