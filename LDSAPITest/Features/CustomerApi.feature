Feature: CustomerApiTests
  As an API client
  I want to manage customers via REST API
  So that I can integrate with the hotel booking system

  @smoke @testcase:10
  Scenario: Get all customers
    When I send a request to get all customers
    Then the response status should be OK
    And the response should contain the expected customers

  @regression @testcase:11
  Scenario: Get customer by ID 1
    When I send a GET request to get customer with ID 1
    Then the response status should be OK
    And the response should contain the expected customer

  @regression @testcase:12
  Scenario: Get customer by ID 7
    When I send a GET request to get customer with ID 7
    Then the response status should be OK
    And the response should contain the expected customer

  @regression @database @testcase:13
  Scenario: Create a new customer
    Given I have a new customer with the following details:
      | Field | Value      |
      | Name  | John Smith |
      | Bookings | |
    When I create a new customer
    Then the response status should be OK
    And the response should contain the expected customer