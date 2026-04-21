Feature: CustomersPageTest
    As a hotel administrator
    I want to view and filter customer information
    So that I can manage customer data effectively

Background:
    Given the browser is initialized
    And I navigate to the customers page

@testcase:1
Scenario: Verify all data on Customers page
    Then the page title should be "All Customers"
    And the customers table should be displayed
    And the customers table should contain the expected data

@testcase:2
Scenario: Filter customers by ID
    When I filter customers by ID "7"
    Then the customers table should show only customer with ID "7"
    And the filtered customers table should contain the expected data

@testcase:3
Scenario: Filter customers by name
    When I filter customers by name "son"
    Then the customers table should show only customers with name containing "son"
    And the filtered customers table should contain the expected data

Scenario Outline: Filter customers by different IDs
    When I filter customers by ID "<CustomerId>"
    Then the customers table should show only customer with ID "<CustomerId>"
    And the filtered customers table should contain the expected data

    Examples:
        | CustomerId |
        | 7          |
        | 5          |
        | 10         |

Scenario Outline: Filter customers by different name patterns
    When I filter customers by name "<NamePattern>"
    Then the customers table should show only customers with name containing "<NamePattern>"
    And the filtered customers table should contain the expected data

    Examples:
        | NamePattern |
        | son         |
        | Smith       |
        | John        |

Scenario: Clear customer filters
    Given I filter customers by name "son"
    When I clear all filters
    Then the customers table should show all customers
    And the customers table should contain the expected data

Scenario: Verify filter persistence
    When I filter customers by ID "7"
    And I refresh the page
    Then the customer ID filter should be cleared
    And the customers table should show all customers