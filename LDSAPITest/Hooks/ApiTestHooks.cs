using LDSAPITest;
using TechTalk.SpecFlow;

[Binding]
public class ApiTestHooks : BaseApiTest
{
    private readonly ScenarioContext _scenarioContext;

    public ApiTestHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        // Initialize BaseApiTest (calls BaseOneTimeSetUp)
        BaseOneTimeSetUp();

        // Share BaseApiTest instance with step definitions
        _scenarioContext.Add("BaseApiTest", this);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        BaseOneTimeTearDown();
    }
}