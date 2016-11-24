using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using System.IO;
using System.Text;

namespace OECGUI
{
	public class MainWindow: Gtk.Window
	{
		[UI] Notebook notebook1;
		BotForm bf;

		public static MainWindow Create (RepositoryManager manager)
		{
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.MainWindow.glade", null);

			MainWindow m = new MainWindow (builder, builder.GetObject ("MainWindow").Handle, manager);


			return m;

		}

		public void ApplyCss (Widget widget, CssProvider provider, uint priority)
		{
			widget.StyleContext.AddProvider (provider, priority);
			var container = widget as Container;
			if (container != null) {
				foreach (var child in container.Children) {
					ApplyCss (child, provider, priority);
				}
			}
		}

		public MainWindow (Builder builder, IntPtr handle, RepositoryManager manager) : base (handle)
		{
			//revealer = new Widget (builder.GetObject ("revealer1").Handle);

			CssProvider provider = new CssProvider ();

			builder.Autoconnect (this);
			DeleteEvent += OnDeleteEvent;



			bf = BotForm.Create (manager);
			//var box = new HBox ();
			//var icon = new Image ("Assets/home.png");
			Label l1 = new Label ("OECBot");
			l1.StyleContext.AddClass ("fontWhite");

			Label l2 = new Label ("Dashboard");

			l2.StyleContext.AddClass ("fontWhite");
			notebook1.AppendPage (DashboardForm.Create(), l2);
			notebook1.AppendPage (bf, l1);




			provider.LoadFromPath ("test.css");
			l1.StyleContext.AddProvider (provider, uint.MaxValue);
			l2.StyleContext.AddProvider (provider, uint.MaxValue);
			ApplyCss (this, provider, uint.MaxValue);
			//ShowAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Console.WriteLine ("ON DELETE MAIN");
			bf.OnDeleteEvent (sender, a);
		}
	}



}

