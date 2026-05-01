Feature: BookingApiTests
  As an API client
  I want to manage bookings via REST API
  So that I can integrate with the hotel booking system

  @smoke @testcase:1
  Scenario: Get all bookings
    When I send a request to get all bookings
    Then the response should contain the expected list of bookings

  @regression @testcase:2
  Scenario: Get booking by ID 1
    When I send a GET request to get booking with ID 1
    Then the response should contain the expected booking

  @regression @testcase:5
  Scenario: Get booking by ID 7
    When I send a GET request to get booking with ID 7
    Then the response should contain the expected booking

  @regression @database @testcase:3
  Scenario: CreateBooking
    Given I have a new booking with the following details:
      | Field       | Value |
      | CustomerId  | 7     |
      | RoomNumber  | 1007  |
    When I create a new booking
    When I send a request to get all bookings
    Then the response should contain the original list of bookings plus the new booking
    Then I delete the booking just created