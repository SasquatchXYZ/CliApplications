# BookmarkrV2
## Chapter 5 - Input/Output and File Handling
## TODO (p113):
- Task 1: Validating the format and the ability to access the input file
  - `bookmarkr import --file <path to input file>`
  - If the input file cannot be accessed, or if its data is not in the correct format, then the application should display a corresponding error message to the user.  Otherwise, the application should import all the bookmarks from the input file and display a success message to the user indicating how many bookmarks have been imported.
- Task 2: Merging existing links from the input file
  - `--merge` option for the `import` command
  - `bookmarkr import --file <path to input file> --merge`
  - If its Url already exists in the list of bookmarks held by the application, the name of the existing bookmark should be updated with the name corresponding to this Url in the input file
  - Otherwise, the bookmark should simply be added to the list of bookmarks held by the application.
