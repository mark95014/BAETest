using LDSAPITest.Utils;
using LDSTest.Shared;
using System.Net;
using System.Net.Http.Json;
using TechTalk.SpecFlow;

namespace LDSAPITest.StepDefinitions
{
    [Binding]
    public class CustomerApiSteps : BaseApiTest
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ExpectedResults _expectedResults = null!;

        public class Customer
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public List<Booking>? Bookings { get; set; }
        }

        public class Booking
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int RoomNumber { get; set; }
        }

        public CustomerApiSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            BaseOneTimeSetUp();
        }

        [Given(@"I have a new customer with the following details:")]
        public void GivenIHaveANewCustomerWithTheFollowingDetails(Table table)
        {
            var customer = new Customer
            {
                Id = 0,
                Name = table.Rows[0]["Value"],
                Bookings = []
            };
            _scenarioContext.Add("NewCustomer", customer);
        }

        [When(@"I create a new customer")]
        public async Task WhenICreateANewCustomer()
        {
            var customer = _scenarioContext.Get<Customer>("NewCustomer");
            _scenarioContext["Response"] = await PostAsync("CreateEditCustomer", customer);
            await new Database().ResetDatabase();
        }

        [When(@"I send a request to get all customers")]
        public async Task GetAllCustomers()
        {
            _scenarioContext["Response"] = await GetAsync("GetAllCustomers");
        }

        [When(@"I send a GET request to get customer with ID (.*)")]
        public async Task GetCustomerById(int customerId)
        {
            _scenarioContext["Response"] = await GetAsync($"GetCustomer/{customerId}");
        }

        [Then(@"the response should contain the expected customer")]
        public async Task ThenTheResponseShouldContainASingleCustomer()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            VerifyResponse.Verify(new { customer }, _expectedResults);
        }

        [Then(@"the response should contain the expected customers")]
        public async Task ThenTheResponseShouldContainAListOfCustomers()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var list = await response.Content.ReadFromJsonAsync<List<Customer>>();
            VerifyResponse.Verify(new { customers = list }, _expectedResults);
        }
    }
}