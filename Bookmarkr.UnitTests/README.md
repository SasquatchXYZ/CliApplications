# Bookmarkr.UnitTests

## Chapter 10—Testing CLI Applications

## TODO (p217):

- Task 1: Write the required unit tests for the remaining functionalities
    - In this chapter, we wrote tests only for the `link` and `import` commands. You are hence challenged to write tests for the other commands.  You will have to figure out what test cases are to be considered and implement them.
- Task 2: Write integration tests for the sync command
    - The `sync` command deals with a database.  For unit testing, you can mock the database using NSubstitute.  However, when implementing an integration test, you need a read database.  You are then challenged to write integration tests for the `sync` command.  You will have to provide a test database and use the appropriate connetion string depending on whether the application is running in production or in testing mode
