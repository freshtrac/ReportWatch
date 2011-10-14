
namespace ReportWatch.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies DayPriceMetadata as the class
    // that carries additional metadata for the DayPrice class.
    [MetadataTypeAttribute(typeof(DayPrice.DayPriceMetadata))]
    public partial class DayPrice
    {

        // This class allows you to attach custom attributes to properties
        // of the DayPrice class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class DayPriceMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private DayPriceMetadata()
            {
            }

            public double[] Candlestick { get; set; }

            public string CandlestickToolTipText { get; set; }

            public decimal DayPriceAdjustedClose { get; set; }

            public decimal DayPriceClose { get; set; }

            public DateTime DayPriceDate { get; set; }

            public decimal DayPriceHigh { get; set; }

            public Guid DayPriceId { get; set; }

            public decimal DayPriceLow { get; set; }

            public decimal DayPriceOpen { get; set; }

            public long DayPriceVolume { get; set; }

            public Symbol Symbol { get; set; }

            public string SymbolName { get; set; }

            public double VolumeChange { get; set; }

            public string VolumeToolTipText { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ExceptionLogMetadata as the class
    // that carries additional metadata for the ExceptionLog class.
    [MetadataTypeAttribute(typeof(ExceptionLog.ExceptionLogMetadata))]
    public partial class ExceptionLog
    {

        // This class allows you to attach custom attributes to properties
        // of the ExceptionLog class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ExceptionLogMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ExceptionLogMetadata()
            {
            }

            public DateTime ExceptionLogDate { get; set; }

            public Guid ExceptionLogId { get; set; }

            public string ExceptionLogMessage { get; set; }

            public string ExceptionLogStackTrace { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ReportMetadata as the class
    // that carries additional metadata for the Report class.
    [MetadataTypeAttribute(typeof(Report.ReportMetadata))]
    public partial class Report
    {

        // This class allows you to attach custom attributes to properties
        // of the Report class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ReportMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ReportMetadata()
            {
            }

            public decimal DayPriceHigh { get; set; }

            public double PriceChange { get; set; }

            public decimal ReportActual { get; set; }

            public DateTime ReportDate { get; set; }

            public decimal ReportExpected { get; set; }

            public Guid ReportId { get; set; }

            public string ReportName { get; set; }

            public decimal ReportPreviousYear { get; set; }

            public string ReportTitle { get; set; }

            public Symbol Symbol { get; set; }

            public string SymbolName { get; set; }

            public string ToolTipText { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies SymbolMetadata as the class
    // that carries additional metadata for the Symbol class.
    [MetadataTypeAttribute(typeof(Symbol.SymbolMetadata))]
    public partial class Symbol
    {

        // This class allows you to attach custom attributes to properties
        // of the Symbol class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class SymbolMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private SymbolMetadata()
            {
            }

            public string CompanyName { get; set; }

            public EntityCollection<DayPrice> DayPriceSet { get; set; }

            public string IndexSymbolName { get; set; }

            public Nullable<DateTime> ReportDate { get; set; }

            public EntityCollection<Report> ReportSet { get; set; }

            public string SymbolName { get; set; }
        }
    }
}
