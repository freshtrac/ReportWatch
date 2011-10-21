using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using ReportWatch.Database;
using System.Collections.Generic;
using System.Data.Services.Client;

namespace ReportWatch.Silverlight
{

    public class ReportWatchViewModel : ViewModelBase
    {

        private DataServiceContext context = null;

        #region Constructor

        // Methods
        public ReportWatchViewModel()
        {
            this.context = new DataServiceContext(new Uri("http://localhost:55555/ReportWatchService.svc", UriKind.Absolute));
            SymbolQueryBegin();
            IndexQueryBegin("^DJI");
            IndexQueryBegin("^GSPC");
            IndexQueryBegin("^IXIC");
        }

        #endregion

        #region Commands

        private Command _MoveNext;
        public ICommand MoveNext
        {
            get
            {
                if (this._MoveNext == null)
                {
                    this._MoveNext = new Command(p => true, delegate(object p)
                    {
                        this.SelectedDate = this.SelectedDate.AddDays(1.0);
                    });
                }
                return this._MoveNext;
            }
        }

        private Command _MovePrevious;
        public ICommand MovePrevious
        {
            get
            {
                if (this._MovePrevious == null)
                {
                    this._MovePrevious = new Command(p => true, delegate(object p)
                    {
                        this.SelectedDate = this.SelectedDate.AddDays(-1.0);
                    });
                }
                return this._MovePrevious;
            }
        }

        #endregion

        #region Properties

        private DateTime _SelectedDate = DateTime.Today.AddDays(1.0);
        public DateTime SelectedDate
        {
            get
            {
                return this._SelectedDate;
            }
            set
            {
                this._SelectedDate = value;
                base.NotifyPropertyChanged("SelectedDate");
                this.SymbolQueryBegin();
            }
        }

        private string _SymbolName = string.Empty;
        public string SymbolName
        {
            get
            {
                return this._SymbolName;
            }
            set
            {
                this._SymbolName = value;
                this.SelectedSymbol = (from s in this.SymbolCollection
                                       where s.SymbolName.Equals(this.SymbolName)
                                       select s).FirstOrDefault<Symbol>();
            }
        }

        private Symbol _SelectedSymbol = null;
        public Symbol SelectedSymbol
        {
            get
            {
                return this._SelectedSymbol;
            }
            set
            {
                this._SelectedSymbol = value;
                base.NotifyPropertyChanged("CompanyName");
                base.NotifyPropertyChanged("IndexChangeCollection");
                this.DayPriceQueryBegin(this._SelectedSymbol.SymbolName);
                this.ReportQueryBegin();
            }
        }

        public string CompanyName
        {
            get
            {
                string companyName = string.Empty;
                if (this.SelectedSymbol != null)
                {
                    companyName = this.SelectedSymbol.CompanyName;
                }
                return companyName;
            }
        }

        public DateTime MinimumDateTime
        {
            get
            {
                DateTime today = DateTime.Today;
                DateTime time2 = DateTime.Today;
                if (this.DayPriceCollection.Count > 0)
                {
                    time2 = (from d in this.DayPriceCollection.Cast<DayPrice>() select d.DayPriceDate).Min<DateTime>();
                }
                if (time2 < today)
                {
                    today = time2;
                }
                return today.AddMinutes(-30.0);
            }
        }

        public DateTime MaximumDateTime
        {
            get
            {
                DateTime time = DateTime.Today.AddDays(1.0);
                DateTime time2 = DateTime.Today.AddDays(1.0);
                if (this.DayPriceCollection.Count > 0)
                {
                    time2 = (from d in this.DayPriceCollection.Cast<DayPrice>() select d.DayPriceDate).Max<DateTime>();
                }
                if (time2 < time)
                {
                    time = time2;
                }
                return time.AddMinutes(30.0);
            }
        }

        public double MinimumDayPrice
        {
            get
            {
                double num = 0.0;
                if (this.DayPriceCollection.Count > 0)
                {
                    num = (double)(from d in this.DayPriceCollection.Cast<DayPrice>() select d.DayPriceLow).Min<decimal>();
                }
                return num;
            }
        }

        public double MaximumDayPrice
        {
            get
            {
                double num = 100.0;
                if (this.DayPriceCollection.Count > 0)
                {
                    num = (double)(from d in this.DayPriceCollection.Cast<DayPrice>() select d.DayPriceHigh).Max<decimal>();
                }
                return num;
            }
        }

        public double MinimumDayChange
        {
            get
            {
                double num = 0.0;
                if (this.DayChangeCollection.Count > 0)
                {
                    num = (double)(from d in this.DayChangeDifferenceCollection.Cast<DayPrice>() select d.DayPriceClose).Min<decimal>();
                }
                return num;
            }
        }

        public double MaximumDayChange
        {
            get
            {
                double num = 1.0;
                if (this.DayChangeCollection.Count > 0)
                {
                    num = (double)(from d in this.DayChangeDifferenceCollection.Cast<DayPrice>() select d.DayPriceClose).Max<decimal>();
                }
                return num;
            }
        }

        private bool _SymbolCollectionIsBusy = false;
        public bool SymbolCollectionIsBusy
        {
            get
            {
                return _SymbolCollectionIsBusy;
            }
            set
            {
                _SymbolCollectionIsBusy = value;
                base.NotifyPropertyChanged("SymbolCollectionIsBusy");
            }
        }

        public bool ChartIsBusy
        {
            get
            {
                return DayPriceQueryIsBusy | ReportQueryIsBusy | ChangeCalculationIsBusy;
            }
        }

        private bool _DayPriceQueryIsBusy = false;
        public bool DayPriceQueryIsBusy
        {
            get
            {
                return _DayPriceQueryIsBusy;
            }
            set
            {
                _DayPriceQueryIsBusy = value;
                base.NotifyPropertyChanged("ChartIsBusy");
            }
        }

        private bool _ReportQueryIsBusy = false;
        public bool ReportQueryIsBusy
        {
            get
            {
                return _ReportQueryIsBusy;
            }
            set
            {
                _ReportQueryIsBusy = value;
                base.NotifyPropertyChanged("ChartIsBusy");
            }
        }

        private bool _ChangeCalculationIsBusy = false;
        public bool ChangeCalculationIsBusy
        {
            get
            {
                return _ChangeCalculationIsBusy;
            }
            set
            {
                _ChangeCalculationIsBusy = value;
                base.NotifyPropertyChanged("ChartIsBusy");
            }
        }

        #endregion

        #region Collections

        private List<Symbol> _SymbolCollection = new List<Symbol>();
        public IOrderedEnumerable<Symbol> SymbolCollection
        {
            get
            {
                return (from s in this._SymbolCollection
                        orderby s.SymbolName ascending
                        select s);
            }
        }

        private ObservableCollection<DayPrice> _DayPriceCollection = new ObservableCollection<DayPrice>();
        public ObservableCollection<DayPrice> DayPriceCollection
        {
            get
            {
                return this._DayPriceCollection;
            }
            set
            {
                this._DayPriceCollection = value;
                base.NotifyPropertyChanged("DayPriceCollection");
                base.NotifyPropertyChanged("MinimumDateTime");
                base.NotifyPropertyChanged("MaximumDateTime");
                base.NotifyPropertyChanged("MinimumDayPrice");
                base.NotifyPropertyChanged("MaximumDayPrice");
            }
        }

        private ObservableCollection<Report> _ReportCollection = new ObservableCollection<Report>();
        public ObservableCollection<Report> ReportCollection
        {
            get
            {
                return this._ReportCollection;
            }
            set
            {
                this._ReportCollection = value;
                base.NotifyPropertyChanged("ReportCollection");
            }
        }

        private ObservableCollection<DayPrice> _DayChangeCollection = new ObservableCollection<DayPrice>();
        public ObservableCollection<DayPrice> DayChangeCollection
        {
            get
            {
                return this._DayChangeCollection;
            }
            set
            {
                this._DayChangeCollection = value;
                base.NotifyPropertyChanged("DayChangeCollection");
                base.NotifyPropertyChanged("MinimumDayChange");
                base.NotifyPropertyChanged("MaximumDayChange");
            }
        }

        private Dictionary<String, List<DayPrice>> _IndexCollectionDictionary = new Dictionary<string, List<DayPrice>>();
        private Dictionary<String, ObservableCollection<DayPrice>> _IndexChangeCollectionDictionary = new Dictionary<string, ObservableCollection<DayPrice>>();
        public ObservableCollection<DayPrice> IndexChangeCollection
        {
            get
            {
                if (SelectedSymbol != null)
                {
                    return this._IndexChangeCollectionDictionary[SelectedSymbol.IndexSymbolName];
                }
                else
                {
                    return new ObservableCollection<DayPrice>();
                }
            }
        }

        private ObservableCollection<DayPrice> _DayChangeDifferenceCollection = new ObservableCollection<DayPrice>();
        public ObservableCollection<DayPrice> DayChangeDifferenceCollection
        {
            get
            {
                return _DayChangeDifferenceCollection;
            }
            set
            {
                _DayChangeDifferenceCollection = value;
                base.NotifyPropertyChanged("DayChangeDifferenceCollection");
                base.NotifyPropertyChanged("MinimumDayChange");
                base.NotifyPropertyChanged("MaximumDayChange"); 
            }
        }

        #endregion

        #region Data Retrieval

        private void SymbolQueryBegin()
        {
            DataServiceQuery<Symbol> symbolQuery = (DataServiceQuery<Symbol>)
                from symbol in this.context.CreateQuery<Symbol>("SymbolSet")
                where symbol.ReportDate == this.SelectedDate
                orderby symbol.SymbolName ascending
                select symbol;
            symbolQuery.BeginExecute(new AsyncCallback(this.SymbolRequestCompleted), symbolQuery);
            SymbolCollectionIsBusy = true;
        }

        private void SymbolRequestCompleted(IAsyncResult asyncResult)
        {
            DataServiceQuery<Symbol> asyncState = asyncResult.AsyncState as DataServiceQuery<Symbol>;
            _SymbolCollection = asyncState.EndExecute(asyncResult).ToList<Symbol>();
            base.NotifyPropertyChanged("SymbolCollection");
            SymbolCollectionIsBusy = false;
        }

        private void ReportQueryBegin()
        {
            DateTime dateStart = DateTime.Parse("2007-07-01");
            DateTime dateEnd = DateTime.Today;
            DataServiceQuery<Report> reportQuery = (DataServiceQuery<Report>)
                from report in this.context.CreateQuery<Report>("ReportSet")
                where report.SymbolName == this.SymbolName
                where report.ReportDate > dateStart
                where report.ReportDate <= dateEnd
                orderby report.ReportDate ascending
                select report;
            reportQuery.BeginExecute(new AsyncCallback(this.ReportRequestCompleted), reportQuery);
            ReportQueryIsBusy = true;
        }

        private void ReportRequestCompleted(IAsyncResult asyncResult)
        {
            DataServiceQuery<Report> asyncState = asyncResult.AsyncState as DataServiceQuery<Report>;
            ReportCollection = new ObservableCollection<Report>(asyncState.EndExecute(asyncResult).ToList<Report>());
            ReportQueryIsBusy = false;
        }

        private void DayPriceQueryBegin(String symbolName)
        {
            DateTime dateStart = DateTime.Parse("2009-01-01");
            DateTime dateEnd = DateTime.Today;
            DataServiceQuery<DayPrice> dayPriceQuery = (DataServiceQuery<DayPrice>)
                from dayPrice in this.context.CreateQuery<DayPrice>("DayPriceSet")
                where dayPrice.SymbolName == symbolName
                where dayPrice.DayPriceDate > dateStart
                where dayPrice.DayPriceDate <= dateEnd
                orderby dayPrice.DayPriceDate ascending
                select dayPrice;
            dayPriceQuery.BeginExecute(new AsyncCallback(this.DayPriceRequestCompleted), dayPriceQuery);
            DayPriceQueryIsBusy = true;
            ChangeCalculationIsBusy = true;
        }

        private void DayPriceRequestCompleted(IAsyncResult asyncResult)
        {
            DataServiceQuery<DayPrice> asyncState = asyncResult.AsyncState as DataServiceQuery<DayPrice>;
            DayPriceCollection = new ObservableCollection<DayPrice>(asyncState.EndExecute(asyncResult).ToList<DayPrice>());
            DayPriceQueryIsBusy = false;
            CalculateDayPriceChange();
        }

        private void CalculateDayPriceChange()
        {
            List<DayPrice> dayChangeCollection = new List<DayPrice>();

            DayPrice previous = null;
            foreach (DayPrice dayPrice in DayPriceCollection)
            {
                if (previous != null)
                {
                    // Calculate percentage change from previous day
                    DayPrice dayPriceChange = new DayPrice();
                    dayPriceChange.DayPriceDate = dayPrice.DayPriceDate;
                    //if (previous.DayPriceOpen > 0) dayPriceChange.DayPriceOpen = ((dayPrice.DayPriceOpen - previous.DayPriceOpen) / previous.DayPriceOpen) * 100.0M;
                    //if (previous.DayPriceHigh > 0) dayPriceChange.DayPriceHigh = ((dayPrice.DayPriceHigh - previous.DayPriceHigh) / previous.DayPriceHigh) * 100.0M;
                    //if (previous.DayPriceLow > 0) dayPriceChange.DayPriceLow = ((dayPrice.DayPriceLow - previous.DayPriceLow) / previous.DayPriceLow) * 100.0M;
                    if (previous.DayPriceClose > 0) dayPriceChange.DayPriceClose = ((dayPrice.DayPriceClose - previous.DayPriceClose) / previous.DayPriceClose) * 100.0M;
                    //if (previous.DayPriceAdjustedClose > 0) dayPriceChange.DayPriceAdjustedClose = ((dayPrice.DayPriceAdjustedClose - previous.DayPriceAdjustedClose) / previous.DayPriceAdjustedClose) * 100.0M;
                    //if (previous.DayPriceVolume > 0) dayPriceChange.VolumeChange = ((double)((dayPrice.DayPriceVolume - previous.DayPriceVolume) / previous.DayPriceVolume)) * 100.0;
                    dayChangeCollection.Add(dayPriceChange);
                }
                previous = dayPrice;
            }

            DayChangeCollection = new ObservableCollection<DayPrice>(dayChangeCollection);

            CalculateDayPriceChangeDifference();
        }

        private void CalculateDayPriceChangeDifference()
        {
            List<DayPrice> dayChangeRelativeCollection = new List<DayPrice>();

            foreach (DayPrice dayPrice in DayChangeCollection)
            {
                DayPrice indexDayPrice = (from index in IndexChangeCollection where index.DayPriceDate == dayPrice.DayPriceDate select index).FirstOrDefault();

                // Calculate change relative to market index
                DayPrice dayPriceChangeRelative = new DayPrice();
                dayPriceChangeRelative.DayPriceDate = dayPrice.DayPriceDate;
                if (indexDayPrice!=null) dayPriceChangeRelative.DayPriceClose = dayPrice.DayPriceClose - indexDayPrice.DayPriceClose;
                dayChangeRelativeCollection.Add(dayPriceChangeRelative);
            }

            DayChangeDifferenceCollection = new ObservableCollection<DayPrice>(dayChangeRelativeCollection);

            ChangeCalculationIsBusy = false;
        }

        private void IndexQueryBegin(String indexName)
        {
            DateTime dateStart = DateTime.Parse("2009-01-01");
            DateTime dateEnd = DateTime.Today;
            DataServiceQuery<DayPrice> dayPriceQuery = (DataServiceQuery<DayPrice>)
                from dayPrice in this.context.CreateQuery<DayPrice>("DayPriceSet")
                where dayPrice.SymbolName == indexName
                where dayPrice.DayPriceDate > dateStart
                where dayPrice.DayPriceDate <= dateEnd
                orderby dayPrice.DayPriceDate ascending
                select dayPrice;
            dayPriceQuery.BeginExecute(new AsyncCallback(this.IndexRequestCompleted), dayPriceQuery);
        }

        private void IndexRequestCompleted(IAsyncResult asyncResult)
        {
            DataServiceQuery<DayPrice> asyncState = asyncResult.AsyncState as DataServiceQuery<DayPrice>;
            List<DayPrice> indexCollection = asyncState.EndExecute(asyncResult).ToList<DayPrice>();
            String indexName = indexCollection.Select(index => index.SymbolName).First();
            _IndexCollectionDictionary.Add(indexName, indexCollection);
            CalculateIndexChange(indexName, indexCollection);
        }

        private void CalculateIndexChange(string indexName, List<DayPrice> indexCollection)
        {
            List<DayPrice> dayChangeCollection = new List<DayPrice>();

            DayPrice previous = null;
            foreach (DayPrice dayPrice in indexCollection)
            {
                if (previous != null)
                {
                    // Calculate percentage change from previous day
                    DayPrice dayPriceChange = new DayPrice();
                    dayPriceChange.DayPriceDate = dayPrice.DayPriceDate;
                    //if (previous.DayPriceOpen > 0) dayPriceChange.DayPriceOpen = ((dayPrice.DayPriceOpen - previous.DayPriceOpen) / previous.DayPriceOpen) * 100.0M;
                    //if (previous.DayPriceHigh > 0) dayPriceChange.DayPriceHigh = ((dayPrice.DayPriceHigh - previous.DayPriceHigh) / previous.DayPriceHigh) * 100.0M;
                    //if (previous.DayPriceLow > 0) dayPriceChange.DayPriceLow = ((dayPrice.DayPriceLow - previous.DayPriceLow) / previous.DayPriceLow) * 100.0M;
                    if (previous.DayPriceClose > 0) dayPriceChange.DayPriceClose = ((dayPrice.DayPriceClose - previous.DayPriceClose) / previous.DayPriceClose) * 100.0M;
                    //if (previous.DayPriceAdjustedClose > 0) dayPriceChange.DayPriceAdjustedClose = ((dayPrice.DayPriceAdjustedClose - previous.DayPriceAdjustedClose) / previous.DayPriceAdjustedClose) * 100.0M;
                    //if (previous.DayPriceVolume > 0) dayPriceChange.VolumeChange = ((double)((dayPrice.DayPriceVolume - previous.DayPriceVolume) / previous.DayPriceVolume)) * 100.0;
                    dayChangeCollection.Add(dayPriceChange);
                }
                previous = dayPrice;
            }

            _IndexChangeCollectionDictionary.Add(indexName, new ObservableCollection<DayPrice>(dayChangeCollection));
        }

        #endregion
    }
}