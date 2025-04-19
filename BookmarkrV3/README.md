# BookmarkrV3
## Chapter 6 - Error Handling and Logging
## TODO (p135):
- Task 1: Handling errors for the Import command
  - If the input file cannot be access, or if its content cannot be deserialized, it is likely that the code will throw exceptions.  Your mission is to identify what exceptions are likely to be thrown and handle them accordingly.
- Task 2: Logging errors to a file
  - In the previous task, the goal was to handle exceptions.  However, it might be useful to log the details of these exceptions to a file so we can review them later and use this information to improve the robustness of our application.
  - You mission here is to use Serilog to log exception data on a daily rolling interval and store these log files in the `logs/errors` folder.
  - You are also asked to customize the output template so that logs contain the following information:
    - Date and time of the event
    - The name of the machine on which the event happened
    - The type of event (warning, error, and so on)
    - The exception's details, including its stack trace
