# How to publish a new release

1. Increase major, minor and/or build version in AssemblyInfo.cs
2. Increase the version number in the file plugin.json
3. Build the release using "Release"-configuration in Visual Studio
4. Zip the content of the folder [...]\Wox.Plugin.SwiftTweet\bin\Release and rename the zip-fileextension to .wox
5. Upload the .wox-file to http://www.getwox.com/#plugin/63/edit
6. Commit changes to the corresponding GitHub branch
7. Create and confirm the pull request to the master branch
8. Optional: Delete the corresponding GitHub branch
9. Create a new GitHub release: https://github.com/NCiher/Wox.Plugin.SwiftTweet/releases
10. Describe the change in the GitHub release
11. Upload the .wox-file to the GitHub release
