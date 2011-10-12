using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using ReportWatch.Database;

namespace ReportWatch.Service
{
    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ReportWatchService : DataService<ReportWatchEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
            config.UseVerboseErrors = true;
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
        }

        [WebGet]
        public List<Symbol> SymbolSetFull()
        {
            List<Symbol> result = this.CurrentDataSource.SymbolSet
                                      .Include("DaySet")
                                      .Include("ReportSet")
                                      .ToList();
            return result;
        }
    }
}
