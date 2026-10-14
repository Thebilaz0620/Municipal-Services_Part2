# Municipal Services Application - Part 2

## 1. Project Overview

This version extends the Part 1 Municipal Services Application. The original
Report Issues functionality is retained and the Local Events and Announcements
feature has been implemented for Part 2.

The Part 2 implementation focuses on advanced data structures, event searching,
filtering and a recommendation feature based on user searches.

## 2. Technology

- C#
- .NET Framework 4.7.2
- Windows Forms
- Visual Studio
- Generic collections from `System.Collections.Generic`

## 3. Part 2 Requirements Covered

The application implements the Part 2 requirements as follows:

### Main Menu - 30 marks

The main menu presents:

1. Report Issues - implemented in Part 1 and retained.
2. Local Events and Announcements - implemented in Part 2 and enabled.
3. Service Request Status - disabled because it is reserved for Part 3.
4. Exit - closes the application.

The Local Events and Announcements button opens the Part 2 Windows Form.

### Local Events and Announcements Page - 70 marks

The page provides:

- A list of upcoming events and announcements.
- Keyword search across event title, category, description and location.
- Category filtering.
- Optional From and To date filtering.
- A clear filters option.
- A DataGridView for organised presentation of results.
- Smart recommendations based on previous searches and selected preferences.
- Recent search history.
- A recently viewed event indicator using a Stack.

## 4. Data Structures Used

### Stack

`Stack<LocalEvent> RecentlyViewed`

The stack stores selected events. The most recently selected event is pushed
to the top of the stack, demonstrating LIFO (Last-In, First-Out) behaviour.

### Queue

`Queue<LocalEvent> UpcomingEvents`

The queue stores upcoming events in date order. It supports first-in,
first-out organisation of upcoming event information.

### Priority Queue

`EventPriorityQueue`

A custom max-heap priority queue is used to organise events according to
priority. Higher-priority announcements or events are returned first. A
binary-heap implementation is used so the priority queue does not depend on
framework versions that do not provide the modern `PriorityQueue<TElement,
TPriority>` class.

### Dictionary / Hash Table

`Dictionary<int, LocalEvent> EventLookup`

This dictionary provides fast lookup of events using their unique event ID.

`Dictionary<string, int> SearchFrequency`

This dictionary records how often users search for categories or terms. The
recommendation feature uses this information as part of its scoring.

### Sorted Dictionary

`SortedDictionary<DateTime, List<LocalEvent>> EventsByDate`

Events are indexed by date. This keeps event dates ordered and allows events
for a particular date to be grouped together.

### Sets

`HashSet<string> Categories`

Stores unique event categories without duplicates.

`HashSet<DateTime> EventDates`

Stores unique event dates without duplicates.

## 5. Search Functionality

The search feature accepts a keyword and can also filter by category and date
range.

Keyword matching checks:

- Event title
- Category
- Description
- Location

The results are sorted by event date and then priority.

If an invalid date range is selected, the application displays a warning and
does not perform the search.

## 6. Smart Recommendation Feature

The recommendation feature analyses the user's searches and preferences.

The recommendation score considers:

- Category matches.
- Previous search frequency for a category.
- Keyword matches in the event information.
- Event priority.
- How close the event is to the current date.

The three highest-scoring upcoming events are displayed in the recommendation
area. This gives the application a simple, explainable recommendation
algorithm while directly using the collected search-pattern data.

## 7. Test Data

The application includes sample municipal events and announcements so that
the Part 2 functionality can be tested immediately after starting the program.
The dates are generated relative to the current date so that the sample events
remain upcoming when the application is demonstrated later.

## 8. How to Compile

1. Open `MunicipalServiceApp.slnx` in Visual Studio.
2. Make sure the .NET Framework development tools are installed.
3. Select the `MunicipalServiceApp` project.
4. Select `Build > Build Solution`, or press `Ctrl + Shift + B`.
5. Confirm that the build completes without errors.

## 9. How to Run

1. Press `F5` in Visual Studio to run with debugging.
2. Alternatively press `Ctrl + F5` to run without debugging.
3. The Municipal Services Application main menu will appear.

## 10. How to Demonstrate Part 2

1. Start the application.
2. Confirm that `Report Issues` is available.
3. Confirm that `Local Events and Announcements` is enabled.
4. Confirm that `Service Request Status` remains disabled for Part 3.
5. Select `Local Events and Announcements`.
6. Confirm that upcoming events are displayed.
7. Search for a keyword such as `water`.
8. Select the `Water` category and search again.
9. Use the From and To date filters.
10. Select an event in the table and confirm that the recently viewed event
    indicator changes.
11. Repeat a category search to build search-frequency information.
12. Observe the Smart Recommendations section after each search.
13. Select `Clear Filters` to return to the complete upcoming-event list.
14. Use `Back to Main Menu` to return to the main form.

## 11. Project Structure

- `MainForm.cs` - application main menu.
- `ReportIssuesForm.cs` - Part 1 issue reporting functionality.
- `Issue.cs` - Part 1 issue model.
- `IssueRepository.cs` - Part 1 List-based issue storage.
- `LocalEvent.cs` - event and announcement model.
- `LocalEventRepository.cs` - event collections and indexes.
- `EventPriorityQueue.cs` - custom priority queue implemented as a max-heap.
- `RecommendationService.cs` - recommendation scoring algorithm.
- `LocalEventsForm.cs` - Part 2 user interface, search, filtering and recommendations.
- `LocalEventsForm.Designer.cs` - Windows Forms controls and layout.

## 12. Part 2 Rubric Alignment

| Rubric Area | Implementation |
|---|---|
| Main Menu - 30 | Main menu contains all required options; Part 2 option is enabled and opens the events form; Part 3 option remains disabled. |
| Stacks, Queues, Priority Queues - 15 | Stack for recently viewed events, Queue for upcoming events, and custom heap-based priority queue for event priority. |
| Hash Tables, Dictionaries, Sorted Dictionaries - 15 | Dictionary for ID lookup and search frequency; SortedDictionary for date indexing. |
| Sets - 10 | HashSet for unique categories and unique event dates. |
| Recommendations - 30 | Search frequency, category preference, keyword matching, priority and date proximity are combined to rank and display recommendations. |

## 13. Important Notes

- The application is intentionally kept on .NET Framework 4.7.2 to remain
  consistent with the Part 1 project being extended.
- Part 1 code is retained rather than replaced.
- Service Request Status is not implemented in Part 2 because the assessment
  specification reserves that feature for Part 3.
