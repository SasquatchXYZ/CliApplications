# BookmarkrV9

## Chapter 9—Working with External APIs and Services

## TODO (p188):

- Task 1: Adding SQLite as a data store
    - Until now, our application has stored its bookmarks in memory.
    - You are asked to add a new dependency to the Bookmarkr application—a SQLite database. This will allow bookmarks
      to be stored in a more permanent manner by Bookmarkr, making it more useful to our users.
    - SQLite is a versatile and lightweight database solution, designed to be both straightforward and easy to use while
      requiring minimal setup and administration. One of its most significant advantages is its portability: the entire
      database is stored in a single file, which makes it easy to move, back up, and distribute. Its self-contained
      nature also means that SQLite doesn't require a separate server process or system configuration, simplifying its
      deployment. That is why it is a great fit for CLI applications.
    - Now, you also need to modify the code of `BookmarkService` to retrieve bookmarks from and store bookmarks in the
      SQLite database.
    - Consider using the `Microsoft.Data.Sqlite` library for .NET, as it is a reliable and lightweight library. Consider
      adding migrations and ensuring thread-safe access for SQLite in concurrent CLI scenarios.
- Task 2: Retrieving the web page name based on its Url
    - Until now, when adding a bookmark, we had to pass both the web page name and Url.
    - Let's tweak the `link add` command so that it makes an HTTP request to retrieve the name of the web page to
      bookmark based on the provided Url. If the name can't be retrieved, we can then use the name passed as a command
      option.
    - If the web page cannot be found, the bookmark's name should be `Unnamed bookmark`. If the request takes more than
      30 seconds, terminate it and also set the name to `Unnamed bookmark`.
