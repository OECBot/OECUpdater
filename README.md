# Team06-Repository

![alt](http://i.imgur.com/nZHE2pK.jpg)

This is the repository for Team06.

Deliverables can be found in folders called deliverables#, where # indicate the deliverable number.

## Install Notes

This software requires the mono runtime to run, this is offered as FOSS by the [mono-project](http://www.mono-project.com/). However included int the root directory are two scripts to help ease this process. 

First, make these scripts executable:

``` shell
chmod u+x dependencies.sh
chmod u+x compileandrun.sh
```

Install dependencies:

`sudo ./dependencies.sh`

Compile and Run:

`./compileandrun.sh`

### Note:
- The solution is built and moved to the 'Release' folder in the root directory.
- 'dependencies.sh' works only on Debian (and derivatives), Fedora (and derivatives), and OSX (theoretically)
- The OSX path hasn't been actually tested, however it should work. It uses the 'installer' command to install a pkg downloaded from the internet
