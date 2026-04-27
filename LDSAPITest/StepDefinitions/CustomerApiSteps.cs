using Azure;
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
            _scenarioContext.Add("Customer", customer);
        }

        [When(@"I create a new customer")]
        public async Task CreateANewCustomer()
        {
            var customer = _scenarioContext.Get<Customer>("Customer");
            var response = await PostAsync("CreateEditCustomer", customer);
            customer = await response.Content.ReadFromJsonAsync<Customer>();
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
            _scenarioContext["Customer"] = customer; // Update the customer with the ID returned from the API
        }

        [Then(@"I delete the customer just created")]
        public async Task DeleteTheCustomerJustCreated()
        {
            var customer = _scenarioContext.Get<Customer>("Customer");
            var response = await DeleteAsync($"DeleteCustomer/{customer!.Id}");
            await BaseApiTest.EnsureSuccessStatusCodeAsync(response);
        }
            
        [When(@"I send a request to get all customers")]
        public async Task GetAllCustomers()
        {
            var response = await GetAsync("GetAllCustomers");
            await AssertStatusCodeAsync(response, HttpStatusCode.OK);
            _scenarioContext["Response"] = response;
        }

        [When(@"I send a request to get customer with ID (.*)")]
        public async Task GetCustomerById(int customerId)
        {
            var response = await GetAsync($"GetCustomer/{customerId}");
            _scenarioContext["Response"] = response;
            _scenarioContext["Customer"] = await response.Content.ReadFromJsonAsync<Customer>();
        }

        [Then(@"the response should contain the expected customer")]
        public void VerifyCustomer()
        {
            var customer = _scenarioContext.Get<Customer>("Customer");
            VerifyResponse.Verify(new { customer }, _expectedResults);
        }

        [Then(@"the response should contain the expected customers")]
        public async Task VerifyListOfCustomers()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var list = await response.Content.ReadFromJsonAsync<List<Customer>>();
            VerifyResponse.Verify(new { customers = list }, _expectedResults);
        }

        [Then(@"the response should contain the expected customers plus the new customer")]
        public async Task VerifyModifiedList()
        {
            await VerifyListOfCustomers();
        }
    }
}