using FluentAssertions;
using LDSAPITest.Utils;
using LDSTest.Shared;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Net.Http.Json;

namespace LDSAPITest.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class CustomerApiTests : BaseApiTest
    {
        // Data models
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

        // Test data source for GetAllCustomers
        private static IEnumerable GetAllCustomersTestCases
        {
            get
            {
                yield return new TestCaseData(10)
                    .SetDescription("Get all customers");
            }
        }

        // Test data source for GetCustomerById
        private static IEnumerable GetCustomerByIdTestCases
        {
            get
            {
                yield return new TestCaseData(11, new Customer { Id = 1 })
                    .SetDescription("Get customer by ID 1");

                yield return new TestCaseData(12, new Customer { Id = 7 })
                    .SetDescription("Get customer by ID 7");
            }
        }

        // Test data source for CreateCustomer
        private static IEnumerable CreateCustomerTestCases
        {
            get
            {
                yield return new TestCaseData(13, new Customer 
                { 
                    Id = 0,
                    Name = "John Smith",
                    Bookings = []
                })
                .SetDescription("Create new customer");
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAllCustomersTestCases))]
        public async Task GetAllCustomers_ShouldReturnListOfCustomers(int testCaseId)
        {
            var response = await GetAsync("GetAllCustomers");

            await EnsureSuccessStatusCodeAsync(response);

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
            customers.Should().NotBeNull();
            customers.Should().NotBeEmpty();
            VerifyResponse.Verify(new { customers = customers! }, ExpectedResults);

            LogInfo($"Retrieved {customers?.Count} customers");
        }

        [TestCaseSource(nameof(GetCustomerByIdTestCases))]
        public async Task GetCustomerById_WithValidId_ShouldReturnCustomer(int testCaseId, Customer customerInput)
        {
            int customerId = customerInput.Id;

            var response = await GetAsync($"GetCustomer/{customerId}");

            await AssertStatusCodeAsync(response, HttpStatusCode.OK);

            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            VerifyResponse.Verify(new { customer = customer! }, ExpectedResults);
        }

        [NonParallelizable]
        [TestCaseSource(nameof(CreateCustomerTestCases))]
        public async Task CreateCustomer_WithValidData_ShouldReturnCreatedCustomer(int testCaseId, Customer customerData)
        {
            var response = await PostAsync("CreateEditCustomer", customerData);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

            var createdCustomer = await response.Content.ReadFromJsonAsync<Customer>();
            VerifyResponse.Verify(new { customer = createdCustomer! }, ExpectedResults);

            await new Database().ResetDatabase();
        }
    }
}