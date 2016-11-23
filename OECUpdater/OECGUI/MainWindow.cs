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

			BotForm bf = BotForm.Create (manager);
			Label l1 = new Label ("OECBot");
			l1.StyleContext.AddClass ("fontWhite");

			//Label l2 = new Label ("   ");
			//l2.StyleContext.AddClass ("fontWhite");
			notebook1.AppendPage (bf, l1);

			//notebook1.AppendPage (BotForm.Create(manager), l2);


			provider.LoadFromPath ("test.css");
			l1.StyleContext.AddProvider (provider, uint.MaxValue);
			//l2.StyleContext.AddProvider (provider, uint.MaxValue);
			ApplyCss (this, provider, uint.MaxValue);
			//ShowAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}



}

