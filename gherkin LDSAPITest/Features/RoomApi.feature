  As an API client
  I want to manage rooms via REST API
  So that I can integrate with the hotel booking system

  @smoke @testcase:20
  Scenario: Get all rooms
    When I send a request to get all rooms
    Then the response should contain the expected list of rooms

  @regression @testcase:21
  Scenario: Get room by number 1001
    When I send a GET request to get room with number 1001
    Then the response should contain the expected room

  @regression @testcase:22
  Scenario: Get room by number 1007
    When I send a GET request to get room with number 1007
    Then the response should contain the expected room

  @regression @database @testcase:23
  Scenario: Create a new room
    Given I have a new room with the following details:
      | Field      | Value |
      | RoomNumber | 2001  |
      | Price      | 150   |
    When I create a new room
    When I send a request to get all rooms
    Then the response should contain the original list of rooms plus the new room
    Then I delete the room just created