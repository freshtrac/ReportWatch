using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportWatch.Library;
using System.Text.RegularExpressions;

namespace ReportWatch.Test
{
    [TestClass]
    public class ReportWatchLibraryTest
    {
        public ReportWatchLibraryTest()
        {

        }

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private bool AsycCompleted = false;
        [TestMethod]
        public void AsyncTest()
        {
            SymbolProcessor symbolProcessor = new SymbolProcessor(DateTime.Parse("2011-10-19"));
            symbolProcessor.OnCompleted += new SymbolProcessor.Completed(symbolProcessor_OnCompleted);
            symbolProcessor.Process();

            while (!AsycCompleted)
            {
            }
        }

        void symbolProcessor_OnCompleted()
        {
            AsycCompleted = true;
        }

    }
}
