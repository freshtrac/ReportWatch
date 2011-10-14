using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportWatch.Database
{
    public partial class Report
    {

        public double PriceChange { get; set; }

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
