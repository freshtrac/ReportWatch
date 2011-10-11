using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportWatch.Database;

namespace ReportWatch.Library
{
    public class LogOps
    {
        public static void LogException(Exception ex)
        {
            using (ReportWatchEntities entities = new ReportWatchEntities())
            {
                ExceptionLog entry = entities.ExceptionLogSet.CreateObject();
                entry.ExceptionLogId = Guid.NewGuid();
                entry.ExceptionLogDate = DateTime.Now;
                entry.ExceptionLogMessage = ex.Message;
                entry.ExceptionLogStackTrace = ex.StackTrace;
                entities.ExceptionLogSet.AddObject(entry);

                entities.SaveChanges();
            }
        }

    }
}
