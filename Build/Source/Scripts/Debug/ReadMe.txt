In order to debug the individual build scripts, you need to open a command prompt, 
change to the Debug directory, where the test scripts reside, 
then execute the individual build script.

This will navigate up to the root dir and execute the script from there.

It would then be advisable to execute the main build-test.cmd script from root,
which will run all the build scripts in debug mode.

Then check to make sure Nuget packages have been added to the build output directory.