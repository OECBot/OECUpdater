GUI Creation:
------

### Getting Started:
- Everything GUI is to be done through Linux.
- First you need to install the dependencies:
  - GTK3 and GTK#3(3.12+) (I suggest you use synaptic package manager to install these just incase)
- Next you need to install Glade
  - Set the Gtk version to 3.12 in preferences, this is the one we are going to use for the project.
- Now you can start to design the GUI.
- Please focus on functionality for now and worry about design/css later!

### Create the window class
- Next in the OECGUI project create your window class (preferably xxxWindow.cs, do not worry about the other windows)
- Look at LoginWindow.cs follow the example exactly, if you have any questions ask on facebook
  - There is little to no actual online reference or documentation on Gtk#3 if you are going to google it google with gtk#2, which is similar enough) (Actually gtk#3 and gtk#2 uses the exact same API except for a few small things)
- Adding the .glade file
  - Import it directly into the project and set the properties to (Copy if new) and (Make Embedded Resource) Now you can reference it as OECGUI.xxx.glade
- It is important that you do *NOT* use the built in GUI editor to do *ANYTHING* as GTK#3 is not supported and all design work should be done with Glade.
- Now you can load the window and do whatever you want with it

### Asset Management
- For testing purposes you can use absolute references but it would be nice if all assets(image,etc) are embedded
- You can do so with the same procedure as adding the glade files.
- Regarding the use of CSS beware that most references are basic CSS properties such as font, color, border, etc. However they may be unique ones so if you have any trouble as on Facebook or google it using GTK3 (No #).

### References to help your progress
*[CSS Style Guide](https://thegnomejournal.wordpress.com/2011/03/15/styling-gtk-with-css/)*

*[GTK# Source Project](https://github.com/mono/gtk-sharp) Lots of samples and demos here*

*[GTK#3 Template Project](http://addins.monodevelop.com/Project/Index/97) You will need a .NET disassembler to view the source code.*

*[GTK#2 Guide](http://www.mono-project.com/docs/gui/gtksharp/beginners-guide/) I think this guide is kinda of outdated.*

