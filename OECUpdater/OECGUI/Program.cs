using System;
using Gtk;

namespace OECGUI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			LoginWindow main = LoginWindow.Create ();
			main.Show ();
			Application.Run ();
		}
			
	}
}
