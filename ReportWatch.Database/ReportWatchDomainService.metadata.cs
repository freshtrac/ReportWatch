
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


    // The MetadataTypeAttribute identifies DayChangeMetadata as the class
    // that carries additional metadata for the DayChange class.
    [MetadataTypeAttribute(typeof(DayChange.DayChangeMetadata))]
    public partial class DayChange
    {

        // This class allows you to attach custom attributes to properties
        // of the DayChange class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class DayChangeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private DayChangeMetadata()
            {
            }

            public double DayChangeAdjustedClose { get; set; }

            public double DayChangeClose { get; set; }

            public DateTime DayChangeDate { get; set; }

            public double DayChangeHigh { get; set; }

            public Guid DayChangeId { get; set; }

            public double DayChangeLow { get; set; }

            public double DayChangeOpen { get; set; }

            public double DayChangeVolume { get; set; }

            public string SymbolName { get; set; }

            public Symbol SymbolSet { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies DayDifferenceMetadata as the class
    // that carries additional metadata for the DayDifference class.
    [MetadataTypeAttribute(typeof(DayDifference.DayDifferenceMetadata))]
    public partial class DayDifference
    {

        // This class allows you to attach custom attributes to properties
        // of the DayDifference class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class DayDifferenceMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private DayDifferenceMetadata()
            {
            }

            public double DayDifferenceAdjustedClose { get; set; }

            public double DayDifferenceClose { get; set; }

            public DateTime DayDifferenceDate { get; set; }

            public double DayDifferenceHigh { get; set; }

            public Guid DayDifferenceId { get; set; }

            public double DayDifferenceLow { get; set; }

            public double DayDifferenceOpen { get; set; }

            public double DayDifferenceVolume { get; set; }

            public string SymbolName { get; set; }

            public Symbol SymbolSet { get; set; }
        }
    }

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

            public EntityCollection<DayChange> DayChangeSet { get; set; }

            public EntityCollection<DayDifference> DayDifferenceSet { get; set; }

            public EntityCollection<DayPrice> DayPriceSet { get; set; }

            public string IndexSymbolName { get; set; }

            public Nullable<DateTime> ReportDate { get; set; }

            public EntityCollection<Report> ReportSet { get; set; }

            public string SymbolName { get; set; }
        }
    }
}
