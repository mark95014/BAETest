using FluentAssertions;
using LDSTest.Shared;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class CommonSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CommonSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the browser is initialized")]
        public void GivenTheBrowserIsInitialized()
        {
            // Browser is already initialized in UITestHooks
            // This step is just for clarity in the feature file
            var driver = _scenarioContext.Get<IWebDriver>("Driver");
            driver.Should().NotBeNull("Browser should be initialized");
        }
    }
}