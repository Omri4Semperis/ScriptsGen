# MVP

GUI
Users says which events they want triggered
Program checks if at least one of its existing scripts can trigger all of these events
If at least one such script exists, it is suggested to the user by ASC order of the other (unspecified, unnecessary) events it also triggers on top of the requested events
If no such script exists, the user is shown a table:
Script # | Script Name | Script description | Event 1 | Event 2 | Event 3 | ... | Event N
-----------------------------------------------------------------------------------------
1        | Name 1      | Description 1      | TRUE    | FALSE   | TRUE    |     | FALSE
2        | Name 2      | Description 2      | TRUE    | TRUE    | FALSE   |     | TRUE
3        | Name 3      | Description 3      | FALSE   | TRUE    | TRUE    |     | FALSE
5        | Name 5      | Description 5      | TRUE    | FALSE   | TRUE    |     | TRUE
...

Scripts to filter out (rows): Those that don't trigger at least one of our requested events
No event (columns) should be filtered out, even if all scripts return FALSE for that event
In the example above, script 4 was filtered out because it did not trigger any of the requested events.

In the table, the event columns we want triggered are highlighted in yellow

From the table, the user may click a script name to open it in the editor

# V2 (Not in our demo scope)
Sorting the MVP table:
    1. By the number of requested events that each script triggers, in descending order (Primary sorting)
    2. By the number of other events that each script triggers, in ascending order (Secondary sorting)

The user may choose from the table multiple scripts and go to a page where a nice GUI allows the user to edit the params of that script