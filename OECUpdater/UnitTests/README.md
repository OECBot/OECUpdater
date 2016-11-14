UnitTests
------

This project of the solution is dedicated to creating unit tests under the NUnit Framework.


Getting started
------
### Creating the Tests:
- Create a new class xxxUnitTests.cs
- Make sure you have the right dependencies
- Add using NUnit.Framework to your class
- [Familiarize yourself with the NUnit API](https://github.com/nunit/docs/wiki)
- Look at the sample project for reference/ideas - [Link](https://github.com/nunit/nunit-csharp-samples/blob/master/)
- Create required unit tests for components

### Running the Tests:
- First build the project, make sure there is a UnitTests.dll in the debug folder
- Run tests with NUnit Console Runner, which can be found in:

`.\team06-Project\OECUpdater\packages\NUnit.ConsoleRunner.3.5.0\tools`
  - YOU MUST USE TERMINAL(OR CMD IN WINDOWS) TO RUN IT!
  - Syntax(For windows only) = `nunit3-console [input assembly location]`
    - The assemblies should be located in the debug folder of the UnitTests project
- Additional information can be found here: [Wiki](https://github.com/nunit/docs/wiki/Console-Command-Line)

#### NEW:
- On windows you can now use the runtests.cmd located in the main directory to run tests!

### Looking at the results:
- Once finished, NUnit will provide a summary of the test results, **ONLY** the failed tests will show up, NUnit will not display infomation of a test if it succeeded.
- DONE!

Selecting specific classes to test
-----
- Use the following parameter to select the class to test:
  - `--where=[Class Name]`
