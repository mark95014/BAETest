@ui @bookings
Feature: BookingsPageTest
  As a hotel manager
  I want to view and filter bookings
  So that I can manage customer reservations effectively

Background:
  Given the browser is initialized
  And I navigate to the bookings page

@regression @testcase:1
Scenario: Verify all data on Bookings page
  Then the bookings table should contain the expected data

@regression @testcase:2
Scenario: Filter bookings by customer ID
  When I filter bookings by customer ID "7"
  Then the bookings table should contain the expected data

@regression @testcase:3
Scenario: Filter bookings by customer name
  When I filter bookings by customer name "son"
  Then the bookings table should contain the expected data