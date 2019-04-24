# Simple Game Accounts

A mile-high view at how game accounts could be implemented if you didn't want to gather any player information. Inspired by the account ids and recovery present in Puzzle and Dragons.

This is a pet project that interested me, so I whipped it up in a day. I have many doubts I will ever touch it again, but I may put it to use for a small multiplayer game eventually.

# This project doesn't:

 * Provide adequate security
 * Provide networking
 * Provide any particular account functionality (IAP, Economy, etc)

# This project does:

 * Create and verify unique player accounts
 * Suggest how adequate security might be added
 * Provide comments describing how data should be used.
 * Suggest how additional account functionality could be added.
 * Use Lightning.NET (LMDB) via Nuget for account storage

# Using this project:

It's not really meant to be directly consumed. However, if you fork it and make the necessary changes to bring it up to standard, then it should be easy to reference the project or its outputs as a DLL.

There is a project of Visual Studio Unit tests that validate the functionality. Lightning.NET only works for x64 builds, so you may need to adjust the default archtecture for the test runner. (In VS2017: Test > Test Settings > Default Processor Archtecture > x64)

If you springboard from this project and feel it helped guide you, feel free to donate to my ko-fi.
https://ko-fi.com/willardf