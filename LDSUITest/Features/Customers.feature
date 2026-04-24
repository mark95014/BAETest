@ui @customers
Feature: CustomersPageTest
    As a hotel administrator
    I want to view and filter customer information
    So that I can manage customer data effectively

Background:
    Given I navigate to the customers page

@testcase:1
Scenario: Verify all data on Customers page
    Then the customers table should contain the expected data

@testcase:2
Scenario: Filter customers by ID
    When I filter customers by ID "7"
    Then the customers table should contain the expected data

@testcase:3
Scenario: Filter customers by name
    When I filter customers by name "son"
    Then the customers table should contain the expected data