@ui @bookings
Feature: BookingsPageTest
  As a hotel manager
  I want to view and filter bookings
  So that I can manage customer reservations effectively

Background:
  Given the hotel booking application is running
  And the database has been reset
  And I am on the bookings page

@smoke @testcase:1
Scenario: View all bookings
  Then the bookings page should display correctly
  And the page data should be verified against expected results

@regression @testcase:2
Scenario: Filter bookings by customer ID
  When I filter bookings by customer ID "7"
  Then the bookings page should display correctly
  And the page data should be verified against expected results

@regression @testcase:3
Scenario: Filter bookings by customer name
  When I filter bookings by customer name "son"
  Then the bookings page should display correctly
  And the page data should be verified against expected results