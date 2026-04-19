Feature: Hotel Booking API
  As an API client
  I want to manage bookings via REST API
  So that I can integrate with the hotel booking system

  Background:
    Given the API is available

  @smoke
  Scenario: Get all bookings
    When I send a GET request to "GetAllBookings"
    Then the response status should be 200
    And the response should contain a list of bookings

  @regression
  Scenario: Get booking by ID
    When I send a GET request to "GetBooking/1"
    Then the response status should be 200
    And the response should contain booking details for ID 1

  @regression @database
  Scenario: Create a new booking
    Given I have a new booking with the following details:
      | Field       | Value |
      | CustomerId  | 7     |
      | RoomNumber  | 1007  |
    When I send a POST request to "CreateEditBooking"
    Then the response status should be 201
    And the created booking should have customer ID 7
    And the created booking should have room number 1007