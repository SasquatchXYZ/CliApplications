# BookmarkrV7

## Chapter 7 - Interactive CLI Applications

## TODO (p135):

- Task 1: Present a bookmark in a user-friendly way
    - You've been asked to add a `show` command that takes the name of a bookmark and displays it in a three-column
      grid—one for the name, one for the Url, and one for the category.
    - The grid should contain a row for the headers (name, Url, and category)
    - The name should be displayed in yellow and bold; the Url should be presented as a link; and the category should be
      presented in italics and green.
    - The syntax of the command should be as follows:
        - `bookmarkr link show --name <name of the bookmark>`
- Task 2: Change the category of a bookmark interactively
    - You've been asked to implement a new command called `category change` that changes the category of an existing
      Url.
    - The command must display the list of existing categories as a selection menu; the user will have to select which
      one will be set as the new category for the bookmark based on its Url. This update will then be saved to the
      database.
    - The syntax of the command should be as follows:
        - `bookmarkr category change --for-url <url of the bookmark>`
