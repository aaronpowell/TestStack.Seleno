﻿using TestStack.BDDfy.Processors.Reporters.Html;
using Configurator = TestStack.BDDfy.Configuration.Configurator;
using NUnit.Framework;

namespace TestStack.Seleno.Samples.Movies.FunctionalTests
{
    [SetUpFixture]
    public class AssemblySetupFixture
    {

        [SetUp]
        public void SetUp()
        {
            InitializeBDDfyReport();
        }

        private void InitializeBDDfyReport()
        {
            Configurator.BatchProcessors.HtmlReport.Disable();
            Configurator.BatchProcessors.Add(new HtmlReporter(new FunctionalTestsHtmlReportConfig()));
        }
    }
}
