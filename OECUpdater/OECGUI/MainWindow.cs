﻿using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OECGUI
{
	public class MainWindow: Gtk.Window
	{
		[UI] Notebook notebook1;
		[UI] Image image1;
		private BotForm bf;
		public static RepositoryManager manager;
		public static Session session;

		public static MainWindow Create (RepositoryManager manager)
		{
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.MainWindow.glade", null);

			MainWindow.manager = manager;
			MainWindow.session = manager.session;
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



			bf = BotForm.Create ();

			var box3 = new HBox ();

			var pixbuffloader = new Gdk.PixbufLoader ("Assets/push.png", 32, 32);
			var pixbuff = pixbuffloader.Pixbuf;
			//pixbuff.ScaleSimple (36, 36, Gdk.InterpType.Hyper);
			var icon2 = new Image (pixbuff);
			
			Label l3 = new Label ("Pull-Requests");
			box3.PackStart (icon2, false, false, 0);
			l3.StyleContext.AddClass ("fontWhite");
			box3.PackStart (l3, false, false, 0);

			var box4 = new HBox ();
			pixbuffloader = new Gdk.PixbufLoader ("Assets/plugin.png", 32, 32);
			var icon3 = new Image (pixbuffloader.Pixbuf);
			box4.PackStart (icon3, false, false, 0);
			var l4 = new Label ("Plugins");
			l4.StyleContext.AddClass ("fontWhite");
			box4.PackStart (l4, false, false, 0);
			box4.ShowAll ();

			var box = new HBox ();
			pixbuffloader = new Gdk.PixbufLoader ("Assets/home.png", 32, 32);
			var img = new Image (pixbuffloader.Pixbuf);


			var box2 = new HBox();
			pixbuffloader = new Gdk.PixbufLoader ("Assets/power.png", 32, 32);
			var icon = new Image (pixbuffloader.Pixbuf);

			box2.PackStart (icon, false, false, 0);

			box.PackStart (img, false, false, 0);
			Label l1 = new Label ("OECBot");
			l1.StyleContext.AddClass ("fontWhite");
			box2.PackStart (l1, false, false, 0);
			Label l2 = new Label ("Dashboard");

			box.PackStart (l2, false, false, 0);
			l2.StyleContext.AddClass ("fontWhite");

			pixbuffloader = new Gdk.PixbufLoader ("Assets/logo.png", 110, 40);
			image1.Pixbuf = pixbuffloader.Pixbuf;

			box.ShowAll ();
			box2.ShowAll ();
			box3.ShowAll ();

			DashboardForm df = DashboardForm.Create ();
			notebook1.AppendPage (df, box);
			bf.dashboard = df;
			notebook1.AppendPage (bf, box2);
			notebook1.AppendPage (DashboardForm.Create (), box4);
			notebook1.AppendPage (RequestWindow.Create(), box3);




			provider.LoadFromPath ("test.css");
			l1.StyleContext.AddProvider (provider, uint.MaxValue);
			l2.StyleContext.AddProvider (provider, uint.MaxValue);
			l3.StyleContext.AddProvider (provider, uint.MaxValue);
			l4.StyleContext.AddProvider (provider, uint.MaxValue);
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

