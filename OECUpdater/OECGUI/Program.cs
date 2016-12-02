using System;
using Gtk;
using OECLib.Utilities;

namespace OECGUI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			Logger.Initialize ();
			LoginWindow main = LoginWindow.Create();
			main.Show ();
			Application.Run ();
		}
			
	}
}
