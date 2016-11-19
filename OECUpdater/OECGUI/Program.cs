using System;
using Gtk;

namespace OECGUI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			RequestWindow main = RequestWindow.Create ();
			main.Show ();
			Application.Run ();
		}
			
	}
}
