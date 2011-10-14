
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
        // To support paging you will need to add ordering to the 'DayPriceSet' query.
        public IQueryable<DayPrice> GetDayPriceSet()
        {
            return this.ObjectContext.DayPriceSet;
        }

        public void InsertDayPrice(DayPrice dayPrice)
        {
            if ((dayPrice.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(dayPrice, EntityState.Added);
            }
            else
            {
                this.ObjectContext.DayPriceSet.AddObject(dayPrice);
            }
        }

        public void UpdateDayPrice(DayPrice currentDayPrice)
        {
            this.ObjectContext.DayPriceSet.AttachAsModified(currentDayPrice, this.ChangeSet.GetOriginal(currentDayPrice));
        }

        public void DeleteDayPrice(DayPrice dayPrice)
        {
            if ((dayPrice.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(dayPrice, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.DayPriceSet.Attach(dayPrice);
                this.ObjectContext.DayPriceSet.DeleteObject(dayPrice);
            }
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

        public void InsertReport(Report report)
        {
            if ((report.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(report, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ReportSet.AddObject(report);
            }
        }

        public void UpdateReport(Report currentReport)
        {
            this.ObjectContext.ReportSet.AttachAsModified(currentReport, this.ChangeSet.GetOriginal(currentReport));
        }

        public void DeleteReport(Report report)
        {
            if ((report.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(report, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ReportSet.Attach(report);
                this.ObjectContext.ReportSet.DeleteObject(report);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'SymbolSet' query.
        public IQueryable<Symbol> GetSymbolSet()
        {
            return this.ObjectContext.SymbolSet;
        }

        public void InsertSymbol(Symbol symbol)
        {
            if ((symbol.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(symbol, EntityState.Added);
            }
            else
            {
                this.ObjectContext.SymbolSet.AddObject(symbol);
            }
        }

        public void UpdateSymbol(Symbol currentSymbol)
        {
            this.ObjectContext.SymbolSet.AttachAsModified(currentSymbol, this.ChangeSet.GetOriginal(currentSymbol));
        }

        public void DeleteSymbol(Symbol symbol)
        {
            if ((symbol.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(symbol, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.SymbolSet.Attach(symbol);
                this.ObjectContext.SymbolSet.DeleteObject(symbol);
            }
        }
    }
}


