using FluentAssertions;
using LDSTest.Shared;
using LDSUITest.pages;
using LDSUITest.utils.PageData;
using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace LDSUITest.StepDefinitions
{
    [Binding]
    public class CustomersSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private IPage _page = null!;
        private CustomersPage _customersPage = null!;
        private ExpectedResults _expectedResults = null!;
        private Results _results = null!;

        public CustomersSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I navigate to the customers page")]
        public async Task GivenINavigateToTheCustomersPage()
        {
            _page = _scenarioContext.Get<IPage>("Page");
            _expectedResults = _scenarioContext.Get<ExpectedResults>("ExpectedResults");
            _results = _scenarioContext.Get<Results>("Results");

            _customersPage = new CustomersPage();
            await _customersPage.GoTo(_page);
        }

        [Then(@"the customers table should be displayed")]
        public async Task ThenTheCustomersTableShouldBeDisplayed()
        {
            var customersTable = _page.Locator("[id='customersTable']");
            await customersTable.WaitForAsync();
            (await customersTable.IsVisibleAsync()).Should().BeTrue("Customers table should be visible");
        }

        [Then(@"the customers table should contain the expected data")]
        public async Task ThenTheCustomersTableShouldContainTheExpectedData()
        {
            var testCaseId = _scenarioContext.Get<int>("TestCaseId");
            await BasePage.VerifyPage<CustomersPageData>(_page, _expectedResults, _results);
        }

        [When(@"I filter customers by ID ""(.*)""")]
        public async Task WhenIFilterCustomersByID(string customerId)
        {
            await _customersPage.FilterCustomersById(_page, customerId);
            _scenarioContext["FilteredCustomerId"] = customerId;
        }

        [When(@"I filter customers by name ""(.*)""")]
        public async Task WhenIFilterCustomersByName(string customerName)
        {
            await _customersPage.FilterCustomersByName(_page, customerName);
            _scenarioContext["FilteredCustomerName"] = customerName;
        }

        [Then(@"the customers table should show only customer with ID ""(.*)""")]
        public async Task ThenTheCustomersTableShouldShowOnlyCustomerWithID(string customerId)
        {
            // Verify the table shows filtered results
            var customersTable = _page.Locator("[id='customersTable']");
            (await customersTable.IsVisibleAsync()).Should().BeTrue("Customers table should be visible after filtering");

            // Get all rows in the table (excluding header)
            var rows = await _page.Locator("[id='customersTable'] tbody tr").CountAsync();
            rows.Should().BeGreaterThan(0, "At least one customer should be displayed");

            // Verify the ID column contains the filtered ID (assuming ID is in first column)
            var firstRowId = await _page.Locator("[id='customersTable'] tbody tr:first-child td:nth-child(1)").TextContentAsync();
            firstRowId.Should().Contain(customerId, $"Displayed customer should have ID {customerId}");
        }

        [Then(@"the customers table should show only customers with name containing ""(.*)""")]
        public async Task ThenTheCustomersTableShouldShowOnlyCustomersWithNameContaining(string namePattern)
        {
            // Verify the table shows filtered results
            var customersTable = _page.Locator("[id='customersTable']");
            (await customersTable.IsVisibleAsync()).Should().BeTrue("Customers table should be visible after filtering");

            // Get all rows in the table (excluding header)
            var rows = await _page.Locator("[id='customersTable'] tbody tr").CountAsync();
            rows.Should().BeGreaterThan(0, "At least one customer should be displayed");

            // Verify at least the first row contains the name pattern (assuming name is in column 2)
            var firstRowName = await _page.Locator("[id='customersTable'] tbody tr:first-child td:nth-child(2)").TextContentAsync();
            firstRowName.Should().Contain(namePattern, $"Displayed customer should have name containing '{namePattern}'", StringComparison.OrdinalIgnoreCase);
        }

        [Then(@"the filtered customers table should contain the expected data")]
        public async Task ThenTheFilteredCustomersTableShouldContainTheExpectedData()
        {
            var testCaseId = _scenarioContext.Get<int>("TestCaseId");
            await BasePage.VerifyPage<CustomersPageData>(_page, _expectedResults, _results);
        }

        [Given(@"I filter customers by name ""(.*)""")]
        public async Task GivenIFilterCustomersByName(string customerName)
        {
            await WhenIFilterCustomersByName(customerName);
        }

        [When(@"I clear all filters")]
        public async Task WhenIClearAllFilters()
        {
            // Clear ID filter
            await _page.Locator("[id='filterId']").FillAsync("");

            // Clear Name filter
            await _page.Locator("[id='filterName']").FillAsync("");

            // Apply filters (which will show all customers)
            await _page.Locator("button:has-text('Apply Filters')").ClickAsync();
            await _customersPage.WaitForPageToLoad(_page);
        }

        [Then(@"the customers table should show all customers")]
        public async Task ThenTheCustomersTableShouldShowAllCustomers()
        {
            var customersTable = _page.Locator("[id='customersTable']");
            (await customersTable.IsVisibleAsync()).Should().BeTrue("Customers table should be visible");

            // Verify we have multiple customers (more than what would be filtered)
            var rows = await _page.Locator("[id='customersTable'] tbody tr").CountAsync();
            rows.Should().BeGreaterThan(1, "Multiple customers should be displayed when filters are cleared");
        }

        [When(@"I refresh the page")]
        public async Task WhenIRefreshThePage()
        {
            await _page.ReloadAsync();
            await _customersPage.WaitForPageToLoad(_page);
        }

        [Then(@"the customer ID filter should be cleared")]
        public async Task ThenTheCustomerIDFilterShouldBeCleared()
        {
            var filterIdValue = await _page.Locator("[id='filterId']").InputValueAsync();
            filterIdValue.Should().BeEmpty("Customer ID filter should be cleared after page refresh");
        }
    }
}