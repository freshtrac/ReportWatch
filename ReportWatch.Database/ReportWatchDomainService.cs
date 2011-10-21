
namespace ReportWatch.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // Implements application logic using the ReportWatchEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class ReportWatchDomainService : LinqToEntitiesDomainService<ReportWatchEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DayChangeSet' query.
        public IQueryable<DayChange> GetDayChangeSet()
        {
            return this.ObjectContext.DayChangeSet;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DayDifferenceSet' query.
        public IQueryable<DayDifference> GetDayDifferenceSet()
        {
            return this.ObjectContext.DayDifferenceSet;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DayPriceSet' query.
        public IQueryable<DayPrice> GetDayPriceSet()
        {
            return this.ObjectContext.DayPriceSet;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ExceptionLogSet' query.
        public IQueryable<ExceptionLog> GetExceptionLogSet()
        {
            return this.ObjectContext.ExceptionLogSet;
        }

        public void InsertExceptionLog(ExceptionLog exceptionLog)
        {
            if ((exceptionLog.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(exceptionLog, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ExceptionLogSet.AddObject(exceptionLog);
            }
        }

        public void UpdateExceptionLog(ExceptionLog currentExceptionLog)
        {
            this.ObjectContext.ExceptionLogSet.AttachAsModified(currentExceptionLog, this.ChangeSet.GetOriginal(currentExceptionLog));
        }

        public void DeleteExceptionLog(ExceptionLog exceptionLog)
        {
            if ((exceptionLog.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(exceptionLog, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ExceptionLogSet.Attach(exceptionLog);
                this.ObjectContext.ExceptionLogSet.DeleteObject(exceptionLog);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportSet' query.
        public IQueryable<Report> GetReportSet()
        {
            return this.ObjectContext.ReportSet;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'SymbolSet' query.
        public IQueryable<Symbol> GetSymbolSet()
        {
            return this.ObjectContext.SymbolSet;
        }
    }
}


