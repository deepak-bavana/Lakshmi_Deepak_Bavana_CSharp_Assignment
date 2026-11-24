=> Bavana Lakshmi Deepak - Employee Time Report - C# Console App

A small .NET console app that reads employee time entries from a REST API and produces two outputs: an HTML table and a pie chart image.

=> What it does
1. Downloads the JSON data from the API with a GET request.
2. Converts the JSON into C# objects.
3. Adds up the hours worked per employee.
4. Writes employees.html: a table of Name and Total time worked, sorted from high to least. Rows with less than 100 hours are highlighted in red.
5. Writes piechart.png: a pie chart showing each employees percentage of the total time worked.

=> Tech stack
1. C# / .NET 10 (Visual Studio)
2. HttpClient to call the API
3. System.Text.Json to parse the JSON
4. System.Drawing.Common to draw the pie chart
5. Plain HTML for the table

=> Data cleaning
The API data had some bad records
1. entries with no employee name
2. entries marked as deleted 
3. entries where the end time is before the start time

Out of 821 entries, this skipped 15 without a name, 40 deleted, and 18 with an invalid time range.