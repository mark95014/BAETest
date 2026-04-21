Feature: RoomsPageTest
    As a hotel administrator
    I want to view and manage room information
    So that I can maintain accurate room data and pricing

Background:
    Given the browser is initialized
    And I navigate to the rooms page

@regression @testcase:1
Scenario: Verify all data on Rooms page
    Then the rooms table should contain the expected data

@regression @testcase:2
Scenario: Edit multiple room prices
    When I edit the following rooms:
        | RoomNumber | Price |
        | 101        | 150   |
        | 103        | 200   |
        | 105        | 175   |
    Then the rooms table should reflect the updated prices
    And the database should be reset for test isolation

Scenario Outline: Verify room price updates
    When I select room number "<RoomNumber>"
    And I update the room price to "<NewPrice>"
    And I save the changes
    Then the room price should be updated to "<NewPrice>" for room "<RoomNumber>"

    Examples:
        | RoomNumber | NewPrice |
        | 101        | 150      |
        | 103        | 200      |
        | 105        | 175      |

Scenario: Verify rooms table pagination
    Given the rooms table has multiple pages
    When I click the next page button
    Then the next page of rooms should be displayed
    And the rooms table should contain valid room data

Scenario: Verify room data persistence after edit
    When I select room number "101"
    And I update the room price to "150"
    And I save the changes
    And I refresh the page
    Then the room price should still be "150" for room "101"