@ui @rooms
Feature: RoomsPageTest
    As a hotel administrator
    I want to view and manage room information
    So that I can maintain accurate room data and pricing

Background:
    Given I navigate to the rooms page

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
    Then the rooms table should contain the expected data
    #Then I reset the database to its initial state