# BookmarkrV13

`dotnet build -c Release`
`dotnet bin/Release/net9.0/BookmarkerV13.dll benchmark`

## Chapter 13—Security Considerations for CLI Applications

## TODO (p277):

- Task 1: Update dependency versions
    - Update the dependencies and validate that the behavior of the application was not impacted.
- Task 2: Use Mend Bolt to scan the code for vulnerabilities
    - Enable Mend Bolt and run a security scan, if any vulnerability is found, fix it.
- Task 3: Allow Bookmarkr Syncr to manage multiple users
    - Although we have updated BookmarkrSyncr to receive and validate a PAT, it does not use this token to retrieve and
      update the appropriate user's data.
    - You are asked to update the code to make this happen. The easiest way to achieve this is to have a separate JSON
      file for every user whose name matches the value of the PAT. Hence, the bookmarks for every user can be stored and
      retrieved from there.
