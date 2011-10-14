using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportWatch.Database
{
    public partial class Report
    {
        public double YValue
        {
            get
            {
                // Place the marker on the chart just above the price display
                return (double)this.DayPriceHigh + 1.0; 
            }
        }

        public string ToolTipText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ReportTitle.ToString());
                sb.Append("Date: ");
                sb.AppendLine(ReportDate.ToString("MM/dd/yyyy"));
                sb.Append("Surprise: ");
                sb.AppendLine((ReportActual - ReportExpected).ToString("N2"));
                sb.Append("Expected: ");
                sb.AppendLine(ReportExpected.ToString("N2"));
                sb.Append("Actual: ");
                sb.AppendLine(ReportActual.ToString("N2"));
                sb.Append("Previous Year: ");
                sb.Append(ReportPreviousYear.ToString("N2"));
                return sb.ToString();
            }
        }
    }
}
