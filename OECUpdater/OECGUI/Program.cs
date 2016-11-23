using System;
using Gtk;

namespace OECGUI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
<<<<<<< HEAD
			LoginWindow main = LoginWindow.Create ();

=======
			LoginWindow main = LoginWindow.Create();
>>>>>>> master
			main.Show ();
			Application.Run ();
		}
			
	}
}
