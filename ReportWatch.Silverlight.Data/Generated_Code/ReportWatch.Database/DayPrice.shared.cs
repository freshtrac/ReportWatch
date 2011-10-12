using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportWatch.Database
{
    public partial class DayPrice
    {
        public double[] Candlestick
        {
            get
            {
                double[] candlestick = new double[] { (double)DayPriceOpen, (double)DayPriceClose, (double)DayPriceHigh, (double)DayPriceLow };
                return candlestick;
            }
        }

        public string CandlestickToolTipText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(DayPriceDate.ToString("MM/dd/yyyy"));
                sb.Append("Open: ");
                sb.AppendLine(DayPriceOpen.ToString("N2"));
                sb.Append("High: ");
                sb.AppendLine(DayPriceHigh.ToString("N2"));
                sb.Append("Low: ");
                sb.AppendLine(DayPriceLow.ToString("N2"));
                sb.Append("Close: ");
                sb.Append(DayPriceClose.ToString("N2"));
                return sb.ToString();
            }
        }

        public string VolumeToolTipText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(DayPriceDate.ToString("MM/dd/yyyy"));
                sb.Append("Volume: ");
                sb.Append(DayPriceVolume.ToString("N0"));
                return sb.ToString();
            }
        }
    }
}
