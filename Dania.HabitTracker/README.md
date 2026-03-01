# Habit Logger
Habit Logger is the second green belt project from the C# Academy, that requires users to track their habit with a date and a quantity. This project introduces me to SQLite and the CRUD concept (Create, Read, Update and Delete). I programmed using C# and SQlite with Visual Studio 2026.

## Requirements
- Database needs to be created initially if it doesn't exist.
- The application used real database to store and retrieve data.
- Users create their own habit to track and let them choose their own unit of measurement. It shouldn't create a table for each habit.
- Users log their habit by inserting dates and the quantity of their measurement.
- Users can also update and delete their logged habits.
- Handle validation on user's input when it comes to numbers and dates.
- Seed Data into the databases
- Writing this Readme!

## Features
- Console based UI where users navigate through number input.
![Image](/Assets/1.png)
- CRUD functions
    - Users can create new habits with their habit name and their unit measurement name.
    - Users can create, read, update and delete entries of their habit log by choosing from the habit table ID, inputting their date and quantity of recorded said habit. 
    - Dates and numbers are validated to check if they're in the right format.
![Image](/Assets/2.png)

## Challenges
- I'm not new to C# since I've done the previous lessons but SQlite is on itself a new programming skill to learn. I had to learn from tutorial sites and videos in order to understand and complete this project.
- The challenge to create their own habit to track is a confusing one to understand. I had to ask the people on Discord what they meant by that before I assumed I had to just rename the habit table only.

## Lessons Learned
- Ask others in the Discord! I'm a bit shy and don't want to sound stupid but every help can from others can point toward the right direction.

## Things I didn't manage to do
- Doing CRUD on the habits table itself. I wanted to focus on one table right now, but I'm also not sure if one of the row being deleted, what would happen to the main habit log itself? Would it be given nulls on the habit that doesn't exists?  

## Resources used
- https://www.youtube.com/watch?v=d1JIJdDVFjs to give some knowledge on how to start the project itself.
- https://www.sqlitetutorial.net/ For understanding how SQlite works in general.
- nwordian from the C# Academy Discord channel for pointing the used of Foreign keys on how to create two table relationships.
- https://sqlitebrowser.org/ for visual database
- https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow for solving the slow seeding on habit log entries issue
